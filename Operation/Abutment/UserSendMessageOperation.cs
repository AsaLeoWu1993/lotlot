using Entity;
using MongoDB.Driver;
using Operation.Common;
using System;
using System.Collections.Generic;

namespace Operation.Abutment
{
    public partial class UserSendMessageOperation
    {
        readonly MongoClient Client = null;
        readonly IMongoDatabase Database = null;
        public readonly FilterDefinitionBuilder<UserSendMessage> Builder = Builders<UserSendMessage>.Filter;

        public UserSendMessageOperation(string path, string dbName)
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

        public IMongoCollection<UserSendMessage> GetCollection(string merchantID)
        {
            var name = "UserSendMessage" + merchantID;
            var collection = Database.GetCollection<UserSendMessage>(name);
            //var list = collection.Indexes.List().ToList();
            //var flag = true;
            //if (list.Count == 1) flag = false;
            //else
            //{
            //    foreach (var item in list)
            //    {
            //        var key = item["key"].AsBsonDocument;
            //        if (key.Contains("Type"))
            //            continue;
            //        else
            //        {
            //            if (key.Contains("_id")) continue;
            //            flag = false;
            //        }
            //    }
            //}
            ////添加索引
            //if (!flag)
            //{
            //    var indexs = new List<CreateIndexModel<UserSendMessage>>();
            //    indexs.Add(new CreateIndexModel<UserSendMessage>(Builders<UserSendMessage>.IndexKeys.Ascending(_ => _.MerchantID)));
            //    indexs.Add(new CreateIndexModel<UserSendMessage>(Builders<UserSendMessage>.IndexKeys.Ascending(_ => _.GameType)));
            //    indexs.Add(new CreateIndexModel<UserSendMessage>(Builders<UserSendMessage>.IndexKeys.Ascending(_ => _.CreatedTime)));
            //    collection.Indexes.CreateMany(indexs);
            //}
            return collection;
        }

        public void AddIndexs(string merchantID)
        {
            var name = "UserSendMessage" + merchantID;
            var collection = Database.GetCollection<UserSendMessage>(name);
            collection.Database.CreateCollection(name);
            var list = collection.Indexes.List().ToList();
            var flag = true;
            if (list.Count == 1) flag = false;
            if (list.Count == 4) return;
            else
            {
                foreach (var item in list)
                {
                    var key = item["key"].AsBsonDocument;
                    if (key.Contains("Type"))
                        continue;
                    else
                    {
                        if (key.Contains("_id")) continue;
                        flag = false;
                    }
                }
            }
            //添加索引
            if (!flag)
            {
                var indexs = new List<CreateIndexModel<UserSendMessage>>();
                indexs.Add(new CreateIndexModel<UserSendMessage>(Builders<UserSendMessage>.IndexKeys.Ascending(_ => _.MerchantID)));
                indexs.Add(new CreateIndexModel<UserSendMessage>(Builders<UserSendMessage>.IndexKeys.Ascending(_ => _.GameType)));
                indexs.Add(new CreateIndexModel<UserSendMessage>(Builders<UserSendMessage>.IndexKeys.Descending(_ => _.CreatedTime)));
                collection.Indexes.CreateMany(indexs);
            }
        }
    }
}
