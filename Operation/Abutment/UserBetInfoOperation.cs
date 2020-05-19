using Entity;
using MongoDB.Bson;
using MongoDB.Driver;
using Operation.Common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Operation.Abutment
{
    public partial class UserBetInfoOperation
    {
        readonly MongoClient Client = null;
        readonly IMongoDatabase Database = null;
        public readonly FilterDefinitionBuilder<UserBetInfo> Builder = Builders<UserBetInfo>.Filter;
        public UserBetInfoOperation(string path, string dbName)
        {
            MongoUrl url = new MongoUrl(path);
            MongoClientSettings settings = MongoClientSettings.FromUrl(url);
            settings.SocketTimeout = TimeSpan.FromMinutes(5);
            settings.ConnectTimeout = TimeSpan.FromSeconds(30);
            settings.WaitQueueTimeout = TimeSpan.FromSeconds(60);
            settings.MaxConnectionPoolSize = 2000;
            settings.MinConnectionPoolSize = 100;
            settings.WaitQueueSize = 5000;
            settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(5);
            settings.MaxConnectionLifeTime = TimeSpan.FromMinutes(5);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
            Client = new MongoClient(settings);
            Database = Client.GetDatabase(dbName);
        }

        public IMongoCollection<UserBetInfo> GetCollection(string merchantID)
        {
            var name = "UserBetInfo" + merchantID;
            var collection = Database.GetCollection<UserBetInfo>(name);
            return collection;
        }

        /// <summary>
        /// 添加索引
        /// </summary>
        /// <param name="merchantID"></param>
        public void AddIndexs(string merchantID)
        {
            try
            {
                var name = "UserBetInfo" + merchantID;
                var collection = Database.GetCollection<UserBetInfo>(name);
                collection.Database.CreateCollection(name);
                var list = collection.Indexes.List().ToList();
                var flag = true;
                if (list.Count == 1) flag = false;
                else if (list.Count == 6) return;
                //添加索引
                if (!flag)
                {
                    var indexs = new List<CreateIndexModel<UserBetInfo>>();
                    indexs.Add(new CreateIndexModel<UserBetInfo>(Builders<UserBetInfo>.IndexKeys.Ascending(_ => _.MerchantID)));
                    indexs.Add(new CreateIndexModel<UserBetInfo>(Builders<UserBetInfo>.IndexKeys.Ascending(_ => _.GameType)));
                    indexs.Add(new CreateIndexModel<UserBetInfo>(Builders<UserBetInfo>.IndexKeys.Descending(_ => _.CreatedTime)));
                    indexs.Add(new CreateIndexModel<UserBetInfo>(Builders<UserBetInfo>.IndexKeys.Ascending(_ => _.BetStatus)));
                    indexs.Add(new CreateIndexModel<UserBetInfo>(Builders<UserBetInfo>.IndexKeys.Ascending(_ => _.UserID)));
                    collection.Indexes.CreateMany(indexs);
                }

                //var stage = new List<IPipelineStageDefinition>();
                //stage.Add(new JsonPipelineStageDefinition<BsonDocument, BsonDocument>("{$match:{\"BetStatus\":{$ne:4}}}"));
                //stage.Add(new JsonPipelineStageDefinition<BsonDocument, BsonDocument>("{$group:{\"_id\":\"$OddNum\",\"UserID\":{$first:\"$UserID\"},\"GameType\":{$first:\"$GameType\"},\"Nper\":{$first:\"$Nper\"},\"Remark\":{$first:\"$Remark\"},\"MediumBonus\":{$push:\"$MediumBonus\"},\"BetTime\":{$max:\"$CreatedTime\"},\"Status\":{$push:\"$BetStatus\"},\"UserBetMoney\":{$push:\"$BetMoney\"}}}"));
                //stage.Add(new JsonPipelineStageDefinition<BsonDocument, BsonDocument>("{$group:{\"_id\":{\"UserID\":\"$UserID\",\"GameType\":\"$GameType\",\"Nper\":\"$Nper\"},\"Remarks\":{$push:\"$Remark\"},\"MediumBonus\":{$push:\"$MediumBonus\"},\"BetTime\":{$max:\"$BetTime\"},\"Status\":{$push:\"$Status\"},\"UserBetMoney\":{$push:\"$UserBetMoney\"}}}"));
                //stage.Add(new JsonPipelineStageDefinition<BsonDocument, BsonDocument>("{$project:{\"_id\":0,\"UserID\":\"$_id.UserID\",\"GameType\":\"$_id.GameType\",\"Nper\":\"$_id.Nper\",\"Remarks\":1,\"BetTime\":1,\"Status\":{$reduce:{input:\"$Status\",initialValue:[],in:{$concatArrays:[\"$$value\",\"$$this\"]}}},\"UserBetMoney\":{$reduce:{input:\"$UserBetMoney\",initialValue:[],in:{$concatArrays:[\"$$value\",\"$$this\"]}}},\"MediumBonus\":{$reduce:{input:\"$MediumBonus\",initialValue:[],in:{$concatArrays:[\"$$value\",\"$$this\"]}}}}}"));
                //PipelineDefinition<BsonDocument, BsonDocument> pipeline = new PipelineStagePipelineDefinition<BsonDocument, BsonDocument>(stage);
                //BsonOperation bsonOperation = new BsonOperation(name);
                //bsonOperation.Collection.Database.DropCollection("View_" + name);
                //bsonOperation.Collection.Database.CreateView("View_" + name, name, pipeline);
            }
            catch (Exception)
            { }
        }
    }
}
