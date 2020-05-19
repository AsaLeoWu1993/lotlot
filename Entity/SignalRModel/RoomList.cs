using System.Collections.Generic;

namespace Entity.SignalRModel
{
    public class RoomList
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        public List<UserInfo> UserInfos = new List<UserInfo>();
    }

    public class UserInfo
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 连接id
        /// </summary>
        public string ConnectionId { get; set; }
    }

    public class RoomGame
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }

        public List<UserInfo> UserInfos = new List<UserInfo>();
    }
}
