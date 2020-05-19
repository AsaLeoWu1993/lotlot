using System.Collections.Generic;

namespace Entity.WebModel
{
    /// <summary>
    /// 对接web基础设置表
    /// </summary>
    public sealed class WebFoundationSetup
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 封盘前时间
        /// </summary>
        public int EntertainedFrontTime { get; set; }

        /// <summary>
        /// 封盘前消息
        /// </summary>
        public string EntertainedFrontMsg { get; set; }

        /// <summary>
        /// 开奖前时间
        /// </summary>
        public List<LotteryItem> LotteryFrontTime { get; set; }

        /// <summary>
        /// 开奖前消息
        /// </summary>
        public string LotteryFrontMsg { get; set; }

        /// <summary>
        /// 封盘后时间
        /// </summary>
        public int EntertainedAfterTime { get; set; }

        /// <summary>
        /// 封盘后消息
        /// </summary>
        public string EntertainedAfterMsg { get; set; }

        /// <summary>
        /// 中奖明细
        /// </summary>
        public string WinningDetails { get; set; }

        /// <summary>
        /// 会员积分
        /// </summary>
        public string MembershipScore { get; set; }

        /// <summary>
        /// 自定义时间
        /// </summary>
        public int CustomTime { get; set; }

        /// <summary>
        /// 自定义消息
        /// </summary>
        public string CustomMsg { get; set; }

        /// <summary>
        /// 封盘前禁止撤单
        /// </summary>
        public bool ProhibitChe { get; set; }

        /// <summary>
        /// 显示表单账单
        /// </summary>
        public bool ShowBillTable { get; set; }

        /// <summary>
        /// 结算内容格式
        /// </summary>
        public string Settlement { get; set; }

        /// <summary>
        /// 未中奖结算内容格式
        /// </summary>
        public string NotSettlement { get; set; }
    }
}
