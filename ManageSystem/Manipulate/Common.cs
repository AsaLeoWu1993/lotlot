using Entity.BaccaratModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Operation.Agent;
using Operation.Baccarat;
using Operation.Common;
using Operation.RedisAggregate;
using Quartz;
using Quartz.Impl;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageSystem.Manipulate
{
    public static class Common
    {
        #region 视讯
        /// <summary>
        /// 游戏房间是否开启
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static async Task<bool> GetVideoGameStatus(string merchantID, BaccaratGameType gameType = BaccaratGameType.百家乐)
        {
            VideoRoomOperation videoRoomOperation = new VideoRoomOperation();
            var room = await videoRoomOperation.GetModelAsync(t => t.MerchantID == merchantID && t.GameType == gameType);
            if (room == null) return false;
            return room.Status == Entity.RoomStatus.开启;
        }
        #endregion

        /// <summary>
        /// 刷新游戏房间列表
        /// </summary>
        public static async Task GetGameList()
        {
            try
            {
                RedisOperation.DeleteKey("Baccarat");
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
                var _redisconn = ConnectionMultiplexer.Connect(config);
                var _db = _redisconn.GetDatabase(0);
                var hash = _db.HashGetAll("DeskStatusModel");
                var dic = new Dictionary<string, string>();
                var videoUrl = RedisOperation.GetString("VideoUrl");
                if (string.IsNullOrEmpty(videoUrl))
                {
                    PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
                    var setup = await platformSetUpOperation.GetModelAsync(t => t._id != "");
                    RedisOperation.UpdateString("VideoUrl", setup.VideoUrl, 600);
                    videoUrl = setup.VideoUrl;
                }
                foreach (var data in hash)
                {
                    var model = JsonConvert.DeserializeObject<DeskStatusModel>(data.Value);
                    #region 百家乐
                    if (model.Gid == 1)
                    {
                        //场次
                        var scene = string.Format("{0}-{1}-{2}", model.Tid, model.Ch, model.Ci);
                        
                        //地址
                        var url = string.Format("{0}/b{1:d3}-0.flv", videoUrl, model.Tid);
                        var info = new GameStatic()
                        {
                            Scene = scene,
                            Url = url,
                            ZName = model.Tname,
                            ZNum = model.Tid,
                            History = model.Data,
                            Cstate = model.Cstate,
                            Ttime = model.Ttime,
                            GameType = BaccaratGameType.百家乐
                        };
                        dic.Add(model.Tid.ToString(), JsonConvert.SerializeObject(info));
                    }
                    #endregion
                }
                RedisOperation.UpdateCacheKey("Baccarat", dic);
                _redisconn.Dispose();
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 机器人自动确认
        /// </summary>
        /// <param name="recordID"></param>
        /// <param name="merchantID"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        public static async Task SendShamUserApply(string recordID, string merchantID, int time)
        {
            var key = Guid.NewGuid().ToString();
            //1、通过调度工厂获得调度器
            //var scheduler = await _schedulerFactory.GetScheduler();
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            //2、开启调度器
            await scheduler.Start();
            IDictionary<string, object> dic = new Dictionary<string, object>
            {
                { "merchantID", merchantID },
                { "recordID", recordID }
            };
            var jobDetail = JobBuilder.Create<SendShamUserApplyJob>()
                            .UsingJobData(new JobDataMap(dic))
                            .WithIdentity(key, "SendShamUserApply")
                            .Build();
            var trigger = (ISimpleTrigger)TriggerBuilder.Create()
            .WithIdentity(key)
            .StartAt(DateBuilder.FutureDate(time, IntervalUnit.Second))
            .ForJob(jobDetail)
            .Build();

            //5、将触发器和任务器绑定到调度器中
            await scheduler.ScheduleJob(jobDetail, trigger);
        }
    }

    /// <summary>
    /// 游戏信息
    /// </summary>
    public class DeskStatusModel
    {
        /// <summary>
        /// 厅号
        /// </summary>
        public int Cid { get; set; }
        /// <summary>
        /// 游戏
        /// </summary>
        public int Gid { get; set; }
        /// <summary>
        /// 桌号
        /// </summary>
        public int Tid { get; set; }
        /// <summary>
        /// 靴
        /// </summary>
        public int Ch { get; set; }
        /// <summary>
        /// 铺
        /// </summary>
        public int Ci { get; set; }
        /// <summary>
        /// 桌名
        /// </summary>
        public string Tname { get; set; }
        /// <summary>
        /// 网络最高限红
        /// </summary>
        public decimal Xh { get; set; }
        /// <summary>
        /// 网络最低限红
        /// </summary>
        public decimal Dx { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 现场最高限红
        /// </summary>
        public decimal Txh { get; set; }
        /// <summary>
        /// 现场最低限红
        /// </summary>
        public decimal Tdx { get; set; }
        /// <summary>
        /// 包台1为包台中
        /// </summary>
        public int Monopolize { get; set; }
        /// <summary>
        /// 倒计时秒数
        /// </summary>
        public int Ttime { get; set; }
        /// <summary>
        /// 桌子状态 free init stop close openning ,init 状态可以下注
        /// </summary>
        public string Cstate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Bt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte Vt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Va { get; set; }
        /// <summary>
        /// 计时开始时间
        /// </summary>
        public DateTime Inittime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<JObject> Vmoney { get; set; } = new List<JObject>();
        /// <summary>
        /// 
        /// </summary>
        public int vcount { get; set; }
    }
}
