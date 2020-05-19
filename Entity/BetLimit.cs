using System;
using System.ComponentModel;

namespace Entity
{
    /// <summary>
    /// 投注限额表
    /// </summary>
    public class BetLimit : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 单个玩家所有玩法总限额
        /// </summary>
        public decimal TotalSingleLimit { get; set; } = 100000;

        /// <summary>
        /// 所有玩家所有玩法总限额
        /// </summary>
        public decimal AllTotalQuotas { get; set; } = 500000;
    }

    /// <summary>
    /// 限额公共属性
    /// </summary>
    public class QuotaAttrInfo
    {
        /// <summary>
        /// 最小投注限额
        /// </summary>
        public decimal MinBet { get; set; } = 2;

        /// <summary>
        /// 最单投注限额
        /// </summary>
        public decimal MaxBet { get; set; } = 2000;

        /// <summary>
        /// 所有人最单投注
        /// </summary>
        public decimal AllMaxBet { get; set; } = 5000;

        /// <summary>
        /// 网盘单手限额
        /// </summary>
        public decimal SingleBet { get; set; } = 10000;
    }

    /// <summary>
    /// 10球游戏
    /// </summary>
    public class BetLimitOrdinary : BetLimit
    {
        #region 数字
        /// <summary>
        /// 1-10名猜数字
        /// </summary>
        [Description("1-10数字")]
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
        [Description("1-10大小单双")]
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
        [Description("1-5龙虎")]
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
        [Description("冠亚和大小单双")]
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
        [Description("冠亚和数字")]
        public QuotaAttrInfo GuessGYHNum { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 100000,
            AllMaxBet = 300000
        };
        #endregion
    }

    public class BetLimitSpecial : BetLimit
    {
        #region 定位数字
        /// <summary>
        /// 定位猜数字
        /// </summary>
        [Description("1-5数字")]
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
        [Description("定位大小单双")]
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
        [Description("龙虎")]
        public QuotaAttrInfo GuessLongHu { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 100000,
            AllMaxBet = 300000
        };

        /// <summary>
        /// 猜和
        /// </summary>
        [Description("和")]
        public QuotaAttrInfo GuessHe { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 30000,
            AllMaxBet = 60000
        };

        /// <summary>
        /// 总和值猜大小单双
        /// </summary>
        [Description("总和大小单双")]
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
        [Description("豹子")]
        public QuotaAttrInfo GuessBaozi { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 10000,
            AllMaxBet = 20000
        };

        /// <summary>
        /// 顺子
        /// </summary>
        [Description("顺子")]
        public QuotaAttrInfo GuessShunzi { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 10000,
            AllMaxBet = 20000
        };

        /// <summary>
        /// 半顺
        /// </summary>
        [Description("半顺")]
        public QuotaAttrInfo GuessBanshun { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 50000,
            AllMaxBet = 100000
        };

        /// <summary>
        /// 对子
        /// </summary>
        [Description("对子")]
        public QuotaAttrInfo GuessDuizi { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 50000,
            AllMaxBet = 100000
        };

        /// <summary>
        /// 杂六
        /// </summary>
        [Description("杂六")]
        public QuotaAttrInfo GuessZaliu { get; set; } = new QuotaAttrInfo()
        {
            MaxBet = 50000,
            AllMaxBet = 100000
        };
        #endregion
    }

    /// <summary>
    /// 百家乐限额
    /// </summary>
    public class BetLimitBaccarat : BaseModel
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 单个玩家所有玩法总限额
        /// </summary>
        public decimal TotalSingleLimit { get; set; } = 50000;

        /// <summary>
        /// 所有玩家所有玩法总限额
        /// </summary>
        public decimal AllTotalQuotas { get; set; } = 100000;

        /// <summary>
        /// 庄闲单小
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
