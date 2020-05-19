using Entity;
using Entity.WebModel;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Operation.Common.Utils;

namespace Operation.Common
{
    /// <summary>
    /// 游戏状态辨别
    /// </summary>
    public static class GameDiscrimination
    {
        static GameDiscrimination()
        {

        }

        public static async Task<WebAppGameInfos> EachpartAsync(GameOfType gameType, string merchantID, GameNextLottery status = null)
        {
            WebAppGameInfos gameInfos = new WebAppGameInfos()
            {
                GameType = gameType
            };
            if (status == null)
                status = RedisOperation.GetValue<GameNextLottery>("GameStatus", Enum.GetName(typeof(GameOfType), gameType));
            try
            {
                //获取商户基础设置游戏设置
                var foundation = RedisOperation.GetFoundationSetup(merchantID);
                if (foundation == null)
                {
                    gameInfos.Status = GameStatusEnum.已关闭;
                    return gameInfos;
                }
                var roomInfo = await Utils.GetRoomInfosAsync(merchantID, gameType);
                if (roomInfo.Status == RoomStatus.关闭)
                {
                    gameInfos.Status = GameStatusEnum.已关闭;
                    return gameInfos;
                }
                if (status.DayNum == 0)
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
                var info = foundation.LotteryFrontTime.Find(t => t.Type == gameType);
                var sealedTime = info.LotteryTime;
                //判断当前游戏是否在时间段内
                if (GameTypeItemize(gameType))
                    gameInfos = await GameBoll10(sealedTime, gameInfos, gameType, merchantID, status, roomInfo);
                else
                    gameInfos = await GameBoll5(sealedTime, gameInfos, gameType, merchantID, status, roomInfo);
                if (gameInfos.Status == GameStatusEnum.等待中 || gameInfos.Status == GameStatusEnum.封盘中)
                {
                    if (gameInfos.Status == GameStatusEnum.等待中)
                        gameInfos.SealingTime = sealedTime;
                }
            }
            catch (NullReferenceException)
            {
                gameInfos.Status = GameStatusEnum.已停售;
            }
            catch (Exception e)
            {
                gameInfos.Status = GameStatusEnum.已停售;
                Utils.Logger.Error(string.Format("游戏：{0}  错误：{1}  商户id：{2}", gameType, e, merchantID));
            }
            return gameInfos;
        }

        /// <summary>
        /// 判断游戏期数是否存在
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="issueNum"></param>
        /// <returns></returns>
        public static async Task<bool> GameExistenceAsync(GameOfType gameType, string issueNum, string merchantID)
        {
            if (Utils.GameTypeItemize(gameType))
            {
                Lottery10Operation lottery10Operation = new Lottery10Operation();
                var model = await lottery10Operation.GetModelAsync(t => t.GameType == gameType && t.IssueNum == issueNum && (t.MerchantID == null || t.MerchantID == merchantID));
                return model == null;
            }
            else
            {
                Lottery5Operation lottery5Operation = new Lottery5Operation();
                var model = await lottery5Operation.GetModelAsync(t => t.GameType == gameType && t.IssueNum == issueNum && (t.MerchantID == null || t.MerchantID == merchantID));
                return model == null;
            }
        }

        /// <summary>
        /// 10球游戏状态
        /// </summary>
        /// <param name="sealedTime"></param>
        /// <param name="gameInfos"></param>
        /// <param name="gameType"></param>
        /// <param name="merchantID"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static async Task<WebAppGameInfos> GameBoll10(int sealedTime, WebAppGameInfos gameInfos, GameOfType gameType, string merchantID, GameNextLottery result, RoomGameDetailed roomInfo)
        {
            Lottery10Operation lottery10Operation = new Lottery10Operation();
            var startTime = new DateTime();
            var endTime = new DateTime();
            if (result == null)
            {
                var gameInfo = await GetGameNextLottery(gameType);
                //添加到redis
                RedisOperation.SetHash("GameStatus", Enum.GetName(typeof(GameOfType), gameType), JsonConvert.SerializeObject(gameInfo));
                gameInfos.Status = GameStatusEnum.已停售;
                return gameInfos;
            }
            if (result.StartTime < DateTime.Now)
            {
                var gameInfo = await GetGameNextLottery(gameType);
                //添加到redis
                RedisOperation.SetHash("GameStatus", Enum.GetName(typeof(GameOfType), gameType), JsonConvert.SerializeObject(gameInfo));
                result = gameInfo;
                if (gameInfo.DayNum == 0)
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
            }

            gameInfos.StartTime = result.StartTime;
            gameInfos.NextIssueNum = result.NextNper;
            var preNper = GameHandle.GetGamePreNper(gameInfos.NextIssueNum, gameType);
            gameInfos.IssueNum = preNper;
            var model = await lottery10Operation.GetModelAsync(t => t.IssueNum == preNper && t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID));
            if (model != null)
            {
                gameInfos.Number = SetupSeparation10(model);
                var msgList = new List<string>();
                msgList.Add(model.Count.ToString().PadLeft(2, '0'));
                msgList.Add(Enum.GetName(typeof(SindouEnum), (int)model.Sindou));
                msgList.Add(Enum.GetName(typeof(SizeEnum), (int)model.CountSize));
                msgList.Add(Enum.GetName(typeof(DraTig), (int)model.DraTig1));
                msgList.Add(Enum.GetName(typeof(DraTig), (int)model.DraTig2));
                msgList.Add(Enum.GetName(typeof(DraTig), (int)model.DraTig3));
                msgList.Add(Enum.GetName(typeof(DraTig), (int)model.DraTig4));
                msgList.Add(Enum.GetName(typeof(DraTig), (int)model.DraTig5));
                gameInfos.Message = string.Join("|", msgList);
            }

