using Entity.BaccaratModel;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;

namespace Entity
{
    /// <summary>
    /// 游戏房间通用属性
    /// </summary>
    public class RoomGameInfos : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 房间ID
        /// </summary>
        public string RoomID { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 游戏房名称
        /// </summary>
        public string GameRoomName { get; set; }

        /// <summary>
        /// 游戏房号码   无用
        /// </summary>
        [Obsolete("游戏房号码", true)]
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
        [Obsolete("50秒能否撤销", true)]
        public bool Che { get; set; } = false;

        /// <summary>
        /// 回水通知
        /// </summary>
        public bool Back { get; set; } = true;

        /// <summary>
        /// 查返回信息
        /// </summary>
        public List<RInfoItem> RInfoItems { get; set; } = new List<RInfoItem>()
                {
                    new RInfoItem()
                    {
                        Index = RInfoEnum.使用
                    },
                      new RInfoItem()
                    {
                        Index = RInfoEnum.投注
                    },
                        new RInfoItem()
                    {
                        Index = RInfoEnum.流水
                    },
                          new RInfoItem()
                    {
                        Index = RInfoEnum.积分
                    },
                            new RInfoItem()
                    {
                        Index = RInfoEnum.输赢
                    }
                };

        /// <summary>
        /// 账单显示最低金额
        /// </summary>
        public decimal? MinimumAmount { get; set; } = 1;

        /// <summary>
        /// 账单
        /// </summary>
        public BillEnum Bill { get; set; } = BillEnum.单排;

        /// <summary>
        /// 回水设置
        /// </summary>
        public bool SwitchBackwater { get; set; } = true;

        /// <summary>
        /// 彩票飞单
        /// </summary>
        public RecordType LotteryRecord { get; set; } = RecordType.不飞单;

        /// <summary>
        /// 视讯飞单
        /// </summary>
        public RecordType VideoRecord { get; set; } = RecordType.不飞单;

        /// <summary>
        /// 禁止取消
        /// </summary>
        public bool Revoke { get; set; } = false;

        /// <summary>
        /// 隐藏赔率说明
        /// </summary>
        public bool Instructions { get; set; } = false;

        /// <summary>
        /// 是否停售
        /// </summary>
        public bool HaltSales { get; set; } = false;

        /// <summary>
        /// 出款开始时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime HaltTime { get; set; } = new DateTime().AddHours(7);

        /// <summary>
        /// 出款结束时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime OnSaleTime { get; set; } = new DateTime().AddHours(9);
    }

    /// <summary>
    /// 游戏类别
    /// </summary>
    public enum GameOfType
    {
        北京赛车 = 1,
        幸运飞艇 = 2,
        重庆时时彩 = 3,
        极速赛车 = 4,
        澳州10 = 5,
        澳州5 = 6,
        爱尔兰赛马 = 7,
        爱尔兰快5 = 8,
        幸运飞艇168 = 9,
        极速时时彩 = 10
    }

    /// <summary>
    /// 房间状态
    /// </summary>
    public enum RoomStatus
    {
        开启 = 1,
        关闭 = 2
    }

    /// <summary>
    /// 返回信息枚举
    /// </summary>
    public enum RInfoEnum
    {
        投注 = 1,
        使用 = 2,
        积分 = 3,
        流水 = 4,
        输赢 = 5
    }

    /// <summary>
    /// 返回信息集合
    /// </summary>
    public class RInfoItem
    {
        public RInfoEnum Index { get; set; }

        public bool Open { get; set; } = true;
    }

    /// <summary>
    /// 账单枚举
    /// </summary>
    public enum BillEnum
    {
        单排 = 1,
        双排 = 2
    }

    #region 视讯
    /// <summary>
    /// 视讯房间
    /// </summary>
    public class VideoRoom : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 房间ID
        /// </summary>
        public string RoomID { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public BaccaratGameType GameType { get; set; }

        /// <summary>
        /// 游戏房名称
        /// </summary>
        public string GameRoomName { get; set; }

        /// <summary>
        /// 游戏房号码   无用
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
        [Obsolete("50秒能否撤销", true)]
        public bool Che { get; set; } = true;

        /// <summary>
        /// 账单
        /// </summary>
        public BillEnum Bill { get; set; } = BillEnum.单排;

        /// <summary>
        /// 回水通知
        /// </summary>
        public bool Back { get; set; } = true;

        /// <summary>
        /// 账单显示最低金额
        /// </summary>
        public decimal? MinimumAmount { get; set; } = 1;

        /// <summary>
        /// 回水设置
        /// </summary>
        public bool SwitchBackwater { get; set; } = true;

        /// <summary>
        /// 开和
        /// </summary>
        public VideoKaiHeEnum KaiEquality { get; set; } = VideoKaiHeEnum.全退;

        /// <summary>
        /// 彩票飞单
        /// </summary>
        public RecordType LotteryRecord { get; set; } = RecordType.不飞单;

        /// <summary>
        /// 视讯飞单
        /// </summary>
        public RecordType VideoRecord { get; set; } = RecordType.不飞单;

        /// <summary>
        /// 禁止取消
        /// </summary>
        public bool Revoke { get; set; } = false;

        /// <summary>
        /// 隐藏赔率说明
        /// </summary>
        public bool Instructions { get; set; } = false;

        /// <summary>
        /// 是否停售
        /// </summary>
        public bool HaltSales { get; set; } = false;

        /// <summary>
        /// 出款开始时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime HaltTime { get; set; } = new DateTime().AddHours(7);

        /// <summary>
        /// 出款结束时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime OnSaleTime { get; set; } = new DateTime().AddHours(9);
    }

    /// <summary>
    /// 视迅开和
    /// </summary>
    public enum VideoKaiHeEnum
    {
        全退 = 1,
        退一半 = 2,
        通杀 = 3
    }
    #endregion
}
