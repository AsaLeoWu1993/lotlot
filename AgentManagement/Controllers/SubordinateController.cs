using Entity;
using Entity.AgentModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Operation.Abutment;
using Operation.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgentManagement.Controllers
{
    /// <summary>
    /// 下级管理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    public class SubordinateController : ControllerBase
    {
        #region 下级代理
        /// <summary>
        /// 获取下级代理信息
        /// </summary>
        /// <param name="name">用户名</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <remarks>
        ///##  参数说明
        ///     name：用户名
        ///     startTime：开始时间
        ///     endTime：结束时间
        ///     start：页码
        ///     pageSize：页数
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             AgentID：代理id
        ///             LoginName：名称
        ///             AgentPrice：代理价格
        ///             SubOutNum：下级人数
        ///             SalesNum：团队销量
        ///             Status：状态 
        ///             LastLoginTime：最后登录时间
        ///             CreateTime：创建时间
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchSubAccount(string name, DateTime startTime, DateTime endTime, int start = 1, int pageSize = 10)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            FilterDefinition<AgentUser> filter = agentUserOperation.Builder.Eq(t => t.SupAgentID, agentID);
            if (!string.IsNullOrEmpty(name))
                filter &= agentUserOperation.Builder.Regex(t => t.LoginName, name);

            var agentList = agentUserOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var total = await agentUserOperation.GetCountAsync(filter);
            SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            var result = new List<dynamic>();
            foreach (var data in agentList)
            {
                //获取下级代理人数
                var subList = await agentUserOperation.GetAgentList(data._id);
                FilterDefinition<SalesRecords> subfilter = salesRecordsOperation.Builder.Where(t => t.CreatedTime >= startTime && t.CreatedTime <= endTime && t.SalesType != SalesTypeEnum.临时)
                    & salesRecordsOperation.Builder.In(t => t.AgentID, subList.Select(t => t.AgentID).ToList());

                var subSales = await salesRecordsOperation.GetCountAsync(subfilter);
                result.Add(new
                {
                    AgentID = data._id,
                    data.LoginName,
                    AgentPrice = data.SubAgentPrice,
                    SubOutNum = subList.FindAll(t => t.AgentID != data._id).Count,
                    SalesNum = subSales,
                    data.Status,
                    LastLoginTime = data.LastLoginTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                    CreateTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        /// <summary>
        /// 修改代理价格
        /// </summary>
        /// <param name="agentID">下级代理id</param>
        /// <param name="price">代理价格</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePrice(string agentID, decimal price)
        {
            var preAgentID = HttpContext.User.FindFirstValue("AgentID");
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID
            && t.SupAgentID == preAgentID);
            if (agent == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息"));
            agent.SubAgentPrice = price;
            await agentUserOperation.UpdateModelAsync(agent);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 锁定/解锁
        /// </summary>
        /// <param name="agentID">下级代理id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LockAgent(string agentID)
        {
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            if (agent == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息"));
            if (agent.Status == AgentStatusEnum.正常)
                agent.Status = AgentStatusEnum.锁定;
            else if (agent.Status == AgentStatusEnum.锁定)
                agent.Status = AgentStatusEnum.正常;
            await agentUserOperation.UpdateModelAsync(agent);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }
        #endregion

        #region 客户

        /// <summary>
        /// 获取客户信息
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <remarks>
        ///##  参数说明
        ///     name：用户名
        ///     start：页码
        ///     pageSize：页数
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             Name：客户名称
        ///             LastRechargeTime：最后充值时间
        ///             MaturityTime：到期时间
        ///             CreateTime：创建时间
        ///             LastLoginTime：最后登录时间
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchMerchantRecharge(string name, int start = 1, int pageSize = 10)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            MerchantOperation merchantOperation = new MerchantOperation();
            FilterDefinition<Merchant> filter = merchantOperation.Builder.Where(t => t.AgentID == agentID && t.MaturityTime != null);
            if (!string.IsNullOrEmpty(name))
                filter &= merchantOperation.Builder.Regex(t => t.MeName, name);
            var merchantList = merchantOperation.GetModelListByPaging(filter, t => t.MaturityTime,
                false, start, pageSize);
            var total = await merchantOperation.GetCountAsync(filter);
            var result = (from data in merchantList
                          select new MerchantToWeb
                          {
                              Name = data.MeName,
                              CreateTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                              LastRechargeTime = data.RechargeTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                              MaturityTime = data.MaturityTime.ToString("yyyy-MM-dd HH:mm:ss"),
                              LastLoginTime = data.LoginTime?.ToString("yyyy-MM-dd HH:mm:ss"),
                          }).ToList();
            return Ok(new RecoverListModel<MerchantToWeb>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        private class MerchantToWeb
        {
            public string Name { get; set; }
            public string LastRechargeTime { get; set; }
            public string MaturityTime { get; set; }

            public string CreateTime { get; set; }

            public string LastLoginTime { get; set; }
        }
        #endregion
    }
}