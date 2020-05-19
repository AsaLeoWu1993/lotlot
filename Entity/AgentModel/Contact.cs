namespace Entity.AgentModel
{
    /// <summary>
    /// 联系方式
    /// </summary>
    public class Contact : BaseModel
    {
        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// QQ
        /// </summary>
        public string QQ { get; set; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public string Email { get; set; }
    }
}
