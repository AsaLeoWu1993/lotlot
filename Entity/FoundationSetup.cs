using System.Collections.Generic;

namespace Entity
{
    /// <summary>
    /// 商户基础设置表
    /// </summary>
    public sealed class FoundationSetup : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 封盘前时间
        /// </summary>
        public int EntertainedFrontTime { get; set; } = 50;

        /// <summary>
        /// 封盘前消息
        /// </summary>
        public string EntertainedFrontMsg { get; set; } =
@"====以下禁止修改====
       距封盘还剩50秒
================";

        /// <summary>
        /// 开奖前时间
        /// </summary>
        public List<LotteryItem> LotteryFrontTime { get; set; }

        /// <summary>
        /// 开奖前消息
        /// </summary>
        public string LotteryFrontMsg { get; set; } =
            @"◆◆◆◆◆封盘◆◆◆◆◆ 
 ◆◆◆◆◆◆◆◆◆◆◆◆ 
     停止竞猜  停止竞猜";

        /// <summary>
        /// 封盘后时间
        /// </summary>
        public int EntertainedAfterTime { get; set; } = 5;

        /// <summary>
        /// 封盘后消息
        /// </summary>
        public string EntertainedAfterMsg { get; set; } =
            @"第{期号}期
游戏:{游戏}
=======停止线======
注单核对:
{核对}
=================
以上有效、以下不收
如有疑问，请咨群主
=================";

        /// <summary>
        /// 中奖明细
        /// </summary>
        public string WinningDetails { get; set; } =
            @"═════════════ 
第{期号}期开号：
 {开奖信息} 
═════════════ 
各位老板本期盈亏：
{结算}";

        /// <summary>
        /// 中奖结算内容格式
        /// </summary>
        public string Settlement { get; set; } = @"[{玩家}][{当期盈亏}]
--------------
{中奖详细}
--------------";

        /// <summary>
        /// 未中奖结算内容格式
        /// </summary>
        public string NotSettlement { get; set; } = @"[{玩家}][{当期盈亏}]";

        /// <summary>
        /// 会员积分
        /// </summary>
        public string MembershipScore { get; set; } =
            @"★★{微群名字
    }★ ★
在线{在线人数}人  
总分:{玩家总分}分
==============
{账单内容}
==============
本群最低投注10元
双面限额：5万
数字限额：5千
和值限额：5千
▼▼▼▼▼▼▼▼▼▼
开始下注  开始下注
开始下注  开始下注";

        /// <summary>
        /// 显示表单账单
        /// </summary>
        public bool ShowBillTable { get; set; } = false;

        /// <summary>
        /// 自定义时间
        /// </summary>
        public int CustomTime { get; set; } = 10;

        /// <summary>
        /// 自定义消息
        /// </summary>
        public string CustomMsg { get; set; } =
            @"财务(微❤信)：123456
财务(支付宝)：123456
===============
查询更多开奖记录 ↓
www.537.net";

        /// <summary>
        /// 封盘前禁止撤单
        /// </summary>
        public bool ProhibitChe { get; set; } = false;
    }

    /// <summary>
    /// 视讯基础设置表
    /// </summary>
    public class VideoFoundationSetup : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 封盘前时间
        /// </summary>
        public int EntertainedFrontTime { get; set; } = 50;

        /// <summary>
        /// 封盘前消息
        /// </summary>
        public string EntertainedFrontMsg { get; set; } =
@"====以下禁止修改====
       距封盘还剩50秒
================";

        /// <summary>
        /// 开奖前消息
        /// </summary>
        public string LotteryFrontMsg { get; set; } =
            @"◆◆◆◆◆封盘◆◆◆◆◆ 
 ◆◆◆◆◆◆◆◆◆◆◆◆ 
     停止竞猜  停止竞猜";

        /// <summary>
        /// 封盘后时间
        /// </summary>
        public int EntertainedAfterTime { get; set; } = 5;

        /// <summary>
        /// 封盘后消息
        /// </summary>
        public string EntertainedAfterMsg { get; set; } =
            @"第{期号}期
游戏:{游戏}
=======停止线======
注单核对:
{核对}
=================
以上有效、以下不收
如有疑问，请咨群主
=================";

        /// <summary>
        /// 中奖明细
        /// </summary>
        public string WinningDetails { get; set; } =
            @"═════════════ 
第{期号}期开号：
 {开奖信息} 
═════════════ 
各位老板本期盈亏：
{结算}";

        /// <summary>
        /// 会员积分
        /// </summary>
        public string MembershipScore { get; set; } =
            @"★★{微群名字}★ ★
在线{在线人数}人  
总分:{玩家总分}分
==============
{账单内容}
==============
本群最低投注10元
双面限额：5万
数字限额：5千
和值限额：5千
▼▼▼▼▼▼▼▼▼▼
开始下注  开始下注
开始下注  开始下注";

        /// <summary>
        /// 显示表单账单
        /// </summary>
        public bool ShowBillTable { get; set; } = false;

        /// <summary>
        /// 自定义时间
        /// </summary>
        public int CustomTime { get; set; } = 10;

        /// <summary>
        /// 自定义消息
        /// </summary>
        public string CustomMsg { get; set; } =
            @"财务(微❤信)：123456
财务(支付宝)：123456";

        /// <summary>
        /// 封盘前禁止撤单
        /// </summary>
        public bool ProhibitChe { get; set; } = false;

        /// <summary>
        /// 中奖结算内容格式
        /// </summary>
        public string Settlement { get; set; } = @"[{玩家}][{当期盈亏}]
--------------
{中奖详细}
--------------";

        /// <summary>
        /// 未中奖结算内容格式
        /// </summary>
        public string NotSettlement { get; set; } = @"[{玩家}][{当期盈亏}]";
    }

    public class LotteryItem
    {
        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType Type { get; set; }

        /// <summary>
        /// 开奖前封盘时间
        /// </summary>
        public int LotteryTime { get; set; }
    }
}
