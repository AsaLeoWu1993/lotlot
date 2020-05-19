using Entity;
using ManageSystem.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Agent;
using Operation.Baccarat;
using Operation.Common;
using Operation.RedisAggregate;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 数据抓取
    /// </summary>
    public class GraspDatas
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="hubContext"></param>
        public GraspDatas(RequestDelegate next, IHubContext<ChatHub> hubContext)
        {
            _next = next;
            try
            {
                //app
                if (!Utils.Lottery && !Utils.Variable && !Utils.Collect)
                {
                    //监听消息队列
                    RabbitMQHelper.ReceiveAdminMessage(hubContext);
                    RabbitMQHelper.ReceiveUserPointChange(hubContext);
                    RabbitMQHelper.ReceiveSendUserMessage(hubContext);
                    RabbitMQHelper.ReceiveAllinMessage(hubContext);
                    RabbitMQHelper.ReceiveOverallMessage(hubContext);
                    RabbitMQHelper.ReceiveGameMessage(hubContext);
                    RabbitMQHelper.ReceiveGameStatus(hubContext);
                    RabbitMQHelper.ReceiveVideoSendUserMessage(hubContext); 
                    RabbitMQHelper.ReceiveBaccaratAdminMessage(hubContext);
                    RabbitMQHelper.ReceiveBaccaratStartBet(hubContext);
                    RabbitMQHelper.ReceiveFlyingSheet(hubContext);
                    RabbitMQHelper.ReceiveMerchantHandicap(hubContext);
                }
                
                //RabbitMQHelper.ReceiveMerchantBetInfo(hubContext);
                //开奖容器
                if (Utils.Lottery)
                {
                    RabbitMQHelper.ReceiveTaskDistribution();
                    RabbitMQHelper.ReceiveVideoTaskDistribution();
                }

                BetManage.BetInit().GetAwaiter().GetResult();

                //if (Utils.Variable)
                //    HandleData().GetAwaiter().GetResult();
            }
            catch (Exception e)
            {
                Utils.Logger.Error(JsonConvert.SerializeObject(e));
            }
        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            await _next.Invoke(context);
        }

        /// <summary>
        /// 添加部分数据
        /// </summary>
        /// <returns></returns>
        public async Task HandleData()
        {
            ////删除所有商户
            //MerchantOperation merchantOperation = new MerchantOperation();
            //await merchantOperation.DeleteModelManyAsync(t => true);
            ////删除所有信息
            //UserOperation userOperation = new UserOperation();
            //await userOperation.DeleteModelManyAsync(t => true);
            //UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            //await userIntegrationOperation.DeleteModelManyAsync(t => true);
            //ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            //await shamRobotOperation.DeleteModelManyAsync(t => true);
            //SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
            //await sendFlyingOperation.DeleteModelManyAsync(t => true);
            //RoomOperation roomOperation = new RoomOperation();
            //await roomOperation.DeleteModelManyAsync(t => true);
            //RoomGameDetailedOperation roomGameDetailedOperation = new RoomGameDetailedOperation();
            //await roomGameDetailedOperation.DeleteModelManyAsync(t => true);
            //VideoRoomOperation videoRoomOperation = new VideoRoomOperation();
            //await videoRoomOperation.DeleteModelManyAsync(t => true);
            //SensitiveOperation sensitiveOperation = new SensitiveOperation();
            //await sensitiveOperation.DeleteModelManyAsync(t => true);
            //ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            //await replySetUpOperation.DeleteModelManyAsync(t => true);
            //OddsBaccaratOperation oddsBaccaratOperation = new OddsBaccaratOperation();
            //await oddsBaccaratOperation.DeleteModelManyAsync(t => true);
            //OddsOrdinaryOperation oddsOrdinaryOperation = new OddsOrdinaryOperation();
            //await oddsOrdinaryOperation.DeleteModelManyAsync(t => true);
            //OddsSpecialOperation oddsSpecialOperation = new OddsSpecialOperation();
            //await oddsSpecialOperation.DeleteModelManyAsync(t => true);
            //MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
            //await merchantSheetOperation.DeleteModelManyAsync(t => true);
            //MerchantInternalOperation merchantInternalOperation = new MerchantInternalOperation();
            //await merchantInternalOperation.DeleteModelManyAsync(t => true);
            //FoundationSetupOperation foundationSetupOperation = new FoundationSetupOperation();
            //await foundationSetupOperation.DeleteModelManyAsync(t => true);
            //BetLimitBaccaratOperation betLimitBaccaratOperation = new BetLimitBaccaratOperation();
            //await betLimitBaccaratOperation.DeleteModelManyAsync(t => true);
            //BetLimitOrdinaryOperation betLimitOrdinaryOperation = new BetLimitOrdinaryOperation();
            //await betLimitOrdinaryOperation.DeleteModelManyAsync(t => true);
            //BetLimitSpecialOperation betLimitSpecialOperation = new BetLimitSpecialOperation();
            //await betLimitSpecialOperation.DeleteModelManyAsync(t => true);
            //BackwaterSetupOperation backwaterSetupOperation = new BackwaterSetupOperation();
            //await backwaterSetupOperation.DeleteModelManyAsync(t => true);
            //BackwaterJournalOperation backwaterJournalOperation = new BackwaterJournalOperation();
            //await backwaterJournalOperation.DeleteModelManyAsync(t => true);
            //AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            //await agentBackwaterOperation.DeleteModelManyAsync(t => true);

            ////销售
            ////账变
            //AgentMainLogOperation agentMainLogOperation = new AgentMainLogOperation();
            //await agentMainLogOperation.DeleteModelManyAsync(t => true);
            //AgentUserOperation agentUserOperation = new AgentUserOperation();
            //await agentUserOperation.DeleteModelManyAsync(t => t.LoginName != "admin");
            //CashWithdrawalOperation cashWithdrawalOperation = new CashWithdrawalOperation();
            //await cashWithdrawalOperation.DeleteModelManyAsync(t => true);
            //MerchantTestRecordOperation merchantTestRecordOperation = new MerchantTestRecordOperation();
            //await merchantTestRecordOperation.DeleteModelManyAsync(t => true);
            //SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            //await salesRecordsOperation.DeleteModelManyAsync(t => true);
            //AccountingRecordOperation accountingRecordOperation = new AccountingRecordOperation();
            //await accountingRecordOperation.DeleteModelManyAsync(t => true);
            //RechargeOperation rechargeOperation = new RechargeOperation();
            //await rechargeOperation.DeleteModelManyAsync(t => true);
            if (Environment.GetEnvironmentVariable("Online") != null)
            {
                Lottery10Operation lottery10Operation = new Lottery10Operation();
                await lottery10Operation.DeleteModelManyAsync(t => t.CreatedTime >= DateTime.Now.Date.AddHours(19)
                && t.CreatedTime <= DateTime.Now);
            }
        }
    }
}
