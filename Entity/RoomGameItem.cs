namespace Entity
{
    /// <summary>
    /// 相关游戏设置
    /// </summary>
    public class RoomGameDetailed : RoomGameInfos
    {
        /// <summary>
        /// 开和  针对5球游戏
        /// </summary>
        public KaiHeEnum? KaiEquality { get; set; }
    }

    /// <summary>
    /// 开和枚举
    /// </summary>
    public enum KaiHeEnum
    {
        返还本金 = 1,
        通杀龙虎 = 2
    }
}
