namespace Entity
{
    /// <summary>
    /// 房间
    /// </summary>
    public sealed class Room : BaseModel
    {
        /// <summary>
        /// 商户ID
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 客服微信
        /// </summary>
        public string WeChat { get; set; }

        /// <summary>
        /// 在线人数
        /// </summary>
        public int Online { get; set; } = 0;

        /// <summary>
        /// 上分帐户
        /// </summary>
        public SubAccount SubAccount { get; set; } = new SubAccount();

        /// <summary>
        /// 房间号
        /// </summary>
        public string RoomNum { get; set; }

        /// <summary>
        /// 虚拟玩家自动确认上下分请求勾选
        /// </summary>
        public bool ShamOnfirm { get; set; } = true;

        /// <summary>
        /// 虚拟玩家自动确认上下分请求时间
        /// </summary>
        public int ShamOnfirmTime { get; set; } = 15;

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

    public class SubAccount
    {
        /// <summary>
        /// 微信帐号
        /// </summary>
        public string WeChatNum { get; set; }

        /// <summary>
        /// 微信二维码
        /// </summary>
        public string WeChatQRcode { get; set; }

        /// <summary>
        /// 支付宝帐号
        /// </summary>
        public string AlipayNum { get; set; }

        /// <summary>
        /// 支付宝二维码
        /// </summary>
        public string AlipayQRcode { get; set; }
    }
}
