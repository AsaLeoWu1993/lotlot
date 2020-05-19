using System;
using System.Collections.Generic;

namespace Entity.WebModel
{
    /// <summary>
    /// web对接房间游戏
    /// </summary>
    public class WebRoomGame
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 游戏房名称
        /// </summary>
        public string GameRoomName { get; set; }

        /// <summary>
        /// 游戏房号码
        /// </summary>
        public string GameRoomNum { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public RoomStatus Status { get; set; } = RoomStatus.开启;

        /// <summary>
        /// 进入房间最低金额
        /// </summary>
        public decimal? Minin { get; set; }

        /// <summary>
        /// 撤销
        /// </summary>
        public bool Che { get; set; } = false;

        /// <summary>
        /// 回水通知
        /// </summary>
        public bool Back { get; set; } = false;

        /// <summary>
        /// 查返回信息
        /// </summary>
        public List<RInfoItem> RInfoItems { get; set; }

        /// <summary>
        /// 账单显示最低金额
        /// </summary>
        public decimal? MinimumAmount { get; set; }

        /// <summary>
        /// 账单
        /// </summary>
        public BillEnum Bill { get; set; } = BillEnum.单排;

        /// <summary>
        /// 回水设置
        /// </summary>
        public bool SwitchBackwater { get; set; }

        /// <summary>
        /// 彩票飞单
        /// </summary>
        public RecordType LotteryRecord { get; set; }

        /// <summary>
        /// 视讯飞单
        /// </summary>
        public RecordType VideoRecord { get; set; }

        /// <summary>
        /// 禁止取消
        /// </summary>
        public bool Revoke { get; set; } = true;

        /// <summary>
        /// 隐藏赔率说明
        /// </summary>
        public bool Instructions { get; set; } = true;

        /// <summary>
        /// 是否停售
        /// </summary>
        public bool HaltSales { get; set; }

        /// <summary>
        /// 出款开始时间
        /// </summary>
        public DateTime HaltTime { get; set; }

        /// <summary>
        /// 出款结束时间
        /// </summary>
        public DateTime OnSaleTime { get; set; }
    }

    /// <summary>
    /// web对接房间开和游戏
    /// </summary>
    public sealed class WebRoomGameKai : WebRoomGame
    {
        /// <summary>
        /// 开和
        /// </summary>
        public KaiHeEnum? KaiEquality { get; set; }
    }

    public class WebVideoRoomGame
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 游戏房名称
        /// </summary>
        public string GameRoomName { get; set; }

        /// <summary>
        /// 游戏房号码
        /// </summary>
        public string GameRoomNum { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public RoomStatus Status { get; set; }

        /// <summary>
        /// 进入房间最低金额
        /// </summary>
        public decimal? Minin { get; set; }

        /// <summary>
        /// 撤销
        /// </summary>
        public bool Che { get; set; }

        /// <summary>
        /// 回水通知
        /// </summary>
        public bool Back { get; set; }

        /// <summary>
        /// 账单显示最低金额
        /// </summary>
        public decimal? MinimumAmount { get; set; }

        /// <summary>
        /// 账单
        /// </summary>
        public BillEnum Bill { get; set; }

        /// <summary>
        /// 回水设置
        /// </summary>
        public bool SwitchBackwater { get; set; }

        /// <summary>
        /// 开和
        /// </summary>
        public VideoKaiHeEnum KaiEquality { get; set; }

        /// <summary>
        /// 彩票飞单
        /// </summary>
        public RecordType LotteryRecord { get; set; }

        /// <summary>
        /// 视讯飞单
        /// </summary>
        public RecordType VideoRecord { get; set; }

        /// <summary>
        /// 禁止取消
        /// </summary>
        public bool Revoke { get; set; } = true;

        /// <summary>
        /// 隐藏赔率说明
        /// </summary>
        public bool Instructions { get; set; } = true;

        /// <summary>
        /// 是否停售
        /// </summary>
        public bool HaltSales { get; set; }

        /// <summary>
        /// 出款开始时间
        /// </summary>
        public DateTime HaltTime { get; set; }

        /// <summary>
        /// 出款结束时间
        /// </summary>
        public DateTime OnSaleTime { get; set; }
    }
}
