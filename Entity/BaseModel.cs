using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Entity
{
    /// <summary>
    /// 通用属性
    /// </summary>
    public class BaseModel
    {
        /// <summary>
        /// mongo 标识id
        /// </summary>
        ///public string _id { get; set; } = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
        public string _id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 创建时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [BsonDateTimeOptions(Kind = DateTimeKind.Local)]
        public DateTime LastUpdateTime { get; set; } = DateTime.Now;
    }
}
