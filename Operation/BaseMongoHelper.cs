using Entity;
using MongoDB.Driver;
using Operation.Common;
using System;
using System.Configuration;

namespace Operation
{
    public static class BaseMongoHelper
    {
        private static readonly string mongoConn = Environment.GetEnvironmentVariable("Online") == null ? ConfigurationManager.AppSettings["MongoConn"] : ConfigurationManager.AppSettings["MongoConnOnline"];
        public static MongoClient Client { get; private set; } = null;
        public static IMongoDatabase Database { get; private set; } = null;
        static BaseMongoHelper()
        {
            Reconnect();
        }

        /// <summary>
        /// 连接
        /// </summary>
        public static void Reconnect()
        {
            try
            {
                var aesCode = AESHelper.Decrypt(mongoConn);
                MongoUrl url = new MongoUrl(aesCode);
                MongoClientSettings settings = MongoClientSettings.FromUrl(url);
                settings.SocketTimeout = TimeSpan.FromMinutes(5);
                settings.ConnectTimeout = TimeSpan.FromSeconds(60);
                settings.HeartbeatInterval = TimeSpan.FromSeconds(60);
                settings.HeartbeatTimeout = TimeSpan.FromSeconds(60);
                settings.WaitQueueTimeout = TimeSpan.FromSeconds(60);
                settings.MaxConnectionPoolSize = 2000;
                settings.MinConnectionPoolSize = 100;
                settings.WaitQueueSize = 5000;
                settings.MaxConnectionIdleTime = TimeSpan.FromMinutes(5);
                settings.MaxConnectionLifeTime = TimeSpan.FromMinutes(5);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(30);
                Client = new MongoClient(settings);
                Database = Client.GetDatabase("Manage");
            }
            catch (Exception e)
            {
                Utils.Logger.Error(string.Format("地址：{0}, {1}", AESHelper.Decrypt(mongoConn), e));
            }
        }

        public static IMongoCollection<T> GetCollection<T>() where T : BaseModel
        {
            Type type = typeof(T);
            return Database.GetCollection<T>(type.Name);
        }
    }
}
