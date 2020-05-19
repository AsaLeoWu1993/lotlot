using Entity;
using Entity.BaccaratModel;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Baccarat;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static Operation.Common.WinPrize;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 下发任务
    /// </summary>
    public class VideoDistributionLow
    {
        /// <summary>
        /// 游戏任务状态
        /// </summary>
        public static Dictionary<int, TaskStatusEnum> GameTaskStatus { get; set; } = new Dictionary<int, TaskStatusEnum>();

        public static readonly int limit = 20;

        /// <summary>
        /// 中奖处理
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task DistributionAsync(VideoTaskDistributionModel data)
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
                    SemaphoreSlim Lock = new SemaphoreSlim(limit, limit);
                    var gttasks = new List<Task>();
                    foreach (var merchantID in data.MerchantIDList)
                    {
                        var task = Task.Run(async () =>
                        {
                            await Lock.WaitAsync();
                            try
                            {
                                var list = await WinBaccaratAsync(model, merchantID);
                                if (!list.IsNull())
                                    result.AddRange(list);
                                //var address = await Utils.GetAddress(merchantID);
                                //UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
                                //var collection = userSendMessageOperation.GetCollection(merchantID);
                                //await collection.DeleteManyAsync(userSendMessageOperation.Builder.Where(t => t.CreatedTime < DateTime.Now.AddHours(-1) && t.ZNum == data.ZNum && t.VGameType == data.GameType));
                                var setup = RedisOperation.GetVideoFoundationSetup(merchantID);
                                //中奖推送
                                var message = await WinBaccaratMessageAsync(model, merchantID);
                                await RabbitMQHelper.SendBaccaratAdminMessage(message, merchantID, model.ZNum, model.GameType);

                                ////用户积分 账单推送
                                //var bills = await GetMembershipScore(merchantID, setup.MembershipScore, model.GameType, model.ZNum);
                                //await RabbitMQHelper.SendBaccaratAdminMessage(bills, merchantID, model.ZNum, model.GameType);
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
                    await RabbitMQHelper.SendUserPointChange(infos.UserID, infos.MerchantID);
                }
            }
            catch (MongoWriteException)
            {

            }
            catch (Exception)
            {

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
    }

    /// <summary>
    /// 游戏开奖下发任务信息
    /// </summary>
    public class VideoTaskDistributionModel
    {
        /// <summary>
        /// 期号 
        /// </summary>
        public string Nper { get; set; }
        /// <summary>
        /// 游戏类型
        /// </summary>
        public BaccaratGameType GameType { get; set; }

        /// <summary>
        /// 开奖信息
        /// </summary>
        public string Lottery { get; set; }

        /// <summary>
        /// 桌号
        /// </summary>
        public int ZNum { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public List<string> MerchantIDList { get; set; }

        /// <summary>
        /// 唯一码
        /// </summary>
        public string UUID { get; set; } = Guid.NewGuid().ToString();
    }
}
