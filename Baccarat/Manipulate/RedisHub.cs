using Baccarat.Interactive;
using Baccarat.RedisModel;
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

namespace Baccarat.Manipulate
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
                EndPoints = { "60.12.124.242:6380" }
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
                                await GameLottery(BaccaratGameType.百家乐, scene, JsonConvert.SerializeObject(result), tid);
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

        #region 游戏开奖
        /// <summary>
        /// 发送开奖任务
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="nper">期号</param>
        /// <param name="lottery">开奖结果</param>
        /// <param name="znum">桌号</param>
        /// <returns></returns>
        public static async Task GameLottery(BaccaratGameType gameType, string nper, string lottery, int znum)
        {
            try
            {
                AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
                var setup = await advancedSetupOperation.GetModelAsync(t => t._id != null);
                if (setup == null) return;
                Expression<Func<Merchant, bool>> filter = null;
                if (setup.Formal)
                    filter = t => t.Status == 1 && t.MaturityTime >= DateTime.Now;
                else
                    filter = t => t.Status == 1;
                //查询商户
                MerchantOperation merchantOperation = new MerchantOperation();
                //商户总数量
                var count = await merchantOperation.GetCountAsync(filter);
                //一组数量
                var pageSize = DistributionLow.limit;
                var pageNum = count % pageSize == 0 ? count / pageSize : count / pageSize + 1;
                var result = new List<TaskDistributionModel>();
                for (int num = 1; num <= pageNum; num++)
                {
                    var merchantList = merchantOperation.GetModelListByPaging(filter, t => t.CreatedTime, true, num, pageSize);
                    if (merchantList.IsNull()) continue;
                    var data = new TaskDistributionModel()
                    {
                        GameType = gameType,
                        Lottery = lottery,
                        Nper = nper,
                        ZNum = znum,
                        MerchantIDList = merchantList.Select(t => t._id).ToList()
                    };
                    result.Add(data);
                    RabbitMQHelper.SendTaskDistribution(data);
                }
                await TaskRetransmission(result);
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
            finally
            {

            }
        }

        /// <summary>
        /// 补发任务
        /// </summary>
        /// <param name="result"></param>
        private static async Task TaskRetransmission(List<TaskDistributionModel> result)
        {
            if (result.IsNull()) return;
            DistributionOperation distributionOperation = new DistributionOperation();
            for (int i = 0; i < result.Count; i++)
            {
                var data = result[i];
                var bson = await distributionOperation.GetModelAsync(t => t._id == data.UUID);
                if (bson == null)
                {
                    //再次发送直到有服务器接收
                    RabbitMQHelper.SendTaskDistribution(data);
                }
                else
                {
                    result.RemoveAt(i);
                    i--;
                }
            }
            await TaskRetransmission(result);
        }
        #endregion

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
            var url = string.Format("{0}/b{1:d3}-0.flv", "http://down.jdy7.com/live/tl", tid);
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
            var receive = await RedisOperation.GetValueAsync("Baccarat", tid.ToString());
            if (!string.IsNullOrEmpty(receive))
            {
                var oldData = JsonConvert.DeserializeObject<GameStatic>(receive);
                result.ZName = oldData.ZName;
                if (string.IsNullOrEmpty(result.History))
                {
                    result.History = oldData.History;
                }
            }
            await RedisOperation.SetHashAsync("Baccarat", tid.ToString(), JsonConvert.SerializeObject(result));
            await RabbitMQHelper.SendGameStatus(JsonConvert.SerializeObject(result));
        }
    }
}
