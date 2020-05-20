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
            var path = "mongodb://admin:123456@172.27.0.11:27017/";
            Console.WriteLine(AESHelper.Encrypt(path));
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
