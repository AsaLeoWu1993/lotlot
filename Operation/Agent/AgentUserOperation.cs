using Entity.AgentModel;
using MongoDB.Driver;
using Operation.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Operation.Agent
{
    /// <summary>
    /// 代理帐号
    /// </summary>
    public partial class AgentUserOperation : MongoMiddleware<AgentUser>
    {
        /// <summary>
        /// 加
        /// </summary>
        /// <param name="agentID">代理id</param>
        /// <param name="change">变动金额</param>
        /// <param name="accountType">变动类型</param>
        /// <param name="remark">备注</param>
        /// <param name="isRecharge">是否为充值</param>
        /// <returns></returns>
        public async Task UpScore(string agentID, decimal change, AccountTypeEnum accountType, string remark, bool isRecharge)
        {
            var agent = await base.GetModelAsync(t => t._id == agentID);
            agent.UserBalance += change;
            if (isRecharge)
                agent.CleanCode += change;
            await base.UpdateModelAsync(agent);

            //添加账变日志
            AccountingRecordOperation accountingRecordOperation = new AccountingRecordOperation();
            var result = new AccountingRecord()
            {
                Balance = agent.UserBalance,
                Remark = remark,
                VariableAmount = change,
                Type = accountType,
                AgentID = agentID
            };
            await accountingRecordOperation.InsertModelAsync(result);
        }

        /// <summary>
        /// 减
        /// </summary>
        /// <param name="agentID">代理id</param>
        /// <param name="change">变动金额</param>
        /// <param name="accountType">变动类型</param>
        /// <param name="remark">备注</param>
        /// <param name="rebate">是否返利</param>
        /// <param name="isRecharge">是否是提现</param>
        /// <returns></returns>
        public async Task DownScore(string agentID, decimal change, AccountTypeEnum accountType, string remark, bool rebate = true, bool isRecharge = false)
        {
            var agent = await base.GetModelAsync(t => t._id == agentID);
            agent.UserBalance -= change;
            if (isRecharge)
                agent.CleanCode = (agent.CleanCode - change) < 0 ? 0 : agent.CleanCode - change;
            await base.UpdateModelAsync(agent);

            //添加账变日志
            AccountingRecordOperation accountingRecordOperation = new AccountingRecordOperation();
            var result = new AccountingRecord()
            {
                Balance = agent.UserBalance,
                Remark = remark,
                VariableAmount = -change,
                Type = accountType,
                AgentID = agentID
            };
            await accountingRecordOperation.InsertModelAsync(result);
            if (rebate)
                await AgentRebate(agentID, agent.LoginName);
        }

        /// <summary>
        /// 逐级代理回水
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="agentName"></param>
        /// <returns></returns>
        private async Task AgentRebate(string agentID, string agentName = null)
        {
            var agent = await base.GetModelAsync(t => t._id == agentID);
            if (string.IsNullOrEmpty(agent.SupAgentID)) return;
            //查询上级
            var preAgent = await base.GetModelAsync(t => t._id == agent.SupAgentID);
            //代理差
            var price = agent.SubAgentPrice - preAgent.SubAgentPrice;
            if (price <= 0) return;
            await this.UpScore(agent.SupAgentID, price, AccountTypeEnum.团队返利,
                        string.Format("代理[{0}]返利", string.IsNullOrEmpty(agentName) ? agent.LoginName : agentName), false);

            await AgentRebate(agent.SupAgentID, agentName);
        }

        /// <summary>
        /// 获取代理层级关系
        /// </summary>
        /// <param name="agentID"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        public async Task<List<AgentRank>> GetAgentList(string agentID, string relation = null)
        {
            var result = new List<AgentRank>();
            var agentInfo = await this.GetModelAsync(t => t._id == agentID);
            if (agentInfo != null)
            {
                relation = string.IsNullOrEmpty(relation) ? agentInfo.LoginName : string.Format("{0} > {1}", relation, agentInfo.LoginName);
                result.Add(new AgentRank()
                {
                    AgentID = agentInfo._id,
                    AgentName = agentInfo.LoginName,
                    CreateTime = agentInfo.CreatedTime,
                    SubAgentID = agentInfo.SupAgentID,
                    Relation = relation
                });
                //查询下级
                FilterDefinition<AgentUser> filter = this.Builder.Eq(t => t.SupAgentID, agentID);
                var nextAgentList = await this.GetModelListAsync(filter);
                foreach (var agent in nextAgentList)
                {
                    var subList = await GetAgentList(agent._id, relation);
                    if (!subList.IsNull())
                        result.AddRange(subList);
                }
                return result;
            }
            return null;
        }

        /// <summary>
        /// 代理层级关系
        /// </summary>
        public class AgentRank
        {
            /// <summary>
            /// 代理id
            /// </summary>
            public string AgentID { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime { get; set; }

            /// <summary>
            /// 上级代理id
            /// </summary>
            public string SubAgentID { get; set; }

            /// <summary>
            /// 代理名称
            /// </summary>
            public string AgentName { get; set; }

            /// <summary>
            /// 关系链
            /// </summary>
            public string Relation { get; set; }
        }
    }

    public partial class AdvancedSetupOperation : MongoMiddleware<AdvancedSetup>
    {

    }
}
