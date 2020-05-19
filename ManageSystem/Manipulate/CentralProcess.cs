using Entity;
using Microsoft.AspNetCore.Http;
using Operation.Abutment;
using Operation.Common;
using Operation.RedisAggregate;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public static class CentralProcess
    {
        /// <summary>
        /// 设置时间偏移
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTimeOffset ToDateTimeOffset(this DateTime dateTime)
        {
            return dateTime.ToUniversalTime() <= DateTimeOffset.MinValue.UtcDateTime
                       ? DateTimeOffset.MinValue
                       : new DateTimeOffset(dateTime);
        }

        /// <summary>
        /// 生成6位数
        /// </summary>
        /// <returns></returns>
        public static string SetExtensionCode()
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();
            for (int i = 0; i < 6; i++)
            {
                var num = random.Next(0, 10).ToString();
                sb.Append(num);
            }
            return sb.ToString();
        }

        /// <summary>
        /// 获取ip
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <returns></returns>
        public static string GetIP(this HttpContext HttpContext)
        {
            var headers = HttpContext.Request.Headers;
            if (headers.ContainsKey("X-Forwarded-For"))
            {
                return IPAddress.Parse(headers["X-Forwarded-For"].ToString().Split(',', StringSplitOptions.RemoveEmptyEntries)[0]).MapToIPv4().ToString();
            }
            var ip = HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
            if (ip.Contains(":"))
                ip = ip.Split(':').Last();
            return ip;
        }

        /// <summary>
        /// 获取商户redis缓存信息
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetMerchantRedisCookie(this HttpContext HttpContext)
        {
            var key = HttpContext.Request.Headers["MerchantAuthorization"].ToString();
            if (string.IsNullOrEmpty(key)) return null;
            return RedisOperation.GetValues(key);
        }

        /// <summary>
        /// 获取商户redis缓存信息
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetUserRedisCookie(this HttpContext HttpContext)
        {
            var key = HttpContext.Request.Headers["UserAuthorization"].ToString();
            if (string.IsNullOrEmpty(key)) return null;
            return RedisOperation.GetValues(key);
        }

        /// <summary>
        /// 获取对应商户盘口信息
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static async Task<object> GetMerchantHandicap(string merchantID, string userID)
        {
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            //查询目标商户下注信息
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == userID && t.Status == UserStatusEnum.正常);
            var alltotal = user.UserMoney;
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(merchant.AddressID);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var betList = await collection.FindListAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.Notes == NotesEnum.正常 && t.CreatedTime >= DateTime.Now.Date);
            //已结
            var knot = betList.FindAll(t => t.BetStatus == BetStatus.已开奖);
            var already = knot.Sum(t => t.AllMediumBonus);
            //盈亏
            var proLoss = knot.IsNull() ? 0 : knot.Sum(t => t.AllMediumBonus) - knot.Sum(t => t.AllUseMoney);
            //未结
            var nalready = betList.FindAll(t => t.BetStatus == BetStatus.未开奖).Sum(t => t.AllUseMoney);

            var result = new
            {
                Already = already,
                NAlready = nalready,
                MerchantName = merchant.MeName,
                Balance = alltotal,
                Proloss = proLoss,
                UserName = user.LoginName
            };
            return result;
        }

        /// <summary>
        /// 补发飞单任务
        /// </summary>
        /// <param name="model">飞单模型</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="nper">期号 </param>
        /// <param name="retry">重试次数</param>
        /// <returns></returns>
        public static async Task Replacement(SendFlying model, string merchantID, GameOfType gameType, string nper, int retry)
        {
            //1、通过调度工厂获得调度器
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            //2、开启调度器
            await scheduler.Start();
            var jobKey = Guid.NewGuid().ToString();
            IDictionary<string, object> dic = new Dictionary<string, object>
            {
                { "merchantID", merchantID },
                { "gameType", gameType },
                { "model", model },
                { "nper", nper },
                { "retry", retry}
            };
            var jobDetail = JobBuilder.Create<ReplacementJob>()
                            .UsingJobData(new JobDataMap(dic))
                            .WithIdentity(jobKey, "Replacement")
                            .Build();
            var trigger = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity(jobKey, "Replacement")
            //3秒
            .StartAt(DateBuilder.FutureDate(3, IntervalUnit.Second))
            .ForJob(jobDetail)
            .Build();

            //5、将触发器和任务器绑定到调度器中
            await scheduler.ScheduleJob(jobDetail, trigger);
        }
    }
}
