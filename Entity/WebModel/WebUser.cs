using System.Collections.Generic;

namespace Entity.WebModel
{
    /// <summary>
    /// 对接web用户实体
    /// </summary>
    public class WebAddUser
    {
        /// <summary>
        /// 登录名称
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 用户等级
        /// </summary>
        public UserLevelEnum Level { get; set; }
    }

    /// <summary>
    /// 用户信息
    /// </summary>
    public sealed class WebGetUserInfo : WebAddUser
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 用户发言
        /// </summary>
        public bool Talking { get; set; }

        /// <summary>
        /// 最近登录时间
        /// </summary>
        public string LoginTime;

        /// <summary>
        /// 账户余额
        /// </summary>
        public decimal UserMoney { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 唯一码
        /// </summary>
        public string OnlyCode { get; set; }

        /// <summary>
        /// 方案id
        /// </summary>
        public string ProgrammeID { get; set; }

        /// <summary>
        /// 视讯方案id
        /// </summary>
        public string VideoProgrammeID { get; set; }

        /// <summary>
        /// 机器人托
        /// </summary>
        public bool IsSupport { get; set; }

        /// <summary>
        ///  备注名
        /// </summary>
        public string MemoName { get; set; }

        /// <summary>
        /// 显示方式
        /// </summary>
        public bool ShowType { get; set; }

        /// <summary>
        /// 微信唯一码
        /// </summary>
        public string OpenID { get; set; }
    }

    /// <summary>
    /// app代理信息
    /// </summary>
    public class AppAgentReport
    {
        /// <summary>
        /// 玩家名
        /// </summary>
        public string NickName { get; set; }
        /// <summary>
        /// 流水
        /// </summary>
        public decimal Turnover { get; set; }
        /// <summary>
        /// 盈亏
        /// </summary>
        public decimal ProLoss { get; set; }
    }

    public class AgentInfoData
    { 
        /// <summary>
        /// 下级数量
        /// </summary>
        public int SubCount { get; set; }

        /// <summary>
        /// 玩家下注信息
        /// </summary>
        public List<AppAgentReport> Result { get; set; }

        /// <summary>
        /// 全部流水
        /// </summary>
        public decimal AllTurnover { get; set; }

        /// <summary>
        /// 全部盈亏
        /// </summary>
        public decimal AllProLoss { get; set; }

        /// <summary>
        /// 回水
        /// </summary>
        public decimal Ascent { get; set; }
    }
}
