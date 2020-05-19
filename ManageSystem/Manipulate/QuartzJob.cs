using Entity;
using Operation.Abutment;
using Operation.Common;
using Quartz;
using System;
using System.Threading.Tasks;

namespace ManageSystem.Manipulate
{
    public class SendAdminMessageJob : IJob
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            var jobData = context.JobDetail.JobDataMap;
            var triggerData = context.Trigger.JobDataMap;
            var data = context.MergedJobDataMap;

            var message = jobData.GetString("message");
            var merchantID = jobData.GetString("merchantID");
            var gameType = (GameOfType)jobData.Get("gameType");

            return Task.Run(async () =>
            {
                message = Utils.SealedTransformation(gameType, message);
                await RabbitMQHelper.SendAdminMessage(message, merchantID, gameType);
            });
        }
    }

    /// <summary>
    /// 游戏下注确认
    /// </summary>
    public class GameBetConfirmationJob : IJob
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            var jobData = context.JobDetail.JobDataMap;
            var triggerData = context.Trigger.JobDataMap;
            var data = context.MergedJobDataMap;

            var merchantID = jobData.GetString("merchantID");
            var gameType = (GameOfType)jobData.Get("gameType");
            var msg = jobData.GetString("msg");
            var nper = jobData.GetString("nper");

            //return SignalRSendMessage.SendAdminMessage(checkMsg, merchantID, gameType, hubContext);
            return Task.Run(async () =>
            {
                var checkMsg = await SealupMessage.GetAllBetsAsync(merchantID, gameType, msg, nper);
                await RabbitMQHelper.SendAdminMessage(checkMsg, merchantID, gameType);
            });
        }
    }

    /// <summary>
    /// 飞单任务
    /// </summary>
    public class FlyingTaskJob : IJob
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            var jobData = context.JobDetail.JobDataMap;
            var triggerData = context.Trigger.JobDataMap;
            var data = context.MergedJobDataMap;

            var merchantID = jobData.GetString("merchantID");
            var gameType = (GameOfType)jobData.Get("gameType");
            var nper = jobData.GetString("nper");

            return Task.Run(async () =>
            {
                var result = await FlyingSheet.FlyingSheetMethod(merchantID, nper, gameType);
                await RabbitMQHelper.SendFlyingSheet(merchantID, gameType, nper, result, 0);
            });
        }
    }

    /// <summary>
    /// 飞单补发任务 
    /// </summary>
    public class ReplacementJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var jobData = context.JobDetail.JobDataMap;
            var triggerData = context.Trigger.JobDataMap;
            var data = context.MergedJobDataMap;

            var merchantID = jobData.GetString("merchantID");
            var gameType = (GameOfType)jobData.Get("gameType");
            var nper = jobData.GetString("nper");
            var retry = jobData.GetInt("retry");
            var model = (SendFlying)jobData.Get("model");

            return Task.Run(async () =>
            {
                await RabbitMQHelper.SendFlyingSheet(merchantID, gameType, nper, model, retry);
            });
        }
    }
    #region
    //[DisallowConcurrentExecution]
    //public class ShamUserSendMessageJob : IJob
    //{
    //    public Task Execute(IJobExecutionContext context)
    //    {
    //        var jobData = context.JobDetail.JobDataMap;
    //        var triggerData = context.Trigger.JobDataMap;
    //        var data = context.MergedJobDataMap;

    //        var merchantID = jobData.GetString("merchantID");
    //        var gameType = (GameOfType)jobData.Get("gameType");
    //        var hubContext = jobData.Get("hubContext") as IHubContext<ChatHub>;
    //        var allShamUser = (List<User>)jobData.Get("allShamUser");

    //        UserOperation userOperation = new UserOperation();
    //        ShamUserOperation shamUserOperation = new ShamUserOperation();
    //        Random random = new Random();

    //        var user = allShamUser[random.Next(0, allShamUser.Count)];
    //        user = userOperation.GetModel(t => t.MerchantID == user.MerchantID && t._id == user._id);
    //        var shamInfo = shamUserOperation.GetModel(t => t.MerchantID == merchantID && t.UserID == user._id);
    //        if (shamInfo == null) return null;
    //        if (user.UserMoney <= Convert.ToDecimal(shamInfo.UpperScore))
    //        {
    //            var upScore = random.Next(1000, 10000);
    //            string message = string.Format("用户{0}上分{1}成功！剩余积分：{2}", user.NickName, upScore, user.UserMoney + upScore);
    //            Task.Run(async () =>
    //            {
    //                await userOperation.UpperScore(user._id, merchantID,
    //                upScore,
    //                ChangeTargetEnum.系统,
    //                msg: "系统上分" + upScore,
    //                remark: "系统上分" + upScore,
    //                orderStatus: OrderStatusEnum.上分成功);
    //            });
    //            //上分提示
    //            return SignalRSendMessage.SendAdminMessage(message, merchantID, gameType, hubContext);
    //        }
    //        else if (user.UserMoney >= Convert.ToDecimal(shamInfo.LowerScore))
    //        {
    //            var downScore = random.Next(1000, 10000);
    //            string message = string.Format("用户{0}下分{1}成功！剩余积分：{2}", user.NickName, downScore, user.UserMoney + downScore);
    //            Task.Run(async () =>
    //            {
    //                await userOperation.LowerScore(user._id, merchantID,
    //                downScore,
    //                ChangeTargetEnum.系统,
    //                msg: "系统下分" + downScore,
    //                remark: "系统下分" + downScore,
    //                orderStatus: OrderStatusEnum.下分成功);
    //            });
    //            //下分提示
    //            return SignalRSendMessage.SendAdminMessage(message, merchantID, gameType, hubContext);
    //        }
    //        var item = shamInfo.GameBetInfo.Find(t => t.Check && t.GameType == gameType);
    //        if (item != null)
    //        {
    //            if (string.IsNullOrEmpty(item.BetInfo)) return null;
    //            var betMsg = item.BetInfo.Split(',');
    //            //最新一期期号
    //            var nper = CancelAnnouncement.GetGameNper(item.GameType);
    //            var nextNper = GameHandle.GetGameNper(nper, item.GameType);
    //            string message = betMsg[random.Next(0, betMsg.Length)];

    //            return ShamUserBet.ShamUserSendMessage(item, user, message, merchantID, nextNper, hubContext);
    //        }
    //        return null;
    //    }
    //}
    #endregion

    #region 机器人
    ///// <summary>
    ///// 机器人发送消息
    ///// </summary>
    //public class SendShamUserMessageJob : IJob
    //{
    //    public Task Execute(IJobExecutionContext context)
    //    {
    //        var jobData = context.JobDetail.JobDataMap;
    //        var triggerData = context.Trigger.JobDataMap;
    //        var data = context.MergedJobDataMap;

    //        var message = jobData.GetString("message");
    //        var merchantID = jobData.GetString("merchantID");
    //        var userID = jobData.GetString("userID");
    //        var gameType = (GameOfType)jobData.Get("gameType");

    //        return Task.Run(async () =>
    //        {
    //            BaseMongoHelper baseMongo = new BaseMongoHelper();
    //            RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType, baseMongo);
    //            UserOperation userOperation = new UserOperation(baseMongo);
    //            //最新一期期号
    //            var nper = CancelAnnouncement.GetGameNper(gameType, baseMongo);
    //            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation(baseMongo);
    //            var reply = replySetUpOperation.GetModel(t => t.MerchantID == merchantID);
    //            if (message == "查")
    //            {
    //                string output = string.Empty;
    //                //管理员回复
    //                var bson = Utils.GetRoomInfos(merchantID, gameType, baseMongo);
    //                var items = bson.RInfoItems;
    //                output = await CancelAnnouncement.CheckStream(userID, gameType, merchantID, nper, reply, baseMongo);
    //                await GameCollection.SendAdminMessage(output, merchantID, gameType, 1);
    //            }
    //            else if (message == "返水")
    //            {
    //                var user = userOperation.GetModel(t => t._id == userID && t.MerchantID == merchantID);
    //                var result = await BackwaterKind.UserBackwaterAsync(userID, gameType, merchantID, baseMongo);
    //                if (result.Status != RecoverEnum.成功)
    //                {
    //                    await GameCollection.SendAdminMessage(string.Format("@{0}{1}", user.NickName, result.Message), merchantID, gameType, 1);
    //                }
    //            }
    //            else
    //            {
    //                var user = userOperation.GetModel(t => t._id == userID && t.MerchantID == merchantID);
    //                #region 投注
    //                string[] strChar = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "大", "小", "单", "双", "和", "龙",
    //        "虎", " ", "前三", "后三", "中三", "和", "通买", "豹子", "对子", "顺子","半顺", "杂六",
    //        "万个"};
    //                if (message.Contains(strChar))
    //                {
    //                    string output = string.Empty;
    //                    var lottery = await GameDiscrimination.EachpartAsync(gameType, merchantID, baseMongo);
    //                    if (lottery.Status == GameStatusEnum.封盘中)
    //                    {
    //                        if (reply.NoticeSealing)
    //                        {
    //                            var result = await Utils.InstructionConversion(reply.Sealing, userID, merchantID, gameType, nper, baseMongo);
    //                            await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
    //                        }
    //                    }
    //                    else if (lottery.Status != GameStatusEnum.等待中)
    //                    {
    //                        var result = string.Format("@{0}{1}，禁止投注", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, Enum.GetName(typeof(GameStatusEnum), (int)lottery.Status));
    //                        await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
    //                    }
    //                    else if (lottery.Status == GameStatusEnum.等待中)
    //                    {
    //                        var charItem = message.Split(' ');
    //                        foreach (var str in charItem)
    //                        {
    //                            if (string.IsNullOrEmpty(str)) continue;
    //                            var result = BetStatuEnum.格式错误;
    //                            decimal useMoney = 0;
    //                            var status = user.Status == UserStatusEnum.正常 ?
    //                                NotesEnum.正常 : NotesEnum.虚拟;
    //                            if (Utils.GameTypeItemize(gameType))
    //                                result = General(userID, gameType, str, merchantID, nper, ref output, ref useMoney, baseMongo, status);
    //                            else
    //                                result = Special(userID, gameType, str, merchantID, nper, ref output, ref useMoney, baseMongo, status);
    //                            if (result == BetStatuEnum.正常)
    //                            {
    //                                user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
    //                                RabbitMQHelper.SendUserPointChange(userID, merchantID);
    //                                if (reply.NoticeBetSuccess)
    //                                {
    //                                    var msgResult = await Utils.InstructionConversion(reply.GameSuccess, userID, merchantID, gameType, nper, baseMongo);
    //                                    await RabbitMQHelper.SendAdminMessage(msgResult, merchantID, gameType);
    //                                }
    //                            }
    //                            else if (result == BetStatuEnum.积分不足 && reply.NoticeInsufficientIntegral)
    //                            {
    //                                var msgResult = await Utils.InstructionConversion(reply.NotEnough, userID, merchantID, gameType, nper, baseMongo);
    //                                await RabbitMQHelper.SendAdminMessage(msgResult, merchantID, gameType);
    //                            }
    //                            else if (result == BetStatuEnum.限额 && reply.NoticeQuota)
    //                            {
    //                                await RabbitMQHelper.SendAdminMessage(string.Format("@{0} {1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, output), merchantID, gameType);
    //                            }
    //                            else if (result == BetStatuEnum.格式错误 && reply.NoticeInvalidSub)
    //                            {
    //                                var msgResult = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, gameType, nper, baseMongo);
    //                                await RabbitMQHelper.SendAdminMessage(msgResult, merchantID, gameType);
    //                            }
    //                        }
    //                    }
    //                }
    //                #endregion
    //            }
    //        });
    //    }
    //}
    #endregion

    /// <summary>
    /// 假人申请信息处理
    /// </summary>
    public class SendShamUserApplyJob : IJob
    {
        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public Task Execute(IJobExecutionContext context)
        {
            var jobData = context.JobDetail.JobDataMap;
            var triggerData = context.Trigger.JobDataMap;

            var recordID = jobData.GetString("recordID");
            var merchantID = jobData.GetString("merchantID");

            return Task.Run(async () =>
            {
                UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
                var data = await userIntegrationOperation.GetModelAsync(t => t._id == recordID && t.Management == ManagementEnum.未审批);
                if (data == null) return;
                data.Management = ManagementEnum.已同意;
                data.Message = Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType) + "成功";
                data.Remark = Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType) + "成功";
                #region 
                //修改用户积分
                UserOperation userOperation = new UserOperation();
                var user = await userOperation.GetModelAsync(t => t._id == data.UserID && t.MerchantID == merchantID);
                if (user == null) return;
                if (data.ChangeType == ChangeTypeEnum.上分)
                {
                    user.UserMoney += data.Amount;
                }
                //else
                //{
                //    if (user.UserMoney < data.Amount)
                //    {
                //        if (data.GameType != null)
                //            await RabbitMQHelper.SendAdminMessage(string.Format("用户{0}积分不足，下分失败", user.NickName), merchantID, data.GameType.Value);
                //        data.Management = ManagementEnum.已拒绝;
                //        data.Message = Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType) + "失败（用户积分不足）";
                //        data.Remark = Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType) + "失败（用户积分不足）";
                //        await userIntegrationOperation.UpdateModelAsync(data);
                //        return;
                //    }
                //    user.UserMoney -= data.Amount;
                //};
                data.Balance = user.UserMoney;
                await userIntegrationOperation.UpdateModelAsync(data);
                await userOperation.UpdateModelAsync(user);
                ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
                var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
                if (data.GameType != null)
                {
                    var amount = data.ChangeType == ChangeTypeEnum.上分 ? data.Amount : -data.Amount;
                    string message = reply.Remainder.Replace("{昵称}", user.NickName)
                        .Replace("{变动分数}", amount.ToString("#0.00"))
                        .Replace("{剩余分数}", user.UserMoney.ToString("#0.00"));
                    await RabbitMQHelper.SendAdminMessage(message, merchantID, data.GameType.Value);
                }
                #endregion
                await RabbitMQHelper.SendUserPointChange(data.UserID, merchantID);
                return;
            });
        }
    }

    /// <summary>
    /// 10球采集任务
    /// </summary>
    public class RegularLottery10Job : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            var jobData = context.JobDetail.JobDataMap;
            var triggerData = context.Trigger.JobDataMap;

            var urlName = jobData.GetString("urlName");
            var lotteryInfo = (Utils.GameNextLottery)jobData.Get("lotteryInfo");
            var gameType = (GameOfType)jobData.Get("gameType");
            var errorCount = jobData.GetInt("errorCount");

            return null;
            //return GameCollection.GetGrasp(urlName, lotteryInfo, gameType, errorCount);
        }
    }

    ///// <summary>
    ///// 5球采集任务
    ///// </summary>
    //public class RegularLottery5Job : IJob
    //{
    //    public Task Execute(IJobExecutionContext context)
    //    {
    //        var jobData = context.JobDetail.JobDataMap;
    //        var triggerData = context.Trigger.JobDataMap;

    //        var urlName = jobData.GetString("urlName");
    //        var lotteryInfo = (Utils.GameNextLottery)jobData.Get("lotteryInfo");
    //        var gameType = (GameOfType)jobData.Get("gameType");
    //        var errorCount = jobData.GetInt("errorCount");

    //        return GameCollection.GetGrasp5(urlName, lotteryInfo, gameType, errorCount);
    //    }
    //}
}
