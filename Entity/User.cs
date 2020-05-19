using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Entity
{
    /// <summary>
    /// 用户表
    /// </summary>
    public sealed class User : BaseModel
    {
        /// <summary>
        /// 用户名称
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 用户密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public UserLevelEnum Level { get; set; }

        /// <summary>
        /// 用户发言
        /// </summary>
        public bool Talking { get; set; } = true;

        /// <summary>
        /// 最近登陆
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? LoginTime { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal UserMoney { get; set; } = 0;

        /// <summary>
        /// 代理
        /// </summary>
        public bool IsAgent { get; set; } = false;

        /// <summary>
        /// 飞单状态
        /// </summary>
        public bool Record { get; set; } = false;

        /// <summary>
        /// 状态
        /// </summary>
        public UserStatusEnum Status { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 收款帐户
        /// </summary>
        public List<Account> Accounts { get; set; } = new List<Account>();

        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 唯一码
        /// </summary>
        public string OnlyCode { get; set; }

        /// <summary>
        /// 微信帐号
        /// </summary>
        public string WeChat { get; set; }

        /// <summary>
        /// 微信图片地址
        /// </summary>
        public string WeChatUrl { get; set; }

        /// <summary>
        /// 支付宝帐号
        /// </summary>
        public string Alipay { get; set; }

        /// <summary>
        /// 支付宝图片地址
        /// </summary>
        public string AlipayUrl { get; set; }

        /// <summary>
        /// 房间码  推广码
        /// </summary>
        public string RoomNum { get; set; }

        /// <summary>
        /// 方案id
        /// </summary>
        public string ProgrammeID { get; set; }

        /// <summary>
        /// 视讯方案id
        /// </summary>
        public string VideoProgrammeID { get; set; }

        /// <summary>
        /// 添加时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime? ProgrammeAddTime { get; set; } = DateTime.Now.AddDays(-5);

        /// <summary>
        /// 微信唯一码
        /// </summary>
        public string Unionid { get; set; }

        /// <summary>
        /// 机器人托
        /// </summary>
        public bool IsSupport { get; set; } = false;

        /// <summary>
        ///  备注名
        /// </summary>
        public string MemoName { get; set; }

        /// <summary>
        /// 显示方式
        /// </summary>
        public bool ShowType { get; set; } = true;

        /// <summary>
        /// 彩票飞单
        /// </summary>
        [Obsolete("不使用", true)]
        public RecordType LotteryRecord { get; set; } = RecordType.不飞单;

        /// <summary>
        /// 视讯飞单
        /// </summary>
        [Obsolete("不使用", true)]
        public RecordType VideoRecord { get; set; } = RecordType.不飞单;
    }

    /// <summary>
    /// 用户等级
    /// </summary>
    public enum UserLevelEnum
    {
        黄铜 = 1,
        白银 = 2,
        黄金 = 3,
        铂金 = 4,
        钻石 = 5,
        银皇冠 = 6,
        金皇冠 = 7
    }

    /// <summary>
    /// 用户状态
    /// </summary>
    public enum UserStatusEnum
    {
        正常 = 1,
        冻结 = 2,
        删除 = 3,
        假人 = 4
    }

    public class Account
    {
        /// <summary>
        /// 账户
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 账户类型
        /// </summary>
        public string AccountType { get; set; }
    }

    public enum RecordType
    { 
        不飞单 = 1,
        飞单到高级商户 = 2,
        飞单到外部网盘 = 3
    }
}
