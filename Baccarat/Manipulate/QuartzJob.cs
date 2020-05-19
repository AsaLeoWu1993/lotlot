using Entity;
using Operation.Abutment;
using Quartz;
using System;
using System.Threading.Tasks;

namespace Baccarat.Manipulate
{
    /// <summary>
    /// 定时任务
    /// </summary>
    public class QuartzJob
    {
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
                        await RabbitMQHelper.SendBaccaratAdminMessage(message, merchantID, data.Znum.Value, data.BaccaratGameType.Value);
                    }
                    #endregion
                    RabbitMQHelper.SendUserPointChange(data.UserID, merchantID);
                    return;
                });
            }
        }
    }
}
