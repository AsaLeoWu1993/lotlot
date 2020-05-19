namespace Entity.WebModel
{
    /// <summary>
    /// 对接web商户实体
    /// </summary>
    public sealed class WebMerchant
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Passwd { get; set; }

        /// <summary>
        /// 安全码(商户号)
        /// </summary>
        public string SeurityNo { get; set; }
    }
}
