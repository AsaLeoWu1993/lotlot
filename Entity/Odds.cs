namespace Entity
{
    /// <summary>
    /// 赔率表
    /// </summary>
    public class Odds : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }
    }

    /// <summary>
    /// 通常赔率表
    /// </summary>
    public class OddsOrdinary : Odds
    {
        #region 数字
        /// <summary>
        /// 数字1
        /// </summary>
        public decimal Num1 { get; set; } = 9.8m;

        /// <summary>
        /// 数字2
        /// </summary>
        public decimal Num2 { get; set; } = 9.8m;

        /// <summary>
        /// 数字3
        /// </summary>
        public decimal Num3 { get; set; } = 9.8m;

        /// <summary>
        /// 数字4
        /// </summary>
        public decimal Num4 { get; set; } = 9.8m;

        /// <summary>
        /// 数字5
        /// </summary>
        public decimal Num5 { get; set; } = 9.8m;

        /// <summary>
        /// 数字6
        /// </summary>
        public decimal Num6 { get; set; } = 9.8m;

        /// <summary>
        /// 数字7
        /// </summary>
        public decimal Num7 { get; set; } = 9.8m;

        /// <summary>
        /// 数字8
        /// </summary>
        public decimal Num8 { get; set; } = 9.8m;

        /// <summary>
        /// 数字9
        /// </summary>
        public decimal Num9 { get; set; } = 9.8m;

        /// <summary>
        /// 数字10
        /// </summary>
        public decimal Num10 { get; set; } = 9.8m;
        #endregion
        #region 大小单双
        /// <summary>
        /// 大
        /// </summary>
        public decimal Da { get; set; } = 1.96m;

        /// <summary>
        /// 小
        /// </summary>
        public decimal Xiao { get; set; } = 1.96m;

        /// <summary>
        /// 单
        /// </summary>
        public decimal Dan { get; set; } = 1.96m;

        /// <summary>
        /// 双
        /// </summary>
        public decimal Shuang { get; set; } = 1.96m;
        #endregion
        #region 龙虎
        /// <summary>
        /// 龙
        /// </summary>
        public decimal Long { get; set; } = 1.96m;

        /// <summary>
        /// 虎
        /// </summary>
        public decimal Hu { get; set; } = 1.96m;
        #endregion
        #region 冠亚和数字
        /// <summary>
        /// 冠亚和3
        /// </summary>
        public decimal SNum3 { get; set; } = 40;

        /// <summary>
        /// 冠亚和4
        /// </summary>
        public decimal SNum4 { get; set; } = 40;

        /// <summary>
        /// 冠亚和5
        /// </summary>
        public decimal SNum5 { get; set; } = 20;

        /// <summary>
        /// 冠亚和6
        /// </summary>
        public decimal SNum6 { get; set; } = 20;

        /// <summary>
        /// 冠亚和7
        /// </summary>
        public decimal SNum7 { get; set; } = 12;

        /// <summary>
        /// 冠亚和8
        /// </summary>
        public decimal SNum8 { get; set; } = 12;

        /// <summary>
        /// 冠亚和9
        /// </summary>
        public decimal SNum9 { get; set; } = 9.5m;

        /// <summary>
        /// 冠亚和10
        /// </summary>
        public decimal SNum10 { get; set; } = 9.5m;

        /// <summary>
        /// 冠亚和11
        /// </summary>
        public decimal SNum11 { get; set; } = 7.5m;

        /// <summary>
        /// 冠亚和12
        /// </summary>
        public decimal SNum12 { get; set; } = 9.5m;

        /// <summary>
        /// 冠亚和13
        /// </summary>
        public decimal SNum13 { get; set; } = 9.5m;

        /// <summary>
        /// 冠亚和14
        /// </summary>
        public decimal SNum14 { get; set; } = 12;

        /// <summary>
        /// 冠亚和15
        /// </summary>
        public decimal SNum15 { get; set; } = 12;

        /// <summary>
        /// 冠亚和16
        /// </summary>
        public decimal SNum16 { get; set; } = 20;

        /// <summary>
        /// 冠亚和17
        /// </summary>
        public decimal SNum17 { get; set; } = 20;

        /// <summary>
        /// 冠亚和18
        /// </summary>
        public decimal SNum18 { get; set; } = 40;

        /// <summary>
        /// 冠亚和19
        /// </summary>
        public decimal SNum19 { get; set; } = 40;
        #endregion
        #region 冠亚和大小单双
        /// <summary>
        /// 冠亚和大
        /// </summary>
        public decimal SDa { get; set; } = 2.15m;

        /// <summary>
        /// 冠亚和小
        /// </summary>
        public decimal SXiao { get; set; } = 1.75m;

        /// <summary>
        /// 冠亚和单
        /// </summary>
        public decimal SDan { get; set; } = 1.75m;

        /// <summary>
        /// 冠亚和双
        /// </summary>
        public decimal SShuang { get; set; } = 2.15m;
        #endregion
    }

    /// <summary>
    /// 特殊赔率表
    /// </summary>
    public class OddsSpecial : Odds
    {
        #region 定位猜数字
        /// <summary>
        /// 0
        /// </summary>
        public decimal Num0 { get; set; } = 9.8m;

        /// <summary>
        /// 1
        /// </summary>
        public decimal Num1 { get; set; } = 9.8m;

        /// <summary>
        /// 2
        /// </summary>
        public decimal Num2 { get; set; } = 9.8m;

        /// <summary>
        /// 3
        /// </summary>
        public decimal Num3 { get; set; } = 9.8m;

        /// <summary>
        /// 4
        /// </summary>
        public decimal Num4 { get; set; } = 9.8m;

        /// <summary>
        /// 5
        /// </summary>
        public decimal Num5 { get; set; } = 9.8m;

        /// <summary>
        /// 6
        /// </summary>
        public decimal Num6 { get; set; } = 9.8m;

        /// <summary>
        /// 7
        /// </summary>
        public decimal Num7 { get; set; } = 9.8m;

        /// <summary>
        /// 8
        /// </summary>
        public decimal Num8 { get; set; } = 9.8m;

        /// <summary>
        /// 9
        /// </summary>
        public decimal Num9 { get; set; } = 9.8m;
        #endregion
        #region 定位猜大小单双
        /// <summary>
        /// 大
        /// </summary>
        public decimal Da { get; set; } = 1.96m;

        /// <summary>
        /// 小
        /// </summary>
        public decimal Xiao { get; set; } = 1.96m;

        /// <summary>
        /// 单
        /// </summary>
        public decimal Dan { get; set; } = 1.96m;

        /// <summary>
        /// 双
        /// </summary>
        public decimal Shuang { get; set; } = 1.96m;
        #endregion
        #region 龙虎和
        /// <summary>
        /// 龙
        /// </summary>
        public decimal Long { get; set; } = 1.96m;

        /// <summary>
        /// 虎
        /// </summary>
        public decimal Hu { get; set; } = 1.96m;

        /// <summary>
        /// 和
        /// </summary>
        public decimal He { get; set; } = 9;
        #endregion
        #region 和值大小单双
        /// <summary>
        /// 大
        /// </summary>
        public decimal CDa { get; set; } = 1.96m;

        /// <summary>
        /// 小
        /// </summary>
        public decimal CXiao { get; set; } = 1.96m;

        /// <summary>
        /// 单
        /// </summary>
        public decimal CDan { get; set; } = 1.96m;

        /// <summary>
        /// 双
        /// </summary>
        public decimal CShuang { get; set; } = 1.96m;
        #endregion
        #region 前中后三
        /// <summary>
        /// 豹子
        /// </summary>
        public decimal Baozi { get; set; } = 70;

        /// <summary>
        /// 顺子
        /// </summary>
        public decimal Shunzi { get; set; } = 13;

        /// <summary>
        /// 半顺
        /// </summary>
        public decimal Banshun { get; set; } = 2.3m;

        /// <summary>
        /// 对子
        /// </summary>
        public decimal Duizi { get; set; } = 2.9m;

        /// <summary>
        /// 杂六
        /// </summary>
        public decimal Zaliu { get; set; } = 2.7m;
        #endregion
    }

    /// <summary>
    /// 百家乐赔率
    /// </summary>
    public class OddsBaccarat : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 庄
        /// </summary>
        public decimal Banker { get; set; } = 1.95m;

        /// <summary>
        /// 闲
        /// </summary>
        public decimal Player { get; set; } = 2;

        /// <summary>
        /// 和
        /// </summary>
        public decimal He { get; set; } = 9;

        /// <summary>
        /// 大
        /// </summary>
        public decimal Da { get; set; } = 1.54m;

        /// <summary>
        /// 小
        /// </summary>
        public decimal Xiao { get; set; } = 2.5m;

        /// <summary>
        /// 庄对
        /// </summary>
        public decimal BankerPair { get; set; } = 12;

        /// <summary>
        /// 闲对
        /// </summary>
        public decimal PlayerPair { get; set; } = 12;

        /// <summary>
        /// 任意对子
        /// </summary>
        public decimal AnyPair { get; set; } = 6;

        /// <summary>
        ///完美对子
        /// </summary>
        public decimal PerfectPair { get; set; } = 20;
    }
}
