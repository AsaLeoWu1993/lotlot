using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Entity.AgentModel
{
    /// <summary>
    /// 高级设置
    /// </summary>
    public class PlatformSetUp : BaseModel
    {
        /// <summary>
        /// 商户地址
        /// </summary>
        public string MerchantUrl { get; set; }

        /// <summary>
        /// 商户手机端地址
        /// </summary>
        public string MerchantMUrl { get; set; }

        /// <summary>
        /// 商户app地址
        /// </summary>
        public string MerchantAppUrl { get; set; }

        /// <summary>
        /// app地址1
        /// </summary>
        public string AppUrl1 { get; set; }

        /// <summary>
        /// app地址2
        /// </summary>
        public string AppUrl2 { get; set; }

        /// <summary>
        /// 公众号
        /// </summary>
        public string Subscription { get; set; }

        /// <summary>
        /// app appid
        /// </summary>
        public string App_AppID { get; set; }

        /// <summary>
        /// app appsecret
        /// </summary>
        public string App_AppSecret { get; set; }

        /// <summary>
        /// app appid
        /// </summary>
        public string Web_AppID { get; set; }

        /// <summary>
        /// app appsecret
        /// </summary>
        public string Web_AppSecret { get; set; }

        /// <summary>
        /// 微信登录开关
        /// </summary>
        public bool WeChatSwitch { get; set; } = true;

        /// <summary>
        /// 游戏视频列表
        /// </summary>
        public List<GameVideo> GameVideos { get; set; } = new List<GameVideo>();

        /// <summary>
        /// 游戏设置
        /// </summary>
        public List<GameBasicsSetup> GameBasicsSetups { get; set; } = new List<GameBasicsSetup>();

        /// <summary>
        /// app使用域名
        /// </summary>
        public List<AppCanUserUrl> AppUrls { get; set; } = new List<AppCanUserUrl>();

        /// <summary>
        /// 版本号
        /// </summary>
        public string VersionNum { get; set; }

        /// <summary>
        /// 版本文件地址
        /// </summary>
        public string VersionFileUrl { get; set; }

        /// <summary>
        /// 授权地址
        /// </summary>
        public string GrantUrl { get; set; }

        /// <summary>
        /// 视讯播放地址
        /// </summary>
        public string VideoUrl { get; set; } = "ws://down.huicmm.top/live/tl";
    }

    public class AppCanUserUrl
    {
        /// <summary>
        /// 链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
    }

    public class GameVideo
    {
        public GameOfType GameType { get; set; }

        public string Url { get; set; }
    }

    public class GameBasicsSetup
    {
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 第一期号
        /// </summary>
        public string FirstNper { get; set; }

        /// <summary>
        /// 起点时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime StartTime { get; set; }

        /// <summary>
        /// 间隔时间
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        /// 每天期数量
        /// </summary>
        public int DayNum { get; set; }
    }
}
