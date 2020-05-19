using MongoDB.Bson;
using MongoDB.Driver;

namespace Operation.Abutment
{
    public partial class BsonOperation
    {
        public IMongoCollection<BsonDocument> Collection { get; private set; }
        /// <summary>
        /// 查询条件
        /// </summary>
        public FilterDefinitionBuilder<BsonDocument> Builder = Builders<BsonDocument>.Filter;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="collectionName"></param>
        /// <param name="baseMongo"></param>
        public BsonOperation(string collectionName)
        {
            Collection = BaseMongoHelper.Database.GetCollection<BsonDocument>(collectionName);
        }
    }
}
