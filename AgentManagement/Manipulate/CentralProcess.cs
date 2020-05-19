using Microsoft.AspNetCore.Http;
using Operation.Agent;
using System;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgentManagement.Manipulate
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public static class CentralProcess
    {
        /// <summary>
        /// 获取代理关系链
        /// </summary>
        /// <param name="agentID">代理id</param>
        /// <returns></returns>
        public static async Task<string> GetPerAgentName(string agentID)
        {
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            if (agent == null) return "";
            if (agent.IsHighest)
                return agent.LoginName;
            else
                return await GetPerAgentName(agent.SupAgentID) + " > " + agent.LoginName;
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
        /// 高级管理后台
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <returns></returns>
        public static bool IsAdmin(this HttpContext HttpContext)
        {
            var isHighest = HttpContext.User.FindFirstValue("IsHighest");
            return isHighest == "1";
        }
    }
}
