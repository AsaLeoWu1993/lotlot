using System;

namespace Entity.AgentModel
{
    //数据库地址  下注表
    public class DatabaseAddress : BaseModel
    {
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 现有数
        /// </summary>
        [Obsolete("不使用", true)]
        public int Count { get; set; } = 0;

        /// <summary>
        /// 最大人数
        /// </summary>
        public int MaxCount { get; set; } = 100;

        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DBName { get; set; }
    }
}
