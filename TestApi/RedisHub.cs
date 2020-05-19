using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestApi
{
    public static class RedisHub
    {
        private static ConnectionMultiplexer _redisconn;
        private static IDatabase _db;
        static RedisHub()
        {
            var config = new ConfigurationOptions()
            {
                AbortOnConnectFail = false,
                AllowAdmin = true,
                ConnectTimeout = 15000,
                SyncTimeout = 10000,
                AsyncTimeout = 10000,
                Password = "12345678",//Redis数据库密码
                EndPoints = { "tl-wt-data.122672.com:6380" }
            };
            _redisconn = ConnectionMultiplexer.Connect(config);
            _db = _redisconn.GetDatabase(0);
            Console.WriteLine("连接成功");
            //失去连接
            _redisconn.ConnectionFailed += async (sender, args) =>
            {
                _redisconn = ConnectionMultiplexer.Connect(config);
                _db = _redisconn.GetDatabase(0);
                await ServerToCompany();
            };
            //恢复连接
            _redisconn.ConnectionRestored += async (sender, args) =>
            {
                await ServerToCompany();
            };
        }

        /// <summary>
        /// 监听推送
        /// </summary>
        public static async Task ServerToCompany()
        {
            ISubscriber sub = _redisconn.GetSubscriber();

            await sub.SubscribeAsync("ServerToCompany:*", (channel, message) =>
            {
                if (message.IsNullOrEmpty) return;
                try
                {
                    var drmessage = message.ToString().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (drmessage == null || drmessage.Count() < 2) return;
                    var id = drmessage.First();
                    var json = drmessage.ElementAt(1);
                    //gid  1:百家乐 2:龙虎 3:牛牛
                    var ts = JsonConvert.DeserializeObject<JObject>(json);
                    var gid = ts["gid"].ToString();
                    //桌子状态
                    if (id.Equals("5009"))
                    {
                        #region 百家乐
                        if (gid == "1")
                        {
                            var tid = Convert.ToInt32(ts["tid"].ToString());
                            var status = ts["cstate"].ToString();
                            var ttime = ts.ContainsKey("ttime") ? ts["ttime"].ToString() : "0";
                            //场次
                            var scene = string.Format("{0}-{1}-{2}", tid, ts["ch"].ToString(), ts["ci"].ToString());

                            //if (tid == 21)
                                Console.WriteLine(json);
                        }
                        #endregion
                    }
                    //开奖
                    else if (id.Equals("5010"))
                    {
                        #region 百家乐
                        if (gid == "1")
                        {
                            var tid = Convert.ToInt32(ts["tid"].ToString());
                            var status = ts["cstate"].ToString();
                            var ttime = ts.ContainsKey("ttime") ? ts["ttime"].ToString() : "0";
                            //场次
                            var scene = string.Format("{0}-{1}-{2}", tid, ts["ch"].ToString(), ts["ci"].ToString());
                            #region 开奖
                            //if (tid == 21)
                                Console.WriteLine(json);
                            #endregion
                        }
                        #endregion
                    }
                }
                catch (Exception e)
                {
                    throw e;
                }
            });
        }

    }
}
