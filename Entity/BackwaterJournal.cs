using Entity.BaccaratModel;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Entity
{
    /// <summary>
    /// 回水日志表
    /// </summary>
    public class BackwaterJournal : BaseModel
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 游戏
        /// </summary>
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 模式类型
        /// </summary>
        public PatternEnum Pattern { get; set; }

        /// <summary>
        /// 投入金额
        /// </summary>
        public decimal InputAmount { get; set; }

        /// <summary>
        /// 盈亏
        /// </summary>
        public decimal ProLoss { get; set; }

        /// <summary>
        /// 回水金额
        /// </summary>
        public decimal Ascent { get; set; }

        /// <summary>
        /// 对应用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public BackStatusEnum BackStatus { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public UserStatusEnum UserStatus { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public string AddDataTime { get; set; }
    }

    public class VideoBackwaterJournal : BaseModel
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 游戏
        /// </summary>
        public BaccaratGameType GameType { get; set; }

        /// <summary>
        /// 模式类型
        /// </summary>
        public PatternEnum Pattern { get; set; }

        /// <summary>
        /// 投入金额
        /// </summary>
        public decimal InputAmount { get; set; }

        /// <summary>
        /// 盈亏
        /// </summary>
        public decimal ProLoss { get; set; }

        /// <summary>
        /// 回水金额
        /// </summary>
        public decimal Ascent { get; set; }

        /// <summary>
        /// 对应用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public BackStatusEnum BackStatus { get; set; }

        /// <summary>
        /// 用户状态
        /// </summary>
        public UserStatusEnum UserStatus { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        public string AddDataTime { get; set; }
    }

    /// <summary>
    /// 代理回水
    /// </summary>
    public class UserBackwaterJournal : BaseModel
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 游戏
        /// </summary>
        public GameOfType? GameType { get; set; }

        /// <summary>
        /// 回水金额
        /// </summary>
        public decimal Ascent { get; set; }

        /// <summary>
        /// 投入金额
        /// </summary>
        public decimal InputAmount { get; set; }

        /// <summary>
        /// 对应用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 代理用户id
        /// </summary>
        public string AgentUserID { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public BackStatusEnum BackStatus { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime AddDataTime { get; set; } = DateTime.Today.AddDays(-1);

        /// <summary>
        /// 视讯游戏类型
        /// </summary>
        public BaccaratGameType? VGameType { get; set; }
    }

    public enum BackStatusEnum
    {
        已回水 = 1,
        未回水 = 2,
        可回水 = 3
    }
}
