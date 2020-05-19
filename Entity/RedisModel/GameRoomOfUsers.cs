namespace Entity.RedisModel
{
    public class GameRoomOfUsers
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 连接id
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }
    }

    public class GameRoomList
    {
        /// <summary>
        /// 用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 连接id
        /// </summary>
        public string ConnectionId { get; set; }
    }
}
