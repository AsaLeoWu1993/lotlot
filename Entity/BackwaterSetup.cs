using Entity.BaccaratModel;

namespace Entity
{
    /// <summary>
    /// 回水设置表
    /// </summary>
    public sealed class BackwaterSetup : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

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

    public enum PatternEnum
    {
        流水模式 = 1,
        输赢模式 = 2
    }

    /// <summary>
    /// 视讯游戏回水设置表
    /// </summary>
    public sealed class VideoBackwaterSetup : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

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
