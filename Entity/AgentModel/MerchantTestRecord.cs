using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Entity.AgentModel
{
    /// <summary>
    /// 商户充值测试时长日志表
    /// </summary>
    public class MerchantTestRecord : BaseModel
    {
        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 时长
        /// </summary>
        public int Duration { get; set; }
    }

    /// <summary>
    /// 销售记录
    /// </summary>
    public class SalesRecords : BaseModel
    {
        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 销售方式
        /// </summary>
        public SalesTypeEnum SalesType { get; set; }

        /// <summary>
        /// 使用金额
        /// </summary>
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// 到期时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime MaturityTime { get; set; }
    }

    /// <summary>
    /// 销售方式
    /// </summary>
    public enum SalesTypeEnum
    {
        余额 = 1,
        库存 = 2,
        临时 = 3
    }

    /// <summary>
    /// 账变记录
    /// </summary>
    public class AccountingRecord : BaseModel
    {
        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; } = DateTime.Now.ToString("yyyMMddHHmmss");

        /// <summary>
        /// 变动金额
        /// </summary>
        public decimal VariableAmount { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// 类型
        /// </summary>
        public AccountTypeEnum Type { get; set; }

        /// <summary>
        /// 摘要
        /// </summary>
        public string Remark { get; set; }
    }

    public enum AccountTypeEnum
    {
        余额充值 = 1,
        直充扣费 = 2,
        余额提现 = 3,
        团队返利 = 4,
        提现退回 = 5,
        活动 = 6,
        其他 = 7,
        库存充值 = 8,
        转账 = 9
    }
}
