using Baccarat.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Threading.Tasks;

namespace Baccarat.Manipulate
{
    /// <summary>
    /// 监听
    /// </summary>
    public class Monitor
    {
        private readonly RequestDelegate _next;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="next"></param>
        /// <param name="hubContext"></param>
        public Monitor(RequestDelegate next, IHubContext<ChatHub> hubContext)
        {
            _next = next;
            try
            {
                RedisOperation.StartConn();
                //获取游戏状态
                Common.GetGameList();
                //监听
                RedisHub.ServerToCompany().GetAwaiter().GetResult();
                //初始化下注连接
                BetManage.BaccaratBetInit().GetAwaiter().GetResult();
                //监听管理员消息
                RabbitMQHelper.ReceiveBaccaratAdminMessage(hubContext);
                //监听游戏状态
                RabbitMQHelper.ReceiveGameStatus(hubContext);
                //监听用户消息
                RabbitMQHelper.ReceiveSendUserMessage(hubContext);
                //用户积分变化
                RabbitMQHelper.ReceiveUserPointChange(hubContext);
                //开奖信息
                RabbitMQHelper.ReceiveTaskDistribution();
                //通用方法
                RabbitMQHelper.ReceiveOverallMessage(hubContext);
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
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
    }
}
