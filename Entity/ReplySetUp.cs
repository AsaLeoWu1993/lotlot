namespace Entity
{
    /// <summary>
    /// 回复消息设置
    /// </summary>
    public class ReplySetUp : BaseModel
    {
        /// <summary>
        /// 商户ID
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 游戏成功时回复
        /// </summary>
        public string GameSuccess { get; set; } = @"第{期号}期
{当前玩法明细}
[用:{当前使用分数}][余:{剩余}]";

        /// <summary>
        /// 查分时回复
        /// </summary>
        public string CheckScore { get; set; } = @"第{未开奖期号}期
{未开奖玩法明细}
[用:{当期使用分数}][余:{剩余}]";

        /// <summary>
        /// 查统计流水回复
        /// </summary>
        public string CheckStream { get; set; } = @"流:{当日流水}
盈:{当日盈亏}";

        /// <summary>
        /// 调整余额时回复
        /// </summary>
        public string Remainder { get; set; } = @"@{昵称}  调整学分[{变动分数}]余[{剩余分数}]";

        /// <summary>
        /// 自动回水时回复
        /// </summary>
        public string Backwater { get; set; } = @"[可返学分:{可返流水}]
[返:{返水金额}][余:{剩余}]";

        /// <summary>
        /// 禁止取消订单时回复
        /// </summary>
        public string ProhibitionCancel { get; set; } = "@{昵称}  取消失败。现在已禁止取消！";

        /// <summary>
        /// 玩家被锁定时回复
        /// </summary>
        public string UserLock { get; set; } = "[{昵称}]已被系统锁定，请联系群主！";

        /// <summary>
        /// 命令错误时回复
        /// </summary>
        public string CommandError { get; set; } = "@{昵称}  操作指令有误！";

        /// <summary>
        /// 收到请求时回复
        /// </summary>
        public string ReceivingRequests { get; set; } = "@{昵称}  收到请求，请稍等...！";

        /// <summary>
        /// 取消定单时回复
        /// </summary>
        public string CancelOrder { get; set; } = "@{昵称}  未结定单已取消！";

        /// <summary>
        /// 剩余不足时回复
        /// </summary>
        public string NotEnough { get; set; } = "@{昵称}  余额不足，余{剩余}";

        /// <summary>
        /// 封盘时回复
        /// </summary>
        public string Sealing { get; set; } = "@{昵称}  封盘中，操作无效！";

        /// <summary>
        /// 无下注订单时回复
        /// </summary>
        public string NotOrders { get; set; } = "@{昵称}  当期无下注定单！";

        /// <summary>
        /// 提交无效指令
        /// </summary>
        public bool NoticeInvalidSub { get; set; } = true;

        /// <summary>
        /// 下注时已封盘
        /// </summary>
        public bool NoticeSealing { get; set; } = true;

        /// <summary>
        /// 下注成功
        /// </summary>
        public bool NoticeBetSuccess { get; set; } = true;

        /// <summary>
        /// 取消玩法
        /// </summary>
        public bool NoticeCancel { get; set; } = true;

        /// <summary>
        /// 收到查回请求
        /// </summary>
        public bool NoticeCheckRequest { get; set; } = true;

        /// <summary>
        /// 确认查回分数
        /// </summary>
        public bool NoticeConfirmRequest { get; set; } = true;

        /// <summary>
        /// 下注积分不足
        /// </summary>
        public bool NoticeInsufficientIntegral { get; set; } = true;

        /// <summary>
        /// 下注发生限额
        /// </summary>
        public bool NoticeQuota { get; set; } = true;

        /// <summary>
        /// 回水设置
        /// </summary>
        public bool SwitchBackwater { get; set; } = true;
    }
}
