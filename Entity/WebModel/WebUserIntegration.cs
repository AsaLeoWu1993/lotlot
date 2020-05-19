namespace Entity.WebModel
{
    public sealed class WebUserIntegration
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 变动类型
        /// </summary>
        public ChangeTypeEnum ChangeType { get; set; }

        /// <summary>
        /// 积分数量
        /// </summary>
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; } = 0;

        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 变动时间
        /// </summary>
        public string ChangeTime { get; set; }
    }

    public class WebUserIntegrationHome
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 金额
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 申请时间
        /// </summary>
        public string ApplyTime { get; set; }

        /// <summary>
        /// 唯一码
        /// </summary>
        public string OnlyCode { get; set; }

        /// <summary>
        /// 消息  上分/下分
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public ManagementEnum Management { get; set; }

        /// <summary>
        /// 上下分类型
        /// </summary>
        public ChangeTypeEnum ChangeType { get; set; }

        /// <summary>
        /// 余额
        /// </summary>
        public decimal UserMoney { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public string Status { get; set; }
    }
}
