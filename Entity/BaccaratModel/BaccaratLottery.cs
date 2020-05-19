using System.ComponentModel;

namespace Entity.BaccaratModel
{
    /// <summary>
    /// 百家乐开奖
    /// </summary>
    public class BaccaratLottery : BaseModel
    {
        /// <summary>
        /// 房间号
        /// </summary>
        public int ZNum { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string IssueNum { get; set; }

        ///// <summary>
        ///// 庄牌
        ///// </summary>
        //public List<Brand> Zp { get; set; } = new List<Brand>();

        ///// <summary>
        ///// 闲牌
        ///// </summary>
        //public List<Brand> Xp { get; set; } = new List<Brand>();

        ///// <summary>
        ///// 庄家点数
        ///// </summary>
        //public int ZPoint { get; set; }

        ///// <summary>
        ///// 闲家点数
        ///// </summary>
        //public int XPoint { get; set; }

        /// <summary>
        /// 开奖结果
        /// </summary>
        public BaccaratWinEnum Result { get; set; }

        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public BaccaratGameType GameType { get; set; }
    }

    /// <summary>
    /// 游戏类型
    /// </summary>
    public enum BaccaratGameType
    {
        百家乐 = 1,
        龙虎 = 2,
        牛牛 = 3
    }

    /// <summary>
    /// 开奖结果
    /// </summary>
    public enum BaccaratWinEnum
    {
        未结 = 0,
        [Description("庄")]
        庄 = 1,
        [Description("闲")]
        闲 = 2,
        [Description("和")]
        和 = 3,
        [Description("庄-庄对")]
        庄_庄对 = 4,
        [Description("闲-庄对")]
        闲_庄对 = 5,
        [Description("和-庄对")]
        和_庄对 = 6,
        [Description("庄-闲对")]
        庄_闲对 = 7,
        [Description("闲-闲对")]
        闲_闲对 = 8,
        [Description("和-闲对")]
        和_闲对 = 9,
        [Description("庄-庄闲对")]
        庄_庄闲对 = 10,
        [Description("闲-庄闲对")]
        闲_庄闲对 = 11,
        [Description("和-庄闲对")]
        和_庄闲对 = 12,
        退注 = 99
    }

    /// <summary>
    /// 牌
    /// </summary>
    public class Brand
    {
        /// <summary>
        /// 牌花色
        /// </summary>
        public CardTypeEnum CardType { get; set; }

        /// <summary>
        /// 牌号
        /// </summary>
        public string Num { get; set; }
    }

    /// <summary>
    /// 牌花色
    /// </summary>
    public enum CardTypeEnum
    {
        [Description("◆")]
        方块 = 1,
        [Description("♣")]
        梅花 = 2,
        [Description("♥")]
        红心 = 3,
        [Description("♠")]
        黑桃 = 4
    }
}
