using Baccarat.RedisModel;
using Entity;
using Entity.BaccaratModel;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Baccarat;
using Operation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Operation.Common.WinPrize;

namespace Baccarat.Manipulate
{
    /// <summary>
    /// 下发任务
    /// </summary>
    public class DistributionLow
    {
        /// <summary>
        /// 游戏任务状态
        /// </summary>
        public static Dictionary<int, TaskStatusEnum> GameTaskStatus { get; set; } = new Dictionary<int, TaskStatusEnum>();

        public static readonly int limit = 30;

        /// <summary>
        /// 中奖处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task DistributionAsync(TaskDistributionModel data)
        {
            try
            {
                GameTaskStatus[data.ZNum] = TaskStatusEnum.开启;
                if (data.MerchantIDList.IsNull())
                    return;
                //下发任务
                DistributionOperation distributionOperation = new DistributionOperation();
                var distribution = new Distribution()
                {
                    _id = data.UUID,
                    VGameType = data.GameType,
                    MerchantIDList = data.MerchantIDList,
                    Nper = data.Nper,
                    ZNum = data.ZNum
                };
                //添加记录
                await distributionOperation.InsertModelAsync(distribution);
                var result = new List<WinningPrizeClass>();
                if (data.GameType == BaccaratGameType.百家乐)
                {
                    //开奖
                    var model = JsonConvert.DeserializeObject<BaccaratLottery>(data.Lottery);
                    result = await WinBaccaratAsync(model, data.MerchantIDList);
                    SemaphoreSlim Lock = new SemaphoreSlim(limit, limit);
                    var gttasks = new List<Task>();
                    foreach (var merchantID in data.MerchantIDList)
                    {
                        var task = Task.Run(async () =>
                        {
                            await Lock.WaitAsync();
                            try
                            {
                                var address = Utils.GetAddress(merchantID);
                                UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
                                var collection = userSendMessageOperation.GetCollection(merchantID);
                                await collection.DeleteManyAsync(t => t.CreatedTime < DateTime.Now.AddHours(-1));
                                FoundationSetupOperation foundationSetupOperation = new FoundationSetupOperation();
                                var setup = await foundationSetupOperation.GetModelAsync(t => t.MerchantID == merchantID);
                                //中奖推送
                                var message = await GetWinningDetails(model, merchantID, setup.WinningDetails);
                                await RabbitMQHelper.SendBaccaratAdminMessage(message, merchantID, model.ZNum, model.GameType);

                                //用户积分 账单推送
                                var bills = await GetMembershipScore(merchantID, setup.MembershipScore, model.GameType, model.ZNum);
                                await RabbitMQHelper.SendBaccaratAdminMessage(bills, merchantID, model.ZNum, model.GameType);
                            }
                            catch (Exception e)
                            {
                                Utils.Logger.Error(e);
                            }
                            finally
                            {
                                Lock.Release();
                            }
                        });
                        gttasks.Add(task);
                    }
                    await Task.WhenAll(gttasks.ToArray());
                    Lock.Dispose();
                }
                if (result.IsNull()) return;
                //发送中奖用户积分
                foreach (var infos in result)
                {
                    RabbitMQHelper.SendUserPointChange(infos.UserID, infos.MerchantID);
                }
            }
            catch (MongoWriteException)
            {

            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
            finally
            {
                GameTaskStatus[data.ZNum] = TaskStatusEnum.关闭;
            }
        }

        /// <summary>
        /// 游戏任务状态
        /// </summary>
        public enum TaskStatusEnum
        {
            /// <summary>
            /// 开启
            /// </summary>
            开启 = 1,
            /// <summary>
            /// 关闭
            /// </summary>
            关闭 = 2
        }

        /// <summary>
        /// 获取游戏结算
        /// </summary>
        /// <param name="lottery"></param>
        /// <param name="merchantID"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        private static async Task<string> GetWinningDetails(BaccaratLottery lottery, string merchantID, string message)
        {
            message = message.Replace("{期号}", lottery.IssueNum)
                 .Replace("{游戏}", Enum.GetName(typeof(BaccaratGameType), lottery.GameType))
                 .Replace("{开奖信息}", Utils.GetDescriptionName(typeof(BaccaratWinEnum), lottery.Result));

            #region 结算
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(merchant.AddressID);
            var collection = baccaratBetOperation.GetCollection(merchantID);
            var betList = await collection.FindListAsync(t => t.Nper == lottery.IssueNum && t.BetStatus != BaccaratBetEnum.已投注 && t.BetStatus != BaccaratBetEnum.已取消);
            if (betList.IsNull())
            {
                message = message.Replace("{结算}", "");
                return message;
            }
            var userIDList = betList.Select(t => t.UserID).Distinct().ToList();
            UserOperation userOperation = new UserOperation();
            List<string> result = new List<string>();
            foreach (var userID in userIDList)
            {
                var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
                if (user == null) continue;
                var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                var userBetList = betList.FindAll(t => t.UserID == userID);
                result.Add(string.Format("[{0}][{1}]", nickName, userBetList.Sum(t => t.MediumBonus) - userBetList.Sum(t => t.BetMoney)));
            }
            #endregion
            message = message.Replace("{结算}", string.Join("\r\n", result));
            return message;

        }

        /// <summary>
        /// 账单内容
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="message"></param>
        /// <param name="gameType"></param>
        /// <param name="znum"></param>
        /// <returns></returns>
        private static async Task<string> GetMembershipScore(string merchantID, string message, BaccaratGameType gameType, int znum)
        {
            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            VideoRoomOperation videoRoomOperation = new VideoRoomOperation();
            var roomSetup = await videoRoomOperation.GetModelAsync(t => t.MerchantID == merchantID && t.GameType == gameType);
            message = message.Replace("{微群名字}", roomSetup.GameRoomName);
            #region 在线人数
            var result = await Utils.GetBaccaratGameConnInfos(merchantID, znum);
            //低于分数不显示
            decimal min = roomSetup.MinimumAmount == null ? 0 : roomSetup.MinimumAmount.Value;

            decimal allAmount = 0;
            if (!result.IsNull())
            {
                UserOperation userOperation = new UserOperation();
                var users = await userOperation.GetModelListAsync(userOperation.Builder.In(t => t._id, result.Select(t => t.UserID).ToList()));
                allAmount = users.Sum(t => t.UserMoney);

                //账单
                var bills = new List<string>();
                users = users.FindAll(t => t.UserMoney >= min);
                foreach (var user in users)
                {
                    var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                    bills.Add(string.Format("{0}:{1}", nickName, user.UserMoney));
                }
                message = message.Replace("{玩家数量}", users.Count.ToString())
                .Replace("{账单内容}", string.Join("\r\n", bills));
            }
            else
            {
                message = message.Replace("{玩家数量}", "0")
                .Replace("{账单内容}", "");
            }
            message = message.Replace("{在线人数}", (result.Count + room.Online).ToString())
                .Replace("{玩家总分}", allAmount.ToString("#0.00"));
            #endregion
            return message;
        }
    }
}
