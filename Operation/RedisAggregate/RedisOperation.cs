using CSRedis;
using Entity;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Common;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Operation.RedisAggregate
{
    public static class RedisOperation
    {
        //private static readonly string RedisconnectionStr = ConfigurationManager.AppSettings["RedisConn"];

        //redis 连接
        //private static ConnectionMultiplexer _multiplexer = null;
        //据库
        //private static IDatabase redisDB;
        private static CSRedisClient client = null;
        private static string connstr = string.Empty;
        //public static IDatabase DB
        //{
        //    get { return redisDB; }
        //}

        static RedisOperation()
        {
            try
            {
                if (Environment.GetEnvironmentVariable("Online") == null)
                    connstr = string.Format("{0}:{1},password={2},poolsize=500,defaultDatabase=4",
                        //"redis-app-0.redis-service.default.svc.cluster.local", 6379, "Skhd0euuL5hI8L7afpbwTauppUDaA1MhVNdJ=");
                //"old.teleicu.cn", 8004, "cisRedisqwerasdfzxcv");
                "0.0.0.0", 6379, "123456");
                else
                    connstr = string.Format("{0}:{1},password={2},poolsize=100,defaultDatabase=4",
                        "redis-app-0.redis-service.default.svc.cluster.local", 6379, "Skhd0euuL5hI8LksdjfwTauppUDaA1MhVNdJ=");
                client = new CSRedisClient(connstr);
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
        }


        //public static void StartConn()
        //{
        //    try
        //    {
        //        var config = new ConfigurationOptions();
        //        if (Environment.GetEnvironmentVariable("Online") == null)
        //        {
        //            config = new ConfigurationOptions()
        //            {
        //                AbortOnConnectFail = false,
        //                AllowAdmin = true,
        //                ConnectTimeout = 15000,
        //                SyncTimeout = 10000,
        //                Timeout = 10000,
        //                Ssl = true,
        //                //Password = "Skhd0euuL5hI8L7afpbwTauppUDaA1MhVNdJ=",//Redis数据库密码
        //                //EndPoints = { "redis-app-0.redis-service.default.svc.cluster.local:6379" }
        //                Password = "cisRedisqwerasdfzxcv",
        //                EndPoints = { "old.teleicu.cn:8004" }
        //            };
        //        }
        //        else
        //        {
        //            config = new ConfigurationOptions()
        //            {
        //                AbortOnConnectFail = false,
        //                AllowAdmin = true,
        //                ConnectTimeout = 60000,
        //                SyncTimeout = 60000,
        //                Timeout = 60000,
        //                ConnectRetry = 100,
        //                KeepAlive = 60000,
        //                Password = "Skhd0euuL5hI8LksdjfwTauppUDaA1MhVNdJ=",//Redis数据库密码
        //                EndPoints = { "redis-app-0.redis-service.default.svc.cluster.local:6379" }
        //                //Password = "cisRedisqwerasdfzxcv",
        //                //EndPoints = { "old.teleicu.cn:8004" }
        //            };
        //        }
        //        _multiplexer = ConnectionMultiplexer.Connect(config);
        //        redisDB = _multiplexer.GetDatabase(4);
        //    }
        //    catch (Exception e)
        //    {
        //        Utils.Logger.Error(e);
        //    }
        //}

        //private static readonly object Locker = new object();
        //private static void RedisConnStatus()
        //{
        //    if (_multiplexer == null || !_multiplexer.IsConnected)
        //    {
        //        lock (Locker)
        //        {
        //            StartConn();
        //        }
        //    }
        //}

        /// <summary>
        /// 检测token是否相同
        /// </summary>
        /// <param name="key"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static bool CheckToken(string key, string token)
        {
            try
            {
                var hash = client.HGet(key, "Token");
                if (hash != token)
                    return false;
                return true;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                return CheckToken(key, token);
            }
        }

        /// <summary>
        /// 更新值
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name=dic">字典</param>
        public static void UpdateCacheKey(string key, Dictionary<string, string> dic)
        {
            //RedisConnStatus();
            //if (DB.KeyExists(key)) DB.KeyDelete(key);
            //List<HashEntry> rseult = new List<HashEntry>();
            //foreach (var item in dic)
            //{
            //    rseult.Add(new HashEntry(item.Key, item.Value));
            //}
            //DB.HashSet(key, rseult.ToArray());
            ////设置2小时的生命周期
            //DB.KeyExpire(key, TimeSpan.FromHours(20));
            try
            {
                if (client.Exists(key)) client.Del(key);
                //client.HMSet(key, dic.ToArray());
                foreach (var item in dic)
                {
                    client.HSet(key, item.Key, item.Value);
                    //RedisHelper.HSet(key, item.Key, item.Value);
                }
                client.Expire(key, TimeSpan.FromHours(20));
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                client.Dispose();
                //RedisHelper.Initialization(client);
                UpdateCacheKey(key, dic);
            }
        }

        /// <summary>
        /// 更新hash值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyName"></param>
        /// <param name="value"></param>
        public static void SetHash(string key, string keyName, string value)
        {
            try
            {
                client.HSet(key, keyName, value);
                //RedisHelper.HMSet(key, keyName, value);
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                SetHash(key, keyName, value);
            }
        }

        /// <summary>
        /// 更新值
        /// </summary>
        /// <param name="key">标识</param>
        /// <param name="dic">字典</param>
        /// <param name="hour">时长</param>
        public static void UpdateCacheKey(string key, Dictionary<string, string> dic, int hour = 1)
        {
            try
            {
                if (client.Exists(key))
                    client.Del(key);
                //client.HMSet(key, dic.ToArray());
                foreach (var item in dic)
                {
                    client.HSet(key, item.Key, item.Value);
                    //RedisHelper.HSet(key, item.Key, item.Value);
                }
                client.Expire(key, TimeSpan.FromHours(hour));
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                UpdateCacheKey(key, dic, hour);
            }
        }

        /// <summary>
        /// 删除hash中的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyName"></param>
        public static void DeleteCacheKey(string key, string keyName)
        {
            //RedisConnStatus();
            //DB.HashDelete(key, keyName);
            try
            {
                client.HDel(key, keyName);
                //RedisHelper.HDel(key, keyName);
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                DeleteCacheKey(key, keyName);
            }
        }

        /// <summary>
        /// 删除键
        /// </summary>
        /// <param name="key"></param>
        public static void DeleteKey(string key)
        {
            //RedisConnStatus();
            //DB.KeyDelete(key);
            try
            {
                client.Del(key);
                //RedisHelper.Del(key);
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                DeleteKey(key);
            }
        }

        /// <summary>
        /// 获取key中所有值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Dictionary<string, string> GetValues(string key)
        {
            try
            {
                var dic = client.HGetAll(key);
                return dic;
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                return GetValues(key);
            }
        }

        /// <summary>
        /// 获取值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static string GetValue(string key, string keyName)
        {
            try
            {
                var result = client.HGet(key, keyName);
                return result;
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                return GetValue(key, keyName);
            }
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static T GetValue<T>(string key, string keyName) where T : class, new()
        {
            try
            {
                var result = client.HGet<T>(key, keyName);
                return result;
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                return GetValue<T>(key, keyName);
            }
        }

        /// <summary>
        /// 根据key值获取hash列表
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Dictionary<string, T> GetHashValue<T>(string key) where T : class, new()
        {
            try
            {
                var result = client.HGetAll<T>(key);
                return result;
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                return GetHashValue<T>(key);
            }
        }

        /// <summary>
        /// 设置string
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static void UpdateString(string key, string data, int timeOut = 1)
        {
            //RedisConnStatus();
            //DB.StringSet(key, data);
            //DB.KeyExpire(key, TimeSpan.FromMinutes(timeOut));
            try
            {
                client.Set(key, data);
                client.Expire(key, TimeSpan.FromMinutes(timeOut));
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                UpdateString(key, data, timeOut);
            }
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string GetString(string key)
        {
            try
            {
                var result = client.Get(key);
                return result;
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                return GetString(key);
            }
        }

        #region 管理员头像
        /// <summary>
        /// 获取商户管理员头像
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static string GetAdminPortrait(string merchantID)
        {
            try
            {
                var result = client.HGet("AdminPortrait", merchantID);
                if (string.IsNullOrEmpty(result))
                {
                    RoomOperation roomOperation = new RoomOperation();
                    var room = roomOperation.GetModel(t => t.MerchantID == merchantID);
                    result = room.AdminPortrait;
                    client.HSet("AdminPortrait", merchantID, result);
                }
                return result;
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                return GetAdminPortrait(merchantID);
            }
        }

        /// <summary>
        /// 设置商户管理员头像
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static void SetAdminPortrait(string merchantID, string url)
        {
            try
            {
                client.HSet("AdminPortrait", merchantID, url);
                //RedisHelper.HSet("AdminPortrait", merchantID, url);
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                SetAdminPortrait(merchantID, url);
            }
        }
        #endregion
        #region 商户设置
        /// <summary>
        /// 获取彩票游戏设置
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static FoundationSetup GetFoundationSetup(string merchantID)
        {
            try
            {
                var result = client.HGet<FoundationSetup>("FoundationSetup", merchantID);
                if (result == null)
                {
                    FoundationSetupOperation foundationSetupOperation = new FoundationSetupOperation();
                    var setup = foundationSetupOperation.GetFoundationByNo(merchantID);
                    if (setup == null)
                    {
                        setup = new FoundationSetup()
                        {
                            MerchantID = merchantID,
                            LotteryFrontTime = new List<LotteryItem>()
        {
            new LotteryItem()
            {
                Type = GameOfType.北京赛车,
                LotteryTime = 60
            },
            new LotteryItem()
            {
                Type = GameOfType.幸运飞艇,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.重庆时时彩,
                LotteryTime = 60
            },
            new LotteryItem()
            {
                Type = GameOfType.极速赛车,
                LotteryTime = 20
            },
            new LotteryItem()
            {
                Type = GameOfType.澳州10,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.澳州5,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.爱尔兰赛马,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.爱尔兰快5,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.幸运飞艇168,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.极速时时彩,
                LotteryTime = 20
            }
        }
                        };
                        foundationSetupOperation.InsertModel(setup);
                    }
                    result = setup;
                    client.HSet("FoundationSetup", merchantID, JsonConvert.SerializeObject(result));
                }
                return result;
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                return GetFoundationSetup(merchantID);
            }
        }

        /// <summary>
        /// 设置彩票游戏设置
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void SetFoundationSetup(string merchantID, FoundationSetup data)
        {
            try
            {
                client.HDel("FoundationSetup", merchantID);
                client.HSet("FoundationSetup", merchantID, JsonConvert.SerializeObject(data));
                //RedisHelper.HMSet("FoundationSetup", merchantID, JsonConvert.SerializeObject(data));
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                SetFoundationSetup(merchantID, data);
            }
        }

        /// <summary>
        /// 获取视讯游戏设置
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static VideoFoundationSetup GetVideoFoundationSetup(string merchantID)
        {
            try
            {
                var result = client.HGet<VideoFoundationSetup>("VideoFoundationSetup", merchantID);
                if (result == null)
                {
                    VideoFoundationSetupOperation foundationSetupOperation = new VideoFoundationSetupOperation();
                    var setup = foundationSetupOperation.GetFoundationByNo(merchantID);
                    if (setup == null)
                    {
                        setup = new VideoFoundationSetup()
                        {
                            MerchantID = merchantID
                        };
                        foundationSetupOperation.InsertModel(setup);
                    }
                    result = setup;
                    client.HSet("VideoFoundationSetup", merchantID, JsonConvert.SerializeObject(result));
                }
                return result;
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                return GetVideoFoundationSetup(merchantID);
            }
        }

        /// <summary>
        /// 设置视讯游戏设置
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static void SetVideoFoundationSetup(string merchantID, VideoFoundationSetup data)
        {
            try
            {
                client.HDel("VideoFoundationSetup", merchantID);
                client.HSet("VideoFoundationSetup", merchantID, JsonConvert.SerializeObject(data));
                //RedisHelper.HSet("VideoFoundationSetup", merchantID, JsonConvert.SerializeObject(data));
            }
           catch (Exception e)
            {
                Utils.Logger.Error(e);
                //RedisHelper.Initialization(client);
                SetVideoFoundationSetup(merchantID, data);
            }
        }
        #endregion
    }
}
