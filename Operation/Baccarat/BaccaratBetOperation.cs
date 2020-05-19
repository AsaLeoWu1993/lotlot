using Entity.BaccaratModel;
using MongoDB.Driver;
using System;
using System.Collections.Generic;

namespace Operation.Baccarat
{
    /// <summary>
    /// 百家乐注单
    /// </summary>
    public partial class BaccaratBetOperation
    {
        readonly MongoClient Client = null;
        readonly IMongoDatabase Database = null;
        public readonly FilterDefinitionBuilder<BaccaratBet> Builder = Builders<BaccaratBet>.Filter;
        public BaccaratBetOperation(string path, string dbName)
        {
            MongoUrl url = new MongoUrl(path);
            MongoClientSettings settings = MongoClientSettings.FromUrl(url);
            settings.SocketTimeout = TimeSpan.FromMinutes(5);
            settings.ConnectTimeout = TimeSpan.FromSeconds(30);
            settings.WaitQueueTimeout = TimeSpan.FromSeconds(60);
            settings.MaxConnectionPoolSize = 1500;
            settings.WaitQueueSize = 2000;
            settings.WaitQueueTimeout = TimeSpan.FromSeconds(30);
            settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(5);
            settings.MaxConnectionLifeTime = TimeSpan.FromMinutes(5);
            settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
            Client = new MongoClient(settings);
            Database = Client.GetDatabase(dbName);
        }

        public IMongoCollection<BaccaratBet> GetCollection(string merchantID)
        {
            return Database.GetCollection<BaccaratBet>("BaccaratBet" + merchantID);
        }

        /// <summary>
        /// 添加索引
        /// </summary>
        /// <param name="merchantID"></param>
        public void AddIndexs(string merchantID)
        {
            try
            {
                var name = "BaccaratBet" + merchantID;
                var collection = Database.GetCollection<BaccaratBet>(name);
                collection.Database.CreateCollection(name);
                var list = collection.Indexes.List().ToList();
                var flag = true;
                if (list.Count == 1) flag = false;
                else if (list.Count == 6) return;
                //添加索引
                if (!flag)
                {
                    var indexs = new List<CreateIndexModel<BaccaratBet>>();
                    indexs.Add(new CreateIndexModel<BaccaratBet>(Builders<BaccaratBet>.IndexKeys.Ascending(_ => _.MerchantID)));
                    indexs.Add(new CreateIndexModel<BaccaratBet>(Builders<BaccaratBet>.IndexKeys.Ascending(_ => _.GameType)));
                    indexs.Add(new CreateIndexModel<BaccaratBet>(Builders<BaccaratBet>.IndexKeys.Descending(_ => _.CreatedTime)));
                    indexs.Add(new CreateIndexModel<BaccaratBet>(Builders<BaccaratBet>.IndexKeys.Ascending(_ => _.BetStatus)));
                    indexs.Add(new CreateIndexModel<BaccaratBet>(Builders<BaccaratBet>.IndexKeys.Ascending(_ => _.UserID)));
                    collection.Indexes.CreateMany(indexs);
                }
            }
            catch (Exception)
            { }
        }
    }
}
