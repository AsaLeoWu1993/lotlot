using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Entity
{
    /// <summary>
    /// 商户表
    /// </summary>
    public sealed class Merchant : BaseModel
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string MeName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 安全码(商户号)
        /// </summary>
        public string SeurityNo { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int Status { get; set; } = 1;

        /// <summary>
        /// 到期时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime MaturityTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        /// <summary>
        /// 充值时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? RechargeTime { get; set; }

        /// <summary>
        /// 连接地址
        /// </summary>
        public string AddressID { get; set; }

        /// <summary>
        /// 登录时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LoginTime { get; set; }

        /// <summary>
        /// 在线时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? OnLineTime { get; set; }

        /// <summary>
        /// 设置安全密码
        /// </summary>
        public string SecurityPwd { get; set; }

        /// <summary>
        /// 安全密码状态
        /// </summary>
        public bool SecurityStatus { get; set; } = false;

        /// <summary>
        /// 是否为超级商户
        /// </summary>
        public bool SuperStatus { get; set; } = false;

        /// <summary>
        /// 火星币
        /// </summary>
        public decimal MarsCurrency { get; set; } = 0;

        /// <summary>
        /// h5网页域名
        /// </summary>
        public string H5DomainUrl { get; set; }

        /// <summary>
        /// h5设置安全码
        /// </summary>
        public string SetupSeurityNo { get; set; }

        /// <summary>
        /// h5设置房间码
        /// </summary>
        public string SetupRoomNum { get; set; }
    }
}
