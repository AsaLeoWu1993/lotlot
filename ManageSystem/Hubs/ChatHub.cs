using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using ManageSystem.Manipulate;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Agent;
using Operation.Common;
using Operation.RedisAggregate;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Operation.Common.GameBetsMessage;

namespace ManageSystem.Hubs
{
    /// <summary>
    /// 长连接
    /// </summary>
    [EnableCors("AllowAllOrigin")]
    public class ChatHub : Hub
    {
        /// <summary>
        /// 连接时添加redis
        /// </summary>
        /// <returns></returns>
        public override Task OnConnectedAsync()
        {
            return base.OnConnectedAsync();
        }

        /// <summary>
        /// 创建进入房间初始化
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="gameType">游戏类型</param>
        public async Task InitRoomGame(string userId, string merchantID, GameOfType gameType)
        {
            try
            {
                //var infos = RedisOperation.GetValuesAsync(userId.Replace("-", ""));
                //if (infos == null || infos.Count == 0)
                //{
                //    await Clients.Client(Context.ConnectionId).SendAsync("Subordinate", 1);
                //    return;
                //}
                //Utils.Logger.Error("进入房间");
                await Utils.AddRoomGameItem(userId, merchantID, gameType, Context.ConnectionId);
                //Utils.Logger.Error("添加长连接信息");
                //添加redis信息
                //RedisOperation.SetHashAsync("RoomGame"+merchantID,)
                //发送历史消息前10条
                var address = await Utils.GetAddress(merchantID);
                UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
                var collection = userSendMessageOperation.GetCollection(merchantID);
                var hisMsg = collection.Find(t => t.MerchantID == merchantID && t.GameType == gameType).SortByDescending(t => t.CreatedTime).Limit(20).ToList().OrderBy(t => t.CreatedTime).ToList();
                foreach (var data in hisMsg)
                {
                    WebUserSendMessage userSendMessage = new WebUserSendMessage()
                    {
                        Avatar = data.Avatar,
                        Message = data.Message,
                        MerchantID = data.MerchantID,
                        NickName = data.NickName,
                        UserID = data.UserID,
                        UserType = data.UserType,
                        GameType = data.GameType
                    };
                    if (data == hisMsg.Last())
                        userSendMessage.End = true;
                    await Clients.Clients(Context.ConnectionId).SendAsync("SendMessage", JsonConvert.SerializeObject(userSendMessage));
                }
                //Utils.Logger.Error("拉取房间历史信息");
                //发送最新一期开奖结果
                MerchantOperation merchantOperation = new MerchantOperation();
                var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
                var lottery = await Utils.GetGameStatus(merchant._id, gameType);
                if (lottery != null)
                    await Clients.Clients(Context.ConnectionId).SendAsync("SendRoomMessage", JsonConvert.SerializeObject(lottery));
                //Utils.Logger.Error("拉取游戏状态信息");
                UserOperation userOperation = new UserOperation();
                //在线人数 包括机器人
                var count = await userOperation.GetCountAsync(t => t.MerchantID == merchantID && (t.Status == UserStatusEnum.正常 || t.Status == UserStatusEnum.假人));
                var room = await new RoomOperation().GetModelAsync(t => t.MerchantID == merchantID);
                count += room.Online;
                await Clients.Clients(Context.ConnectionId).SendAsync("SendRoomOnline", count.ToString());
                //Utils.Logger.Error("拉取房间人数");
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
        /// 关闭游戏房间连接
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        public async Task LeaveRoomGame(string userId, string merchantID, GameOfType gameType)
        {
            await Utils.RemoveRoomGameItem(userId, merchantID);
            //MerchantOperation merchantOperation = new MerchantOperation();
            //var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            ////在线人数 包括机器人
            //var count = await Utils.GetOnlineCount(merchantID, gameType);
            //var room = await new RoomOperation().GetModelAsync(t => t.MerchantID == merchantID);
            //count += room.Online;
            //await RabbitMQHelper.SendGameMessage(count.ToString(), merchantID, "SendRoomOnline", gameType);
        }

        ///// <summary>
        ///// 后台连接长连接
        ///// </summary>
        ///// <param name="merchantID"></param>
        ///// <returns></returns>
        //public async Task InitBackstage(string merchantID)
        //{
        //    RedisOperation.SetHashAsync("Backstage", merchantID, Context.ConnectionId);
        //    //返回状态
        //    #region 返回消息
        //    //MerchantOperation merchantOperation = new MerchantOperation();
        //    //var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
        //    var dic = GameBetsMessage.EnumToDictionary(typeof(GameOfType));
        //    foreach (var item in dic)
        //    {
        //        var gameType = GameBetsMessage.GetEnumByStatus<GameOfType>(item.Value);
        //        var code = await Utils.GetGameStatus(merchantID, gameType);
        //        await Clients.Client(Context.ConnectionId).SendAsync("BackstageGameInfo", JsonConvert.SerializeObject(code));
        //    }
            
        //    #endregion
        //}

        /// <summary>
        /// 创建房间列表初始化
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <param name="merchantID">商户id</param>
        public async Task InitRoomList(string userId, string merchantID)
        {
            try
            {
                //var infos = RedisOperation.GetValuesAsync(userId.Replace("-", ""));
                //if (infos == null || infos.Count == 0)
                //{
                //    await Clients.Client(Context.ConnectionId).SendAsync("Subordinate", 1);
                //    return;
                //}
                await Utils.AddRoomListItem(userId, merchantID, Context.ConnectionId);
                #region 返回消息
                MerchantOperation merchantOperation = new MerchantOperation();
                var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
                var dic = GameBetsMessage.EnumToDictionary(typeof(GameOfType));
                var result = new List<WebAppGameInfos>();
                //var tasks = new List<Task>();
                foreach (var item in dic)
                {
                    var gameType = GameBetsMessage.GetEnumByStatus<GameOfType>(item.Value);
                    var code = await Utils.GetGameStatus(merchant._id, gameType);
                    result.Add(code);
                }
                //await Task.WhenAll(tasks.ToArray()).ContinueWith(async t =>
                //{
                //    await Clients.Client(Context.ConnectionId).SendAsync("SendListMessageItem", JsonConvert.SerializeObject(result));
                //});
                await Clients.Client(Context.ConnectionId).SendAsync("SendListMessageItem", JsonConvert.SerializeObject(result));
                #endregion

                //公告
                ArticleOperation articleOperation = new ArticleOperation();
                var article = articleOperation.GetModel(t => t.MerchantID == merchantID && t.ArticleType == ArticleTypeEnum.公告 && t.Open == true, t => t.CreatedTime, false);
                if (article != null)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("SendListArticle", article.Content);
                }
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
        /// 关闭房间列表连接
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="merchantID"></param>
        public async Task LeaveRoomList(string userId, string merchantID)
        {
            await Utils.RemoveRoomListItem(userId, merchantID);
        }

        /// <summary>
        /// 管理员发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userID"></param>
        /// <param name="gameType"></param>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public async Task AdminSendMessage(string message, string userID, GameOfType gameType, string merchantID)
        {
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID);
            var list = await Utils.GetRoomGameConnIDs(merchantID, gameType);
            WebUserSendMessage userSendMessage = new WebUserSendMessage()
            {
                Avatar = "",
                MerchantID = merchantID,
                Message = string.Format("@{0} {1}", user.NickName, message),
                NickName = "管理员",
                UserType = UserEnum.管理员
            };
            await Clients.Clients(list).SendAsync("SendMessage", JsonConvert.SerializeObject(userSendMessage));
        }

        ///// <summary>
        ///// 商户后台初始化连接
        ///// </summary>
        ///// <param name="merchantID"></param>
        ///// <returns></returns>
        //public async Task InitBackstage(string merchantID)
        //{
        //    await Utils.AddBackstage(merchantID, Context.ConnectionId);
        //    MerchantOperation merchantOperation = new MerchantOperation();
        //    var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
        //    List<Task> tasks = new List<Task>();
        //    tasks.Add(Task.Run(async () =>
        //    {
        //        var code = await GameDiscrimination.EachpartAsync(GameOfType.北京赛车, merchant._id);
        //        var result = await GameHandle.GetMonitorInfos(code, merchant._id);
        //        await Clients.Client(Context.ConnectionId).SendAsync("SendMonitorInfo", JsonConvert.SerializeObject(result));
        //    }));
        //    tasks.Add(Task.Run(async () =>
        //    {
        //        var code = await GameDiscrimination.EachpartAsync(GameOfType.幸运飞艇, merchant._id);
        //        var result = await GameHandle.GetMonitorInfos(code, merchant._id);
        //        await Clients.Client(Context.ConnectionId).SendAsync("SendMonitorInfo", JsonConvert.SerializeObject(result));
        //    }));
        //    tasks.Add(Task.Run(async () =>
        //    {
        //        var code = await GameDiscrimination.EachpartAsync(GameOfType.极速赛车, merchant._id);
        //        var result = await GameHandle.GetMonitorInfos(code, merchant._id);
        //        await Clients.Client(Context.ConnectionId).SendAsync("SendMonitorInfo", JsonConvert.SerializeObject(result));
        //    }));
        //    tasks.Add(Task.Run(async () =>
        //    {
        //        var code = await GameDiscrimination.EachpartAsync(GameOfType.澳州10, merchant._id);
        //        var result = await GameHandle.GetMonitorInfos(code, merchant._id);
        //        await Clients.Client(Context.ConnectionId).SendAsync("SendMonitorInfo", JsonConvert.SerializeObject(result));
        //    }));
        //    tasks.Add(Task.Run(async () =>
        //    {
        //        var code = await GameDiscrimination.EachpartAsync(GameOfType.澳州5, merchant._id);
        //        var result = await GameHandle.GetMonitorInfos(code, merchant._id);
        //        await Clients.Client(Context.ConnectionId).SendAsync("SendMonitorInfo", JsonConvert.SerializeObject(result));
        //    }));
        //    tasks.Add(Task.Run(async () =>
        //    {
        //        var code = await GameDiscrimination.EachpartAsync(GameOfType.重庆时时彩, merchant._id);
        //        var result = await GameHandle.GetMonitorInfos(code, merchant._id);
        //        await Clients.Client(Context.ConnectionId).SendAsync("SendMonitorInfo", JsonConvert.SerializeObject(result));
        //    }));
        //    await Task.WhenAll(tasks.ToArray());
        //}

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public async Task LeaveBackstage(string merchantID)
        {
            await Utils.DeleteBackstage(merchantID);
        }

        /// <summary>
        /// 心跳机制
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task Heart(string message)
        {
            await Clients.Clients(Context.ConnectionId).SendAsync("Heart", message);
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public override Task OnDisconnectedAsync(Exception exception)
        {
            BsonOperation bsonOperation = new BsonOperation("SignalR");
            FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("ConnectionId", Context.ConnectionId);
            bsonOperation.Collection.DeleteOneAsync(filter).GetAwaiter().GetResult();

            BsonOperation flybsonOperation = new BsonOperation("SheetFly");
            flybsonOperation.Collection.DeleteOneAsync(filter).GetAwaiter().GetResult();
            Utils.DeleteMerchantFlySheetConn(Context.ConnectionId).GetAwaiter().GetResult();

            BsonOperation baccaratOperation = new BsonOperation("Baccarat");
            FilterDefinition<BsonDocument> vfilter = bsonOperation.Builder.Eq("ConnectionId", Context.ConnectionId);
            baccaratOperation.Collection.DeleteOneAsync(vfilter).GetAwaiter().GetResult();

            return base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// 商户后台登录
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="targetID">目标商户id</param>
        /// <param name="userID">目标用户id</param>
        /// <returns></returns>
        public async Task MerchantSheetLogin(string merchantID, string targetID, string userID)
        {
            //验证成功
            await Utils.AddMerchantFlySheet(merchantID, targetID, userID, Context.ConnectionId);
            await Clients.Client(Context.ConnectionId).SendAsync("LoginStatus", JsonConvert.SerializeObject(new RecoverModel(RecoverEnum.成功, "登录成功！")));

            var infos = await CentralProcess.GetMerchantHandicap(targetID, userID);
            await Clients.Client(Context.ConnectionId).SendAsync("TargetInfo", JsonConvert.SerializeObject(infos));
            return;
        }

        /// <summary>
        /// 商户飞单退出
        /// </summary>
        /// <returns></returns>
        public async Task MerchantSheetExit()
        {
            await Utils.DeleteMerchantFlySheetConn(Context.ConnectionId);
        }

        /// <summary>
        /// 飞单
        /// </summary>
        /// <param name="merchantName"></param>
        /// <param name="merchantPwd"></param>
        /// <returns></returns>
        public async Task SheetLogin(string merchantName, string merchantPwd)
        {
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t.MeName == merchantName);
            if (merchant == null)
            {
                await Utils.DeleteFlySheetConn(Context.ConnectionId);
                await Clients.Client(Context.ConnectionId).SendAsync("LoginStatus", 2);
            }
            else if (merchant.Password != Utils.MD5(merchantPwd))
            {
                await Utils.DeleteFlySheetConn(Context.ConnectionId);
                await Clients.Client(Context.ConnectionId).SendAsync("LoginStatus", 2);
            }
            else if (merchant.MaturityTime < DateTime.Now)
            {
                await Utils.DeleteFlySheetConn(Context.ConnectionId);
                await Clients.Client(Context.ConnectionId).SendAsync("LoginStatus", 3);
            }
            else
            {
                var conn = Utils.GetFlySheet(merchant._id);
                if (!string.IsNullOrEmpty(conn))
                {
                    await Clients.Client(conn).SendAsync("Failure", "登录失效");
                }

                await Utils.AddFlySheet(merchant._id, Context.ConnectionId, merchant.MaturityTime);
                //连接成功
                await Clients.Client(Context.ConnectionId).SendAsync("LoginStatus", 1);
            }
        }

        /// <summary>
        /// 飞单回调   删除数据
        /// </summary>
        /// <param name="uuid"></param>
        /// <returns></returns>
        public async Task SheetCallBack(string uuid)
        {
            SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
            var model = await sendFlyingOperation.GetModelAsync(t => t.uuid == uuid);
            model.Status = SendFlyEnum.已接收;
            await sendFlyingOperation.UpdateModelAsync(model);
        }

        #region 视讯
        /// <summary>
        /// 加入游戏房间
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="userID">用户id</param>
        /// <param name="znum">桌号</param>
        /// <param name="baccaratGameType">游戏类型</param>
        /// <returns></returns>
        public async Task JoinRoom(string merchantID, string userID, int znum, BaccaratGameType baccaratGameType)
        {
            #region 用户状态
            //判断用户状态
            var merchant = await new MerchantOperation().GetModelAsync(t => t._id == merchantID);
            if (merchant == null)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("LoginStatic", JsonConvert.SerializeObject(new RecoverModel(RecoverEnum.失败, "未查询到商户信息！")));
                return;
            }
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchant.AgentID);
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agent.HighestAgentID);
            if (merchant.MaturityTime <= DateTime.Now && setup.Formal)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("LoginStatic", JsonConvert.SerializeObject(new RecoverModel(RecoverEnum.失败, "商户已到期，请联系管理员！")));
                return;
            }
            var roomStatus = await Common.GetVideoGameStatus(merchantID, baccaratGameType);
            if (!roomStatus)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("LoginStatic", JsonConvert.SerializeObject(new RecoverModel(RecoverEnum.失败, "游戏房间关闭！")));
                return;
            }
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == userID && t.Status != UserStatusEnum.删除);
            if (user == null)
            {
                await Clients.Client(Context.ConnectionId).SendAsync("LoginStatic", JsonConvert.SerializeObject(new RecoverModel(RecoverEnum.失败, "用户帐号或密码错误！")));
                return;
            }
            if (!(user.Status == UserStatusEnum.正常 || (user.Status == UserStatusEnum.假人 && user.IsSupport == false)))
            {
                if (user.Status == UserStatusEnum.冻结)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("LoginStatic", JsonConvert.SerializeObject(new RecoverModel(RecoverEnum.身份冻结, "账号被冻结，无法登录！")));
                    return;
                }
            }
            #endregion
            //返回游戏信息
            var gamedicList = RedisOperation.GetHashValue<GameStatic>("Baccarat");
            var gameList = new List<GameStatic>();
            foreach (var item in gamedicList)
            {
                gameList.Add(item.Value);
            }
            for (int i = 0; i < gameList.Count; i++)
            {
                var data = gameList[i];
                data.Ttime = DateTime.Now > data.EndTime ? 0 : (int)(data.EndTime - DateTime.Now).TotalSeconds;
                gameList[i] = data;
            }
            if (!gameList.Exists(t => t.ZNum == znum))
            {
                await Clients.Client(Context.ConnectionId).SendAsync("LoginStatic", JsonConvert.SerializeObject(new RecoverModel(RecoverEnum.失败, "未查询到房间信息！")));
                return;
            }
            else
            {
                var result = gameList.Find(t => t.ZNum == znum);
                //加入房间
                await Utils.AddBaccarat(merchantID, userID, znum, Context.ConnectionId);
                await Clients.Client(Context.ConnectionId).SendAsync("LoginStatic", JsonConvert.SerializeObject(new RecoverClassModel<dynamic>()
                {
                    Message = "进入房间成功！",
                    Model = new
                    {
                        NickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName
                    }
                }));
                var status = await Common.GetVideoGameStatus(merchantID, baccaratGameType);
                await Clients.Client(Context.ConnectionId).SendAsync("BaccaratRoomStatus", status);
                //发送用户余额
                await Clients.Client(Context.ConnectionId).SendAsync("PointChange", user.UserMoney.ToString());
                //游戏信息
                await Clients.Client(Context.ConnectionId).SendAsync("BaccaratRoomInfo", JsonConvert.SerializeObject(result));
                //游戏历史信息
                ////发送历史消息前20条
                //var address = await Utils.GetAddress(merchantID);
                //UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
                //var collection = userSendMessageOperation.GetCollection(merchantID);
                //var allRoomInfos = await collection.FindListAsync(t => t.MerchantID == merchantID && t.VGameType == baccaratGameType && t.ZNum == znum);
                //var hisMsg = allRoomInfos.OrderByDescending(t => t.CreatedTime).Take(20).OrderBy(t => t.CreatedTime).ToList();
                //foreach (var data in hisMsg)
                //{
                //    Entity.WebModel.SendBaccaratMessageClass userSendMessage = new Entity.WebModel.SendBaccaratMessageClass()
                //    {
                //        Avatar = data.Avatar,
                //        Message = data.Message,
                //        MerchantID = data.MerchantID,
                //        NickName = data.NickName,
                //        UserID = data.UserID,
                //        UserType = data.UserType,
                //        GameType = data.VGameType,
                //        Znum = data.ZNum
                //    };
                //    if (data == hisMsg.Last())
                //        userSendMessage.End = true;
                //    await Clients.Clients(Context.ConnectionId).SendAsync("SendVideoMessage", JsonConvert.SerializeObject(userSendMessage));
                //}

                //房间人数
                //在线人数 包括机器人
                var count = await userOperation.GetCountAsync(t => t.MerchantID == merchantID && (t.Status == UserStatusEnum.正常 || t.Status == UserStatusEnum.假人));
                var room = await new RoomOperation().GetModelAsync(t => t.MerchantID == merchantID);
                count += room.Online;
                await Clients.Clients(Context.ConnectionId).SendAsync("SendVideoOnline", count.ToString());
                //发送游戏列表
                await Clients.Client(Context.ConnectionId).SendAsync("BaccaratListInfos", JsonConvert.SerializeObject(result));
            }
        }

        /// <summary>
        /// 离开百家乐房间
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="userID"></param>
        /// <param name="znum"></param>
        /// <returns></returns>
        public async Task LeaveBaccaratRoom(string merchantID, string userID, int znum)
        {
            await Utils.DeleteBaccarat(merchantID, userID, znum);
        }

        /// <summary>
        /// 加入游戏房间列表
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="userID">用户id</param>
        /// <returns></returns>
        public async Task JoinRoomList(string merchantID, string userID)
        {
            //加入列表
            await Utils.AddBaccarat(merchantID, userID, null, Context.ConnectionId, "List");
            var resultDic = RedisOperation.GetHashValue<GameStatic>("Baccarat");
            var result = new List<GameStatic>();
            foreach (var item in resultDic)
            {
                result.Add(item.Value);
            }
            //百家乐
            var status = await Common.GetVideoGameStatus(merchantID, BaccaratGameType.百家乐);
            await Clients.Client(Context.ConnectionId).SendAsync("BaccaratRoomStatus", status);
            for (int i = 0; i < result.Count; i++)
            {
                var data = result[i];
                data.Ttime = DateTime.Now > data.EndTime ? 0 : (int)(data.EndTime - DateTime.Now).TotalSeconds;
                result[i] = data;
            }
            await Clients.Client(Context.ConnectionId).SendAsync("BaccaratListInfos", JsonConvert.SerializeObject(result));
        }
        #endregion
    }
}
