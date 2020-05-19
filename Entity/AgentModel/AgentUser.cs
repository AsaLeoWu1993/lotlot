using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Entity.AgentModel
{
    /// <summary>
    /// 代理帐号
    /// </summary>
    public class AgentUser : BaseModel
    {
        /// <summary>
        /// 登录名称
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 最后登录ip
        /// </summary>
        public string LastLoginIP { get; set; }

        /// <summary>
        /// 最后登录时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LastLoginTime { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal UserBalance { get; set; } = 0;

        /// <summary>
        /// 上级代理id
        /// </summary>
        public string SupAgentID { get; set; }

        /// <summary>
        /// 库存(月卡)
        /// </summary>
        public int Stock { get; set; } = 0;

        /// <summary>
        /// 上级代理价格
        /// </summary>
        public decimal SubAgentPrice { get; set; } = 0;

        /// <summary>
        /// 可开通代理帐号数量
        /// </summary>
        public int OpenAgentNum { get; set; } = 30;

        /// <summary>
        /// 安全密码
        /// </summary>
        public string SafePassWord { get; set; }

        /// <summary>
        /// 锁定时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LockTime { get; set; }

        /// <summary>
        /// 输入安全码次数
        /// </summary>
        public int ErrorCount { get; set; } = 0;

        /// <summary>
        /// 代理帐户状态
        /// </summary>
        public AgentStatusEnum Status { get; set; } = AgentStatusEnum.正常;

        /// <summary>
        /// 是否为最高级代理
        /// </summary>
        public bool IsHighest { get; set; } = false;

        /// <summary>
        /// 最高级代理id
        /// </summary>
        public string HighestAgentID { get; set; }

        /// <summary>
        /// 免费时长
        /// </summary>
        public int FreeDuration { get; set; } = 100;

        /// <summary>
        /// 超出时长费用
        /// </summary>
        public decimal ExcessExpenses { get; set; } = 1;

        /// <summary>
        /// 代理极差值
        /// </summary>
        [Obsolete]
        public decimal AgencyMargin { get; set; } = 100;

        /// <summary>
        /// 管理类型
        /// </summary>
        public ManagementTypeEnum ManagementType { get; set; } = ManagementTypeEnum.普通类型;

        /// <summary>
        /// 洗码量
        /// </summary>
        public decimal CleanCode { get; set; } = 0;
    }

    public enum AgentStatusEnum
    {
        正常 = 1,
        锁定 = 2,
        删除 = 3
    }

    /// <summary>
    /// 管理类型
    /// </summary>
    public enum ManagementTypeEnum
    { 
        超级管理类型 = 1,
        财务类型 = 2,
        普通类型 = 3
    }
}
