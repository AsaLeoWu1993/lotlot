using Baccarat.Interactive;
using Baccarat.RedisModel;
using Entity.BaccaratModel;
using Newtonsoft.Json;
using Operation.Baccarat;
using Operation.Common;
using Operation.RedisAggregate;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Baccarat.Manipulate
{
    /// <summary>
    /// 公共方法
    /// </summary>
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
        public static void GetGameList()
        {
            try
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
                var _redisconn = ConnectionMultiplexer.Connect(config);
                var _db = _redisconn.GetDatabase(0);
                var hash = _db.HashGetAll("DeskStatusModel");
                var dic = new Dictionary<string, string>();
                RedisOperation.DeleteKey("Baccarat");
                foreach (var data in hash)
                {
                    var model = JsonConvert.DeserializeObject<DeskStatusModel>(data.Value);
                    #region 百家乐
                    if (model.Gid == 1)
                    {
                        //场次
                        var scene = string.Format("{0}-{1}-{2}", model.Tid, model.Ch, model.Ci);
                        //地址
                        var url = string.Format("{0}/b{1:d3}-0.flv", "http://down.jdy7.com/live/tl", model.Tid);
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
    }
}
