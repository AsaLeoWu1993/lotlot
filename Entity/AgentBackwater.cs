using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Entity
{
    /// <summary>
    /// 代理回水表
    /// </summary>
    public sealed class AgentBackwater : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 赛车回水率
        /// </summary>
        public decimal Pk10 { get; set; } = 0;

        /// <summary>
        /// 飞艇回水率
        /// </summary>
        public decimal Xyft { get; set; } = 0;

        /// <summary>
        /// 时时彩回水率
        /// </summary>
        public decimal Cqssc { get; set; } = 0;

        /// <summary>
        /// 极速回水率
        /// </summary>
        public decimal Jssc { get; set; } = 0;

        /// <summary>
        /// 澳10回水率
        /// </summary>
        public decimal Azxy10 { get; set; } = 0;

        /// <summary>
        /// 澳5回水率
        /// </summary>
        public decimal Azxy5 { get; set; } = 0;

        /// <summary>
        /// 爱尔兰赛马
        /// </summary>
        public decimal Ireland10 { get; set; } = 0;

        /// <summary>
        /// 爱尔兰快5
        /// </summary>
        public decimal Ireland5 { get; set; } = 0;

        /// <summary>
        /// 幸运飞艇168
        /// </summary>
        public decimal XYft168 { get; set; } = 0;

        /// <summary>
        /// 极速时时彩
        /// </summary>
        public decimal Jsssc { get; set; } = 0;

        /// <summary>
        /// 百家乐
        /// </summary>
        public decimal Baccarat { get; set; } = 0;

        /// <summary>
        /// 下线
        /// </summary>
        public List<OfflineUser> Offline { get; set; } = new List<OfflineUser>();

        /// <summary>
        /// 推广码
        /// </summary>
        public string ExtensionCode { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime AddTime { get; set; } = DateTime.Now;
    }

    public class OfflineUser
    {
        public string UserID { get; set; }

        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime AddTime { get; set; } = DateTime.Now;
    }
}
