namespace Entity.AgentModel
{
    /// <summary>
    /// 提现
    /// </summary>
    public class CashWithdrawal : BaseModel
    {
        /// <summary>
        /// 申请代理id
        /// </summary>
        public string ApplyAgentID { get; set; }

        /// <summary>
        /// 受理代理id
        /// </summary>
        public string AcceptanceAgentID { get; set; }

        /// <summary>
        /// 申请代理名称
        /// </summary>
        public string ApplyAgentName { get; set; }

        /// <summary>
        /// 申请类型
        /// </summary>
        public ApplyTypeEnum ApplyType { get; set; }

        /// <summary>
        /// 提现状态
        /// </summary>
        public WithdrawalStatusEnum Status { get; set; } = WithdrawalStatusEnum.申请;

        /// <summary>
        /// 申请金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 受理账户
        /// </summary>
        public string Acceptance { get; set; }
    }

    public enum ApplyTypeEnum
    {
        银行卡 = 1,
        支付宝 = 2,
        泰达币 = 3
    }

    public enum WithdrawalStatusEnum
    {
        申请 = 1,
        已放款 = 2,
        已拒绝 = 3
    }

    /// <summary>
    /// 充值
    /// </summary>
    public class Recharge : BaseModel
    {
        /// <summary>
        /// 申请代理id
        /// </summary>
        public string ApplyAgentID { get; set; }

        /// <summary>
        /// 受理代理id
        /// </summary>
        public string AcceptanceAgentID { get; set; }

        /// <summary>
        /// 申请代理名称
        /// </summary>
        public string ApplyAgentName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// 实际金额
        /// </summary>
        public decimal ActualAmount { get; set; } = 0;

        /// <summary>
        /// 是否确认
        /// </summary>
        public bool IsHandle { get; set; } = false;

        /// <summary>
        /// 订单方式
        /// </summary>
        public string OrderType { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public ROrderStatusEnum OrderStatus { get; set; } = ROrderStatusEnum.成功;
    }

    public enum ROrderStatusEnum
    {
        成功 = 1,
        失败 = 2
    }
}
