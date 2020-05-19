using Entity;
using Entity.BaccaratModel;
using Entity.GraspModel;
using Entity.WebModel;
using Hangfire;
using Hangfire.Common;
using Hangfire.Server;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Operation.Abutment;
using Operation.Agent;
using Operation.Baccarat;
using Operation.Common;
using Operation.RedisAggregate;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Operation.Common.GameBetsMessage;
using static Operation.Common.Utils;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 游戏采集
    /// </summary>
    public class GameCollection
    {
        /// <summary>
        /// 游戏重试时间
        /// </summary>
        private static TimeSpan timeSpan => TimeSpan.FromSeconds(2);
        /// <summary>
        /// 间隔时间
        /// </summary>
        private static int interval => 2;
        private static FilterDefinition<Merchant> filter;

        private static async Task GetStatus()
        {
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = await advancedSetupOperation.GetModelAsync(t => t._id != null);
            if (setup == null) return;
            MerchantOperation merchantOperation = new MerchantOperation();
            if (setup.Formal)
                filter = merchantOperation.Builder.Where(t => t.Status == 1 && t.MaturityTime >= DateTime.Now);
            else
                filter = merchantOperation.Builder.Where(t => t.Status == 1);
        }

        /// <summary>
        /// 游戏对应连接
        /// </summary>
        readonly static Dictionary<GameOfType, string> dicUrls = new Dictionary<GameOfType, string>()
        {
            { GameOfType.北京赛车, "pk10Url" },
            { GameOfType.幸运飞艇, "xyftUrl" },
            { GameOfType.澳州10, "azxy10Url" },
            { GameOfType.澳州5, "azxy5Url" },
            { GameOfType.重庆时时彩, "cqsscUrl" },
            { GameOfType.爱尔兰快5, "ireland5Url" },
            { GameOfType.爱尔兰赛马, "ireland10Url" },
            { GameOfType.幸运飞艇168, "xyft168Url" },
            { GameOfType.极速赛车, "jsscUrl"},
            { GameOfType.极速时时彩, "jssscUrl"}
        };

        #region 游戏开奖
        /// <summary>
        /// 发送开奖任务
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="nper"></param>
        /// <param name="lottery"></param>
        /// <returns></returns>
        public static async Task GameLottery(GameOfType gameType, string nper, string lottery)
        {
            try
            {
                await GetStatus();
                //查询商户
                MerchantOperation merchantOperation = new MerchantOperation();
                //分库
                DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
                var databases = await databaseAddressOperation.GetModelListAsync(t => true);
                if (databases.IsNull()) return;
                var result = new List<TaskDistributionModel>();
                var allCount = 0;
                foreach (var database in databases)
                {
                    var condition = filter & merchantOperation.Builder.Eq(t => t.AddressID, database._id);
                    //商户总数量
                    var count = await merchantOperation.GetCountAsync(condition);
                    allCount += (int)count;
                    //一组数量
                    var pageSize = DistributionLow.limit;
                    var pageNum = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
                    for (int num = 1; num <= pageNum; num++)
                    {
                        var merchantList = merchantOperation.GetModelListByPaging(condition, t => t.CreatedTime, true, num, pageSize);
                        if (merchantList.IsNull()) continue;
                        var data = new TaskDistributionModel()
                        {
                            GameType = gameType,
                            Lottery = lottery,
                            Nper = nper,
                            MerchantIDList = merchantList.Select(t => t._id).ToList()
                        };
                        result.Add(data);
                        //Utils.Logger.Error(string.Format("游戏：{1}  分发开奖任务 任务号：{0}", data.UUID, gameType));
                        RabbitMQHelper.SendTaskDistribution(data);
                    }
                }
                await TaskRetransmission(result);
                //Utils.Logger.Error(string.Format("游戏：{1}  开奖结束，共处理{0}个商户", allCount, gameType));
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
            finally
            {
                //Utils.Logger.Error("结束开奖");
            }
        }

        /// <summary>
        /// 补发任务
        /// </summary>
        /// <param name="result"></param>
        public static async Task TaskRetransmission(List<TaskDistributionModel> result)
        {
            if (result.IsNull()) return;
            await Task.Delay(500);
            DistributionOperation distributionOperation = new DistributionOperation();
            for (int i = 0; i < result.Count; i++)
            {
                var data = result[i];
                var bson = await distributionOperation.GetModelAsync(t => t._id == data.UUID);
                if (bson == null)
                {
                    //Utils.Logger.Error(string.Format("游戏：{1}  继续分发开奖任务 任务号：{0}", data.UUID, data.GameType));
                    //再次发送直到有服务器接收
                    RabbitMQHelper.SendTaskDistribution(data);
                }
                else
                {
                    result.RemoveAt(i);
                    i--;
                }
            }
            await TaskRetransmission(result);
        }
        #endregion

        #region 视讯游戏开奖
        /// <summary>
        /// 发送开奖任务
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="nper">期号</param>
        /// <param name="lottery">开奖结果</param>
        /// <param name="znum">桌号</param>
        /// <returns></returns>
        public static async Task VideoGameLottery(BaccaratGameType gameType, string nper, string lottery, int znum)
        {
            try
            {
                await GetStatus();
                //查询商户
                MerchantOperation merchantOperation = new MerchantOperation();
                //分库
                DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
                var databases = await databaseAddressOperation.GetModelListAsync(t => true);
                if (databases.IsNull()) return;
                var result = new List<VideoTaskDistributionModel>();
                var allCount = 0;
                foreach (var database in databases)
                {
                    var condition = filter & merchantOperation.Builder.Eq(t => t.AddressID, database._id);
                    //商户总数量
                    var count = await merchantOperation.GetCountAsync(condition);
                    allCount += (int)count;
                    //一组数量
                    var pageSize = DistributionLow.limit;
                    var pageNum = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
                    for (int num = 1; num <= pageNum; num++)
                    {
                        var merchantList = merchantOperation.GetModelListByPaging(condition, t => t.CreatedTime, true, num, pageSize);
                        if (merchantList.IsNull()) continue;
                        var data = new VideoTaskDistributionModel()
                        {
                            GameType = gameType,
                            Lottery = lottery,
                            Nper = nper,
                            ZNum = znum,
                            MerchantIDList = merchantList.Select(t => t._id).ToList()
                        };
                        result.Add(data);
                        //Utils.Logger.Error(string.Format("游戏：{1}  分发开奖任务 任务号：{0}", data.UUID, gameType));
                        RabbitMQHelper.SendVideoTaskDistribution(data);
                    }
                }
                await VideoTaskRetransmission(result);
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 发送开奖任务
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="nper">期号</param>
        /// <param name="znum">桌号</param>
        /// <param name="type">类型</param>
        /// <returns></returns>
        public static async Task SendVideoGameLotteryMsg(BaccaratGameType gameType, string nper, int znum, int type)
        {
            try
            {
                await GetStatus();
                //查询商户
                MerchantOperation merchantOperation = new MerchantOperation();
                //分库
                DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
                var databases = await databaseAddressOperation.GetModelListAsync(t => true);
                if (databases.IsNull()) return; ;
                var allCount = 0;
                foreach (var database in databases)
                {
                    var condition = filter & merchantOperation.Builder.Eq(t => t.AddressID, database._id);
                    //商户总数量
                    var count = await merchantOperation.GetCountAsync(condition);
                    allCount += (int)count;
                    //一组数量
                    var pageSize = DistributionLow.limit;
                    var pageNum = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
                    for (int num = 1; num <= pageNum; num++)
                    {
                        var merchantList = merchantOperation.GetModelListByPaging(condition, t => t.CreatedTime, true, num, pageSize);
                        if (merchantList.IsNull()) continue;
                        RabbitMQHelper.SendBaccaratStartBet(merchantList.Select(t => t._id).ToList(), znum, gameType, nper, type);
                    }
                }
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 补发视讯任务
        /// </summary>
        /// <param name="result"></param>
        private static async Task VideoTaskRetransmission(List<VideoTaskDistributionModel> result)
        {
            if (result.IsNull()) return;
            DistributionOperation distributionOperation = new DistributionOperation();
            for (int i = 0; i < result.Count; i++)
            {
                var data = result[i];
                var bson = await distributionOperation.GetModelAsync(t => t._id == data.UUID);
                if (bson == null)
                {
                    //再次发送直到有服务器接收
                    RabbitMQHelper.SendVideoTaskDistribution(data);
                }
                else
                {
                    result.RemoveAt(i);
                    i--;
                }
            }
            await VideoTaskRetransmission(result);
        }
        #endregion

        #region 游戏采集
        /// <summary>
        /// 游戏对应锁定
        /// </summary>
        static Dictionary<GameOfType, object> GameLock = new Dictionary<GameOfType, object>()
        {
            { GameOfType.北京赛车, new object()},
            { GameOfType.幸运飞艇, new object()},
            { GameOfType.幸运飞艇168, new object()},
            { GameOfType.极速时时彩, new object()},
            { GameOfType.极速赛车, new object()},
            { GameOfType.澳州10, new object()},
            { GameOfType.澳州5, new object()},
            { GameOfType.爱尔兰快5, new object()},
            { GameOfType.爱尔兰赛马, new object()},
            { GameOfType.重庆时时彩, new object()},
        };

        /// <summary>
        /// 添加游戏定时器
        /// </summary>
        /// <param name="gameType"></param>
        public static void StartGameCollect(GameOfType gameType)
        {
            BackgroundJob.Schedule(() => StartGameTimer(gameType), timeSpan);
        }

        /// <summary>
        /// 开启采集
        /// </summary>
        /// <param name="gameType"></param>
        public static void StartGameTimer(GameOfType gameType)
        {
            lock (GameLock[gameType])
            {
                try
                {
                    var urls = ConfigurationManager.AppSettings[dicUrls[gameType]];
                    var tasks = new List<Task>();
                    //Utils.Logger.Error("采集数据");
                    foreach (var url in urls.Split(';'))
                    {
                        tasks.Add(Task.Run(async () =>
                        {
                            try
                            {
                                var msg = await GetAsync(url);
                                if (string.IsNullOrEmpty(msg)) return;
                                //10球
                                if (Utils.GameTypeItemize(gameType))
                                {
                                    var list = JsonConvert.DeserializeObject<List<Grasp10>>(msg).OrderByDescending(t => t.IssueNum).ToList();
                                    if (list.IsNull()) return;
                                    var lottery10Operation = new Lottery10Operation();
                                    if (string.IsNullOrEmpty(GameNper[gameType]))
                                    {
                                        var addList = new List<Lottery10>();
                                        //全部验证
                                        foreach (var data in list)
                                        {
                                            var flag = await lottery10Operation.GetModelAsync(t => t.IssueNum == data.IssueNum && t.GameType == gameType);
                                            if (flag == null)
                                            {
                                                var item = GameAlgorithms.Algorithms10<Lottery10>(data, gameType);
                                                item._id = data.IssueNum + data.AddTime.Replace("-", "").Replace(":", "").Replace(" ", "");
                                                addList.Add(item);
                                            }
                                        }
                                        if (!addList.IsNull())
                                            await lottery10Operation.InsertManyAsync(addList);
                                        GameNper[gameType] = list.First().IssueNum;
                                        return;
                                    }
                                    else
                                    {
                                        var first = list.First();
                                        if (first.IssueNum == GameNper[gameType]) return;
                                        var handleData = GameAlgorithms.Algorithms10<Lottery10>(first, gameType);
                                        handleData._id = first.IssueNum + first.AddTime.Replace("-", "").Replace(":", "").Replace(" ", "");
                                        await lottery10Operation.InsertModelAsync(handleData);
                                        Utils.Logger.Error(string.Format("添加游戏：{0} 期号：{1}", gameType, first.IssueNum));
                                        GameNper[gameType] = first.IssueNum;
                                        return;
                                    }
                                }
                                else
                                {
                                    var list = JsonConvert.DeserializeObject<List<Grasp5>>(msg).OrderByDescending(t => t.IssueNum).ToList();
                                    if (list.IsNull()) return;
                                    var lottery5Operation = new Lottery5Operation();
                                    if (string.IsNullOrEmpty(GameNper[gameType]))
                                    {
                                        var addList = new List<Lottery5>();
                                        //全部验证
                                        foreach (var data in list)
                                        {
                                            var flag = await lottery5Operation.GetModelAsync(t => t.IssueNum == data.IssueNum && t.GameType == gameType);
                                            if (flag == null)
                                            {
                                                var item = GameAlgorithms.Algorithms5<Lottery5>(data, gameType);
                                                item._id = data.IssueNum + data.AddTime.Replace("-", "").Replace(":", "").Replace(" ", "");
                                                addList.Add(item);
                                            }
                                        }
                                        if (!addList.IsNull())
                                            await lottery5Operation.InsertManyAsync(addList);
                                        GameNper[gameType] = list.First().IssueNum;
                                        return;
                                    }
                                    else
                                    {
                                        var first = list.First();
                                        if (first.IssueNum == GameNper[gameType]) return;
                                        var handleData = GameAlgorithms.Algorithms5<Lottery5>(first, gameType);
                                        handleData._id = first.IssueNum + first.AddTime.Replace("-", "").Replace(":", "").Replace(" ", "");
                                        await lottery5Operation.InsertModelAsync(handleData);
                                        Utils.Logger.Error(string.Format("添加游戏：{0} 期号：{1}", gameType, first.IssueNum));
                                        GameNper[gameType] = first.IssueNum;
                                        return;
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                return;
                            }
                        }));
                    }
                    Task.WaitAny(tasks.ToArray());
                }
                catch (Exception e)
                {
                    Utils.Logger.Error(string.Format("游戏：{0}  异常：{1}", gameType, e));
                }
            }
            BackgroundJob.Schedule(() => StartGameTimer(gameType), timeSpan);
        }

        /// <summary>
        /// 开启游戏
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        public static async Task GameStart(GameOfType gameType)
        {
            //Utils.Logger.Error("开启下期游戏");
            var gameInfo = await GetGameNextLottery(gameType);
            if (gameInfo == null || gameInfo.DayNum == 0)
            {
                BackgroundJob.Schedule(() => GameStart(gameType), TimeSpan.FromMinutes(3));
                return;
            }
            if (gameInfo.StartTime > DateTime.Now && gameType != GameOfType.极速赛车)
            {
                if ((gameInfo.StartTime - DateTime.Now).TotalSeconds <= 20)
                {
                    BackgroundJob.Schedule(() => GameStart(gameType), TimeSpan.FromSeconds(3));
                    return;
                }
            }
            if (Utils.GameTypeItemize(gameType))
            {
                Lottery10Operation lottery10Operation = new Lottery10Operation();
                var result = await lottery10Operation.GetModelAsync(t => t.IssueNum == gameInfo.NextNper && t.GameType == gameType);
                if (result != null)
                {
                    BackgroundJob.Schedule(() => GameStart(gameType), TimeSpan.FromSeconds(10));
                    return;
                }
            }
            else
            {
                Lottery5Operation lottery5Operation = new Lottery5Operation();
                var result = await lottery5Operation.GetModelAsync(t => t.IssueNum == gameInfo.NextNper && t.GameType == gameType);
                if (result != null)
                {
                    BackgroundJob.Schedule(() => GameStart(gameType), TimeSpan.FromSeconds(10));
                    return;
                }
            }
            //添加到redis
            RedisOperation.SetHash("GameStatus", Enum.GetName(typeof(GameOfType), gameType), JsonConvert.SerializeObject(gameInfo));
            var waitTime = gameInfo.StartTime.AddSeconds(-1);
            var waitSpan = waitTime > DateTime.Now ? waitTime - DateTime.Now :
                timeSpan;
            Utils.Logger.Error(string.Format("游戏：{0}  期号：{2} 将后在{1}开始采集", gameType, DateTime.Now.Add(waitSpan), gameInfo.NextNper));
            //开启游戏采集
            var jobID = BackgroundJob.Schedule(() => GetGrasp(dicUrls[gameType], gameInfo, gameType, 0, null), waitSpan);
            GameJobID[gameType] = jobID;
        }

        /// <summary>
        /// 采集进程
        /// </summary>
        static readonly Dictionary<GameOfType, string> GameNper = new Dictionary<GameOfType, string>()
        {
            { GameOfType.北京赛车, null},
            { GameOfType.幸运飞艇, null},
            { GameOfType.幸运飞艇168, null},
            { GameOfType.极速时时彩, null},
            { GameOfType.极速赛车, null},
            { GameOfType.澳州10, null},
            { GameOfType.澳州5, null},
            { GameOfType.爱尔兰快5, null},
            { GameOfType.爱尔兰赛马, null},
            { GameOfType.重庆时时彩, null},
        };

        /// <summary>
        /// 游戏任务id
        /// </summary>
        static readonly Dictionary<GameOfType, string> GameJobID = new Dictionary<GameOfType, string>()
        {
            { GameOfType.北京赛车, null},
            { GameOfType.幸运飞艇, null},
            { GameOfType.幸运飞艇168, null},
            { GameOfType.极速时时彩, null},
            { GameOfType.极速赛车, null},
            { GameOfType.澳州10, null},
            { GameOfType.澳州5, null},
            { GameOfType.爱尔兰快5, null},
            { GameOfType.爱尔兰赛马, null},
            { GameOfType.重庆时时彩, null},
        };

        /// <summary>
        /// 获取采集游戏信息
        /// </summary>
        /// <param name="urlName"></param>
        /// <param name="lotteryInfo"></param>
        /// <param name="gameType"></param>
        /// <param name="errorCount"></param>
        /// <param name="context">传空 hangfire会自动识别</param>
        /// <returns></returns>
        public static async Task GetGrasp(string urlName, GameNextLottery lotteryInfo, GameOfType gameType, int errorCount, PerformContext context)
        {
            var jobID = context.BackgroundJob.Id;
            //Utils.Logger.Error(string.Format("游戏：{0} 任务号：{1}", gameType, jobID));
            Utils.Logger.Error(string.Format("游戏：{0} 期号：{2} 第{1}次采集", gameType, errorCount + 1, lotteryInfo.NextNper));
            if (jobID != GameJobID[gameType]) return;
            try
            {
                if (lotteryInfo.ExpirationDate < DateTime.Now)
                {
                    await GameStart(gameType);
                    await DataRefresh(gameType);
                    return;
                }
                //10球游戏
                if (Utils.GameTypeItemize(gameType))
                {
                    Lottery10Operation lottery10Operation = new Lottery10Operation();
                    //获取当期数据
                    var result = await lottery10Operation.GetModelAsync(t => t.IssueNum == lotteryInfo.NextNper && t.GameType == gameType);
                    if (result == null)
                    {
                        ++errorCount;
                        await Task.Delay(1000);
                        jobID = BackgroundJob.Enqueue(() => GetGrasp(urlName, lotteryInfo, gameType, errorCount, null));
                        GameJobID[gameType] = jobID;
                        return;
                    }
                    else
                    {
                        Utils.Logger.Error(string.Format("游戏：{0} 期号：{1} 采集成功", gameType, lotteryInfo.NextNper));
                        await GameStart(gameType);
                        var history = SealupMessage.GetGameHistoryTop(gameType);
                        RedisOperation.SetHash("Gamehistory", Enum.GetName(typeof(GameOfType), gameType), history);
                        BackgroundJob.Enqueue(() => GameLottery(gameType, result.IssueNum, JsonConvert.SerializeObject(result)));
                    }
                }
                //5球游戏
                else
                {
                    Lottery5Operation lottery5Operation = new Lottery5Operation();
                    //获取当期数据
                    var result = await lottery5Operation.GetModelAsync(t => t.IssueNum == lotteryInfo.NextNper && t.GameType == gameType);
                    if (result == null)
                    {
                        ++errorCount;
                        await Task.Delay(1000);
                        jobID = BackgroundJob.Enqueue(() => GetGrasp(urlName, lotteryInfo, gameType, errorCount, null));
                        GameJobID[gameType] = jobID;
                        return;
                    }
                    else
                    {
                        Utils.Logger.Error(string.Format("游戏：{0} 期号：{1} 采集成功", gameType, lotteryInfo.NextNper));
                        await GameStart(gameType);
                        var history = SealupMessage.GetGameHistoryTop(gameType);
                        RedisOperation.SetHash("Gamehistory", Enum.GetName(typeof(GameOfType), gameType), history);
                        BackgroundJob.Enqueue(() => GameLottery(gameType, result.IssueNum, JsonConvert.SerializeObject(result)));
                    }
                }
            }
            catch (Exception e)
            {
                Utils.Logger.Error(string.Format("游戏：{0} 期号：{1} 异常：{2}", gameType, lotteryInfo.NextNper, e));
                await GameStart(gameType);
                return;
            }
        }

        /// <summary>
        /// 发送游戏消息
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="userConns"></param>
        /// <param name="addressID"></param>
        /// <returns></returns>
        public static async Task SendGameMsg(string merchantID, GameOfType gameType, List<string> userConns, string addressID)
        {
            UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(addressID);
            var collection = userSendMessageOperation.GetCollection(merchantID);
            var list = new List<UserSendMessage>();
            var url = RedisOperation.GetAdminPortrait(merchantID);

            var message = RedisOperation.GetValue("GameWinMessage", merchantID + Enum.GetName(typeof(GameOfType), gameType));
            var bill = RedisOperation.GetValue("GameBill", merchantID + Enum.GetName(typeof(GameOfType), gameType));
            var history = RedisOperation.GetValue("Gamehistory", Enum.GetName(typeof(GameOfType), gameType));

            list.Add(new UserSendMessage()
            {
                Avatar = url,
                MerchantID = merchantID,
                Message = message,
                NickName = "管理员",
                UserType = UserEnum.管理员,
                GameType = gameType
            });
            list.Add(new UserSendMessage()
            {
                Avatar = url,
                MerchantID = merchantID,
                Message = bill,
                NickName = "管理员",
                UserType = UserEnum.管理员,
                GameType = gameType
            });
            list.Add(new UserSendMessage()
            {
                Avatar = url,
                MerchantID = merchantID,
                Message = history,
                NickName = "管理员",
                UserType = UserEnum.管理员,
                GameType = gameType
            });
            await collection.InsertManyAsync(list);
            if (userConns.IsNull()) return;
            var task = RabbitMQHelper.SendAdminMessage(message, merchantID, gameType, true, userConns);
            await task.ContinueWith(async job =>
                {
                    await RabbitMQHelper.SendAdminMessage(bill, merchantID, gameType, true, userConns);
                    await RabbitMQHelper.SendAdminMessage(history, merchantID, gameType, true, userConns);
                });
        }
        #endregion

        #region 推送
        /// <summary>
        /// 开启游戏状态实时刷新
        /// </summary>
        /// <returns></returns>
        public static async Task DataRefresh(GameOfType? gameTypePre = null)
        {
            #region 查询各个商户游戏状态
            await GetStatus();
            //查询商户
            MerchantOperation merchantOperation = new MerchantOperation();
            //分库
            DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
            var databases = await databaseAddressOperation.GetModelListAsync(t => true);
            if (databases.IsNull()) return;
            var result = new List<TaskDistributionModel>();
            var allCount = 0;
            List<Task> tasks = new List<Task>();
            SemaphoreSlim Lock = new SemaphoreSlim(DistributionLow.limit, DistributionLow.limit);
            var dic = EnumToDictionary(typeof(GameOfType));
            var gameStatusDic = new Dictionary<GameOfType, GameNextLottery>();
            foreach (var item in dic)
            {
                var gameType = GetEnumByStatus<GameOfType>(item.Value);
                var status = RedisOperation.GetValue<GameNextLottery>("GameStatus", Enum.GetName(typeof(GameOfType), gameType));
                gameStatusDic.Add(gameType, status);
            }
            foreach (var database in databases)
            {
                var condition = filter & merchantOperation.Builder.Eq(t => t.AddressID, database._id);
                //商户总数量
                var count = await merchantOperation.GetCountAsync(condition);
                allCount += (int)count;
                //一组数量
                var pageSize = DistributionLow.limit;
                var pageNum = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
                for (int num = 1; num <= pageNum; num++)
                {
                    var merchantList = merchantOperation.GetModelListByPaging(condition, t => t.CreatedTime, true, num, pageSize);
                    if (merchantList.IsNull()) continue;
                    foreach (var merchant in merchantList)
                    {
                        try
                        {
                            await Lock.WaitAsync();
                            if (gameTypePre == null)
                            {
                                foreach (var item in dic)
                                {
                                    var gameType = GetEnumByStatus<GameOfType>(item.Value);
                                    tasks.Add(GameTypeAsync(merchant._id, gameType, gameStatusDic[gameType]));
                                }
                            }
                            else
                            {
                                tasks.Add(GameTypeAsync(merchant._id, gameTypePre.Value, gameStatusDic[gameTypePre.Value]));
                            }
                        }
                        catch { }
                        finally
                        {
                            Lock.Release();
                        }
                    }

                    await Task.WhenAll(tasks.ToArray());
                    Lock.Dispose();
                }
            }
            #endregion
        }

        /// <summary>
        /// 推送各个商户游戏状态消息
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="gameStatus"></param>
        /// <returns></returns>
        public static async Task GameTypeAsync(string merchantID, GameOfType gameType, GameNextLottery gameStatus)
        {
            try
            {
                var foundation = RedisOperation.GetFoundationSetup(merchantID);
                var result = await GameDiscrimination.EachpartAsync(gameType, merchantID, gameStatus);

                //查询当前在游戏房间用户
                RedisOperation.SetHash("MerchantGameStatus", merchantID + Enum.GetName(typeof(GameOfType), gameType), JsonConvert.SerializeObject(result));
                //先发送游戏状态
                var userConns = await Utils.GetRoomGameConnIDs(merchantID, gameType);
                await SendListMessage(JsonConvert.SerializeObject(result), merchantID, gameType, userConns);
                //var monitor = await GameHandle.GetMonitorInfos(result, merchantID);
                //await SendMonitorMessage(JsonConvert.SerializeObject(monitor), merchantID);

                //查询当期已开奖
                var address = await Utils.GetAddress(merchantID);
                await SendGameMsg(merchantID, gameType, userConns, address);

                switch (result.Status)
                {
                    #region
                    case GameStatusEnum.等待中:
                        //封盘消息
                        var jobID = BackgroundJob.Schedule(() => SendAdminMessage(foundation.LotteryFrontMsg, merchantID, gameType), TimeSpan.FromSeconds(result.Surplus));
                        //await SendAdminMessage(foundation.LotteryFrontMsg, merchantID, gameType, result.Surplus);
                        //封盘核对
                        BackgroundJob.ContinueJobWith(jobID, () => SealUpCheckMsg(merchantID, gameType, foundation.EntertainedAfterMsg, result.NextIssueNum));
                        //BackgroundJob.Schedule(() => SealUpCheckMsg(merchantID, gameType, foundation.EntertainedAfterMsg, result.NextIssueNum), TimeSpan.FromSeconds(result.Surplus + foundation.EntertainedAfterTime));
                        //await SealUpCheckMsg(merchantID, gameType, foundation.EntertainedAfterMsg, result.NextIssueNum, result.Surplus + foundation.EntertainedAfterTime);
                        //自定义消息
                        //await SendAdminMessage(foundation.CustomMsg, merchantID, gameType, foundation.CustomTime + result.Surplus);
                        BackgroundJob.Schedule(() => SendAdminMessage(foundation.CustomMsg, merchantID, gameType), TimeSpan.FromSeconds(foundation.CustomTime + result.Surplus));

                        //封盘前消息
                        var time = foundation.ProhibitChe ? (result.Surplus - foundation.EntertainedFrontTime) > 0 ? result.Surplus - foundation.EntertainedFrontTime : 0 : result.Surplus;
                        if (result.Surplus >= foundation.EntertainedFrontTime)
                        {
                            BackgroundJob.Schedule(() => SendAdminMessage(foundation.EntertainedFrontMsg, merchantID, gameType), TimeSpan.FromSeconds(result.Surplus - foundation.EntertainedFrontTime));
                            //await SendAdminMessage(foundation.EntertainedFrontMsg, merchantID, gameType, result.Surplus - foundation.EntertainedFrontTime);
                        }
                        else
                        {
                            var message = SealedTransformation(gameType, foundation.EntertainedFrontMsg);
                            await RabbitMQHelper.SendAdminMessage(message, merchantID, gameType);
                        }
                        //飞单
                        BackgroundJob.Schedule(() => FlyingTask(merchantID, gameType, result.NextIssueNum), TimeSpan.FromSeconds(time));
                        //await FlyingTask(merchantID, gameType, result.NextIssueNum, time);
                        //假人发送下注
                        await NewShamUserSendBetInfo(merchantID, result);
                        break;
                    case GameStatusEnum.封盘中:

                        break;
                    case GameStatusEnum.已停售:
                        //机器人清分
                        await IncomeDistribution(merchantID, result);
                        //var status = RedisOperation.GetValue<GameNextLottery>("GameStatus", Enum.GetName(typeof(GameOfType), gameType));
                        //if (status == null) return;
                        //if (status.DayNum > 0)
                        //    BackgroundJob.Schedule(() => GameTypeAsync(merchantID, gameType, status),
                        //        status.StartTime - DateTime.Now.AddSeconds(status.Interval));
                        break;
                    default:
                        break;
                        #endregion
                };

                //清理消息数据
                UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
                var msgcollection = userSendMessageOperation.GetCollection(merchantID);
                await msgcollection.DeleteManyAsync(t => t.CreatedTime < DateTime.Now.AddHours(-1) && t.GameType == gameType);
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
        }
        #region 消息
        /// <summary>
        /// 推送消息
        /// </summary>
        /// <param name="Message"></param>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="userConns"></param>
        public static async Task SendListMessage(string Message, string merchantID, GameOfType gameType, List<string> userConns)
        {
            //Utils.Logger.Error(string.Format("游戏：{0}  信息：{1}", gameType, Message));
            await RabbitMQHelper.SendOverallMessage(Message, merchantID, "SendListMessage");
            //await RabbitMQHelper.SendBackstageMessage(merchantID, Message, "BackstageGameInfo");
            await RabbitMQHelper.SendGameMessage(Message, merchantID, "SendRoomMessage", gameType, userConns: userConns);
        }

        /// <summary>
        /// 向管理后台发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task SendMonitorMessage(string message, string merchantID)
        {
            await RabbitMQHelper.SendOverallMessage(message, merchantID, "SendMonitorInfo");
        }

        /// <summary>
        /// 发送管理员消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public static async Task SendAdminMessage(string message, string merchantID, GameOfType gameType)
        {
            message = Utils.SealedTransformation(gameType, message);
            await RabbitMQHelper.SendAdminMessage(message, merchantID, gameType);
            //var key = Guid.NewGuid().ToString();
            //var jobName = key + Enum.GetName(typeof(GameOfType), gameType);
            //NameValueCollection properties = new NameValueCollection
            //{
            //    [StdSchedulerFactory.PropertySchedulerInstanceName] = jobName
            //};
            ////1、通过调度工厂获得调度器
            //StdSchedulerFactory fac = new StdSchedulerFactory(properties);
            //var scheduler = await fac.GetScheduler();
            ////2、开启调度器
            //await scheduler.Start();
            //IDictionary<string, object> dic = new Dictionary<string, object>
            //{
            //    { "merchantID", merchantID },
            //    { "message", message },
            //    { "gameType", gameType }
            //};
            //var jobDetail = JobBuilder.Create<SendAdminMessageJob>()
            //                .UsingJobData(new JobDataMap(dic))
            //                .WithIdentity(key, "SendAdminMessage")
            //                .Build();
            //var trigger = (ISimpleTrigger)TriggerBuilder.Create()
            //.WithIdentity(key)
            //.StartAt(DateBuilder.FutureDate(time, IntervalUnit.Second))
            //.ForJob(jobDetail)
            //.Build();

            ////5、将触发器和任务器绑定到调度器中
            //await scheduler.ScheduleJob(jobDetail, trigger);
        }

        /// <summary>
        /// 封盘确认
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="message"></param>
        /// <param name="nper"></param>
        /// <returns></returns>
        public static async Task SealUpCheckMsg(string merchantID, GameOfType gameType, string message, string nper)
        {
            var checkMsg = await SealupMessage.GetAllBetsAsync(merchantID, gameType, message, nper);
            await RabbitMQHelper.SendAdminMessage(checkMsg, merchantID, gameType);
            //var key = Guid.NewGuid().ToString();
            //var jobName = key + Enum.GetName(typeof(GameOfType), gameType);
            //NameValueCollection properties = new NameValueCollection
            //{
            //    [StdSchedulerFactory.PropertySchedulerInstanceName] = jobName
            //};
            ////1、通过调度工厂获得调度器
            //StdSchedulerFactory fac = new StdSchedulerFactory(properties);
            //var scheduler = await fac.GetScheduler();
            ////2、开启调度器
            //await scheduler.Start();
            //var jobKey = Guid.NewGuid().ToString();
            //IDictionary<string, object> dic = new Dictionary<string, object>
            //{
            //    { "merchantID", merchantID },
            //    { "gameType", gameType },
            //    { "msg", message },
            //    { "nper", nper }
            //};
            //var jobDetail = JobBuilder.Create<GameBetConfirmationJob>()
            //                .UsingJobData(new JobDataMap(dic))
            //                .WithIdentity(jobKey, "SealUpCheckMsg")
            //                .Build();
            //var trigger = (ISimpleTrigger)TriggerBuilder.Create()
            //.WithIdentity(jobKey, "SealUpCheckMsg")
            //.StartAt(DateBuilder.FutureDate(time, IntervalUnit.Second))
            //.ForJob(jobDetail)
            //.Build();

            ////5、将触发器和任务器绑定到调度器中
            //await scheduler.ScheduleJob(jobDetail, trigger);
        }

        /// <summary>
        /// 飞单任务
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="nper">期号</param>
        /// <returns></returns>
        public static async Task FlyingTask(string merchantID, GameOfType gameType, string nper)
        {
            var result = await FlyingSheet.FlyingSheetMethod(merchantID, nper, gameType);
            await RabbitMQHelper.SendFlyingSheet(merchantID, gameType, nper, result, 0);
            //var key = Guid.NewGuid().ToString();
            //var jobName = key + Enum.GetName(typeof(GameOfType), gameType);
            //NameValueCollection properties = new NameValueCollection
            //{
            //    [StdSchedulerFactory.PropertySchedulerInstanceName] = jobName
            //};
            ////1、通过调度工厂获得调度器
            //StdSchedulerFactory fac = new StdSchedulerFactory(properties);
            //var scheduler = await fac.GetScheduler();
            ////2、开启调度器
            //await scheduler.Start();
            //var jobKey = Guid.NewGuid().ToString();
            //IDictionary<string, object> dic = new Dictionary<string, object>
            //{
            //    { "merchantID", merchantID },
            //    { "gameType", gameType },
            //    { "nper", nper }
            //};
            //var jobDetail = JobBuilder.Create<FlyingTaskJob>()
            //                .UsingJobData(new JobDataMap(dic))
            //                .WithIdentity(jobKey, "FlyingTask")
            //                .Build();
            //var trigger = (ISimpleTrigger)TriggerBuilder.Create()
            //.WithIdentity(jobKey, "FlyingTask")
            //.StartAt(DateBuilder.FutureDate(time, IntervalUnit.Second))
            //.ForJob(jobDetail)
            //.Build();

            ////5、将触发器和任务器绑定到调度器中
            //await scheduler.ScheduleJob(jobDetail, trigger);
        }
        #endregion
        #endregion

        #region 其它 
        /// <summary>
        /// 其它事件
        /// </summary>
        /// <returns></returns>
        public static async Task OtherThinks()
        {
            ErrorRecordOperation errorRecordOperation = new ErrorRecordOperation();
            await errorRecordOperation.DeleteModelManyAsync(t => t.CreatedTime <= DateTime.Now.AddDays(-7));
            DistributionOperation distributionOperation = new DistributionOperation();
            await distributionOperation.DeleteModelManyAsync(t => t.CreatedTime <= DateTime.Now.AddHours(-1));
            SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
            await sendFlyingOperation.DeleteModelManyAsync(t => t.CreatedTime <= DateTime.Now.AddDays(-7));

            //删除15天之前下注信息
            MerchantOperation merchantOperation = new MerchantOperation();
            DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
            var databases = await databaseAddressOperation.GetModelListAsync(t => true);
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            foreach (var database in databases)
            {
                var merchantList = await merchantOperation.GetModelListAsync(merchantOperation.Builder.Eq(t => t.AddressID, database._id));
                foreach (var merchant in merchantList)
                {
                    UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(merchant.AddressID);
                    var collection = userBetInfoOperation.GetCollection(merchant._id);
                    var filter = userBetInfoOperation.Builder.Where(t => t.Notes == NotesEnum.正常 && t.CreatedTime <= DateTime.Today.AddDays(-35));
                    await collection.DeleteManyAsync(filter);

                    filter = userBetInfoOperation.Builder.Where(t => t.Notes == NotesEnum.虚拟 && t.CreatedTime <= DateTime.Today.AddDays(-3));
                    await collection.DeleteManyAsync(filter);

                    BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(merchant.AddressID);
                    var bcollection = baccaratBetOperation.GetCollection(merchant._id);
                    var bfilter = baccaratBetOperation.Builder.Where(t => t.Notes == NotesEnum.正常 && t.CreatedTime <= DateTime.Today.AddDays(-35));
                    await bcollection.DeleteManyAsync(bfilter);

                    bfilter = baccaratBetOperation.Builder.Where(t => t.Notes == NotesEnum.虚拟 && t.CreatedTime <= DateTime.Today.AddDays(-3));
                    await bcollection.DeleteManyAsync(bfilter);

                    //查询机器人
                    var robotList = await shamRobotOperation.GetModelListAsync(t => t.MerchantID == merchant._id);
                    await userIntegrationOperation.DeleteModelManyAsync(userIntegrationOperation.Builder.Where(t => t.MerchantID == merchant._id
                     && t.CreatedTime <= DateTime.Now.AddDays(-3)) & userIntegrationOperation.Builder.In(t => t.UserID, robotList.Select(t => t.UserID).ToList()));
                }
            }
            BackgroundJob.Schedule(() => OtherThinks(), TimeSpan.FromDays(1));
            #region
            //PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            //var setup = await platformSetUpOperation.GetModelAsync(t => t._id != "");
            ////自动设置最新期号和开奖时间
            //if (setup == null) return;
            //Lottery10Operation lottery10Operation = new Lottery10Operation();
            //GameOfType[] game10Item = { GameOfType.北京赛车, GameOfType.幸运飞艇, GameOfType.极速赛车, GameOfType.澳州10, GameOfType.爱尔兰赛马 };
            //foreach (var gameType in game10Item)
            //{
            //    var lottery = lottery10Operation.GetModel(t => t.GameType == gameType && t.CreatedTime <= DateTime.Today, t => t.IssueNum, false);
            //    if (lottery != null)
            //    {
            //        var gameSetup = setup.GameBasicsSetups.Find(t => t.GameType == gameType);
            //        if (gameSetup != null)
            //        {
            //            setup.GameBasicsSetups.ForEach(t =>
            //            {
            //                if (t.GameType == gameType)
            //                {
            //                    t.FirstNper = lottery.IssueNum;
            //                    t.StartTime = Convert.ToDateTime(lottery.LotteryTime);
            //                }
            //            });
            //        }
            //    }
            //}
            //Lottery5Operation lottery5Operation = new Lottery5Operation();
            //GameOfType[] game5Item = { GameOfType.重庆时时彩, GameOfType.爱尔兰快5, GameOfType.澳州5 };
            //foreach (var gameType in game5Item)
            //{
            //    var lottery = lottery5Operation.GetModel(t => t.GameType == gameType && t.CreatedTime <= DateTime.Today, t => t.IssueNum, false);
            //    if (lottery != null)
            //    {
            //        var gameSetup = setup.GameBasicsSetups.Find(t => t.GameType == gameType);
            //        if (gameSetup != null)
            //        {
            //            setup.GameBasicsSetups.ForEach(t =>
            //            {
            //                if (t.GameType == gameType)
            //                {
            //                    t.FirstNper = lottery.IssueNum;
            //                    t.StartTime = Convert.ToDateTime(lottery.LotteryTime);
            //                }
            //            });
            //        }
            //    }
            //}
            //await platformSetUpOperation.UpdateModelAsync(setup);
            #endregion
        }
        #endregion

        #region 假人
        /// <summary>
        /// 假人
        /// </summary>
        /// <param name="merchantID">商户号</param>
        /// <param name="gameInfos">游戏信息</param>
        /// <returns></returns>
        public static async Task NewShamUserSendBetInfo(string merchantID, WebAppGameInfos gameInfos)
        {
            var gameType = gameInfos.GameType;
            var nper = gameInfos.NextIssueNum;
            Random random = new Random();
            //查询所有虚假用户和设置
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID && t.Status == 1);
            if (merchant == null) return;
            if (merchant.OnLineTime == null) return;
            var tips = merchant.LoginTime == null ? false : (DateTime.Now - merchant.LoginTime.Value).TotalDays >= 1 ? true : false;
            if (tips) return;
            UserOperation userOperation = new UserOperation();
            var allShamUser = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.假人);
            if (allShamUser.IsNull()) return;
            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (reply == null) return;
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            RobotBehaviorOperation robotBehaviorOperation = new RobotBehaviorOperation();
            RobotProgramOperation robotProgramOperation = new RobotProgramOperation();
            var address = await GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var tasks = new List<Task>();
            foreach (var user in allShamUser)
            {
                var task = Task.Run(async () =>
                {
                    try
                    {
                        #region 
                        //查询机器人设置
                        var shamInfo = await shamRobotOperation.GetModelAsync(t => t.UserID == user._id && t.MerchantID == merchantID);
                        if (shamInfo == null || !shamInfo.Check) return;
                        //查看游戏是否开启
                        var gameInfo = shamInfo.GameCheckInfo.Find(t => t.GameType == gameType && t.Check);
                        if (gameInfo == null) return;
                        //未绑定行为
                        if (string.IsNullOrEmpty(gameInfo.BehaviorID)) return;
                        //查询行为
                        var behavior = await robotBehaviorOperation.GetModelAsync(t => t._id == gameInfo.BehaviorID && t.MerchantID == merchantID);
                        if (behavior == null) return;
                        //刷新用户余额
                        var amount = await userOperation.GetUserMoney(merchantID, user._id);
                        //是否勾选停止下注
                        if (behavior.StopCmd.Check && amount < behavior.StopCmd.Limit)
                            return;
                        //上分
                        if (behavior.UpCmd.Check && amount < behavior.UpCmd.Limit)
                        {
                            var time = random.Next(1, 20);
                            var jobID = BackgroundJob.Schedule(() => RabbitMQHelper.SendUserMessage(string.Format("{0}{1}", behavior.UpCmd.Keyword, (int)behavior.UpCmd.Variety), user._id, merchantID, gameType), TimeSpan.FromSeconds(time));

                            if (behavior.UpCmd.Keyword == "上分")
                            {
                                //上分提示
                                if (reply.NoticeCheckRequest)
                                {
                                    var result = await InstructionConversion(reply.ReceivingRequests, user._id, merchantID, gameType, nper);
                                    jobID = BackgroundJob.ContinueJobWith(jobID, () => RabbitMQHelper.SendAdminMessage(result, merchantID, gameType, false, null));
                                }
                                BackgroundJob.ContinueJobWith(jobID, () => UpDivisionHandle(user, merchantID, behavior, reply, gameType, nper, amount, TimeSpan.FromSeconds(random.Next(5, 20))));
                            }
                        }
                        //下分
                        if (behavior.DownCmd.Check && amount > behavior.DownCmd.Limit)
                        {
                            var time = random.Next(1, 20);
                            var jobID = BackgroundJob.Schedule(() => RabbitMQHelper.SendUserMessage(string.Format("{0}{1}", behavior.DownCmd.Keyword, (int)behavior.DownCmd.Variety), user._id, merchantID, gameType), TimeSpan.FromSeconds(time));
                            if (behavior.DownCmd.Keyword == "下分")
                            {
                                if (reply.NoticeCheckRequest)
                                {
                                    var result = await Utils.InstructionConversion(reply.ReceivingRequests, user._id, merchantID, gameType, nper);
                                    jobID = BackgroundJob.ContinueJobWith(jobID, () => RabbitMQHelper.SendAdminMessage(result, merchantID, gameType, false, null));
                                }
                                BackgroundJob.ContinueJobWith(jobID, () => DownDivisionHandle(user._id, merchantID, reply, gameType, behavior.DownCmd.Variety, TimeSpan.FromSeconds(random.Next(5, 20))));
                            }
                        }

                        //攻击查询
                        if (behavior.AttackQuery.Check && behavior.AttackQuery.StartTime <= behavior.AttackQuery.EndTime)
                        {
                            var win = WinningBid(behavior.AttackQuery.Probability);
                            if (win)
                            {
                                var num = random.Next(behavior.AttackQuery.StartTime, behavior.AttackQuery.EndTime);
                                if (num > gameInfos.Surplus) return;
                                BackgroundJob.Schedule(() => SendShamUserMessage(behavior.AttackQuery.Keyword, merchantID, user._id, gameType), TimeSpan.FromSeconds(num));
                            }
                        }

                        //停战查询
                        if (behavior.ArmisticeQuery.Check && behavior.ArmisticeQuery.StartTime <= behavior.ArmisticeQuery.EndTime)
                        {
                            var win = WinningBid(behavior.ArmisticeQuery.Probability);
                            if (win)
                            {
                                if (behavior.ArmisticeQuery.StartTime > behavior.ArmisticeQuery.EndTime)
                                    return;
                                var num = random.Next(behavior.ArmisticeQuery.StartTime + gameInfos.Surplus, behavior.ArmisticeQuery.EndTime + gameInfos.Surplus);
                                if (num > gameInfos.SealingTime) return;
                                BackgroundJob.Schedule(() => SendShamUserMessage(behavior.ArmisticeQuery.Keyword, merchantID, user._id, gameType), TimeSpan.FromSeconds(num));
                            }
                        }

                        if (behavior.Attack.Check)
                        {
                            //游戏下注  查找方案
                            var programList = await robotProgramOperation.GetModelListAsync(t => t.BehaviorID == behavior._id && t.MerchantID == merchantID);
                            foreach (var program in programList)
                            {
                                //是否开启
                                if (program.IsEnable && behavior.Attack.StartTime <= behavior.Attack.EndTime)
                                {
                                    if (string.IsNullOrEmpty(program.Amountset)) continue;
                                    //金额
                                    var amountSplit = program.Amountset.Split(',').ToList();
                                    if (amountSplit.IsNull()) continue;
                                    var allAmount = amountSplit.Select(t => Convert.ToDecimal(t)).ToList();
                                    var useAmount = allAmount[random.Next(0, allAmount.Count)];
                                    //是否翻倍
                                    if (program.DoubleType != DoubleEnum.不翻倍)
                                    {
                                        var maxAmount = allAmount.Max();
                                        var preNper = GameHandle.GetGamePreNper(nper, gameType);
                                        //上期是否有中奖
                                        var betWin = await collection.FindListAsync(t => t.Nper == preNper
                                        && t.MerchantID == merchantID && t.UserID == user._id && t.BetStatus == BetStatus.已开奖);
                                        bool isWin = betWin.Exists(t => t.BetRemarks.Exists(x => x.OddBets.Exists(y => y.BetStatus == BetStatusEnum.已中奖)));
                                        if ((isWin && program.DoubleType == DoubleEnum.中翻倍) || (!isWin && program.DoubleType == DoubleEnum.不中翻倍))
                                            useAmount = useAmount * 2 > maxAmount ? maxAmount : useAmount * 2;
                                    }

                                    if (program.BetTypeList.IsNull()) continue;
                                    //下注信息
                                    var betInfo = program.BetTypeList[random.Next(0, program.BetTypeList.Count)];
                                    var message = betInfo.Replace("{m}", useAmount.ToString());

                                    //下注时间
                                    var num = random.Next(behavior.Attack.StartTime, behavior.Attack.EndTime);
                                    if (num > gameInfos.Surplus) continue;
                                    BackgroundJob.Schedule(() => SendShamUserMessage(message, merchantID, user._id, gameType), TimeSpan.FromSeconds(num));
                                }
                            }
                        }
                        #endregion
                    }
                    catch (Exception e)
                    {
                        Utils.Logger.Error(e);
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        /// 清分
        /// </summary>
        /// <param name="merchantID">商户Id</param>
        /// <param name="gameInfos"></param>
        /// <returns></returns>
        public static async Task IncomeDistribution(string merchantID, WebAppGameInfos gameInfos)
        {
            Random random = new Random();
            //查询所有虚假用户和设置
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID && t.Status == 1);
            if (merchant == null) return;
            if (merchant.OnLineTime == null) return;
            var tips = merchant.LoginTime == null ? false : (DateTime.Now - merchant.LoginTime.Value).TotalDays >= 1 ? true : false;
            if (tips) return;
            UserOperation userOperation = new UserOperation();
            var allShamUser = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.假人);
            if (allShamUser.IsNull()) return;
            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (reply == null) return;
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            RobotBehaviorOperation robotBehaviorOperation = new RobotBehaviorOperation();
            RobotProgramOperation robotProgramOperation = new RobotProgramOperation();
            var address = await GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var tasks = new List<Task>();
            foreach (var user in allShamUser)
            {
                var task = Task.Run(async () =>
                {
                    #region 
                    //查询机器人设置
                    var shamInfo = await shamRobotOperation.GetModelAsync(t => t.UserID == user._id && t.MerchantID == merchantID);
                    if (shamInfo == null || !shamInfo.Check) return;
                    //查看游戏是否开启
                    var gameInfo = shamInfo.GameCheckInfo.Find(t => t.GameType == gameInfos.GameType && t.Check);
                    if (gameInfo == null) return;
                    //未绑定行为
                    if (string.IsNullOrEmpty(gameInfo.BehaviorID)) return;
                    //查询行为
                    var behavior = await robotBehaviorOperation.GetModelAsync(t => t._id == gameInfo.BehaviorID && t.MerchantID == merchantID);
                    if (behavior == null) return;
                    //刷新用户余额
                    var amount = (int)(await userOperation.GetUserMoney(merchantID, user._id));
                    if (amount == 0) return;
                    if (behavior.EndPoint)
                    {
                        var time = random.Next(60, 300);
                        var jobID = BackgroundJob.Schedule(() => RabbitMQHelper.SendUserMessage(string.Format("{0}{1}", "下分", amount), user._id, merchantID, gameInfos.GameType), TimeSpan.FromSeconds(time));
                        var result = await Utils.InstructionConversion(reply.ReceivingRequests, user._id, merchantID, gameInfos.GameType, gameInfos.NextIssueNum);
                        jobID = BackgroundJob.ContinueJobWith(jobID, () => RabbitMQHelper.SendAdminMessage(result, merchantID, gameInfos.GameType, false, null));
                        BackgroundJob.ContinueJobWith(jobID, () => DownDivisionHandle(user._id, merchantID, reply, gameInfo.GameType, amount, TimeSpan.FromSeconds(random.Next(5, 20))));
                    }
                    #endregion
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks.ToArray());
        }

        /// <summary>
        /// 假人上分
        /// </summary>
        /// <param name="user"></param>
        /// <param name="merchantID"></param>
        /// <param name="behavior"></param>
        /// <param name="reply"></param>
        /// <param name="gameType"></param>
        /// <param name="nper"></param>
        /// <param name="amount"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static async Task UpDivisionHandle(User user, string merchantID, RobotBehavior behavior, ReplySetUp reply, GameOfType gameType, string nper, decimal amount, TimeSpan span)
        {
            await Task.Delay(span);
            UserOperation userOperation = new UserOperation();
            await userOperation.UpperScore(user._id, merchantID,
                                    (int)behavior.UpCmd.Variety,
                                    ChangeTargetEnum.系统,
                                    msg: "系统上分" + (int)behavior.UpCmd.Variety,
                                    remark: "系统上分" + (int)behavior.UpCmd.Variety,
                                    orderStatus: OrderStatusEnum.上分成功);
            amount = await userOperation.GetUserMoney(merchantID, user._id);
            //系统回复
            string message = reply.Remainder.Replace("{昵称}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName)
            .Replace("{变动分数}", ((int)behavior.UpCmd.Variety).ToString("#0.00"))
            .Replace("{剩余分数}", amount.ToString("#0.00"));
            await RabbitMQHelper.SendAdminMessage(message, merchantID, gameType, false, null);
        }

        /// <summary>
        /// 假人下分
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="merchantID"></param>
        /// <param name="reply"></param>
        /// <param name="gameType"></param>
        /// <param name="amount"></param>
        /// <param name="span"></param>
        /// <returns></returns>
        public static async Task DownDivisionHandle(string userID, string merchantID, ReplySetUp reply, GameOfType gameType, decimal amount, TimeSpan span)
        {
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
            if ((int)user.UserMoney < amount)
            {
                await RabbitMQHelper.SendAdminMessage(string.Format("@{0}余额不足", nickName), merchantID, gameType);
            }
            else
            {
                var result = await userOperation.LowerScore(user._id, merchantID,
                (int)amount,
                ChangeTargetEnum.系统,
                msg: "系统下分" + (int)amount,
                remark: "系统下分" + (int)amount,
                orderStatus: OrderStatusEnum.下分成功);

                if (!result)
                {
                    await RabbitMQHelper.SendAdminMessage(string.Format("@{0}余额不足", nickName), merchantID, gameType);
                    return;
                }
                var nowamount = await userOperation.GetUserMoney(merchantID, user._id);
                //系统回复
                string message = reply.Remainder.Replace("{昵称}", nickName)
            .Replace("{变动分数}", (-(int)amount).ToString("#0.00"))
            .Replace("{剩余分数}", nowamount.ToString("#0.00"));
                await Task.Delay(span);
                await RabbitMQHelper.SendAdminMessage(message, merchantID, gameType, false, null);
            }
        }

        /// <summary>
        /// 机器人发送消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static async Task SendShamUserMessage(string message, string merchantID, string userID, GameOfType gameType)
        {
            try
            {
                var lottery = await GetGameStatus(merchantID, gameType);
                var nper = lottery.NextIssueNum;
                UserOperation userOperation = new UserOperation();
                ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
                var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
                if (message == "查")
                {
                    await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType);
                    string output = string.Empty;
                    //管理员回复
                    var bson = await Utils.GetRoomInfosAsync(merchantID, gameType);
                    var items = bson.RInfoItems;
                    output = await CancelAnnouncement.CheckStream(userID, gameType, merchantID, nper, reply);
                    BackgroundJob.Schedule(() => SendAdminMessage(output, merchantID, gameType), TimeSpan.FromSeconds(1));
                    //await GameCollection.SendAdminMessage(output, merchantID, gameType, 1);
                }
                else if (message == "返水")
                {
                    await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType);
                    var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
                    var result = await BackwaterKind.UserBackwaterAsync(userID, gameType, merchantID);
                    if (result.Status != RecoverEnum.成功)
                    {
                        BackgroundJob.Schedule(() => SendAdminMessage(string.Format("@{0}{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, result.Message), merchantID, gameType), TimeSpan.FromSeconds(1));
                        //await GameCollection.SendAdminMessage(string.Format("@{0}{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, result.Message), merchantID, gameType, 1);
                    }
                }
                else
                {
                    var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
                    #region 投注
                    string[] strChar = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "大", "小", "单", "双", "和", "龙",
            "虎", " ", "前三", "后三", "中三", "和", "通买", "豹子", "对子", "顺子","半顺", "杂六",
            "万个"};
                    if (message.Contains(strChar))
                    {
                        //if (lottery.Status == GameStatusEnum.封盘中)
                        //{
                        //    if (reply.NoticeSealing)
                        //    {
                        //        var result = await Utils.InstructionConversion(reply.Sealing, userID, merchantID, gameType, nper);
                        //        await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                        //    }
                        //}
                        //else if (lottery.Status != GameStatusEnum.等待中)
                        //{
                        //    var result = string.Format("@{0}{1}，禁止投注", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, Enum.GetName(typeof(GameStatusEnum), (int)lottery.Status));
                        //    await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                        //}
                        if (lottery.Status == GameStatusEnum.等待中)
                        {
                            await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType);
                            var charItem = message.Split(' ');
                            foreach (var str in charItem)
                            {
                                if (string.IsNullOrEmpty(str)) continue;
                                var result = new GameBetStatus();
                                var status = user.Status == UserStatusEnum.正常 ?
                                    NotesEnum.正常 : NotesEnum.虚拟;
                                if (Utils.GameTypeItemize(gameType))
                                    result = await General(userID, gameType, str, merchantID, nper, status);
                                else
                                    result = await Special(userID, gameType, str, merchantID, nper, status);
                                if (result.Status == BetStatuEnum.正常)
                                {
                                    user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
                                    if (reply.NoticeBetSuccess)
                                    {
                                        var msgResult = await Utils.InstructionConversion(reply.GameSuccess, userID, merchantID, gameType, nper, result.OddNum);
                                        await RabbitMQHelper.SendAdminMessage(msgResult, merchantID, gameType);

                                        //飞单
                                        if (user.Record)
                                        {
                                            //发送自己盘口
                                            await RabbitMQHelper.SendMerchantHandicap(merchantID, userID);

                                            var foundationSetup = RedisOperation.GetFoundationSetup(merchantID);
                                            //房间设置
                                            var roomSetup = await Utils.GetRoomInfosAsync(merchantID, gameType);
                                            if (roomSetup.LotteryRecord == RecordType.飞单到外部网盘)
                                            {
                                                //直接飞单
                                                if (roomSetup.Revoke || (foundationSetup.ProhibitChe && lottery.Surplus <= foundationSetup.EntertainedFrontTime))
                                                {
                                                    await FlyingSheet.ProhibitionWithdrawal(merchantID, gameType, result.BetInfos, nper);
                                                }
                                                else
                                                {
                                                    var orders = Utils.GetFlyingBet(gameType, result.BetInfos);
                                                    var key = merchantID + Enum.GetName(typeof(GameOfType), gameType);
                                                    RedisOperation.SetHash(key, result.OddNum, JsonConvert.SerializeObject(orders));
                                                }
                                            }
                                            else if (roomSetup.LotteryRecord == RecordType.飞单到高级商户)
                                            {
                                                //飞单设置
                                                MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
                                                var merchantSheet = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);

                                                if (lottery.Surplus < foundationSetup.EntertainedFrontTime && merchantSheet != null && merchantSheet.OpenSheet)
                                                {
                                                    var dicInfo = await Utils.GetMerchantFlySheetInfo(merchantID);
                                                    if (dicInfo != null)
                                                    {
                                                        await FlyingSheet.MerchantInternalSheet(merchantID, dicInfo["TargetID"].ToString(), userID, dicInfo["UserID"].ToString(), gameType, str, nper);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (result.Status == BetStatuEnum.积分不足 && reply.NoticeInsufficientIntegral)
                                {
                                    var msgResult = await Utils.InstructionConversion(reply.NotEnough, userID, merchantID, gameType, nper);
                                    await RabbitMQHelper.SendAdminMessage(msgResult, merchantID, gameType);
                                }
                                else if (result.Status == BetStatuEnum.限额 && reply.NoticeQuota)
                                {
                                    await RabbitMQHelper.SendAdminMessage(string.Format("@{0} {1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, result.OutPut), merchantID, gameType);
                                }
                                else if (result.Status == BetStatuEnum.格式错误 && reply.NoticeInvalidSub)
                                {
                                    var msgResult = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, gameType, nper);
                                    await RabbitMQHelper.SendAdminMessage(msgResult, merchantID, gameType);
                                }
                            }
                        }
                    }
                    #endregion
                }
                //await Lock.WaitAsync();
                //var key = Guid.NewGuid().ToString();
                ////1、通过调度工厂获得调度器
                ////var scheduler = await _schedulerFactory.GetScheduler();
                //var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
                ////2、开启调度器
                //await scheduler.Start();
                //IDictionary<string, object> dic = new Dictionary<string, object>();
                //dic.Add("merchantID", merchantID);
                //dic.Add("message", message);
                //dic.Add("gameType", gameType);
                //dic.Add("userID", userID);
                //var jobDetail = JobBuilder.Create<SendShamUserMessageJob>()
                //                .UsingJobData(new JobDataMap(dic))
                //                .WithIdentity(key, "SendShamUserMessage")
                //                .Build();
                //var trigger = (ISimpleTrigger)TriggerBuilder.Create()
                //.WithIdentity(key)
                //.StartAt(DateBuilder.FutureDate(time, IntervalUnit.Second))
                //.ForJob(jobDetail)
                //.Build();

                ////5、将触发器和任务器绑定到调度器中
                //await scheduler.ScheduleJob(jobDetail, trigger);
                //await Task.Delay(50);
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 是否中标
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static bool WinningBid(double num)
        {
            Random random = new Random();
            return num >= random.NextDouble();
        }
        #endregion

        #region 自动修改封盘时间
        /// <summary>
        /// 自动修正当前登录商户对应高级商户封盘时间
        /// </summary>
        /// <returns></returns>
        public static async Task AutoRevise()
        {
            //查询在线商户
            BsonOperation bsonOperation = new BsonOperation("MerchantSheetFly");
            var bsons = (await bsonOperation.Collection.FindAsync(t => true)).ToList();
            if (!bsons.IsNull())
            {
                var list = bsons.Select(t => new { MerchantID = t["MerchantID"].ToString(), TargetID = t["TargetID"].ToString() })
                    .GroupBy(t => new { t.TargetID }).Select(t => new { t.Key.TargetID, MerchantIDs = t.Select(x => x.MerchantID).ToList() }).ToList();
                FoundationSetupOperation foundationSetupOperation = new FoundationSetupOperation();
                MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
                foreach (var data in list)
                {
                    var targetSetup = await foundationSetupOperation.GetModelAsync(t => t.MerchantID == data.TargetID);
                    foreach (var merchantID in data.MerchantIDs)
                    {
                        var setup = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);
                        if (setup == null) return;
                        var foundation = await foundationSetupOperation.GetModelAsync(t => t.MerchantID == merchantID);
                        foundation.LotteryFrontTime = targetSetup.LotteryFrontTime;
                        foundation.LotteryFrontTime.ForEach(t =>
                        {
                            t.LotteryTime = (t.LotteryTime - setup.AdvanceTime) <= 0 ? 0 : t.LotteryTime - setup.AdvanceTime;
                        });
                        await foundationSetupOperation.UpdateModelAsync(foundation);
                        RedisOperation.SetFoundationSetup(merchantID, foundation);
                    }
                }
            }
            BackgroundJob.Schedule(() => AutoRevise(), TimeSpan.FromMinutes(10));
        }
        #endregion

        #region 推送百家乐消息
        /// <summary>
        /// 百家乐消息推送
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="nper"></param>
        /// <param name="znum"></param>
        /// <param name="gameType"></param>
        /// <param name="conns"></param>
        /// <returns></returns>
        public static async Task BaccaratMsgHandle(string merchantID, string nper, int znum, BaccaratGameType gameType, List<string> conns)
        {
            var setup = RedisOperation.GetVideoFoundationSetup(merchantID);
            await RabbitMQHelper.SendBaccaratAdminMessage(setup.LotteryFrontMsg, merchantID, znum, gameType, conns);

            //确认
            var msg = await SealupMessage.GetBaccaratBetsAsync(merchantID, gameType, setup.EntertainedAfterMsg, nper, znum);
            BackgroundJob.Schedule(() => RabbitMQHelper.SendBaccaratAdminMessage(msg, merchantID, znum, gameType, conns, null), TimeSpan.FromSeconds(setup.EntertainedAfterTime));

            //自定义消息
            BackgroundJob.Schedule(() => RabbitMQHelper.SendBaccaratAdminMessage(setup.CustomMsg, merchantID, znum, gameType, conns, null), TimeSpan.FromSeconds(setup.CustomTime));
        }
        #endregion
    }
}
