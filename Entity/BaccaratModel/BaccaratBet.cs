using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Entity.BaccaratModel
{
    /// <summary>
    /// 用户百家乐下注
    /// </summary>
    public class BaccaratBet : BaseModel
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string Nper { get; set; }

        /// <summary>
        /// 当期所有投注信息
        /// </summary>
        public List<BaccaratBetRemarkInfo> BetRemarks { get; set; } = new List<BaccaratBetRemarkInfo>();

        /// <summary>
        /// 注单
        /// </summary>
        public NotesEnum Notes { get; set; } = NotesEnum.正常;

        /// <summary>
        /// 桌号
        /// </summary>
        public int ZNum { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public BaccaratGameType GameType { get; set; } = BaccaratGameType.百家乐;

        /// <summary>
        /// 是否返水
        /// </summary>
        public bool IsBackwater { get; set; } = false;

        /// <summary>
        /// 游戏开奖状态
        /// </summary>
        public BetStatus BetStatus { get; set; } = BetStatus.未开奖;

        /// <summary>
        /// 总使用分数
        /// </summary>
        public decimal AllUseMoney { get; set; } = 0;

        /// <summary>
        /// 总中奖金额
        /// </summary>
        public decimal AllMediumBonus { get; set; } = 0;
    }

    /// <summary>
    /// 百家乐下注状态
    /// </summary>
    public enum BaccaratBetEnum
    {
        已投注 = 1,
        未中奖 = 2,
        已中奖 = 3,
        已取消 = 4,
        已退注 = 5,
        开和退注 = 6,
        开和退一半 = 7
    }

    /// <summary>
    /// 百家乐下注类型
    /// </summary>
    public enum BaccaratBetType
    {
        庄 = 1,
        闲 = 2,
        和 = 3,
        庄对 = 4,
        闲对 = 5,
        任意对子 = 6,
        //完美对子 = 7,
        //大 = 8,
        //小 = 9
    }

    /// <summary>
    /// 当期下注信息
    /// </summary>
    public class BaccaratBetRemarkInfo
    {
        /// <summary>
        /// 下注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OddNum { get; set; } = Guid.NewGuid().ToString().Replace("-", "");

        /// <summary>
        /// 下注时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BetTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal BetMoney { get; set; }

        /// <summary>
        /// 中奖金额
        /// </summary>
        public decimal MediumBonus { get; set; }

        /// <summary>
        /// 注单状态
        /// </summary>
        public BaccaratBetEnum BetStatus { get; set; }

        /// <summary>
        /// 注单类型
        /// </summary>
        public BaccaratBetType BetRule { get; set; }
    }
}
