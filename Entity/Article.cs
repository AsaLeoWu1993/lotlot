namespace Entity
{
    /// <summary>
    /// 文章
    /// </summary>
    public sealed class Article : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 公告类型
        /// </summary>
        public ArticleTypeEnum ArticleType { get; set; } = ArticleTypeEnum.公告;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Open { get; set; } = true;
    }

    public enum ArticleTypeEnum
    {
        公告 = 1,
        消息 = 2
    }
}
