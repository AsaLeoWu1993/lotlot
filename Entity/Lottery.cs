namespace Entity
{
    #region 赛车  飞艇  极速  澳10
    /// <summary>
    /// 开奖结果表（赛车  飞艇  极速  澳10）
    /// </summary>
    public class Lottery10 : BaseModel
    {
        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string IssueNum { get; set; }

        /// <summary>
        /// 开奖时间
        /// </summary>
        public string LotteryTime { get; set; }

        /// <summary>
        /// 1
        /// </summary>
        public string Num1 { get; set; }

        /// <summary>
        ///2
        /// </summary>
        public string Num2 { get; set; }

        /// <summary>
        /// 3
        /// </summary>
        public string Num3 { get; set; }

        /// <summary>
        /// 4
        /// </summary>
        public string Num4 { get; set; }

        /// <summary>
        /// 5
        /// </summary>
        public string Num5 { get; set; }

        /// <summary>
        /// 6
        /// </summary>
        public string Num6 { get; set; }

        /// <summary>
        /// 7
        /// </summary>
        public string Num7 { get; set; }

        /// <summary>
        /// 8
        /// </summary>
        public string Num8 { get; set; }

        /// <summary>
        /// 9
        /// </summary>
        public string Num9 { get; set; }

        /// <summary>
        /// 10
        /// </summary>
        public string Num10 { get; set; }

        /// <summary>
        /// 冠亚和
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 冠亚和大小
        /// </summary>
        public SizeEnum CountSize { get; set; }

        /// <summary>
        /// 冠亚和单双
        /// </summary>
        public SindouEnum Sindou { get; set; }

        /// <summary>
        /// 龙虎1
        /// </summary>
        public DraTig DraTig1 { get; set; }

        /// <summary>
        /// 龙虎2
        /// </summary>
        public DraTig DraTig2 { get; set; }

        /// <summary>
        /// 龙虎3
        /// </summary>
        public DraTig DraTig3 { get; set; }

        /// <summary>
        /// 龙虎4
        /// </summary>
        public DraTig DraTig4 { get; set; }

        /// <summary>
        /// 龙虎5
        /// </summary>
        public DraTig DraTig5 { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }
    }

    public enum SizeEnum
    {
        大 = 1,
        小 = 2
    }

    public enum SindouEnum
    {
        单 = 1,
        双 = 2
    }

    public enum DraTig
    {
        龙 = 1,
        虎 = 2
    }
    #endregion

    #region 时时彩  澳5
    /// <summary>
    /// 开奖结果表（时时彩  澳5）
    /// </summary>
    public class Lottery5 : BaseModel
    {
        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string IssueNum { get; set; }

        /// <summary>
        /// 开奖时间
        /// </summary>
        public string LotteryTime { get; set; }

        /// <summary>
        /// 1
        /// </summary>
        public string Num1 { get; set; }

        /// <summary>
        ///2
        /// </summary>
        public string Num2 { get; set; }

        /// <summary>
        /// 3
        /// </summary>
        public string Num3 { get; set; }

        /// <summary>
        /// 4
        /// </summary>
        public string Num4 { get; set; }

        /// <summary>
        /// 5
        /// </summary>
        public string Num5 { get; set; }

        /// <summary>
        /// 总和
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 总和大小
        /// </summary>
        public SizeEnum CountSize { get; set; }

        /// <summary>
        /// 总和单双
        /// </summary>
        public SindouEnum CountSinDou { get; set; }

        /// <summary>
        /// 龙虎和
        /// </summary>
        public DraTig5 DraTig { get; set; }

        /// <summary>
        /// 前三
        /// </summary>
        public RuleEnum Front3 { get; set; }

        /// <summary>
        /// 中三
        /// </summary>
        public RuleEnum Middle3 { get; set; }

        /// <summary>
        /// 后三
        /// </summary>
        public RuleEnum Back3 { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }
    }

    public enum DraTig5
    {
        龙 = 1,
        虎 = 2,
        和 = 3
    }

    public enum RuleEnum
    {
        豹子 = 1,
        顺子 = 2,
        半顺 = 3,
        对子 = 4,
        杂六 = 5
    }
    #endregion
}
