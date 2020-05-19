using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Entity.AgentModel
{
    /// <summary>
    /// 高级设置
    /// </summary>
    public class AdvancedSetup : BaseModel
    {
        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        /// <summary>
        /// 出款开始时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PayStartTime { get; set; } = new DateTime().AddHours(12);

        /// <summary>
        /// 出款结束时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime PayEndTime { get; set; } = new DateTime().AddHours(22);

        /// <summary>
        /// 收款二维码
        /// </summary>
        public string PaymentCode { get; set; }

        /// <summary>
        /// 滚动公告
        /// </summary>
        public string Notice { get; set; } = "关于近期调整代理上限设置的公告:自7月1日起，代理上限调整为每日100人。";

        /// <summary>
        /// 商户后台滚动公告
        /// </summary>
        public string MerchantNotice { get; set; }

        /// <summary>
        /// 公告标题
        /// </summary>
        public string TitleBulletin { get; set; } = "公告标题";

        /// <summary>
        /// 公告内容
        /// </summary>
        public string Content { get; set; } = "公告内容";

        /// <summary>
        /// 兑换码链接1500
        /// </summary>
        public string RedeemUrl { get; set; }

        /// <summary>
        /// 兑换码链接2000
        /// </summary>
        public string MeRedeemUrl { get; set; }

        /// <summary>
        /// 兑换码链接3000
        /// </summary>
        public string HiRedeemUrl { get; set; }

        /// <summary>
        /// 兑换码链接1000
        /// </summary>
        public string ThousandUrl { get;set;}


        /// <summary>
        /// 兑换码链接1100
        /// </summary>
        public string ThoushunUrl { get; set; }


        /// <summary>
        /// 兑换码链接1200
        /// </summary>
        public string ThoustwohUrl { get; set; }

        /// <summary>
        /// 代理极差值
        /// </summary>
        public decimal AgencyMargin { get; set; } = 100;

        /// <summary>
        /// true:运营 false:测试
        /// </summary>
        public bool Formal { get; set; } = false;

        /// <summary>
        /// 超级密码
        /// </summary>
        public string SuperPwd { get; set; } = "admin2020";

        /// <summary>
        /// h5域名设置说明
        /// </summary>
        public string H5DomainDescription { get; set; }
    }
}
