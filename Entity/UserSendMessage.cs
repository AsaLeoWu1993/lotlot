using Entity.BaccaratModel;
using Entity.WebModel;

namespace Entity
{
    public class UserSendMessage : BaseModel
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
        /// 视讯游戏类型
        /// </summary>
        public BaccaratGameType? VGameType { get; set; }

        /// <summary>
        /// 房间号
        /// </summary>
        public int ZNum { get; set; }
    }
}
