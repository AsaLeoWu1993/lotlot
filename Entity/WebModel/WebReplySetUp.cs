namespace Entity.WebModel
{
    public class WebReplySetUp
    {
        /// <summary>
        /// 游戏成功时回复
        /// </summary>
        public string GameSuccess { get; set; }

        /// <summary>
        /// 查分时回复
        /// </summary>
        public string CheckScore { get; set; }

        /// <summary>
        /// 查统计流水回复
        /// </summary>
        public string CheckStream { get; set; }

        /// <summary>
        /// 调整余额时回复
        /// </summary>
        public string Remainder { get; set; }

        /// <summary>
        /// 自动回水时回复
        /// </summary>
        public string Backwater { get; set; }

        /// <summary>
        /// 禁止取消订单时回复
        /// </summary>
        public string ProhibitionCancel { get; set; }

        /// <summary>
        /// 玩家被锁定时回复
        /// </summary>
        public string UserLock { get; set; }

        /// <summary>
        /// 命令错误时回复
        /// </summary>
        public string CommandError { get; set; }

        /// <summary>
        /// 收到请求时回复
        /// </summary>
        public string ReceivingRequests { get; set; }

        /// <summary>
        /// 取消定单时回复
        /// </summary>
        public string CancelOrder { get; set; }

        /// <summary>
        /// 剩余不足时回复
        /// </summary>
        public string NotEnough { get; set; }

        /// <summary>
        /// 封盘时回复
        /// </summary>
        public string Sealing { get; set; }

        /// <summary>
        /// 无下注订单时回复
        /// </summary>
        public string NotOrders { get; set; }

        /// <summary>
        /// 提交无效指令
        /// </summary>
        public bool NoticeInvalidSub { get; set; }

        /// <summary>
        /// 下注时已封盘
        /// </summary>
        public bool NoticeSealing { get; set; }

        /// <summary>
        /// 下注成功
        /// </summary>
        public bool NoticeBetSuccess { get; set; }

        /// <summary>
        /// 取消玩法
        /// </summary>
        public bool NoticeCancel { get; set; }

        /// <summary>
        /// 收到查回请求
        /// </summary>
        public bool NoticeCheckRequest { get; set; }

        /// <summary>
        /// 确认查回分数
        /// </summary>
        public bool NoticeConfirmRequest { get; set; }

        /// <summary>
        /// 下注积分不足
        /// </summary>
        public bool NoticeInsufficientIntegral { get; set; }

        /// <summary>
        /// 下注发生限额
        /// </summary>
        public bool NoticeQuota { get; set; }


        /// <summary>
        /// 回水设置
        /// </summary>
        public bool SwitchBackwater { get; set; }
    }
}
