namespace Entity.WebModel
{
    /// <summary>
    /// web对接管理员发送消息
    /// </summary>
    public sealed class WebAdminMessage
    {
        /// <summary>
        /// 赛车
        /// </summary>
        public bool Pk10 { get; set; } = false;

        /// <summary>
        /// 飞艇
        /// </summary>
        public bool Xyft { get; set; } = false;

        /// <summary>
        /// 时时彩
        /// </summary>
        public bool Cqssc { get; set; } = false;

        /// <summary>
        /// 极速回水率
        /// </summary>
        public bool Jssc { get; set; } = false;

        /// <summary>
        /// 澳10
        /// </summary>
        public bool Azxy10 { get; set; } = false;

        /// <summary>
        /// 澳5
        /// </summary>
        public bool Azxy5 { get; set; } = false;

        /// <summary>
        /// 爱尔兰赛马
        /// </summary>
        public bool Ireland10 { get; set; } = false;

        /// <summary>
        /// 爱尔兰快5
        /// </summary>
        public bool Ireland5 { get; set; } = false;

        /// <summary>
        /// 幸运飞艇168
        /// </summary>
        public bool Xyft168 { get; set; } = false;

        /// <summary>
        /// 极速时时彩
        /// </summary>
        public bool Jsssc { get; set; } = false;

        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
    }
}
