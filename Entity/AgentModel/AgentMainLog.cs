namespace Entity.AgentModel
{
    /// <summary>
    /// 代理高级设置日志
    /// </summary>
    public class AgentMainLog : BaseModel
    {
        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        /// <summary>
        /// 登录ip
        /// </summary>
        public string LoginIP { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationMsg { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public OperationStatusEnum Status { get; set; } = OperationStatusEnum.成功;

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

    public enum OperationStatusEnum
    {
        成功 = 1,
        失败 = 2
    }

    /// <summary>
    /// 加扣款
    /// </summary>
    public enum DeductionStatusEnum
    {
        加款 = 1,
        扣款 = 2
    }

    /// <summary>
    /// 加扣款类型
    /// </summary>
    public enum DeductionTypeEnum
    {
        充值 = 1,
        提现 = 2,
        冻结 = 3,
        违规 = 4,
        活动 = 5,
        其他 = 6
    }
}
