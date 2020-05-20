using Entity;
using Entity.BaccaratModel;
using Entity.RedisModel;
using Entity.WebModel;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using NLog;
using NLog.Config;
using NLog.Targets;
using Operation.Abutment;
using Operation.Agent;
using Operation.Baccarat;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Operation.Common
{
    public static class Utils
    {
        /// <summary>
        /// 是否为采集
        /// </summary>
        public static bool Variable { get => Environment.GetEnvironmentVariable("open") != null; }

        /// <summary>
        /// 采集系统
        /// </summary>
        public static bool Collect { get => Environment.GetEnvironmentVariable("collect") != null; }

        /// <summary>
        /// 是否为开奖容器
        /// </summary>
        public static bool Lottery { get => Environment.GetEnvironmentVariable("lottery") != null; }

        /// <summary>
        /// 日志
        /// </summary>
        public static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        static Utils()
        {
            //初始化配置日志
            //LogManager.Configuration = new XmlLoggingConfiguration(AppDomain.CurrentDomain.BaseDirectory.ToString() + "/NLog.config");

            var config = new LoggingConfiguration();
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);
            //fileTarget.FileName = "./wwwroot/Images/Logs/${shortdate}/${level:uppercase=false:padding=-5}.txt";
            fileTarget.FileName = "./wwwroot/Images/Logs/${shortdate}/ErrorTest.txt";
            fileTarget.Layout = "${longdate}|${message}${onexception:${exception:format=tostring}${newline}${stacktrace}${newline}";
            var rule = new LoggingRule("*", LogLevel.Error, fileTarget);
            //var ruleBebug = new LoggingRule("*", LogLevel.Debug, fileTarget);
            config.LoggingRules.Add(rule);
            //config.LoggingRules.Add(ruleBebug);
            LogManager.Configuration = config;
        }

        /// <summary>
        /// 判断游戏分类  10球为true  5球为false
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public static bool GameTypeItemize(GameOfType gameType)
        {
            if (gameType == GameOfType.澳州5 || gameType == GameOfType.爱尔兰快5 || gameType == GameOfType.重庆时时彩 || gameType == GameOfType.极速时时彩)
                return false;
            return true;
        }

        /// <summary>
        /// 获取游戏状态
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static async Task<WebAppGameInfos> GetGameStatus(string merchantID, GameOfType gameType)
        {
            var status = RedisOperation.GetValue("MerchantGameStatus", merchantID + Enum.GetName(typeof(GameOfType), gameType));
            if (string.IsNullOrEmpty(status))
            {
                return await GameDiscrimination.EachpartAsync(gameType, merchantID);
            }
            var result = JsonConvert.DeserializeObject<WebAppGameInfos>(status);
            if ((DateTime.Now - result.StartTime).TotalHours > 1)
            {
                var gameInfo = await GetGameNextLottery(gameType);
                //添加到redis
                RedisOperation.SetHash("GameStatus", Enum.GetName(typeof(GameOfType), gameType), JsonConvert.SerializeObject(gameInfo));
                return await GameDiscrimination.EachpartAsync(gameType, merchantID, gameInfo);
            }
            if (result.Status == GameStatusEnum.等待中)
            {
                //等待中
                if (result.StartTime.AddSeconds(-result.SealingTime) > DateTime.Now)
                {
                    result.Surplus = (int)(result.StartTime.AddSeconds(-result.SealingTime) - DateTime.Now).TotalSeconds;
                }
                else if (result.StartTime.AddSeconds(-result.SealingTime) <= DateTime.Now && result.StartTime > DateTime.Now)
                {
                    result.Status = GameStatusEnum.封盘中;
                    result.SealingTime = 0;
                    result.Surplus = (int)(result.StartTime - DateTime.Now).TotalSeconds;
                }
                else
                {
                    var flag = false;
                    if (GameTypeItemize(gameType))
                    {
                        var lottery10Operation = new Lottery10Operation();
                        //最新开奖
                        var newLottery = await lottery10Operation.GetModelAsync(t => t.IssueNum == result.NextIssueNum && t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID));
                        flag = newLottery == null;
                    }
                    else
                    {
                        var lottery5Operation = new Lottery5Operation();
                        //最新开奖
                        var newLottery = await lottery5Operation.GetModelAsync(t => t.IssueNum == result.NextIssueNum && t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID));
                        flag = newLottery == null;
                    }
                    //开奖时间
                    var lotteryTime = 0;
                    if (gameType == GameOfType.极速赛车)
                    {
                        lotteryTime = 10 * 3;
                    }
                    else
                    {
                        lotteryTime = 300;
                    }
                    if (flag && result.StartTime.AddSeconds(lotteryTime) >= DateTime.Now)
                    {
                        result.Status = GameStatusEnum.开奖中;
                        result.Surplus = 0;
                        result.SealingTime = 0;
                    }
                    else return await GameDiscrimination.EachpartAsync(gameType, merchantID);
                }
            }
            else if (result.Status == GameStatusEnum.封盘中)
            {
                if (result.StartTime.AddSeconds(-result.SealingTime) <= DateTime.Now && result.StartTime > DateTime.Now)
                {
                    result.Status = GameStatusEnum.封盘中;
                    result.SealingTime = 0;
                    result.Surplus = (int)(result.StartTime - DateTime.Now).TotalSeconds;
                }
                else
                {
                    var flag = false;
                    if (GameTypeItemize(gameType))
                    {
                        var lottery10Operation = new Lottery10Operation();
                        //最新开奖
                        var newLottery = await lottery10Operation.GetModelAsync(t => t.IssueNum == result.NextIssueNum && t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID));
                        flag = newLottery == null;
                    }
                    else
                    {
                        var lottery5Operation = new Lottery5Operation();
                        //最新开奖
                        var newLottery = await lottery5Operation.GetModelAsync(t => t.IssueNum == result.NextIssueNum && t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID));
                        flag = newLottery == null;
                    }
                    //开奖时间
                    var lotteryTime = 0;
                    if (gameType == GameOfType.极速赛车)
                    {
                        lotteryTime = 10 * 3;
                    }
                    else
                    {
                        lotteryTime = 300;
                    }
                    if (flag && result.StartTime.AddSeconds(lotteryTime) >= DateTime.Now)
                    {
                        result.Status = GameStatusEnum.开奖中;
                        result.Surplus = 0;
                        result.SealingTime = 0;
                    }
                    else return await GameDiscrimination.EachpartAsync(gameType, merchantID);
                }
            }
            return result;
        }
        #region SignalR group
        /// <summary>
        /// 添加房间列表SignalR用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="merchantID"></param>
        /// <param name="connectionId"></param>
        public static async Task AddRoomListItem(string userID, string merchantID, string connectionId)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "RoomList") & bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID);
                await bsonOperation.Collection.DeleteManyAsync(filter);
                BsonDocument bsons = new BsonDocument
                {
                    { "Type", "RoomList" },
                    { "MerchantID", merchantID },
                    { "UserID", userID },
                    { "ConnectionId", connectionId }
                };
                await bsonOperation.Collection.InsertOneAsync(bsons);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 获取用户连接id
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static string GetConnID(string userID, string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID);
                var bson = bsonOperation.Collection.Find(filter).FirstOrDefault();
                return bson?["ConnectionId"].ToString();
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 移除房间列表SignalR用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="merchantID"></param>
        public static async Task RemoveRoomListItem(string userID, string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "RoomList") & bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID);
                await bsonOperation.Collection.DeleteManyAsync(filter);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 获取用户连接id
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static string GetRoomListConnID(string userID, string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "RoomList")
                    & bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID);
                var bson = bsonOperation.Collection.Find(filter).FirstOrDefault();
                return bson?["ConnectionId"].ToString();
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 获取房间列表所有用户连接id
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetRoomListConnIDs(string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "RoomList")
                    & bsonOperation.Builder.Eq("MerchantID", merchantID);
                var task = await bsonOperation.Collection.FindAsync(filter);
                var bsons = task.ToList();
                if (!bsons.IsNull())
                {
                    var result = new List<string>();
                    foreach (var bson in bsons)
                    {
                        result.Add(bson["ConnectionId"].ToString());
                    }
                    return result;
                }
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }
        #endregion

        #region SignalR game
        /// <summary>
        /// 添加房间列表SignalR用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="merchantID"></param>
        /// <param name="connectionId"></param>
        public static async Task AddRoomGameItem(string userID, string merchantID, GameOfType gameType, string connectionId)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "RoomGame") & bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID);
                await bsonOperation.Collection.DeleteManyAsync(filter);
                BsonDocument bsons = new BsonDocument
                {
                    { "Type", "RoomGame" },
                    { "MerchantID", merchantID },
                    { "UserID", userID },
                    { "GameType", (int)gameType },
                    { "ConnectionId", connectionId }
                };
                await bsonOperation.Collection.InsertOneAsync(bsons);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 移除房间列表SignalR用户信息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="merchantID"></param>
        public static async Task RemoveRoomGameItem(string userID, string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "RoomGame") & bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID);
                await bsonOperation.Collection.DeleteManyAsync(filter);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 获取用户连接id
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="gameType"></param>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static string GetRoomGameConnID(string userID, GameOfType? gameType, string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "RoomGame")
                    & bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID);
                if (gameType != null)
                    filter &= bsonOperation.Builder.Eq("GameType", (int)gameType.Value);
                var bson = bsonOperation.Collection.Find(filter).FirstOrDefault();
                return bson?["ConnectionId"].ToString();
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 获取房间列表所有用户连接id
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetRoomGameConnIDs(string merchantID, GameOfType gameType, string userID = null)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "RoomGame")
                    & bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("GameType", (int)gameType);
                if (!string.IsNullOrEmpty(userID))
                    filter &= bsonOperation.Builder.Ne("UserID", userID);
                var task = await bsonOperation.Collection.FindAsync(filter);
                var bsons = task.ToList();
                if (!bsons.IsNull())
                {
                    var result = new List<string>();
                    foreach (var bson in bsons)
                    {
                        result.Add(bson["ConnectionId"].ToString());
                    }
                    return result;
                }
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 获取房间列表所有用户连接信息
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task<List<GameRoomOfUsers>> GetRoomGameConnInfos(string merchantID, GameOfType gameType)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "RoomGame")
                    & bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("GameType", (int)gameType);
                var task = await bsonOperation.Collection.FindAsync(filter);
                var bsons = task.ToList();
                var result = new List<GameRoomOfUsers>();
                if (!bsons.IsNull())
                {
                    foreach (var bson in bsons)
                    {
                        GameRoomOfUsers data = new GameRoomOfUsers()
                        {
                            MerchantID = bson["MerchantID"].ToString(),
                            UserID = bson["UserID"].ToString(),
                            ConnectionId = bson["ConnectionId"].ToString(),
                            GameType = GameBetsMessage.GetEnumByStatus<GameOfType>(bson["GameType"].ToInt32())
                        };
                        result.Add(data);
                    }
                }
                return result;
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        public static async Task<GameOfType?> GetRoomGameToUserGameType(string merchantID, string userID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "RoomGame")
                    & bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID);
                var task = await bsonOperation.Collection.FindAsync(filter);
                var bson = task.FirstOrDefault();
                if (bson != null && !bson.IsBsonNull)
                {
                    var item = bson["GameType"].AsInt32;
                    return GameBetsMessage.GetEnumByStatus<GameOfType>(item);
                }
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }
        #endregion

        #region SignalR 后台
        /// <summary>
        /// 添加后台长连接
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public static async Task AddBackstage(string merchantID, string connectionId)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "Backstage")
                    & bsonOperation.Builder.Eq("MerchantID", merchantID);
                var task = await bsonOperation.Collection.FindOneAndDeleteAsync(filter);
                BsonDocument bsons = new BsonDocument
                {
                    { "Type", "Backstage" },
                    { "MerchantID", merchantID },
                    { "ConnectionId", connectionId }
                };
                await bsonOperation.Collection.InsertOneAsync(bsons);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 获取应对conn
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task<string> GetBackstageConn(string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "Backstage")
                    & bsonOperation.Builder.Eq("MerchantID", merchantID);
                var bson = (await bsonOperation.Collection.FindAsync(filter)).FirstOrDefault();
                return bson?["ConnectionId"].ToString();
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 删除后台键
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task DeleteBackstage(string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "Backstage")
                    & bsonOperation.Builder.Eq("MerchantID", merchantID);
                await bsonOperation.Collection.FindOneAndDeleteAsync(filter);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }
        #endregion

        #region SignalR 飞单
        /// <summary>
        /// 添加飞单长连接
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="connectionId"></param>
        /// <param name="maturityTime"></param>
        /// <returns></returns>
        public static async Task AddFlySheet(string merchantID, string connectionId, DateTime maturityTime)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SheetFly");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID);
                await bsonOperation.Collection.FindOneAndDeleteAsync(filter);
                BsonDocument bsons = new BsonDocument
                {
                    { "MerchantID", merchantID },
                    { "ConnectionId", connectionId },
                    { "MaturityTime", maturityTime.ToString("yyyy-MM-dd HH:mm:ss") }
                };
                await bsonOperation.Collection.InsertOneAsync(bsons);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 获取商户对应connid
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static string GetFlySheet(string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SheetFly");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID);
                var bson = bsonOperation.Collection.Find(filter).FirstOrDefault();
                return bson?["ConnectionId"].ToString();
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 判断商户到期
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static bool FlySheetJudgementExpires(string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SheetFly");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID);
                var bson = bsonOperation.Collection.Find(filter).FirstOrDefault();
                if (bson == null) return false;
                if (Convert.ToDateTime(bson["MaturityTime"].ToString()) < DateTime.Now)
                    return false;
                return true;
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return false;
        }

        /// <summary>
        /// 删除后台键
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task DeleteFlySheet(string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SheetFly");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID);
                await bsonOperation.Collection.FindOneAndDeleteAsync(filter);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 删除对应conn
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static async Task DeleteFlySheetConn(string conn)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("SheetFly");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("ConnectionId", conn);
                await bsonOperation.Collection.FindOneAndDeleteAsync(filter);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 添加商户飞单长连接地址
        /// </summary>
        /// <param name="merchantID">对应商户id</param>
        /// <param name="targetID">目标商户id</param>
        /// <param name="targetID">目标用户id</param>
        /// <param name="connectionId"></param>
        /// <returns></returns>
        public static async Task AddMerchantFlySheet(string merchantID, string targetID, string userID, string connectionId)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("MerchantSheetFly");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID);
                await bsonOperation.Collection.FindOneAndDeleteAsync(filter);
                BsonDocument bsons = new BsonDocument
                {
                    { "MerchantID", merchantID },
                    { "TargetID", targetID},
                    { "UserID", userID},
                    { "ConnectionId", connectionId }
                };
                await bsonOperation.Collection.InsertOneAsync(bsons);
            }
            catch (MongoConnectionException e)
            {
                Logger.Error(e);
            }
        }

        /// <summary>
        /// 删除对应conn
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        public static async Task DeleteMerchantFlySheetConn(string conn)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("MerchantSheetFly");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("ConnectionId", conn);
                await bsonOperation.Collection.FindOneAndDeleteAsync(filter);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 获取对应商户地址
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="targetID">目标商户id</param>
        /// <param name="userID">目标用户id</param>
        /// <returns></returns>
        public static async Task<string> GetMerchantFlySheetConn(string merchantID, string targetID, string userID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("MerchantSheetFly");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("TargetID", targetID) &
                    bsonOperation.Builder.Eq("UserID", userID) & bsonOperation.Builder.Eq("MerchantID", merchantID);
                var bson = (await bsonOperation.Collection.FindAsync(filter)).FirstOrDefault();
                return bson?["ConnectionId"].ToString();
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
                return null;
            }
        }

        /// <summary>
        /// 获取登录飞单账户所有的连接
        /// </summary>
        /// <param name="targetID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static async Task<List<string>> GetMerchantFlySheetConns(string targetID, string userID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("MerchantSheetFly");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("TargetID", targetID) &
                    bsonOperation.Builder.Eq("UserID", userID);
                var bsons = (await bsonOperation.Collection.FindAsync(filter)).ToList();
                var result = new List<string>();
                foreach (var bson in bsons)
                {
                    result.Add(bson["ConnectionId"].ToString());
                }
                return result;
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
                return null;
            }
        }

        /// <summary>
        /// 获取当前商户登录信息
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task<BsonDocument> GetMerchantFlySheetInfo(string merchantID)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("MerchantSheetFly");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID);
                var bson = (await bsonOperation.Collection.FindAsync(filter)).FirstOrDefault();
                return bson;
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
                return null;
            }
        }
        #endregion

        #region SignalR 百家乐
        /// <summary>
        /// 添加百家乐房间数据
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="userID">用户id</param>
        /// <param name="znum">房间号</param>
        /// <param name="connectionId">地址</param>
        /// <param name="type">加入类型</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static async Task AddBaccarat(string merchantID, string userID, int? znum, string connectionId, string type = "Room", BaccaratGameType? gameType = null)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("Baccarat");
                //删除
                await bsonOperation.Collection.DeleteManyAsync(bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID)
                    & bsonOperation.Builder.Eq("Type", type));
                BsonDocument bsons = new BsonDocument
                {
                    { "MerchantID", merchantID },
                    { "UserID", userID },
                    { "ZNum", znum },
                    { "ConnectionId", connectionId },
                    { "Type", type },
                    { "GameType", gameType}
                };
                await bsonOperation.Collection.InsertOneAsync(bsons);
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 删除百家乐房间信息
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="userID">用户id</param>
        /// <param name="znum">桌号</param>
        /// <returns></returns>
        public static async Task DeleteBaccarat(string merchantID, string userID, int znum)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("Baccarat");
                //删除
                await bsonOperation.Collection.DeleteManyAsync(bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID)
                    & bsonOperation.Builder.Eq("ZNum", znum));
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 获取对应connid
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="userID">用户id</param>
        /// <param name="znum">房间号</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static async Task<string> GetBaccarat(string merchantID, string userID, string znum, BaccaratGameType gameType)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("Baccarat");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID) & bsonOperation.Builder.Eq("ZNum", znum)
                    & bsonOperation.Builder.Eq("GameType", gameType);
                var bson = (await bsonOperation.Collection.FindAsync(filter)).FirstOrDefault();
                return bson?["ConnectionId"].ToString();
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 获取房间列表所有用户连接id
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="znum">房间号</param>
        /// <param name="userID">除开的用户id</param>
        /// <param name="type">加入类型</param>
        /// <returns></returns>
        public static async Task<List<string>> GetBaccaratConnIDs(string merchantID, int znum, string userID = null, string type = "Room")
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("Baccarat");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("ZNum", znum) & bsonOperation.Builder.Eq("Type", type);
                if (!string.IsNullOrEmpty(userID))
                    filter &= bsonOperation.Builder.Ne("UserID", userID);
                var bsons = (await bsonOperation.Collection.FindAsync(filter)).ToList();
                if (!bsons.IsNull())
                {
                    var result = new List<string>();
                    foreach (var bson in bsons)
                    {
                        result.Add(bson["ConnectionId"].ToString());
                    }
                    return result;
                }
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 获取用户在百家乐所在房间id
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static async Task<Tuple<string, int>> GetBaccaratUserConn(string merchantID, string userID, BaccaratGameType gameType)
        {
            try
            {
                Tuple<string, int> tuple;
                BsonOperation bsonOperation = new BsonOperation("Baccarat");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID) & bsonOperation.Builder.Eq("Type", "Room")
                    & bsonOperation.Builder.Eq("GameType", gameType);
                var bson = (await bsonOperation.Collection.FindAsync(filter)).FirstOrDefault();
                if (bson == null) return null;
                tuple = Tuple.Create(bson["ConnectionId"].ToString(), Convert.ToInt32(bson["ZNum"].ToString()));
                return tuple;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 获取对应用户conn
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static async Task<string> GetBaccaratConnIDByUserID(string merchantID, string userID, string type = "Room")
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("Baccarat");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("UserID", userID);
                if (!string.IsNullOrEmpty(type))
                    filter &= bsonOperation.Builder.Eq("Type", type);
                var bson = (await bsonOperation.Collection.FindAsync(filter)).FirstOrDefault();
                return bson?["ConnectionId"].ToString();
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 获取游戏房间中的所有人  不区分商户   不区分是否在房间中
        /// </summary>
        /// <returns></returns>
        public static async Task<List<string>> GetBaccaratConnIDsByTid()
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("Baccarat");
                var bsons = (await bsonOperation.Collection.FindAsync(t => true)).ToList();
                if (!bsons.IsNull())
                {
                    var result = new List<string>();
                    foreach (var bson in bsons)
                    {
                        result.Add(bson["ConnectionId"].ToString());
                    }
                    return result;
                }
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 获取对应房间在线人数
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="znum"></param>
        /// <returns></returns>
        public static async Task<List<GameRoomOfUsers>> GetBaccaratGameConnInfos(string merchantID, int znum)
        {
            try
            {
                BsonOperation bsonOperation = new BsonOperation("Baccarat");
                FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("Type", "Room")
                    & bsonOperation.Builder.Eq("MerchantID", merchantID)
                    & bsonOperation.Builder.Eq("ZNum", znum);
                var task = await bsonOperation.Collection.FindAsync(filter);
                var bsons = task.ToList();
                var result = new List<GameRoomOfUsers>();
                if (!bsons.IsNull())
                {
                    foreach (var bson in bsons)
                    {
                        GameRoomOfUsers data = new GameRoomOfUsers()
                        {
                            MerchantID = bson["MerchantID"].ToString(),
                            UserID = bson["UserID"].ToString(),
                            ConnectionId = bson["ConnectionId"].ToString()
                        };
                        result.Add(data);
                    }
                }
                return result;
            }
            catch (MongoConnectionException e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }
        #endregion

        #region MD5加密
        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string MD5(string pwd)
        {
            MD5CryptoServiceProvider md5Hasher = new MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.UTF8.GetBytes(pwd));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            md5Hasher.Dispose();
            return sBuilder.ToString();
        }
        #endregion

        #region 数据转换
        /// <summary>
        /// 赔率表与对接web表数据转换
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="result"></param>
        /// <param name="target"></param>
        public static void GetOddsData<T1, T2>(ref T1 result, T2 target) where T1 : BaseModel where T2 : WebOdds
        {
            var targetType = target.GetType();
            var resultType = result.GetType();
            var targetProperties = targetType.GetProperties();
            foreach (var propertie in targetProperties)
            {
                if (propertie.Name == "ID") continue;
                resultType.GetProperty(propertie.Name).SetValue(result, propertie.GetValue(target));
            }
        }

        /// <summary>
        /// 赔率表与对接web表数据转换
        /// </summary>
        /// <typeparam name="T1"></typeparam>
        /// <typeparam name="T2"></typeparam>
        /// <param name="target"></param>
        /// <param name="result"></param>
        public static T2 GetWebOddsData<T1, T2>(T1 target) where T1 : BaseModel where T2 : WebOdds, new()
        {
            var result = new T2();
            var targetType = target.GetType();
            var resultType = result.GetType();
            var targetProperties = targetType.GetProperties();
            var tasks = new List<Task>();
            foreach (var propertie in targetProperties)
            {
                var task = Task.Run(() =>
                {
                    if (propertie.Name == "_id")
                        resultType.GetProperty("ID").SetValue(result, propertie.GetValue(target));
                    else
                    {
                        if (resultType.GetProperty(propertie.Name) == null) return;
                        resultType.GetProperty(propertie.Name).SetValue(result, propertie.GetValue(target));
                    }
                });
                tasks.Add(task);
            }
            Task.WaitAll(tasks.ToArray());
            return result;
        }
        #endregion

        #region 获取实体内容
        public static string GetEntityContent<T>(T model) where T : Odds
        {
            var type = model.GetType();
            var gameType = model.GameType;
            StringBuilder sb = new StringBuilder();
            //5球
            if (!Utils.GameTypeItemize(gameType))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>
                {
                    { "Da", "大" },
                    { "Xiao", "小" },
                    { "Dan", "单" },
                    { "Shuang", "双" },
                    { "Long", "龙" },
                    { "Hu", "虎" },
                    { "He", "和" },
                    { "CDa", "和大" },
                    { "CXiao", "和小" },
                    { "CDan", "和单" },
                    { "CShuang", "和双" },
                    { "Baozi", "豹子" },
                    { "Shunzi", "顺子" },
                    { "Banshun", "半顺" },
                    { "Duizi", "对子" },
                    { "Zaliu", "杂六" }
                };
                foreach (var propertie in type.GetProperties())
                {
                    if (propertie.Name == "_id" || propertie.Name == "CreatedTime" || propertie.Name == "LastUpdateTime" || propertie.Name == "SeurityNo"
                        || propertie.Name == "GameType" || propertie.Name == "MerchantID") continue;
                    //数字
                    if (propertie.Name.StartsWith("Num"))
                    {
                        sb.AppendFormat("球{0}:{1}\r\n", propertie.Name.Replace("Num", ""), propertie.GetValue(model).ToString());
                        continue;
                    }
                    sb.AppendFormat("{0}:{1}\r\n", dic[propertie.Name], propertie.GetValue(model).ToString());
                }
            }
            else
            {
                Dictionary<string, string> dic = new Dictionary<string, string>
                {
                    { "Da", "大" },
                    { "Xiao", "小" },
                    { "Dan", "单" },
                    { "Shuang", "双" },
                    { "Long", "龙" },
                    { "Hu", "虎" },
                    { "SDa", "冠亚和大" },
                    { "SXiao", "冠亚和小" },
                    { "SDan", "冠亚和单" },
                    { "SShuang", "冠亚和双" }
                };
                foreach (var propertie in type.GetProperties())
                {
                    if (propertie.Name == "_id" || propertie.Name == "CreatedTime" || propertie.Name == "LastUpdateTime" || propertie.Name == "SeurityNo"
                        || propertie.Name == "GameType" || propertie.Name == "MerchantID") continue;
                    //数字
                    if (propertie.Name.StartsWith("Num"))
                    {
                        sb.AppendFormat("球{0}:{1}\r\n", propertie.Name.Replace("Num", ""), propertie.GetValue(model).ToString());
                        continue;
                    }
                    //和值
                    if (propertie.Name.StartsWith("SNum"))
                    {
                        sb.AppendFormat("冠亚和值{0}:{1}\r\n", propertie.Name.Replace("SNum", ""), propertie.GetValue(model).ToString());
                        continue;
                    }

                    sb.AppendFormat("{0}:{1}\r\n", dic[propertie.Name], propertie.GetValue(model).ToString());
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 特殊设置数据转换
        /// </summary>
        /// <param name="reply"></param>
        /// <returns></returns>
        public static WebReplySetUp ReplyDataToWeb(ReplySetUp reply)
        {
            var result = new WebReplySetUp();
            var resultType = result.GetType();
            var replyType = reply.GetType();
            foreach (var propertie in replyType.GetProperties())
            {
                if (propertie.Name == "_id" || propertie.Name == "CreatedTime" || propertie.Name == "LastUpdateTime"
                    || propertie.Name == "MerchantID") continue;
                resultType.GetProperty(propertie.Name).SetValue(result, propertie.GetValue(reply));
            }
            return result;
        }

        public static void WebReplyToData(WebReplySetUp webReply, ref ReplySetUp result)
        {
            var resultType = result.GetType();
            var webType = webReply.GetType();
            foreach (var propertie in webType.GetProperties())
            {
                resultType.GetProperty(propertie.Name).SetValue(result, propertie.GetValue(webReply));
            }
        }
        #endregion

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var invokedExpression = Expression.Invoke(expression2, expression1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.Or(expression1.Body, invokedExpression), expression1.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> expression1, Expression<Func<T, bool>> expression2)
        {
            var invokedExpression = Expression.Invoke(expression2, expression1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.And(expression1.Body, invokedExpression), expression1.Parameters);
        }

        /// <summary>
        /// get获取数据
        /// </summary>
        /// <param name="url">地址</param>
        /// <returns></returns>
        public static async Task<string> HttpGetAsync(string url)
        {
            try
            {
                HttpClientHandler handler = new HttpClientHandler()
                {
                    AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
                };
                using var client = new HttpClient(handler)
                {
                    Timeout = TimeSpan.FromSeconds(60)
                };
                CancellationToken token = new CancellationToken();
                var responseString = await client.GetAsync(url, token);
                if (responseString.IsSuccessStatusCode)
                {
                    byte[] data = await responseString.Content.ReadAsByteArrayAsync();
                    return Encoding.UTF8.GetString(data);
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> GetAsync(string url)
        {
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; }
                };
                HttpClient client = new HttpClient(clientHandler)
                {
                    Timeout = TimeSpan.FromSeconds(10)
                };
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    using var sr = new StreamReader(response.Content.ReadAsStreamAsync().Result, Encoding.UTF8);
                    client.Dispose();
                    return sr.ReadToEnd();
                }
                return null;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public static async Task<string> OtherHttpGetAsync(string url)
        {
            try
            {
                WebRequest request = WebRequest.Create(url);
                request.Method = "GET";
                request.ContentType = "text/json;charset=UTF8";
                request.Timeout = 60000;
                WebResponse webresponse = request.GetResponse();
                Stream stream = webresponse.GetResponseStream();
                StreamReader reader = new StreamReader(stream);
                string content = await reader.ReadToEndAsync();
                return content;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 实体转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<WebGameHistory> GetGameHistories10<T>(List<T> list) where T : Lottery10
        {
            var result = new List<WebGameHistory>();
            foreach (var data in list)
            {
                WebGameHistory history = new WebGameHistory();
                var type = data.GetType();
                history.IssueNum = type.GetProperty("IssueNum").GetValue(data).ToString();
                history.Number = GameDiscrimination.SetupSeparation10(data);
                var msgList = new List<string>
                {
                    data.Count.ToString().PadLeft(2, '0'),
                    Enum.GetName(typeof(SindouEnum), (int)data.Sindou),
                    Enum.GetName(typeof(SizeEnum), (int)data.CountSize),
                    Enum.GetName(typeof(DraTig), (int)data.DraTig1),
                    Enum.GetName(typeof(DraTig), (int)data.DraTig2),
                    Enum.GetName(typeof(DraTig), (int)data.DraTig3),
                    Enum.GetName(typeof(DraTig), (int)data.DraTig4),
                    Enum.GetName(typeof(DraTig), (int)data.DraTig5)
                };
                history.Message = string.Join("|", msgList);
                result.Add(history);
            }
            return result;
        }

        /// <summary>
        /// 实体转换
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<WebGameHistory> GetGameHistories5<T>(List<T> list) where T : Lottery5
        {
            var result = new List<WebGameHistory>();
            foreach (var data in list)
            {
                WebGameHistory history = new WebGameHistory();
                var type = data.GetType();
                history.IssueNum = type.GetProperty("IssueNum").GetValue(data).ToString();
                history.Number = GameDiscrimination.SetupSeparation5(data);
                var msgList = new List<string>
                {
                    data.Count.ToString().PadLeft(2, '0'),
                    Enum.GetName(typeof(SizeEnum), (int)data.CountSize),
                    Enum.GetName(typeof(SindouEnum), (int)data.CountSinDou),
                    Enum.GetName(typeof(DraTig5), (int)data.DraTig),
                    Enum.GetName(typeof(RuleEnum), (int)data.Front3),
                    Enum.GetName(typeof(RuleEnum), (int)data.Middle3),
                    Enum.GetName(typeof(RuleEnum), (int)data.Back3)
                };
                history.Message = string.Join("|", msgList);
                result.Add(history);
            }
            return result;
        }

        /// <summary>
        /// 获取对应房间信息
        /// </summary>
        /// <param name="merchantID">商户号</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static async Task<RoomGameDetailed> GetRoomInfosAsync(string merchantID, GameOfType gameType)
        {
            RoomGameDetailedOperation roomGameDetailedOperation = new RoomGameDetailedOperation();
            var roomInfo = await roomGameDetailedOperation.GetModelAsync(t => t.MerchantID == merchantID && t.GameType == gameType);
            if (roomInfo == null)
            {
                roomInfo = new RoomGameDetailed()
                { 
                    MerchantID = merchantID,
                    GameType = gameType
                };
                if (!GameTypeItemize(gameType)) roomInfo.KaiEquality = KaiHeEnum.返还本金;
                await roomGameDetailedOperation.InsertModelAsync(roomInfo);
            }
            return roomInfo;
        }

        /// <summary>
        /// 获取对应视讯房间信息
        /// </summary>
        /// <param name="merchantID">商户号</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static async Task<VideoRoom> GetVideoRoomInfosAsync(string merchantID, BaccaratGameType gameType)
        {
            VideoRoomOperation videoRoomOperation = new VideoRoomOperation();
            var roomInfo = await videoRoomOperation.GetModelAsync(t => t.MerchantID == merchantID && t.GameType == gameType);
            if (roomInfo == null)
            {
                roomInfo = new VideoRoom()
                {
                    MerchantID = merchantID,
                    GameType = gameType
                };
                await videoRoomOperation.InsertModelAsync(roomInfo);
            }
            return roomInfo;
        }

        public static bool IsNull<T>(this IEnumerable<T> source) where T : class
        {
            return source == null || source.Count() == 0;
        }

        public static bool Contains(this String str, List<string> strChar)
        {
            foreach (var sc in strChar)
            {
                if (str.Contains(sc))
                    return true;
            }
            return false;
        }

        public static bool Contains(this String str, string[] strChar)
        {
            foreach (var sc in strChar)
            {
                if (str.Contains(sc))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 生成随机码
        /// </summary>
        /// <param name="length">位数</param>
        /// <returns></returns>
        public static string GetRandomNum(int length)
        {
            char[] verification = new char[length];
            char[] dictionary = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z',
                'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z'
            };
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                verification[i] = dictionary[random.Next(dictionary.Length - 1)];
            }
            return new string(verification);
        }

        /// <summary>
        /// 获取不重复安全码
        /// </summary>
        /// <returns></returns>
        public static async Task<string> GetMerchantSeurityNo()
        {
            MerchantOperation merchantOperation = new MerchantOperation();
            var seurityNo = GetRandomNumber(6);
            var exic = await merchantOperation.GetModelAsync(t => t.SeurityNo == seurityNo);
            if (exic == null) return seurityNo;
            return await GetMerchantSeurityNo();
        }

        public static string GetRandomNumber(int length)
        {
            char[] verification = new char[length];
            char[] dictionary = { '1', '2', '3', '4', '5', '6', '7', '8', '9', '0' };
            Random random = new Random();
            for (int i = 0; i < length; i++)
            {
                verification[i] = dictionary[random.Next(dictionary.Length - 1)];
            }
            return new string(verification);
        }

        /// <summary>
        /// 游戏指令转换
        /// </summary>
        /// <param name="message">原本消息</param>
        /// <param name="userID">用户id</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="nper">期号</param>
        /// <param name="original">消息</param>
        /// <returns></returns>
        public static async Task<string> InstructionConversion(string message, string userID, string merchantID, GameOfType? gameType, string nper, string original = null, BaccaratGameType? vgameType = null)
        {
            UserOperation userOperation = new UserOperation();
            var address = await GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var user = await userOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == userID);
            BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
            var vcollection = baccaratBetOperation.GetCollection(merchantID);

            if (message.Contains("{昵称}"))
            {
                message = message.Replace("{昵称}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName);
            }

            if (message.Contains("{游戏}"))
            {
                if (gameType != null)
                    message = message.Replace("{游戏}", Enum.GetName(typeof(GameOfType), gameType));
                else
                    message = message.Replace("{游戏}", Enum.GetName(typeof(BaccaratGameType), vgameType));
            }

            if (message.Contains("{游戏名称}"))
            {
                if (gameType != null)
                    message = message.Replace("{游戏名称}", Enum.GetName(typeof(GameOfType), gameType));
                else
                    message = message.Replace("{游戏名称}", Enum.GetName(typeof(BaccaratGameType), vgameType));
            }

            if (message.Contains("{期号}"))
            {
                message = message.Replace("{期号}", nper);
            }

            if (message.Contains("{剩余}"))
            {
                message = message.Replace("{剩余}", user.UserMoney.ToString("#0.00"));
            }

            if (message.Contains("{当期玩法}"))
            {
                if (gameType != null)
                {
                    var userBet = await collection.FindOneAsync(t => t.MerchantID == merchantID && t.Nper == nper && t.GameType == gameType && t.BetStatus == BetStatus.未开奖 && t.UserID == userID);
                    if (userBet == null)
                        message = message.Replace("{当期玩法}", "");
                    //数据处理
                    message = message.Replace("{当期玩法}", string.Join("\r\n", userBet.BetRemarks.Select(t => t.Remark).ToList()));
                }
                else
                {
                    var userBet = await vcollection.FindOneAsync(t => t.MerchantID == merchantID && t.Nper == nper && t.GameType == vgameType && t.BetStatus == BetStatus.未开奖 && t.UserID == userID);
                    if (userBet == null)
                        message = message.Replace("{当期玩法}", "");
                    //数据处理
                    message = message.Replace("{当期玩法}", string.Join("\r\n", userBet.BetRemarks.Select(t => t.Remark).ToList()));
                }
            }

            if (message.Contains("当前玩法明细") || message.Contains("{当前使用分数}") || message.Contains("{当前玩法}") || message.Contains("{当前多行玩法}"))
            {
                if (gameType != null)
                {
                    var userBet = await collection.FindOneAsync(t => t.MerchantID == merchantID && t.Nper == nper && t.GameType == gameType && t.BetStatus == BetStatus.未开奖 && t.UserID == userID);
                    if (userBet == null)
                    {
                        message = message.Replace("{当前玩法明细}", "").Replace("{当前使用分数}", "0.00")
                                .Replace("{当前玩法}", "").Replace("{当前多行玩法}", "");
                    }
                    var userFirstBet = userBet.BetRemarks.Find(t => t.OddNum == original);
                    if (userFirstBet == null)
                    {
                        message = message.Replace("{当前玩法明细}", "").Replace("{当前使用分数}", "0.00")
                                .Replace("{当前玩法}", "").Replace("{当前多行玩法}", "");
                    }
                    else
                    {
                        var result = (from data in userFirstBet.OddBets
                                      select new
                                      {
                                          Info = string.Format("{0}{1}-{2}", Enum.GetName(typeof(BetTypeEnum), data.BetRule), data.BetNo, data.BetMoney)
                                      });

                        message = message.Replace("{当前玩法明细}", string.Join(",", result.Select(t => t.Info).ToList()))
                            .Replace("{当前使用分数}", userFirstBet.OddBets.Sum(t => t.BetMoney).ToString("#0.00"))
                            .Replace("{当前玩法}", userFirstBet == null ? "" : userFirstBet.Remark)
                            .Replace("{当前多行玩法}", userFirstBet == null ? "" : userFirstBet.Remark);
                    }
                }
                else
                {
                    var userBet = await vcollection.FindOneAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.Nper == nper && t.GameType == vgameType && t.BetStatus == BetStatus.未开奖);
                    if (userBet == null)
                    {
                        message = message.Replace("{当前玩法明细}", "").Replace("{当前使用分数}", "0.00")
                                .Replace("{当前玩法}", "").Replace("{当前多行玩法}", "");
                    }
                    var userFirstBet = userBet.BetRemarks.Find(t => t.OddNum == original);
                    if (userFirstBet == null)
                    {
                        message = message.Replace("{当前玩法明细}", "").Replace("{当前使用分数}", "0.00")
                                .Replace("{当前玩法}", "").Replace("{当前多行玩法}", "");
                    }
                    else
                    {
                        message = message.Replace("{当前玩法明细}", string.Join(",", string.Format("{0}-{1}", Enum.GetName(typeof(BaccaratBetType), userFirstBet.BetRule), userFirstBet.BetMoney)))
                            .Replace("{当前使用分数}", userFirstBet.BetMoney.ToString("#0.00"))
                            .Replace("{当前玩法}", userFirstBet == null ? "" : userFirstBet.Remark)
                            .Replace("{当前多行玩法}", userFirstBet == null ? "" : userFirstBet.Remark);
                    }
                }
            }

            if (message.Contains("{简写期号}"))
            {
                if (gameType != null)
                    message = message.Replace("{简写期号}", nper.Substring(nper.Length - 3));
                else
                    message = message.Replace("{简写期号}", nper);
            }

            if (message.Contains("{当期得分}"))
            {
                if (gameType != null)
                {
                    string preNper = GameHandle.GetGamePreNper(nper, gameType.Value);
                    var userBet = await collection.FindOneAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.Nper == preNper && t.GameType == gameType && t.BetStatus == BetStatus.已开奖);
                    message = message.Replace("{当期得分}", userBet.AllMediumBonus.ToString("#0.00"));
                }
                else
                    message = message.Replace("{当期得分}", "");
            }

            if (message.Contains("{当期盈亏}"))
            {
                if (gameType != null)
                {
                    string preNper = GameHandle.GetGamePreNper(nper, gameType.Value);
                    var userBet = await collection.FindOneAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.Nper == preNper && t.GameType == gameType && t.BetStatus == BetStatus.已开奖);
                    message = message.Replace("{当期盈亏}", (userBet.AllMediumBonus - userBet.AllUseMoney).ToString("#0.00"));
                }
                else
                    message = message.Replace("{当期盈亏}", "");
            }

            if (message.Contains("{当日流水}") || message.Contains("{当日得分}") || message.Contains("{当日盈亏}"))
            {
                DateTime startTime = new DateTime();
                DateTime endTime = new DateTime();
                if (DateTime.Now < DateTime.Today.AddHours(6))
                {
                    startTime = DateTime.Today.AddHours(6).AddDays(-1);
                    endTime = DateTime.Today.AddHours(6);
                }
                else
                {
                    startTime = DateTime.Today.AddHours(6);
                    endTime = DateTime.Today.AddHours(6).AddDays(1);
                }
                if (gameType != null)
                {
                    var userBetList = await collection.FindListAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.GameType == gameType && t.BetStatus == BetStatus.已开奖
                    && t.CreatedTime >= startTime && t.CreatedTime <= endTime);
                    message = message.Replace("{当日流水}", userBetList.Sum(t => t.AllUseMoney).ToString("#0.00"))
                        .Replace("{当日盈亏}", (userBetList.Sum(t => t.AllMediumBonus) - userBetList.Sum(t => t.AllUseMoney)).ToString("#0.00"))
                        .Replace("{当日得分}", userBetList.Sum(t => t.AllMediumBonus).ToString("#0.00"));
                }
                else
                {
                    var userBetList = await vcollection.FindListAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.GameType == vgameType && t.BetStatus == BetStatus.已开奖
                    && t.CreatedTime >= startTime && t.CreatedTime <= endTime);
                    message = message.Replace("{当日流水}", userBetList.Sum(t => t.AllUseMoney).ToString("#0.00"))
                        .Replace("{当日盈亏}", (userBetList.Sum(t => t.AllMediumBonus) - userBetList.Sum(t => t.AllUseMoney)).ToString("#0.00"))
                        .Replace("{当日得分}", userBetList.Sum(t => t.AllMediumBonus).ToString("#0.00"));
                }
            }

            if (message.Contains("{未开奖使用分数}") || message.Contains("{未开奖玩法}") || message.Contains("{未开奖玩法明细}") || message.Contains("当期玩法明细") || message.Contains("{当期使用分数}"))
            {
                if (gameType != null)
                {
                    var userBet = await collection.FindOneAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.Nper == nper && t.GameType == gameType && t.BetStatus == BetStatus.未开奖);
                    if (userBet == null)
                    {
                        message = message.Replace("{未开奖使用分数}", "0.00")
                            .Replace("{未开奖玩法}","")
                            .Replace("{未开奖玩法明细}", "")
                            .Replace("{当期玩法明细}}", "0")
                            .Replace("{当期使用分数}", "0.00");
                    }
                    else
                    {
                        message = message.Replace("{未开奖使用分数}", userBet.AllUseMoney.ToString("#0.00"));
                        message = message.Replace("{未开奖玩法}", string.Join("\r\n", userBet.BetRemarks.Select(t => t.Remark).ToList()));
                        var result = (from data in userBet.BetRemarks
                                      from dt in data.OddBets
                                      select new
                                      {
                                          Info = string.Format("{0}{1}-{2}", Enum.GetName(typeof(BetTypeEnum), dt.BetRule), dt.BetNo, dt.BetMoney)
                                      });
                        message = message.Replace("{未开奖玩法明细}", string.Join(",", result.Select(t => t.Info).ToList()))
                            .Replace("{当期玩法明细}", string.Format("{0}:{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, string.Join(",", result.Select(t => t.Info).ToList())))
                            .Replace("{当期使用分数}", userBet.AllUseMoney.ToString("#0.00"));
                    }
                }
                else
                {
                    var userBet = await vcollection.FindOneAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.Nper == nper && t.GameType == vgameType && t.BetStatus == BetStatus.未开奖);
                    if (userBet == null)
                    {
                        message = message.Replace("{未开奖使用分数}", "0.00")
                            .Replace("{未开奖玩法}", "")
                            .Replace("{未开奖玩法明细}", "")
                            .Replace("{当期玩法明细}}", "0")
                            .Replace("{当期使用分数}", "0.00");
                    }
                    else
                    {
                        message = message.Replace("{未开奖使用分数}", userBet.AllUseMoney.ToString("#0.00"));
                        message = message.Replace("{未开奖玩法}", string.Join("\r\n", userBet.BetRemarks.Select(t => t.Remark).ToList()));
                        var result = (from data in userBet.BetRemarks
                                      select new
                                      {
                                          Info = string.Format("{0}-{1}", Enum.GetName(typeof(BaccaratBetType), data.BetRule), data.BetMoney)
                                      });
                        message = message.Replace("{未开奖玩法明细}", string.Join(",", result.Select(t => t.Info).ToList()))
                            .Replace("{当期玩法明细}", string.Format("{0}:{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, string.Join(",", result.Select(t => t.Info).ToList())))
                            .Replace("{当期使用分数}", userBet.AllUseMoney.ToString("#0.00"));
                    }
                }
            }

            if (message.Contains("{未开奖期号}") || message.Contains("{未开奖简写期号}"))
            {
                if (gameType != null)
                {
                    message = message.Replace("{未开奖期号}", nper)
                        .Replace("{未开奖简写期号}", nper.Substring(nper.Length - 3));
                }
                else
                {
                    message = message.Replace("{未开奖期号}", nper)
                            .Replace("{未开奖简写期号}", nper);
                }
            }
            return message;
        }

        /// <summary>
        /// 期号和游戏类型转换
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="message">内容</param>
        /// <returns></returns>
        public static string SealedTransformation(GameOfType gameType, string message)
        {
            var nper = CancelAnnouncement.GetGameNper(gameType);
            //var nextNper = GameHandle.GetGameNper(nper, gameType);
            message = message.Replace("{期号}", nper)
                .Replace("{游戏}", Enum.GetName(typeof(GameOfType), (int)gameType));
            return message;
        }


        /// <summary>
        /// 获取在线人数
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public static async Task<int> GetOnlineCount(string merchantID, GameOfType gameType)
        {
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            UserOperation userOperation = new UserOperation();
            var robotList = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.假人);
            var count = 0;
            foreach (var robot in robotList)
            {
                //var sham = await shamUserOperation.GetModelAsync(t => t.MerchantID == merchantID && t.UserID == robot._id);
                var sham = await shamRobotOperation.GetModelAsync(t => t.MerchantID == merchantID && t.UserID == robot._id);
                if (sham == null) continue;
                var item = sham.GameCheckInfo.Find(t => t.GameType == gameType);
                if (item == null) continue;
                if (item.Check) ++count;
            }
            var connIDs = await GetRoomGameConnIDs(merchantID, gameType);
            if (connIDs.IsNull()) return 0;
            count += connIDs.Count;
            return count;
        }

        /// <summary>
        /// 获取连接地址id
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task<string> GetAddress(string merchantID)
        {
            var address = RedisOperation.GetValue("Address", merchantID);
            if (string.IsNullOrEmpty(address))
            {
                //查询商户
                MerchantOperation merchantOperation = new MerchantOperation();
                var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
                if (merchant == null) return null;
                //查询连接地址
                if (string.IsNullOrEmpty(merchant.AddressID))
                    return address;
                else
                {
                    DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
                    var model = await databaseAddressOperation.GetModelAsync(t => t._id == merchant.AddressID);
                    address = model._id;
                }
                RedisOperation.SetHash("Address", merchantID, address);
            }
            return address;
        }

        #region mongodb
        public static T FindOne<T>(this IMongoCollection<T> collection, FilterDefinition<T> filter) where T : BaseModel
        {
            return collection.Find(filter).FirstOrDefault();
        }

        public static async Task<T> FindOneAsync<T>(this IMongoCollection<T> collection, FilterDefinition<T> filter) where T : BaseModel
        {
            var result = await collection.FindAsync(filter);
            return result.ToList().FirstOrDefault();
        }

        public static T FindOne<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter) where T : BaseModel
        {
            return collection.Find(filter).FirstOrDefault();
        }

        public static async Task<T> FindOneAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter) where T : BaseModel
        {
            var result = await collection.FindAsync(filter);
            return result.ToList().FirstOrDefault();
        }

        public static async Task UpdateOneAsync<T>(this IMongoCollection<T> collection, T model) where T : BaseModel
        {
            FilterDefinitionBuilder<T> Builder = Builders<T>.Filter;
            var type = model.GetType();
            var id = type.GetProperty("_id").GetValue(model);
            FilterDefinition<T> filter = Builder.Eq("_id", id);
            UpdateDefinition<T> update = Builders<T>.Update
            .Set("LastUpdateTime", DateTime.Now);
            foreach (var item in type.GetProperties())
            {
                if (item.Name == "_id" || item.Name == "CreatedTime") continue;
                var value = item.GetValue(model);
                update = update.Set(item.Name, value);
            }
            update = update.Set("LastUpdateTime", DateTime.Now);
            var result = await collection.UpdateOneAsync(filter, update);
        }

        public static List<T> FindList<T>(this IMongoCollection<T> collection, FilterDefinition<T> filter) where T : BaseModel
        {
            return collection.Find(filter).ToList();
        }

        public static List<T> FindList<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter) where T : BaseModel
        {
            return collection.Find(filter).ToList();
        }

        public static async Task<List<T>> FindListAsync<T>(this IMongoCollection<T> collection, FilterDefinition<T> filter) where T : class
        {
            var result = await collection.FindAsync(filter);
            return result.ToList();
        }

        public static async Task<List<T>> FindListAsync<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter) where T : class
        {
            var result = await collection.FindAsync(filter);
            return result.ToList();
        }

        public static List<T> FindList<T>(this IMongoCollection<T> collection, FilterDefinition<T> filter, Expression<Func<T, object>> sort, bool desc) where T : BaseModel
        {
            if (desc)
                return collection.Find(filter).SortBy(t => t.CreatedTime).ToList();
            else
                return collection.Find(filter).SortByDescending(sort).ToList();
        }

        public static List<T> FindList<T>(this IMongoCollection<T> collection, Expression<Func<T, bool>> filter, Expression<Func<T, object>> sort, bool desc) where T : BaseModel
        {
            if (desc)
                return collection.Find(filter).SortBy(t => t.CreatedTime).ToList();
            else
                return collection.Find(filter).SortByDescending(sort).ToList();
        }
        #endregion

        /// <summary>
        /// 获取游戏下期开奖时间  相隔期号   下期期号
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public static async Task<GameNextLottery> GetGameNextLottery(GameOfType gameType)
        {
            var now = DateTime.Now;
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var platformSetUp = await platformSetUpOperation.GetModelAsync(t => t._id != "");
            if (platformSetUp == null) return null;
            var setup = platformSetUp.GameBasicsSetups.Find(t => t.GameType == gameType);
            if (setup == null) return null;
            var result = new GameNextLottery
            {
                DayNum = setup.DayNum,
                Interval = setup.Interval
            };
            if (setup.DayNum == 0)
                return result;
            if (setup.StartTime > now)
            {
                result.StartTime = setup.StartTime;
                result.NextNper = setup.FirstNper;
                result.ExpirationDate = setup.StartTime.AddSeconds(dicGameOverdue[gameType]);
                return result;
            }
            int secondDiff = 0;
            int actualDiff = 0;
            switch (gameType)
            {
                #region 北京赛车
                case GameOfType.北京赛车:
                    var dayDiff = (now - setup.StartTime).Days;
                    var nperDiff = dayDiff * setup.DayNum;
                    result.DayNum = setup.DayNum;
                    setup.StartTime = setup.StartTime.AddDays(dayDiff);
                    var startSpan = new TimeSpan(9, 33, setup.StartTime.Second);
                    var endSpan = new TimeSpan(23, 53, setup.StartTime.Second);

                    //在当天停售之前
                    var pk10StopTime = setup.StartTime.Date.Add(endSpan);
                    if (now > pk10StopTime)
                    {
                        var df = (int)(pk10StopTime - setup.StartTime).TotalSeconds;
                        //相差期数
                        nperDiff += df / setup.Interval;
                        if (df % setup.Interval > 0) nperDiff++;
                        //是否在停售区间
                        if (now >= now.Date.Add(endSpan)
                            || now <= now.Date.Add(startSpan))
                        {
                            nperDiff++;
                            result.StartTime = now.Date.Add(startSpan);
                            result.NextNper = (Convert.ToInt64(setup.FirstNper) + nperDiff).ToString();
                        }
                        else
                        {
                            nperDiff++;
                            var gameStartTime = now.Date.Add(startSpan);
                            var newDiff = (int)(now - gameStartTime).TotalSeconds;
                            var reality = newDiff / setup.Interval;
                            if (newDiff % setup.Interval > 0) reality++;
                            result.StartTime = gameStartTime.AddSeconds(reality * setup.Interval);
                            result.NextNper = (Convert.ToInt64(setup.FirstNper) + nperDiff + reality).ToString();
                        }
                    }
                    else
                    {
                        var newDiff = (int)(now - setup.StartTime).TotalSeconds;
                        var reality = newDiff / setup.Interval;
                        if (newDiff % setup.Interval > 0) reality++;
                        result.StartTime = setup.StartTime.AddSeconds(reality * setup.Interval);
                        result.NextNper = (Convert.ToInt64(setup.FirstNper) + nperDiff + reality).ToString();
                    }
                    break;
                #endregion
                #region 幸运飞艇
                case GameOfType.幸运飞艇:
                case GameOfType.幸运飞艇168:
                    dayDiff = (now - setup.StartTime).Days;
                    setup.StartTime = setup.StartTime.AddDays(dayDiff);
                    nperDiff = setup.DayNum * dayDiff;
                    var xyftNper = (new DateTime(Convert.ToInt32(setup.FirstNper.Substring(0, 4)),
                        Convert.ToInt32(setup.FirstNper.Substring(4, 2)), Convert.ToInt32(setup.FirstNper.Substring(6, 2)))).AddDays(dayDiff).ToString("yyyyMMdd") + setup.FirstNper.Substring(setup.FirstNper.Length - 3);
                    setup.FirstNper = xyftNper;
                    //在停售区间
                    if (now >= now.Date.AddHours(4).AddMinutes(4).AddSeconds(setup.StartTime.Second)
                        && now <= now.Date.AddHours(13).AddMinutes(4).AddSeconds(setup.StartTime.Second))
                    {
                        result.StartTime = now.Date.AddHours(13).AddMinutes(9).AddSeconds(setup.StartTime.Second);
                        var xyftSecondDiff = (int)(now.Date.AddHours(4).AddMinutes(4).AddSeconds(setup.StartTime.Second) - setup.StartTime).TotalSeconds;
                        var xyftNperDiff = xyftSecondDiff / setup.Interval;
                        if (xyftSecondDiff % setup.Interval > 0) xyftNperDiff++;
                        result.NextNper = string.Format("{0}{1:d3}", now.ToString("yyyyMMdd"), 1);
                    }
                    //在停售区间之前
                    else if (now < now.Date.AddHours(4).AddMinutes(4).AddSeconds(setup.StartTime.Second))
                    {
                        var lastNper = Convert.ToInt32(setup.FirstNper.Substring(setup.FirstNper.Length - 3));
                        var stopDiff = setup.DayNum - lastNper;
                        var xyftSecondDiff = (int)(now - setup.StartTime).TotalSeconds;
                        var xyftNperDiff = xyftSecondDiff / setup.Interval;
                        if (xyftSecondDiff % setup.Interval > 0) xyftNperDiff++;
                        if (xyftNperDiff > stopDiff)
                        {
                            nperDiff += stopDiff + 1;
                            var newDiffStartTime = (new DateTime(Convert.ToInt32(setup.FirstNper.Substring(0, 4)),
                        Convert.ToInt32(setup.FirstNper.Substring(4, 2)), Convert.ToInt32(setup.FirstNper.Substring(6, 2)))).AddDays(1);
                            setup.FirstNper = string.Format("{0}{1:d3}", newDiffStartTime.ToString("yyyyMMdd"), 1);
                            setup.StartTime = newDiffStartTime.AddHours(13).AddMinutes(8).AddSeconds(setup.StartTime.Second);
                            //再来做处理
                            xyftSecondDiff = (int)(now - setup.StartTime).TotalSeconds;
                            xyftNperDiff = xyftSecondDiff / setup.Interval;
                            if (xyftSecondDiff % setup.Interval > 0) xyftNperDiff++;
                        }
                        result.NextNper = string.Format("{0}{1:d3}", setup.FirstNper.Substring(0, 8), Convert.ToInt32(setup.FirstNper.Substring(setup.FirstNper.Length - 3)) + xyftNperDiff);
                        result.StartTime = setup.StartTime.AddSeconds(xyftNperDiff * setup.Interval);
                    }
                    //在开售之后
                    else
                    {
                        var newStartTime = now.Date.AddHours(13).AddMinutes(9).AddSeconds(setup.StartTime.Second);
                        var newNper = string.Format("{0}{1:d3}", now.ToString("yyyyMMdd"), 1);
                        var xyftNperDiff = 0;
                        var endTime = now.Date.AddHours(4).AddMinutes(4).AddSeconds(setup.StartTime.Second);
                        if (endTime > setup.StartTime)
                        {
                            var xyftSecondDiff = (int)(endTime - setup.StartTime).TotalSeconds;
                            xyftNperDiff = xyftSecondDiff / setup.Interval;
                            if (xyftSecondDiff % setup.Interval > 0) xyftNperDiff++;
                        }

                        //新的一天相差期数
                        var newSecondDiff = (int)(now - newStartTime).TotalSeconds;
                        var newNperDiff = newSecondDiff / setup.Interval;
                        if (newSecondDiff % setup.Interval > 0) newNperDiff++;
                        //计算时间和期数
                        result.StartTime = newStartTime.AddSeconds(newNperDiff * setup.Interval);
                        result.NextNper = newNper.Substring(0, 8) + (1 + newNperDiff).ToString().PadLeft(3, '0');
                    }
                    break;
                #endregion
                #region 重庆时时彩
                case GameOfType.重庆时时彩:
                    dayDiff = (now - setup.StartTime).Days;
                    setup.StartTime = setup.StartTime.AddDays(dayDiff);
                    nperDiff = setup.DayNum * dayDiff;
                    result.DayNum = setup.DayNum;
                    var cqsscNper = (new DateTime(Convert.ToInt32(setup.FirstNper.Substring(0, 4)),
                        Convert.ToInt32(setup.FirstNper.Substring(4, 2)), Convert.ToInt32(setup.FirstNper.Substring(6, 2)))).AddDays(dayDiff).ToString("yyyyMMdd") + setup.FirstNper.Substring(setup.FirstNper.Length - 3);
                    setup.FirstNper = cqsscNper;
                    if (now < now.Date.AddMinutes(10))
                    {
                        result.StartTime = now.Date.AddMinutes(30);
                        result.NextNper = string.Format("{0}{1:d3}", now.ToString("yyyyMMdd"), 1);
                    }
                    else if (now > now.Date.AddHours(23).AddMinutes(50))
                    {
                        result.StartTime = now.Date.AddDays(1).AddMinutes(30);
                        result.NextNper = string.Format("{0}{1:d3}", now.AddDays(1).ToString("yyyyMMdd"), 1);
                    }
                    //在停售区间
                    else if (now >= now.Date.AddHours(3).AddMinutes(10).AddSeconds(setup.StartTime.Second)
                        && now <= now.Date.AddHours(7).AddMinutes(10).AddSeconds(setup.StartTime.Second))
                    {
                        result.StartTime = now.Date.AddHours(7).AddMinutes(30).AddSeconds(setup.StartTime.Second);
                        var cqsscSecondDiff = (int)(now.Date.AddHours(3).AddMinutes(10).AddSeconds(setup.StartTime.Second) - setup.StartTime).TotalSeconds;
                        var cqsscNperDiff = cqsscSecondDiff / setup.Interval;
                        if (cqsscSecondDiff % setup.Interval > 0) cqsscNperDiff++;
                        result.NextNper = now.ToString("yyyyMMdd") + "010";
                    }
                    //在停售区间之前
                    else if (now < now.Date.AddHours(3).AddMinutes(10).AddSeconds(setup.StartTime.Second))
                    {
                        var lastNper = Convert.ToInt32(setup.FirstNper.Substring(setup.FirstNper.Length - 3));
                        var stopDiff = setup.DayNum - lastNper;
                        var cqsscSecondDiff = (int)(now - setup.StartTime).TotalSeconds;
                        var cqsscNperDiff = cqsscSecondDiff / setup.Interval;
                        if (cqsscSecondDiff % setup.Interval > 0) cqsscNperDiff++;
                        if (cqsscNperDiff > stopDiff)
                        {
                            var newDiffStartTime = (new DateTime(Convert.ToInt32(setup.FirstNper.Substring(0, 4)),
                        Convert.ToInt32(setup.FirstNper.Substring(4, 2)), Convert.ToInt32(setup.FirstNper.Substring(6, 2)))).AddDays(1);
                            nperDiff += stopDiff + 1;
                            setup.FirstNper = newDiffStartTime.ToString("yyyyMMdd") + "001";
                            setup.StartTime = newDiffStartTime.AddMinutes(32).AddSeconds(setup.StartTime.Second);
                            //再来做处理
                            cqsscSecondDiff = (int)(now - setup.StartTime).TotalSeconds;
                            cqsscNperDiff = cqsscSecondDiff / setup.Interval;
                            if (cqsscSecondDiff % setup.Interval > 0) cqsscNperDiff++;
                        }
                        result.NextNper = setup.FirstNper.Substring(0, 8) + (Convert.ToInt32(setup.FirstNper.Substring(setup.FirstNper.Length - 3)) + cqsscNperDiff).ToString().PadLeft(3, '0');
                        result.StartTime = setup.StartTime.AddSeconds(cqsscNperDiff * setup.Interval);
                    }
                    //在开售之后
                    else
                    {
                        var newStartTime = now.Date.AddHours(7).AddMinutes(10).AddSeconds(setup.StartTime.Second);
                        var newNper = now.ToString("yyyyMMdd") + "010";
                        var cqsscNperDiff = 0;
                        var endTime = now.Date.AddHours(3).AddMinutes(10).AddSeconds(setup.StartTime.Second);
                        if (endTime > setup.StartTime)
                        {
                            var cqsscSecondDiff = (int)(endTime - setup.StartTime).TotalSeconds;
                            cqsscNperDiff = cqsscSecondDiff / setup.Interval;
                            if (cqsscSecondDiff % setup.Interval > 0) cqsscNperDiff++;
                        }

                        //新的一天相差期数
                        var newSecondDiff = (int)(now - newStartTime).TotalSeconds;
                        var newNperDiff = newSecondDiff / setup.Interval;
                        if (newSecondDiff % setup.Interval > 0) newNperDiff++;
                        //计算时间和期数
                        result.StartTime = newStartTime.AddSeconds(newNperDiff * setup.Interval);
                        result.NextNper = newNper.Substring(0, 8) + (9 + newNperDiff).ToString().PadLeft(3, '0');
                    }
                    break;
                #endregion
                #region 爱尔兰赛马
                case GameOfType.爱尔兰赛马:
                    dayDiff = (now - setup.StartTime).Days;
                    nperDiff = dayDiff * setup.DayNum;
                    setup.StartTime = setup.StartTime.AddDays(dayDiff);
                    //在当天停售之前
                    var ireland10StopTime = setup.StartTime.Date.AddDays(1).AddHours(4).AddMinutes(3).AddSeconds(setup.StartTime.Second);
                    if (now > ireland10StopTime)
                    {
                        var df = (int)(ireland10StopTime - setup.StartTime).TotalSeconds;
                        //相差期数
                        nperDiff += df / setup.Interval;
                        if (df % setup.Interval > 0) nperDiff++;
                        //是否在停售区间
                        if (now >= now.Date.AddHours(4).AddMinutes(3).AddSeconds(setup.StartTime.Second)
                            && now <= now.Date.AddHours(8).AddMinutes(8).AddSeconds(setup.StartTime.Second))
                        {
                            nperDiff++;
                            result.StartTime = now.Date.AddHours(8).AddMinutes(8).AddSeconds(setup.StartTime.Second);
                            result.NextNper = (Convert.ToInt64(setup.FirstNper) + nperDiff).ToString();
                        }
                        else
                        {
                            nperDiff++;
                            var gameStartTime = now.Date.AddHours(8).AddMinutes(8).AddSeconds(setup.StartTime.Second);
                            var newDiff = (int)(now - gameStartTime).TotalSeconds;
                            var reality = newDiff / setup.Interval;
                            if (newDiff % setup.Interval > 0) reality++;
                            result.StartTime = gameStartTime.AddSeconds(reality * setup.Interval);
                            result.NextNper = (Convert.ToInt64(setup.FirstNper) + nperDiff + reality).ToString();
                        }
                    }
                    else
                    {
                        var newDiff = (int)(now - setup.StartTime).TotalSeconds;
                        var reality = newDiff / setup.Interval;
                        if (newDiff % setup.Interval > 0) reality++;
                        result.StartTime = setup.StartTime.AddSeconds(reality * setup.Interval);
                        result.NextNper = (Convert.ToInt64(setup.FirstNper) + nperDiff + reality).ToString();
                    }
                    break;
                #endregion
                #region 爱尔兰快5
                case GameOfType.爱尔兰快5:
                    dayDiff = (now - setup.StartTime).Days;
                    nperDiff = dayDiff * setup.DayNum;
                    setup.StartTime = setup.StartTime.AddDays(dayDiff);
                    //在当天停售之前
                    var ireland5StopTime = setup.StartTime.Date.AddDays(1).AddHours(4).AddMinutes(1).AddSeconds(setup.StartTime.Second);
                    if (now > ireland5StopTime)
                    {
                        var df = (int)(ireland5StopTime - setup.StartTime).TotalSeconds;
                        //相差期数
                        nperDiff += df / setup.Interval;
                        if (df % setup.Interval > 0) nperDiff++;
                        //是否在停售区间
                        if (now >= now.Date.AddHours(4).AddMinutes(1).AddSeconds(setup.StartTime.Second)
                            && now <= now.Date.AddHours(8).AddMinutes(6).AddSeconds(setup.StartTime.Second))
                        {
                            nperDiff++;
                            result.StartTime = now.Date.AddHours(8).AddMinutes(6).AddSeconds(setup.StartTime.Second);
                            result.NextNper = (Convert.ToInt64(setup.FirstNper) + nperDiff).ToString();
                        }
                        else
                        {
                            nperDiff++;
                            var gameStartTime = now.Date.AddHours(8).AddMinutes(6).AddSeconds(setup.StartTime.Second);
                            var newDiff = (int)(now - gameStartTime).TotalSeconds;
                            var reality = newDiff / setup.Interval;
                            if (newDiff % setup.Interval > 0) reality++;
                            result.StartTime = gameStartTime.AddSeconds(reality * setup.Interval);
                            result.NextNper = (Convert.ToInt64(setup.FirstNper) + nperDiff + reality).ToString();
                        }
                    }
                    else
                    {
                        var newDiff = (int)(now - setup.StartTime).TotalSeconds;
                        var reality = newDiff / setup.Interval;
                        if (newDiff % setup.Interval > 0) reality++;
                        result.StartTime = setup.StartTime.AddSeconds(reality * setup.Interval);
                        result.NextNper = (Convert.ToInt64(setup.FirstNper) + nperDiff + reality).ToString();
                    }
                    break;
                #endregion
                #region 其它
                default:
                    dayDiff = (now - setup.StartTime).Days;
                    setup.StartTime = setup.StartTime.AddDays(dayDiff);
                    nperDiff = setup.DayNum * dayDiff;
                    //算出相差期数
                    secondDiff = (int)(now - setup.StartTime).TotalSeconds;
                    actualDiff = secondDiff / setup.Interval;
                    if (secondDiff % setup.Interval > 0) actualDiff++;
                    var startTime = setup.StartTime.AddSeconds(actualDiff * setup.Interval);
                    result.StartTime = startTime;
                    result.NextNper = (Convert.ToInt32(setup.FirstNper) + nperDiff + actualDiff).ToString();
                    result.DayNum = setup.DayNum;
                    break;
                    #endregion
            }
            result.ExpirationDate = result.StartTime.AddSeconds(dicGameOverdue[gameType]);
            return result;
        }

        public static Dictionary<GameOfType, int> dicGameOverdue { get; } =
            new Dictionary<GameOfType, int>()
            {
                { GameOfType.北京赛车, 1900 },
                { GameOfType.幸运飞艇, 290 },
                { GameOfType.澳州10, 290 },
                { GameOfType.澳州5, 290 },
                { GameOfType.重庆时时彩, 1900 },
                { GameOfType.爱尔兰快5, 290 },
                { GameOfType.爱尔兰赛马, 290 },
                { GameOfType.幸运飞艇168, 290},
                { GameOfType.极速赛车, 65},
                { GameOfType.极速时时彩, 65}
            };

        public class GameNextLottery
        {
            public DateTime StartTime { get; set; }
            public string NextNper { get; set; }
            public int DayNum { get; set; }
            public int Interval { get; set; }
            public DateTime ExpirationDate { get; set; }
        }

        /// <summary>
        /// 获取游戏信息
        /// </summary>
        /// <returns></returns>
        public static List<GameListItem> GetGameList()
        {
            var rseult = new List<GameListItem>
            {
                new GameListItem { GameType = GameOfType.北京赛车, GameName = "北京赛车", NickName = "Pk10" },
                new GameListItem { GameType = GameOfType.幸运飞艇, GameName = "幸运飞艇", NickName = "Xyft" },
                new GameListItem { GameType = GameOfType.极速赛车, GameName = "极速赛车", NickName = "Jssc" },
                new GameListItem { GameType = GameOfType.澳州10, GameName = "澳州10", NickName = "Azxy10" },
                new GameListItem { GameType = GameOfType.澳州5, GameName = "澳州5", NickName = "Azxy5", Type = 2 },
                new GameListItem { GameType = GameOfType.爱尔兰快5, GameName = "爱尔兰快5", NickName = "Ireland5", Type = 2 },
                new GameListItem { GameType = GameOfType.爱尔兰赛马, GameName = "爱尔兰赛马", NickName = "Ireland10" },
                new GameListItem { GameType = GameOfType.重庆时时彩, GameName = "重庆时时彩", NickName = "Cqssc", Type = 2 },
                new GameListItem { GameType = GameOfType.幸运飞艇168, GameName = "幸运飞艇168", NickName = "Xyft168" },
                new GameListItem { GameType = GameOfType.极速时时彩, GameName = "极速时时彩", NickName = "Jsssc", Type = 2 },
            };
            return rseult;
        }

        public class GameListItem
        {
            public GameOfType GameType { get; set; }

            public string GameName { get; set; }
            public string NickName { get; set; }

            /// <summary>
            /// 1:10  2:5
            /// </summary>
            public int Type { get; set; } = 1;
        }

        /// <summary>
        /// 获取枚举Description值
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescriptionName(Type enumType, object value)
        {
            // 获取枚举常数名称。
            string name = Enum.GetName(enumType, value);
            if (name != null)
            {
                // 获取枚举字段。
                FieldInfo fieldInfo = enumType.GetField(name);
                if (fieldInfo != null)
                { 
                    // 获取描述的属性。
                    if (Attribute.GetCustomAttribute(fieldInfo,
                        typeof(DescriptionAttribute), false) is DescriptionAttribute attr)
                    {
                        return attr.Description;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// 获取飞单内容集合
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="userBetInfos"></param>
        /// <returns></returns>
        public static List<FlyingBet> GetFlyingBet(GameOfType gameType, BetRemarkInfo userBetInfos)
        {
            var result = new List<FlyingBet>();
            if (GameTypeItemize(gameType))
            {
                foreach (var bet in userBetInfos.OddBets)
                {
                    var model = new FlyingBet();
                    if ((int)bet.BetRule >= (int)BetTypeEnum.第一名
                        && (int)bet.BetRule <= (int)BetTypeEnum.第十名)
                    {
                        var message = Enum.GetName(typeof(BetTypeEnum), (int)bet.BetRule);
                        var middchar = string.Empty;
                        if (message.Contains("一")) middchar = "1";
                        else if (message.Contains("二")) middchar = "2";
                        else if (message.Contains("三")) middchar = "3";
                        else if (message.Contains("四")) middchar = "4";
                        else if (message.Contains("五")) middchar = "5";
                        else if (message.Contains("六")) middchar = "6";
                        else if (message.Contains("七")) middchar = "7";
                        else if (message.Contains("八")) middchar = "8";
                        else if (message.Contains("九")) middchar = "9";
                        else if (message.Contains("十")) middchar = "10";

                        model.content = string.Format("第{0}球-{1}", middchar, bet.BetNo);
                        model.money = bet.BetMoney;
                        model.OddNum = userBetInfos.OddNum;
                        result.Add(model);
                        continue;
                    }
                    else if (bet.BetRule == BetTypeEnum.冠亚)
                    {
                        model.content = string.Format("冠亚和-{0}", bet.BetNo);
                        model.money = bet.BetMoney;
                        model.OddNum = userBetInfos.OddNum;
                        result.Add(model);
                        continue;
                    }
                }
            }
            else
            {
                foreach (var bet in userBetInfos.OddBets)
                {
                    var model = new FlyingBet();
                    if ((int)bet.BetRule >= (int)BetTypeEnum.第一球
                        && (int)bet.BetRule <= (int)BetTypeEnum.第五球)
                    {
                        var message = Enum.GetName(typeof(BetTypeEnum), (int)bet.BetRule);
                        var middchar = string.Empty;
                        if (message.Contains("一")) middchar = "1";
                        else if (message.Contains("二")) middchar = "2";
                        else if (message.Contains("三")) middchar = "3";
                        else if (message.Contains("四")) middchar = "4";
                        else if (message.Contains("五")) middchar = "5";

                        model.content = string.Format("第{0}球-{1}", middchar, bet.BetNo);
                        model.money = bet.BetMoney;
                        model.OddNum = userBetInfos.OddNum;
                        result.Add(model);
                        continue;
                    }
                    else if (bet.BetRule == BetTypeEnum.总和)
                    {
                        model.content = string.Format("总和{0}", bet.BetNo);
                        model.money = bet.BetMoney;
                        model.OddNum = userBetInfos.OddNum;
                        result.Add(model);
                        continue;
                    }
                    else if ((int)bet.BetRule >= (int)BetTypeEnum.前三 && (int)bet.BetRule <= (int)BetTypeEnum.后三)
                    {
                        model.content = string.Format("{0}-{1}", Enum.GetName(typeof(BetTypeEnum), (int)bet.BetRule), bet.BetNo);
                        model.money = bet.BetMoney;
                        model.OddNum = userBetInfos.OddNum;
                        result.Add(model);
                        continue;
                    }
                    else if ((int)bet.BetRule >= (int)BetTypeEnum.龙 && (int)bet.BetRule <= (int)BetTypeEnum.和)
                    {
                        model.content = string.Format("{0}", Enum.GetName(typeof(BetTypeEnum), (int)bet.BetRule));
                        model.money = bet.BetMoney;
                        model.OddNum = userBetInfos.OddNum;
                        result.Add(model);
                        continue;
                    }
                }
            }
            return result;
        }
    }
}
