using Entity;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Operation.Common.Utils;
using static Operation.Common.WinPrize;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 下发任务
    /// </summary>
    public class DistributionLow
    {
        /// <summary>
        /// 游戏任务状态
        /// </summary>
        public static Dictionary<GameOfType, TaskStatusEnum> GameTaskStatus { get; private set; } =
            new Dictionary<GameOfType, TaskStatusEnum>()
            {
                { GameOfType.北京赛车, TaskStatusEnum.关闭},
                { GameOfType.幸运飞艇, TaskStatusEnum.关闭},
                { GameOfType.重庆时时彩, TaskStatusEnum.关闭},
                { GameOfType.极速赛车, TaskStatusEnum.关闭},
                { GameOfType.澳州10, TaskStatusEnum.关闭},
                { GameOfType.澳州5, TaskStatusEnum.关闭},
                { GameOfType.爱尔兰赛马, TaskStatusEnum.关闭},
                { GameOfType.爱尔兰快5, TaskStatusEnum.关闭},
                { GameOfType.幸运飞艇168, TaskStatusEnum.关闭},
                { GameOfType.极速时时彩, TaskStatusEnum.关闭}
            };
        /// <summary>
        /// 数量分组
        /// </summary>
        public static readonly int limit = 20;

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
        /// 下发任务
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static async Task DistributionAsync(TaskDistributionModel data)
        {
            SemaphoreSlim Lock = new SemaphoreSlim(limit, limit);
            try
            {
                GameTaskStatus[data.GameType] = TaskStatusEnum.开启;
                if (data.MerchantIDList.IsNull())
                    return;
                //下发任务
                DistributionOperation distributionOperation = new DistributionOperation();
                var distribution = new Distribution()
                {
                    _id = data.UUID,
                    GameType = data.GameType,
                    MerchantIDList = data.MerchantIDList,
                    Nper = data.Nper
                };
                //添加记录
                await distributionOperation.InsertModelAsync(distribution);
                //Utils.Logger.Error(string.Format("游戏：{0}  接收任务号：{1}", data.GameType, data.UUID));
                var result = new List<WinningPrizeClass>();

                SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
                var status = RedisOperation.GetValue<GameNextLottery>("GameStatus", Enum.GetName(typeof(GameOfType), data.GameType));
                #region 10球
                if (Utils.GameTypeItemize(data.GameType))
                {
                    //开奖
                    var model = JsonConvert.DeserializeObject<Lottery10>(data.Lottery);
                    var gttasks = new List<Task>();
                    foreach (var merchantID in data.MerchantIDList)
                    {
                        var task = Task.Run(async ()=>
                        {
                            var record = await Win10Async(model, data.GameType, merchantID);
                            if (!record.IsNull()) result.AddRange(record);
                            await GameCollection.GameTypeAsync(merchantID, data.GameType, status);
                        });
                        gttasks.Add(task);
                    }
                    Task.WaitAll(gttasks.ToArray());
                }
                #endregion
                #region 5球
                else
                {
                    //开奖
                    var model = JsonConvert.DeserializeObject<Lottery5>(data.Lottery);
                    var gttasks = new List<Task>();
                    foreach (var merchantID in data.MerchantIDList)
                    {
                        var task = Task.Run(async () =>
                        {
                            var record = await Win5Async(model, data.GameType, merchantID);
                            if (!record.IsNull()) result.AddRange(record);
                            await GameCollection.GameTypeAsync(merchantID, data.GameType, status);
                        });
                        gttasks.Add(task);
                    }
                    Task.WaitAll(gttasks.ToArray());
                }
                #endregion
                //Utils.Logger.Error(string.Format("游戏：{0}  接收任务号完成：{1}", data.GameType, data.UUID));
                if (result.IsNull()) return;
                //发送中奖用户积分
                foreach (var infos in result)
                {
                    await RabbitMQHelper.SendUserPointChange(infos.UserID, infos.MerchantID);
                }
            }
            catch (Exception)
            {
                //Utils.Logger.Error(e);
            }
            finally
            {
                GameTaskStatus[data.GameType] = TaskStatusEnum.关闭;
                Lock.Dispose();
            }
        }
    }


    /// <summary>
    /// 任务下发类
    /// </summary>
    public class TaskDistributionModel
    {
        /// <summary>
        /// 期号 
        /// </summary>
        public string Nper { get; set; }
        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 开奖信息
        /// </summary>
        public string Lottery { get; set; }

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
