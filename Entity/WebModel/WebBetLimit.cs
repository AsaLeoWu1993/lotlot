namespace Entity.WebModel
{
    public class WebBetLimit
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 单个玩家所有玩法总限额
        /// </summary>
        public decimal TotalSingleLimit { get; set; }

        /// <summary>
        /// 所有玩家所有玩法总限额
        /// </summary>
        public decimal AllTotalQuotas { get; set; }
    }

    /// <summary>
    /// 通常限额
    /// </summary>
    public sealed class WebBetLimitOrdinary : WebBetLimit
    {
        #region 数字
        /// <summary>
        /// 1-10名猜数字
        /// </summary>
        public QuotaAttrInfo GuessNum { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 30000,
            AllMaxBet = 60000
        };
        #endregion

        #region 大小单双
        /// <summary>
        /// 1-10名猜大小单双
        /// </summary>
        public QuotaAttrInfo GuessDxds { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 100000,
            AllMaxBet = 300000
        };
        #endregion

        #region 龙虎
        /// <summary>
        /// 1-5名猜龙虎
        /// </summary>
        public QuotaAttrInfo GuessLongHu { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 100000,
            AllMaxBet = 300000
        };
        #endregion

        #region 冠亚和单小单双
        /// <summary>
        /// 冠亚和值猜大小单双
        /// </summary>
        public QuotaAttrInfo GuessGYHDxds { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 100000,
            AllMaxBet = 300000
        };
        #endregion

        #region 冠亚和数字
        /// <summary>
        /// 冠亚和值猜数字
        /// </summary>
        public QuotaAttrInfo GuessGYHNum { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 100000,
            AllMaxBet = 300000
        };
        #endregion
    }

    /// <summary>
    /// 特殊限额
    /// </summary>
    public sealed class WebBetLimitSpecial : WebBetLimit
    {
        #region 定位数字
        /// <summary>
        /// 定位猜数字
        /// </summary>
        public QuotaAttrInfo GuessNum { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 30000,
            AllMaxBet = 60000
        };
        #endregion

        #region 定位大小单双
        /// <summary>
        /// 定位猜单小单双
        /// </summary>
        public QuotaAttrInfo GuessDxds { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 100000,
            AllMaxBet = 300000
        };
        #endregion

        #region 龙虎和 总和
        /// <summary>
        /// 猜龙虎
        /// </summary>
        public QuotaAttrInfo GuessLongHu { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 100000,
            AllMaxBet = 300000
        };

        /// <summary>
        /// 猜和
        /// </summary>
        public QuotaAttrInfo GuessHe { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 30000,
            AllMaxBet = 60000
        };

        /// <summary>
        /// 总和值猜大小单双
        /// </summary>
        public QuotaAttrInfo GuessCountDxds { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 100000,
            AllMaxBet = 300000
        };
        #endregion

        #region 特殊
        /// <summary>
        /// 豹子
        /// </summary>
        public QuotaAttrInfo GuessBaozi { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 10000,
            AllMaxBet = 20000
        };

        /// <summary>
        /// 顺子
        /// </summary>
        public QuotaAttrInfo GuessShunzi { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 10000,
            AllMaxBet = 20000
        };

        /// <summary>
        /// 半顺
        /// </summary>
        public QuotaAttrInfo GuessBanshun { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 50000,
            AllMaxBet = 100000
        };

        /// <summary>
        /// 对子
        /// </summary>
        public QuotaAttrInfo GuessDuizi { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 50000,
            AllMaxBet = 100000
        };

        /// <summary>
        /// 杂六
        /// </summary>
        public QuotaAttrInfo GuessZaliu { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 50000,
            AllMaxBet = 100000
        };
        #endregion
    }

    /// <summary>
    /// 视讯限额
    /// </summary>
    public sealed class WebVideoBetLimit
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 单个玩家所有玩法总限额
        /// </summary>
        public decimal TotalSingleLimit { get; set; } = 50000;

        /// <summary>
        /// 所有玩家所有玩法总限额
        /// </summary>
        public decimal AllTotalQuotas { get; set; } = 100000;

        /// <summary>
        /// 庄闲大小
        /// </summary>
        public QuotaAttrInfo GuessQueue { get; set; } = new QuotaAttrInfo()
        {
            MinBet = 5,
            MaxBet = 5000,
            AllMaxBet = 20000
        };

        /// <summary>
        /// 庄闲对
        /// </summary>
        public QuotaAttrInfo GuessBPPair { get; set; } = new QuotaAttrInfo()
        {
            MinBet = 5,
            MaxBet = 3000,
            AllMaxBet = 20000
        };

        /// <summary>
        /// 任意/完美对子
        /// </summary>
        public QuotaAttrInfo GuessAPPair { get; set; } = new QuotaAttrInfo()
        {
            MinBet = 5,
            MaxBet = 2000,
            AllMaxBet = 10000
        };

        /// <summary>
        /// 和
        /// </summary>
        public QuotaAttrInfo GuessHe { get; set; } = new QuotaAttrInfo()
        {
            MinBet = 5,
            MaxBet = 2000,
            AllMaxBet = 10000
        };
    }
}
