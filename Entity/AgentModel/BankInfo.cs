namespace Entity.AgentModel
{
    /// <summary>
    /// 银行卡信息
    /// </summary>
    public class BankInfo : BaseModel
    {
        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 银行类型
        /// </summary>
        public BankEnum BankType { get; set; }

        /// <summary>
        /// 收款帐户
        /// </summary>
        public string BankNum { get; set; }
    }

    /// <summary>
    /// 银行列表 
    /// </summary>
    public enum BankEnum
    {
        中国农业银行 = 1,
        中国工商银行 = 2,
        中国建设银行 = 3,
        中国招商银行 = 4,
        中国银行 = 5,
        交通银行 = 6
    }
}
