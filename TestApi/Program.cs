using Entity;
using Entity.GraspModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Agent;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TestApi
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var path = "mongodb://admin:123456@0.0.0.0:27027/";
            MongoUrl url = new MongoUrl(path);
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
            var Client = new MongoClient(settings);
            var Database = Client.GetDatabase("Manage");
            End();
        }

        static void End()
        {
            var key = Console.ReadLine();
            if (key.ToLower() != "end")
                End();
        }
    }
}
