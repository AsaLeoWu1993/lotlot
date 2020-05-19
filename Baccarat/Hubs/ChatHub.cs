using Baccarat.Interactive;
using Baccarat.Manipulate;
using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Agent;
using Operation.Baccarat;
using Operation.Common;
using Operation.RedisAggregate;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Baccarat.Manipulate.QuartzJob;
using static Operation.Common.GameBetsMessage;

namespace Baccarat.Hubs
{
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
            BsonOperation bsonOperation = new BsonOperation("Baccarat");
            FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("ConnectionId", Context.ConnectionId);
            bsonOperation.Collection.DeleteOneAsync(filter).GetAwaiter().GetResult();

            return base.OnDisconnectedAsync(exception);
        }

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
            var gameList = await RedisOperation.GetHashValue<GameStatic>("Baccarat");
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
                //发送历史消息前20条
                var address = Utils.GetAddress(merchantID);
                UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
                var collection = userSendMessageOperation.GetCollection(merchantID);
                var allRoomInfos = await collection.FindListAsync(t => t.MerchantID == merchantID && t.VGameType == baccaratGameType && t.ZNum == znum);
                var hisMsg = allRoomInfos.OrderByDescending(t => t.CreatedTime).Take(20).OrderBy(t => t.CreatedTime).ToList();
                foreach (var data in hisMsg)
                {
                    SendBaccaratMessageClass userSendMessage = new SendBaccaratMessageClass()
                    {
                        Avatar = data.Avatar,
                        Message = data.Message,
                        MerchantID = data.MerchantID,
                        NickName = data.NickName,
                        UserID = data.UserID,
                        UserType = data.UserType,
                        GameType = data.VGameType,
                        Znum = data.ZNum
                    };
                    if (data == hisMsg.Last())
                        userSendMessage.End = true;
                    await Clients.Clients(Context.ConnectionId).SendAsync("SendVideoMessage", JsonConvert.SerializeObject(userSendMessage));
                }

                //房间人数
                var online = await Utils.GetBaccaratConnIDs(merchantID, znum);
                RoomOperation roomOperation = new RoomOperation();
                var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
                var count = online.Count + room.Online;
                await RabbitMQHelper.SendOverallMessage(count.ToString(), merchantID, "SendVideoOnline", znum);

                //发送游戏列表
                await Clients.Client(Context.ConnectionId).SendAsync("BaccaratListInfos", JsonConvert.SerializeObject(result));
            }
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
            var result = await RedisOperation.GetHashValue<GameStatic>("Baccarat");
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

        #region 用户下注
        /// <summary>
        /// 用户下注
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="userID">用户id</param>
        /// <param name="znum">桌号</param>
        /// <param name="message">消息</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="oid">标识</param>
        /// <returns></returns>
        public async Task UserBetMessage(string merchantID, string userID, int znum, string message, BaccaratGameType gameType, string oid)
        {
            try
            {
                MerchantOperation merchantOperation = new MerchantOperation();
                var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
                if (merchant == null)
                {
                    await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.失败, "未查询到商户信息！", oid)));
                    return;
                }
                var infos = await RedisOperation.GetValueAsync<GameStatic>("Baccarat", znum.ToString());
                var nper = infos.Scene;
                //查询用户信息
                UserOperation userOperation = new UserOperation();
                var user = await userOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == userID && t.Status != UserStatusEnum.删除);
                var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
                var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
                if (message.Contains("上分") || message.Contains("下分") || message == "取消" || message == "全部取消" || message == "查" || message == "返水")
                {
                    await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType, znum);
                    try
                    {
                        #region 上下分
                        if (message.Contains("上分"))
                        {
                            var amount = Convert.ToDecimal(message.Replace("上分", ""));
                            if (amount <= 0)
                            {
                                var result = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, null, nper, null, gameType);
                                await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                                await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                                return;
                            }

                            //判断用户类型
                            if (user.Status == UserStatusEnum.正常)
                            {
                                await CancelAnnouncement.UpperScore(userID, null, merchantID, amount, baccaratGameType: gameType, znum: znum);

                                if (reply.NoticeCheckRequest)
                                {
                                    var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, null, nper, null, gameType);
                                    await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                                }
                                await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                                return;
                            }
                            else
                            {
                                var recordID = await CancelAnnouncement.UpperScore(userID, null, merchantID, amount, status: NotesEnum.虚拟, baccaratGameType: gameType, znum: znum);

                                if (reply.NoticeCheckRequest)
                                {
                                    var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, null, nper, null, gameType);
                                    await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                                }

                                //查询定时
                                RoomOperation roomOperation = new RoomOperation();
                                var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
                                if (room == null)
                                {
                                    await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.失败, "未查询到房间信息！", oid)));
                                    return;
                                }
                                if (room.ShamOnfirm)
                                {
                                    await SendShamUserApply(recordID, merchantID, room.ShamOnfirmTime);
                                }
                                await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                                return;
                            }
                        }
                        else if (message.Contains("下分"))
                        {
                            var amount = Convert.ToDecimal(message.Replace("下分", ""));
                            if (amount <= 0)
                            {
                                var result = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, null, nper, null, gameType);
                                await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                                await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                                return;
                            }
                            if (user.UserMoney < amount)
                            {
                                await RabbitMQHelper.SendBaccaratAdminMessage(string.Format("@{0} 积分不足，下分失败", nickName), merchantID, znum, gameType);
                                await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                                return;
                            }
                            //判断用户类型
                            if (user.Status == UserStatusEnum.正常)
                            {
                                await CancelAnnouncement.LowerScore(userID, null, merchantID, amount, NotesEnum.正常, gameType, znum);
                                if (reply.NoticeCheckRequest)
                                {
                                    var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, null, nper, null, gameType);
                                    await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                                }
                                await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                                return;
                            }
                            else
                            {
                                var recordID = await CancelAnnouncement.LowerScore(userID, null, merchantID, amount, NotesEnum.虚拟, gameType, znum);
                                if (reply.NoticeCheckRequest)
                                {
                                    var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, null, nper, null, gameType);
                                    await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                                    RabbitMQHelper.SendUserPointChange(userID, merchantID);
                                }
                                //查询定时
                                RoomOperation roomOperation = new RoomOperation();
                                var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
                                if (room == null)
                                {
                                    await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.失败, "未查询到房间信息！", oid)));
                                    return;
                                }
                                if (room.ShamOnfirm)
                                {
                                    await SendShamUserApply(recordID, merchantID, room.ShamOnfirmTime);
                                }
                                await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                                return;
                            }
                        }
                        #endregion
                        #region 查
                        else if (message == "查")
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                            var result = await Utils.InstructionConversion(reply.CheckScore + "\r\n" + reply.CheckStream, userID, merchantID, null, nper, null, gameType);
                            await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                            return;
                        }
                        #endregion
                        #region 取消
                        else if (message == "取消" || message == "全部取消")
                        {
                            await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                            FoundationSetupOperation foundationSetupOperation = new FoundationSetupOperation();
                            var setup = await foundationSetupOperation.GetModelAsync(t => t.MerchantID == merchantID);
                            if (setup.ProhibitChe || infos.Cstate != "init")
                            {
                                var result = await Utils.InstructionConversion(reply.ProhibitionCancel, userID, merchantID, null, nper, null, gameType);
                                await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                                return;
                            }
                            decimal userAmount = 0;
                            var output = string.Empty;
                            if (message == "取消")
                                CancelAnnouncement.CancelVideoOne(userID, gameType, merchantID, nper, ref output, reply, ref userAmount);
                            else
                                CancelAnnouncement.CancelVideoAll(userID, gameType, merchantID, nper, ref output, reply, ref userAmount);
                            if (!string.IsNullOrEmpty(output) && reply.NoticeCancel)
                            {
                                await RabbitMQHelper.SendBaccaratAdminMessage(output, merchantID, znum, gameType);
                            }
                            if (userAmount != 0)
                            {

                            }
                            return;
                        }
                        #endregion
                    }
                    catch
                    {
                        var result = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, null, nper, null, gameType);
                        await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                    }
                }
                var keywords = new List<string>()
                { 
                    "庄","闲","和","庄对","闲对", "任意对子"
                };
                if (message.Contains(keywords))
                {
                    await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType, znum);
                    var status = await Common.GetVideoGameStatus(merchantID, gameType);
                    if (!status)
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.失败, "游戏房间已关闭！", oid)));
                        await RabbitMQHelper.SendBaccaratAdminMessage(string.Format("@{0} 游戏房间已关闭，禁止下注！", nickName), merchantID, znum, gameType);
                        return;
                    }
                    //获取当前房间状态
                    if (infos.Cstate != "init")
                    {
                        await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.失败, "未到下注时间！", oid)));
                        await RabbitMQHelper.SendBaccaratAdminMessage(string.Format("@{0} 未到下注时间，禁止下注！", nickName), merchantID, znum, gameType);
                        return;
                    }
                    else
                    {
                        var userBetStatus = user.Status == UserStatusEnum.正常 ?
                            NotesEnum.正常 : NotesEnum.虚拟;
                        var result = await GameBetsMessage.Baccarat(userID, znum, message, merchantID, nper, userBetStatus);
                        if (result.Status == BetStatuEnum.正常)
                        {
                            if (reply.NoticeBetSuccess)
                            {
                                var msgResult = await Utils.InstructionConversion(reply.GameSuccess, userID, merchantID, null, nper, result.OddNum, gameType);
                                var money = await userOperation.GetUserMoney(merchantID, userID);
                                //发送用户积分
                                await Clients.Clients(Context.ConnectionId).SendAsync("PointChange", money.ToString("#0.00"));
                                await RabbitMQHelper.SendBaccaratAdminMessage(msgResult, merchantID, znum, gameType);
                            }
                        }
                        else if (result.Status == BetStatuEnum.积分不足 && reply.NoticeInsufficientIntegral)
                        {
                            var msgResult = await Utils.InstructionConversion(reply.NotEnough, userID, merchantID, null, nper, null, gameType);
                            await RabbitMQHelper.SendBaccaratAdminMessage(msgResult, merchantID, znum, gameType);
                        }
                        else if (result.Status == BetStatuEnum.限额 && reply.NoticeQuota)
                        {
                            await RabbitMQHelper.SendBaccaratAdminMessage(string.Format("@{0} {1}", nickName, result.OutPut), merchantID, znum, gameType);
                        }
                        else if (result.Status == BetStatuEnum.格式错误 && reply.NoticeInvalidSub)
                        {
                            var msgResult = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, null, nper, null, gameType);
                            await RabbitMQHelper.SendBaccaratAdminMessage(msgResult, merchantID, znum, gameType);
                        }
                    }
                    await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                    return;
                }
                //else if ()
                //{
                //    await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType, znum);
                //    var msgResult = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, null, nper, null, gameType);
                //    await RabbitMQHelper.SendBaccaratAdminMessage(msgResult, merchantID, znum, gameType);
                //    await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                //    return;
                //}
                if (!user.Talking)
                {
                    await RabbitMQHelper.SendBaccaratAdminMessage(string.Format("@{0}禁止发送聊天消息", nickName), merchantID, znum, gameType);
                }
                else
                {
                    await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType, znum);
                }
                await Clients.Client(Context.ConnectionId).SendAsync("BetMessage", JsonConvert.SerializeObject(new RecoverKeywordModel(RecoverEnum.成功, "发送成功！", oid)));
                return;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 定时确认上下分
        /// </summary>
        /// <param name="recordID"></param>
        /// <param name="merchantID"></param>
        /// <param name="time"></param>
        /// <returns></returns>
        private async Task SendShamUserApply(string recordID, string merchantID, int time)
        {
            var key = Guid.NewGuid().ToString();
            //1、通过调度工厂获得调度器
            //var scheduler = await _schedulerFactory.GetScheduler();
            var scheduler = await StdSchedulerFactory.GetDefaultScheduler();
            //2、开启调度器
            await scheduler.Start();
            IDictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("merchantID", merchantID);
            dic.Add("recordID", recordID);
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
        #endregion
    }
}
