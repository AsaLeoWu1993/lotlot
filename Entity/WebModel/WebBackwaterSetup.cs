using Entity.BaccaratModel;

namespace Entity.WebModel
{
    /// <summary>
    /// web对接回水设置表
    /// </summary>
    public sealed class WebBackwaterSetup
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType? GameType { get; set; }

        /// <summary>
        /// 最小统计
        /// </summary>
        public decimal Minrecord { get; set; }

        /// <summary>
        /// 最大统计
        /// </summary>
        public decimal Maxrecord { get; set; }

        /// <summary>
        /// 回水比例
        /// </summary>
        public decimal Odds { get; set; }

        /// <summary>
        /// 模式
        /// </summary>
        public PatternEnum Pattern { get; set; }
    }

    /// <summary>
    /// 视讯回水
    /// </summary>
    public sealed class WebVideoBackwaterSetup
    {
        /// <summary>
        /// id
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public BaccaratGameType? GameType { get; set; }

        /// <summary>
        /// 最小统计
        /// </summary>
        public decimal Minrecord { get; set; }

        /// <summary>
        /// 最大统计
        /// </summary>
        public decimal Maxrecord { get; set; }

        /// <summary>
        /// 回水比例
        /// </summary>
        public decimal Odds { get; set; }

        /// <summary>
        /// 模式
        /// </summary>
        public PatternEnum Pattern { get; set; }
    }
}
