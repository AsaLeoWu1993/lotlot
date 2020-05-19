namespace Entity.WebModel
{
    /// <summary>
    /// web对接房间
    /// </summary>
    public sealed class WebRoom
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 客服微信
        /// </summary>
        public string WeChat { get; set; }

        /// <summary>
        /// 上分帐户
        /// </summary>
        public SubAccount SubAccount { get; set; } = new SubAccount();

        /// <summary>
        /// 在线人数
        /// </summary>
        public int Online { get; set; }

        /// <summary>
        /// 虚拟玩家自动确认上下分请求勾选
        /// </summary>
        public bool ShamOnfirm { get; set; }

        /// <summary>
        /// 虚拟玩家自动确认上下分请求时间
        /// </summary>
        public int ShamOnfirmTime { get; set; }

        /// <summary>
        /// 800客服地址
        /// </summary>
        public string CustomerUrl { get; set; }

        /// <summary>
        /// 地址开关
        /// </summary>
        public bool CustomerOpen { get; set; }

        /// <summary>
        /// 管理员头像
        /// </summary>
        public string AdminPortrait { get; set; }
    }
}
