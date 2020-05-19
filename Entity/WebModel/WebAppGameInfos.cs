using System;

namespace Entity.WebModel
{
    public class WebAppGameInfos
    {
        /// <summary>
        /// 期号
        /// </summary>
        public string IssueNum { get; set; }

        /// <summary>
        /// 下期期号
        /// </summary>
        public string NextIssueNum { get; set; }

        /// <summary>
        /// 号码
        /// </summary>
        public string Number { get; set; }

        /// <summary>
        /// 其它信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 剩余时间  秒
        /// </summary>
        public int Surplus { get; set; }

        /// <summary>
        /// 游戏状态
        /// </summary>
        public GameStatusEnum Status { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 封盘时间
        /// </summary>
        public int SealingTime { get; set; } = 0;

        /// <summary>
        /// 开奖时间
        /// </summary>
        public DateTime StartTime { get; set; }
    }

    /// <summary>
    /// 监控前端数据
    /// </summary>
    public class WebMonitorInfos : WebAppGameInfos
    {
        /// <summary>
        /// 今日总盈亏
        /// </summary>
        public decimal ProfitLoss { get; set; }

        /// <summary>
        /// 上期总投注
        /// </summary>
        public decimal PreBetMoney { get; set; }

        /// <summary>
        /// 上期总盈亏
        /// </summary>
        public decimal PreProfitLoss { get; set; }
    }

    public enum GameStatusEnum
    {
        等待中 = 1,
        封盘中 = 2,
        开奖中 = 3,
        已停售 = 4,
        未开奖 = 5,
        已关闭 = 6
    }
}
