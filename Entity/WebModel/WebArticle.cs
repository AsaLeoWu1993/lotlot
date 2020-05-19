namespace Entity.WebModel
{
    /// <summary>
    /// web对接文章表
    /// </summary>
    public class WebArticle
    {
        public string ID { get; set; }

        /// <summary>
        /// 文章类型
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 公告类型
        /// </summary>
        public ArticleTypeEnum ArticleType { get; set; }

        /// <summary>
        /// 是否开启
        /// </summary>
        public bool Open { get; set; }

        /// <summary>
        /// 发布时间
        /// </summary>
        public string Time { get; set; }
    }
}