            //判断游戏房间是否关闭
            #region 判断当前游戏状态
            if (roomInfo.HaltSales)
            {
                if (roomInfo.HaltTime == roomInfo.OnSaleTime)
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
                if (DateTime.Now >= DateTime.Now.Date.AddHours(roomInfo.HaltTime.Hour).AddMinutes(roomInfo.HaltTime.Minute)
                    && DateTime.Now <= DateTime.Now.Date.AddHours(roomInfo.OnSaleTime.Hour).AddMinutes(roomInfo.OnSaleTime.Minute))
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
            }
            #endregion
            if (gameType == GameOfType.幸运飞艇 || gameType == GameOfType.幸运飞艇168)
            {
                if (gameInfos.NextIssueNum.Substring(gameInfos.NextIssueNum.Length - 3) == "001"
                    && (gameInfos.StartTime - DateTime.Now).TotalMinutes > 30)
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
                if (DateTime.Now < DateTime.Now.Date.AddHours(4).AddMinutes(5))
                {
                    startTime = DateTime.Now.Date.AddDays(-1).AddHours(13).AddMinutes(9);
                    endTime = DateTime.Now.Date.AddHours(4).AddMinutes(5);
                }
                else if (DateTime.Now > DateTime.Now.Date.AddHours(13).AddMinutes(4))
                {
                    startTime = DateTime.Now.Date.AddHours(13).AddMinutes(9);
                    endTime = DateTime.Now.Date.AddDays(1).AddHours(4).AddMinutes(5);
                }
                else
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
            }
            else if (gameType == GameOfType.北京赛车)
            {
                if (DateTime.Now > DateTime.Now.Date.AddHours(9).AddMinutes(13) && DateTime.Now < DateTime.Now.Date.AddDays(1).AddMinutes(-5))
                {
                    startTime = DateTime.Now.Date.AddHours(9).AddMinutes(13);
                    endTime = DateTime.Now.Date.AddDays(1).AddMinutes(-5);
                }
                else
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
            }
            else if (gameType == GameOfType.爱尔兰赛马)
            {
                if (DateTime.Now < DateTime.Now.Date.AddHours(4).AddMinutes(4))
                {
                    startTime = DateTime.Now.Date.AddDays(-1).AddHours(8).AddMinutes(3);
                    endTime = DateTime.Now.Date.AddHours(4).AddMinutes(4);
                }
                else if (DateTime.Now > DateTime.Today.AddHours(8).AddMinutes(3))
                {
                    startTime = DateTime.Now.Date.AddHours(8).AddMinutes(3);
                    endTime = DateTime.Now.Date.AddHours(4).AddDays(1).AddMinutes(4);
                }
                else
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
            }
            else
            {
                startTime = DateTime.Today;
                endTime = DateTime.Today.AddDays(1).AddSeconds(-1);
            }
            var count = await lottery10Operation.GetCountAsync(t => t.GameType == gameType && t.CreatedTime >= startTime && t.CreatedTime <= endTime);
            if (count > result.DayNum)
            {
                gameInfos.Status = GameStatusEnum.已停售;
                return gameInfos;
            }
            #region 状态判断
            //开奖时间
            var lotteryTime = 30;
            //封盘时间
            if (result.StartTime.AddSeconds(-sealedTime) > DateTime.Now)
            {
                //时间超过30分钟   也默认为停售
                if ((result.StartTime.AddSeconds(-sealedTime) - DateTime.Now).TotalMinutes >= 30)
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
                gameInfos.Status = GameStatusEnum.等待中;
                gameInfos.Surplus = (int)(result.StartTime.AddSeconds(-sealedTime) - DateTime.Now).TotalSeconds;
                return gameInfos;
            }
            else if (result.StartTime.AddSeconds(-sealedTime) <= DateTime.Now && result.StartTime >= DateTime.Now)
            {
                gameInfos.Status = GameStatusEnum.封盘中;
                gameInfos.Surplus = (int)(result.StartTime - DateTime.Now).TotalSeconds;
                return gameInfos;
            }
            //开奖时间
            else if (result.StartTime.AddSeconds(lotteryTime) >= DateTime.Now)
            {
                //最新开奖
                var newLottery = await lottery10Operation.GetModelAsync(t => t.IssueNum == result.NextNper && t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID));
                if (newLottery == null)
                {
                    gameInfos.Status = GameStatusEnum.开奖中;
                    gameInfos.Surplus = 0;
                    return gameInfos;
                }
                else
                    return await GameBoll10(sealedTime, gameInfos, gameType, merchantID, result, roomInfo);
            }
            else
            {
                gameInfos.Status = GameStatusEnum.开奖中;
                return gameInfos;
            }
            #endregion
        }

        /// <summary>
        /// 5球游戏状态
        /// </summary>
        /// <param name="sealedTime"></param>
        /// <param name="gameInfos"></param>
        /// <param name="gameType"></param>
        /// <param name="merchantID"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        private static async Task<WebAppGameInfos> GameBoll5(int sealedTime, WebAppGameInfos gameInfos, GameOfType gameType, string merchantID, GameNextLottery result, RoomGameDetailed roomInfo)
        {
            Lottery5Operation lottery5Operation = new Lottery5Operation();
            if (result == null)
            {
                var gameInfo = await GetGameNextLottery(gameType);
                //添加到redis
                RedisOperation.SetHash("GameStatus", Enum.GetName(typeof(GameOfType), gameType), JsonConvert.SerializeObject(gameInfo));
                gameInfos.Status = GameStatusEnum.已停售;
                return gameInfos;
            }
            if (result.StartTime < DateTime.Now)
            {
                var gameInfo = await GetGameNextLottery(gameType);
                //添加到redis
                RedisOperation.SetHash("GameStatus", Enum.GetName(typeof(GameOfType), gameType), JsonConvert.SerializeObject(gameInfo));
                result = gameInfo;
            }

            gameInfos.StartTime = result.StartTime;
            gameInfos.NextIssueNum = result.NextNper;
            var preNper = GameHandle.GetGamePreNper(gameInfos.NextIssueNum, gameType);
            gameInfos.IssueNum = preNper;
            var model = await lottery5Operation.GetModelAsync(t => t.IssueNum == preNper && t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID));
            if (model != null)
            {
                gameInfos.Number = SetupSeparation5(model);
                var msgList = new List<string>();
                msgList.Add(model.Count.ToString().PadLeft(2, '0'));
                msgList.Add(Enum.GetName(typeof(SizeEnum), (int)model.CountSize));
                msgList.Add(Enum.GetName(typeof(SindouEnum), (int)model.CountSinDou));
                msgList.Add(Enum.GetName(typeof(DraTig5), (int)model.DraTig));
                msgList.Add(Enum.GetName(typeof(RuleEnum), (int)model.Front3));
                msgList.Add(Enum.GetName(typeof(RuleEnum), (int)model.Middle3));
                msgList.Add(Enum.GetName(typeof(RuleEnum), (int)model.Back3));
                gameInfos.Message = string.Join("|", msgList);
            }

            //判断游戏房间是否关闭
            #region 判断当前游戏状态
            if (roomInfo.HaltSales)
            {
                if (roomInfo.HaltTime == roomInfo.OnSaleTime)
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
                if (DateTime.Now >= DateTime.Now.Date.AddHours(roomInfo.HaltTime.Hour).AddMinutes(roomInfo.HaltTime.Minute)
                    && DateTime.Now <= DateTime.Now.Date.AddHours(roomInfo.OnSaleTime.Hour).AddMinutes(roomInfo.OnSaleTime.Minute))
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
            }
            #endregion
            var startTime = new DateTime();
            var endTime = new DateTime();
            if (gameType == GameOfType.重庆时时彩)
            {
                if (DateTime.Now > DateTime.Now.Date.AddMinutes(10) && DateTime.Now < DateTime.Now.Date.AddHours(3).AddMinutes(10))
                {
                    startTime = DateTime.Now.Date.AddDays(-1).AddHours(7).AddMinutes(10);
                    endTime = DateTime.Now.Date.AddHours(3).AddMinutes(10);
                }
                else if (DateTime.Now > DateTime.Now.Date.AddHours(7).AddMinutes(10) && DateTime.Now < DateTime.Now.Date.AddHours(23).AddMinutes(50))
                {
                    startTime = DateTime.Now.Date.AddHours(7).AddMinutes(10);
                    endTime = DateTime.Now.Date.AddDays(1).AddHours(3).AddMinutes(10);
                }
                else
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
            }
            else if (gameType == GameOfType.爱尔兰快5)
            {
                if (DateTime.Now < DateTime.Today.AddHours(4).AddMinutes(2))
                {
                    startTime = DateTime.Today.AddDays(-1).AddHours(8).AddMinutes(1);
                    endTime = DateTime.Today.AddHours(4).AddMinutes(2);
                }
                else if (DateTime.Now > DateTime.Today.AddHours(8).AddMinutes(1))
                {
                    startTime = DateTime.Today.AddHours(8).AddMinutes(1);
                    endTime = DateTime.Today.AddDays(1).AddHours(4).AddMinutes(2);
                }
                else
                {
                    gameInfos.Status = GameStatusEnum.已停售;
                    return gameInfos;
                }
            }
            var count = await lottery5Operation.GetCountAsync(t => t.GameType == gameType && t.CreatedTime >= startTime && t.CreatedTime <= endTime);

            if (count > result.DayNum)
            {
                gameInfos.Status = GameStatusEnum.已停售;
                return gameInfos;
            }
            #region 状态判断
            var lotteryTime = 30;
            //最新开奖
            var newLottery = await lottery5Operation.GetModelAsync(t => t.IssueNum == result.NextNper && t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID));
            //封盘时间
            if (result.StartTime.AddSeconds(-sealedTime) > DateTime.Now)
            {
                gameInfos.Status = GameStatusEnum.等待中;
                gameInfos.Surplus = (int)(result.StartTime.AddSeconds(-sealedTime) - DateTime.Now).TotalSeconds;
                return gameInfos;
            }
            else if (result.StartTime.AddSeconds(-sealedTime) <= DateTime.Now && result.StartTime >= DateTime.Now)
            {
                gameInfos.Status = GameStatusEnum.封盘中;
                gameInfos.Surplus = (int)(result.StartTime - DateTime.Now).TotalSeconds;
                return gameInfos;
            }
            //开奖时间
            else if (result.StartTime.AddSeconds(lotteryTime) >= DateTime.Now && newLottery == null)
            {
                gameInfos.Status = GameStatusEnum.开奖中;
                gameInfos.Surplus = 0;
                return gameInfos;
            }
            else if (result.StartTime.AddSeconds(lotteryTime) < DateTime.Now && newLottery == null)
            {
                gameInfos.Status = GameStatusEnum.等待中;
                result.StartTime = result.StartTime.AddSeconds(result.Interval);
                gameInfos.StartTime = result.StartTime;
                gameInfos.IssueNum = result.NextNper;
                gameInfos.NextIssueNum = GameHandle.GetGameNper(result.NextNper, gameType);
                gameInfos.Surplus = (int)(result.StartTime.AddSeconds(-sealedTime) - DateTime.Now).TotalSeconds;
                gameInfos.Number = null;
                gameInfos.Message = null;
                return gameInfos;
            }
            else
            {
                gameInfos.Status = GameStatusEnum.开奖中;
                return gameInfos;
            }
            #endregion
        }

        /// <summary>
        /// 设置游戏号码分隔符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="sep">分隔符</param>
        /// <returns></returns>
        public static string SetupSeparation10<T>(T model, char sep = '|') where T : Lottery10
        {
            var type = model.GetType();
            List<string> nums = new List<string>();
            for (var i = 1; i < 11; i++)
            {
                var property = type.GetProperty("Num" + i.ToString());
                if (property == null) continue;
                nums.Add(property.GetValue(model).ToString());
            }
            return string.Join(sep, nums);
        }

        /// <summary>
        /// 设置游戏号码分隔符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="sep">分隔符</param>
        /// <returns></returns>
        public static string SetupSeparation5<T>(T model, char sep = '|') where T : Lottery5
        {
            var type = model.GetType();
            List<string> nums = new List<string>();
            for (var i = 1; i < 6; i++)
            {
                var property = type.GetProperty("Num" + i.ToString());
                if (property == null) continue;
                nums.Add(property.GetValue(model).ToString());
            }
            #region 龙虎和大小单双 
            //var draTig = Enum.GetName(typeof(DraTig5), type.GetProperty("DraTig").GetValue(model));
            //nums.Add(draTig);
            //var count = type.GetProperty("Count").GetValue(model).ToString();
            //nums.Add(count);
            //var countSize = Enum.GetName(typeof(SizeEnum), type.GetProperty("CountSize").GetValue(model));
            //nums.Add(countSize);
            //var countSinDou = Enum.GetName(typeof(SindouEnum), type.GetProperty("CountSinDou").GetValue(model));
            //nums.Add(countSinDou);
            #endregion
            return string.Join(sep, nums);
        }
    }
}
