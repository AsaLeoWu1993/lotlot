namespace Entity.AgentModel
{
    public class AlipayInfo : BaseModel
    {
        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        /// <summary>
        /// 支付宝帐号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 二维码地址
        /// </summary>
        public string QRPath { get; set; }
    }
}
