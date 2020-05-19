using System.Collections.Generic;

namespace Entity.WebModel
{
    /// <summary>
    /// web对接虚假用户信息
    /// </summary>
    public sealed class WebShamUser
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 用户昵称
        /// </summary>
        public string NickName { get; set; }

        /// <summary>
        /// 用户头像
        /// </summary>
        public string Avatar { get; set; }

        /// <summary>
        /// 游戏及投注内容
        /// </summary>
        public List<GameBetType> GameBetInfo { get; set; }

        /// <summary>
        /// 唯一码
        /// </summary>
        public string OnlyCode { get; set; }

        /// <summary>
        /// 用户余额
        /// </summary>
        public decimal UserMoney { get; set; }

        /// <summary>
        /// 选择游戏
        /// </summary>
        public string OpenGame { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Check { get; set; }
    }
}
