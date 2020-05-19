using Entity.BaccaratModel;
using System;

namespace Baccarat.Interactive
{
    /// <summary>
    /// 游戏房间状态
    /// </summary>
    public class GameStatic
    {
        /// <summary>
        /// 房间号
        /// </summary>
        public int ZNum { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Cstate { get; set; } = "stop";

        /// <summary>
        /// 场次
        /// </summary>
        public string Scene { get; set; }

        /// <summary>
        /// 倒计时
        /// </summary>
        public int Ttime { get; set; } = 0;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public BaccaratGameType GameType { get; set; }

        /// <summary>
        /// 视频地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 桌名
        /// </summary>
        public string ZName { get; set; }

        /// <summary>
        /// 历史记录
        /// </summary>
        public string History { get; set; }
    }
}
