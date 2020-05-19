using Entity;
using Entity.BaccaratModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Operation.Abutment;
using Operation.Agent;
using Operation.Baccarat;
using Operation.Common;
using Operation.RedisAggregate;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// redis连接池
    /// </summary>
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

            await sub.SubscribeAsync("ServerToCompany:*", async (channel, message) =>
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
                            await RefreshGameStatus(tid, status, scene, ttime, BaccaratGameType.百家乐);

                            //封盘
                            if (status == "close")
                            {
                                await GameCollection.SendVideoGameLotteryMsg(BaccaratGameType.百家乐, scene, tid, 1);
                            }
                            //开盘
                            else if (status == "init")
                            {
                                await GameCollection.SendVideoGameLotteryMsg(BaccaratGameType.百家乐, scene, tid, 2);
                            }
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
                            await RefreshGameStatus(tid, status, scene, ttime, BaccaratGameType.百家乐, ts["data"].ToString());
                            #region 开奖
                            if (status == "free")
                            {
                                #region 添加数据
                                BaccaratLottery result = new BaccaratLottery();
                                var dic = GameBetsMessage.EnumToDictionary(typeof(BaccaratWinEnum));
                                result.ZNum = tid;
                                result.GameType = BaccaratGameType.百家乐;
                                result.Result = GameBetsMessage.GetEnumByStatus<BaccaratWinEnum>(Convert.ToInt32(ts["result"].ToString()));
                                result.IssueNum = scene;
                                BaccaratLotteryOperation baccaratLotteryOperation = new BaccaratLotteryOperation();
                                await baccaratLotteryOperation.InsertModelAsync(result);

                                //商户开奖  下分任务
                                await GameCollection.VideoGameLottery(BaccaratGameType.百家乐, scene, JsonConvert.SerializeObject(result), tid);
                                #endregion
                            }
                            #endregion
                        }
                        #endregion
                    }
                }
                catch (Exception e)
                {
                    Utils.Logger.Error(e);
                }
            });
        }

        /// <summary>
        /// 牌型转换
        /// </summary>
        /// <param name="porkers">开牌</param>
        /// <returns></returns>
        public static PorkerClass ShowPorker(string porkers)
        {
            List<string> M = porkers.Split(',').ToList(); 
            var ret = new PorkerClass();
            List<int> point = new List<int>();
            foreach (var item in M)
            {
                int porkervalue = 0;
                int.TryParse(item, out porkervalue);
                CardTypeEnum porkertype = CardTypeEnum.方块;
                string porkername = "";
                switch (porkervalue / 100)
                {
                    case 4:
                        porkertype = CardTypeEnum.黑桃;
                        break;
                    case 3:
                        porkertype = CardTypeEnum.红心;
                        break;
                    case 2:
                        porkertype = CardTypeEnum.梅花;
                        break;
                    case 1:
                        porkertype = CardTypeEnum.方块;
                        break;
                }
                switch (porkervalue % 100)
                {
                    case 1:
                        porkername = "A";
                        point.Add(1);
                        break;
                    case 10:
                        porkername = (porkervalue % 100).ToString();
                        break;
                    case 11:
                        porkername = "J";
                        break;
                    case 12:
                        porkername = "Q";
                        break;
                    case 13:
                        porkername = "K";
                        break;
                    default:
                        porkername = (porkervalue % 100).ToString();
                        point.Add(porkervalue % 100);
                        break;
                }
                ret.Porker.Add(new Brand()
                {
                    CardType = porkertype,
                    Num = porkername
                });
            }
            ret.Point = point.Sum() % 10;
            return ret;
        }

        /// <summary>
        /// 类型
        /// </summary>
        public class PorkerClass
        {
            /// <summary>
            /// 牌型
            /// </summary>
            public List<Brand> Porker { get; set; } = new List<Brand>();

            /// <summary>
            /// 点数
            /// </summary>
            public int Point { get; set; }
        }

        /// <summary>
        /// 刷新游戏状态
        /// </summary>
        /// <param name="tid">房间码</param>
        /// <param name="status">状态</param>
        /// <param name="scene">场次</param>
        /// <param name="ttime">倒计时</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="history">历史开奖</param>
        private static async Task RefreshGameStatus(int tid, string status, string scene, string ttime, BaccaratGameType gameType, string history = null)
        {
            var time = Convert.ToInt32(ttime);
            var videoUrl = RedisOperation.GetString("VideoUrl");
            if (string.IsNullOrEmpty(videoUrl))
            {
                PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
                var setup = await platformSetUpOperation.GetModelAsync(t => t._id != "");
                RedisOperation.UpdateString("VideoUrl", setup.VideoUrl, 600);
                videoUrl = setup.VideoUrl;
            }
            var url = string.Format("{0}/b{1:d3}-0.flv", videoUrl, tid);
            //发送游戏状态
            var result = new GameStatic()
            {
                Cstate = status,
                ZNum = tid,
                Scene = scene,
                Ttime = time,
                EndTime = time > 0 ? DateTime.Now.AddSeconds(time) : DateTime.Now,
                GameType = gameType,
                History = history,
                Url = url
            };
            var receive = RedisOperation.GetValue("Baccarat", tid.ToString());
            if (!string.IsNullOrEmpty(receive))
            {
                var oldData = JsonConvert.DeserializeObject<GameStatic>(receive);
                result.ZName = oldData.ZName;
                if (string.IsNullOrEmpty(result.History))
                {
                    result.History = oldData.History;
                }
            }
            RedisOperation.SetHash("Baccarat", tid.ToString(), JsonConvert.SerializeObject(result));
            await RabbitMQHelper.SendGameStatus(JsonConvert.SerializeObject(result));
        }
    }

    /// <summary>
    /// 游戏房间状态
    /// </summary>
    public class GameStatic
    {
        /// <summary>
        /// 房间号
        /// </summary>
        public int ZNum { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Cstate { get; set; } = "stop";

        /// <summary>
        /// 场次
        /// </summary>
        public string Scene { get; set; }

        /// <summary>
        /// 倒计时
        /// </summary>
        public int Ttime { get; set; } = 0;

        /// <summary>
        /// 结束时间
        /// </summary>
        public DateTime EndTime { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public BaccaratGameType GameType { get; set; }

        /// <summary>
        /// 视频地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 桌名
        /// </summary>
        public string ZName { get; set; }

        /// <summary>
        /// 历史记录
        /// </summary>
        public string History { get; set; }
    }
}
