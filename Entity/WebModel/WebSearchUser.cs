namespace Entity.WebModel
{
    /// <summary>
    /// 查询用户列表返回数据
    /// </summary>
    public sealed class WebSearchUser
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal UserMoney { get; set; } = 0;

        /// <summary>
        /// 代理
        /// </summary>
        public bool IsAgent { get; set; } = false;

        /// <summary>
        /// 飞单状态
        /// </summary>
        public bool Record { get; set; } = false;

        /// <summary>
        /// 用户等级
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 唯一码
        /// </summary>
        public string OnlyCode { get; set; }

        /// <summary>
        /// 下注游戏名称
        /// </summary>
        public string BetGameName { get; set; }

        /// <summary>
        /// 盈亏
        /// </summary>
        public decimal ProLoss { get; set; }

        /// <summary>
        /// 机器人托
        /// </summary>
        public bool IsSupport { get; set; }
    }
}
