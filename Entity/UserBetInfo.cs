using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Entity
{
    /// <summary>
    /// 下注信息
    /// </summary>
    public sealed class UserBetInfo : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string Nper { get; set; }

        /// <summary>
        /// 当期所有投注信息
        /// </summary>
        public List<BetRemarkInfo> BetRemarks { get; set; } = new List<BetRemarkInfo>();

        /// <summary>
        /// 是否回水
        /// </summary>
        public bool IsBackwater { get; set; } = false;

        /// <summary>
        /// 注单
        /// </summary>
        public NotesEnum Notes { get; set; } = NotesEnum.正常;

        /// <summary>
        /// 是否需要飞单
        /// </summary>
        public bool SendFly { get; set; } = false;

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
    /// 当期下注信息
    /// </summary>
    public class BetRemarkInfo
    { 
        /// <summary>
        /// 下注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 单号
        /// </summary>
        public string OddNum { get; set; }

        /// <summary>
        /// 下注信息
        /// </summary>
        public List<OddBetInfo> OddBets { get; set; } = new List<OddBetInfo>();

        /// <summary>
        /// 下注时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime BetTime { get; set; } = DateTime.Now;
    }

    /// <summary>
    /// 单个下注信息
    /// </summary>
    public class OddBetInfo
    {
        /// <summary>
        /// 投注金额
        /// </summary>
        public decimal BetMoney { get; set; }

        /// <summary>
        /// 投注号码
        /// </summary>
        public string BetNo { get; set; }

        /// <summary>
        /// 投注规则
        /// </summary>
        public BetTypeEnum BetRule { get; set; }

        /// <summary>
        /// 投注状态
        /// </summary>
        public BetStatusEnum BetStatus { get; set; } = BetStatusEnum.已投注;

        /// <summary>
        /// 中奖金额
        /// </summary>
        public decimal MediumBonus { get; set; } = 0;
    }

    public enum BetStatusEnum
    {
        已投注 = 1,
        未中奖 = 2,
        已中奖 = 3,
        开和退注 = 4
    }

    /// <summary>
    /// 游戏开奖状态
    /// </summary>
    public enum BetStatus
    { 
        未开奖 = 1,
        已开奖 = 2
    }

    /// <summary>
    /// 投注类型及位置
    /// </summary>
    public enum BetTypeEnum
    {
        冠亚 = 1,
        第一名 = 2,
        第二名 = 3,
        第三名 = 4,
        第四名 = 5,
        第五名 = 6,
        第六名 = 7,
        第七名 = 8,
        第八名 = 9,
        第九名 = 10,
        第十名 = 11,
        第一球 = 12,
        第二球 = 13,
        第三球 = 14,
        第四球 = 15,
        第五球 = 16,
        前三 = 17,
        中三 = 18,
        后三 = 19,
        //万个 = 20,
        总和 = 21,
        龙 = 22,
        虎 = 23,
        和 = 24
    }

    /// <summary>
    /// 注单标记
    /// </summary>
    public enum NotesEnum
    {
        正常 = 1,
        虚拟 = 2
    }
}
