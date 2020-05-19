using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Entity.AgentModel
{
    /// <summary>
    /// 兑换码
    /// </summary>
    public class Redeem : BaseModel
    {
        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        /// <summary>
        /// 使用代理id
        /// </summary>
        public string UseAgentID { get; set; }

        /// <summary>
        /// 使用代理登录名称
        /// </summary>
        public string UseAgentName { get; set; }

        /// <summary>
        /// 有效时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime EffectiveTime { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public RedeemEnum Status { get; set; }

        /// <summary>
        /// 使用时间
        /// </summary>
        public DateTime? UseTime { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 兑换码
        /// </summary>
        public string RedeemCode { get; set; }
    }

    public enum RedeemEnum
    {
        未使用 = 1,
        已使用 = 2
    }
}
