using Entity.BaccaratModel;

namespace Entity.WebModel
{
    public class WebUserSendMessage
    {
        /// <summary>
        /// 用户类型
        /// </summary>
        public UserEnum UserType { get; set; } = UserEnum.普通用户;

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType? GameType { get; set; }

        /// <summary>
        /// 房间号
        /// </summary>
        public int ZNum { get; set; }

        /// <summary>
        /// 最后一条数据
        /// </summary>
        public bool End { get; set; } = false;
    }

    public class SendBaccaratMessageClass
    {
        public SendBaccaratMessageClass()
        {
        }

        public string Avatar { get; set; }
        public string Message { get; set; }
        public string MerchantID { get; set; }
        public string NickName { get; set; }
        public UserEnum UserType { get; set; } = UserEnum.普通用户;
        public BaccaratGameType? GameType { get; set; } = BaccaratGameType.百家乐;
        public int Znum { get; set; }
        public string UserID { get; set; }
        /// <summary>
        /// 最后一条数据
        /// </summary>
        public bool End { get; set; } = false;
    }

    public enum UserEnum
    {
        管理员 = 1,
        普通用户 = 2,
        假人 = 3
    }
}
