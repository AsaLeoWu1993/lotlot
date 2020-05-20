using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using ManageSystem.Hubs;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Operation.Abutment;
using Operation.Common;
using Operation.RedisAggregate;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 消息队列
    /// </summary>
    public class RabbitMQHelper
    {
        static ConnectionFactory factory;
        static IConnection connection;
        static IModel channel;
        static RabbitMQHelper()
        {
            RedisConn();
        }

        static void RedisConn()
        {
            try
            {
                if (Environment.GetEnvironmentVariable("Online") == null)
                {
                    factory = new ConnectionFactory()
                    {
                        //HostName = "rabbitmq-0.rabbitmq.default.svc.cluster.local",
                        //Port = 5672,
                        //UserName = "rabbit",
                        //Password = "123456",
                        //UserName = "rabbit",
                        //Password = "123456",
                        //HostName = "120.24.20.112",
                        //HostName = "localhost",
                        HostName = "172.27.0.11",
                        //HostName = "132.232.70.44",
                        Port = 5672
                    };
                }
                else
                {
                    factory = new ConnectionFactory()
                    {
                        HostName = "rabbitmq-0.rabbitmq.default.svc.cluster.local",
                        Port = 5672,
                        UserName = "rabbit",
                        Password = "123456",
                    };
                }
                factory.AutomaticRecoveryEnabled = true;
                connection = factory.CreateConnection();
                channel = connection.CreateModel();
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
        }

        #region 添加消息队列
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="data"></param>
        private static void SendMould(string methodName, string data)
        {
            try
            {
                Dictionary<String, Object> args = new Dictionary<String, Object>();
                args.Add("x-max-length", int.MaxValue);
                //声明交换机
                channel.ExchangeDeclare(exchange: methodName, type: ExchangeType.Fanout, arguments: args);
                //消息内容
                byte[] body = Encoding.UTF8.GetBytes(data);
                //发送消息
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.DeliveryMode = 1;
                channel.BasicPublish(exchange: methodName, routingKey: "", basicProperties: basicProperties, body: body);
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="action"></param>
        private static void ReceiveMould(string methodName, Action<string> action)
        {
            try
            {
                Dictionary<String, Object> args = new Dictionary<String, Object>();
                args.Add("x-max-length", int.MaxValue);
                //声明交换机
                channel.ExchangeDeclare(exchange: methodName, type: ExchangeType.Fanout, arguments: args);
                //消息队列名称
                string queueName = methodName + "_" + Guid.NewGuid().ToString();
                //声明队列
                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: true, arguments: null);
                //将队列与交换机进行绑定
                channel.QueueBind(queue: queueName, exchange: methodName, routingKey: "");
                //定义消费者
                var consumer = new EventingBasicConsumer(channel);
                //接收事件
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var message = ea.Body;//接收到的消息
                        var data = Encoding.UTF8.GetString(message);
                        action.Invoke(data);
                    }
                    catch (Exception e)
                    {
                        Utils.Logger.Error(e);
                    }
                };
                //开启监听
                channel.BasicConsume(queue: queueName,
                                     autoAck: true,
                                     consumer: consumer);
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
        }
        #endregion

        #region 添加任务消息队列
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="data"></param>
        private static void SendJobMould(string methodName, string data)
        {
            try
            {
                Dictionary<String, Object> args = new Dictionary<String, Object>();
                args.Add("x-max-length", int.MaxValue);
                //声明交换机
                channel.ExchangeDeclare(exchange: methodName, type: ExchangeType.Direct, arguments: args);
                //定义Queues
                string queueName = methodName + "Job";
                bool durable = false;//设RabbitMQ置持久化
                channel.QueueDeclare(queueName, durable, false, false, null);
                //绑定exchanges 和Queues
                channel.QueueBind(queueName, methodName, "", null);
                //消息内容
                byte[] body = Encoding.UTF8.GetBytes(data);
                //发送消息
                var basicProperties = channel.CreateBasicProperties();
                basicProperties.DeliveryMode = 1;
                channel.BasicPublish(exchange: methodName, routingKey: "", basicProperties: basicProperties, body: body);
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
        }

        /// <summary>
        /// 接收消息
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="action"></param>
        private static void ReceiveJobMould(string methodName, Action<string> action)
        {
            try
            {
                //公平调用
                channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                //订阅方式获取message
                var consumer = new EventingBasicConsumer(channel);
                //接收事件
                consumer.Received += (model, ea) =>
                {
                    try
                    {
                        var message = ea.Body;//接收到的消息
                        var data = Encoding.UTF8.GetString(message);
                        action.Invoke(data);
                    }
                    catch (Exception e)
                    {
                        Utils.Logger.Error(e);
                    }
                    finally
                    {
                        //手动设置回复
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);
                    }
                };
                //开启监听
                channel.BasicConsume(queue: methodName + "Job",
                                     autoAck: false,
                                     consumer: consumer);
            }
            catch (OperationInterruptedException)
            {
                RedisConn();
                Dictionary<String, Object> args = new Dictionary<String, Object>();
                args.Add("x-max-length", int.MaxValue);
                //声明交换机
                channel.ExchangeDeclare(exchange: methodName, type: ExchangeType.Direct, arguments: args);
                //定义Queues
                string queueName = methodName + "Job";
                bool durable = false;//设RabbitMQ置持久化
                channel.QueueDeclare(queueName, durable, false, false, null);
                //绑定exchanges 和Queues
                channel.QueueBind(queueName, methodName, "", null);
                ReceiveJobMould(methodName, action);
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
        }
        #endregion

        #region 管理员消息
        /// <summary>
        /// 发送管理员消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="isColl"></param>
        /// <param name="userConns"></param>
        public static async Task SendAdminMessage(string message, string merchantID, GameOfType gameType, bool isColl = false, List<string> userConns = null)
        {
            string methodName = "SendAdminMessage";
            if (userConns.IsNull())
                userConns = await Utils.GetRoomGameConnIDs(merchantID, gameType);
            var url = RedisOperation.GetAdminPortrait(merchantID);
            var data = new SendAdminMessageClass()
            {
                MerchantID = merchantID,
                GameType = gameType,
                Message = message,
                ConnectionIds = userConns,
                Url = url
            };
            if (!isColl)
            {
                //先将数据保存至数据库
                var address = await Utils.GetAddress(merchantID);
                UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
                var collection = userSendMessageOperation.GetCollection(merchantID);
                UserSendMessage insert = new UserSendMessage()
                {
                    Avatar = url,
                    MerchantID = merchantID,
                    Message = message,
                    NickName = "管理员",
                    UserType = UserEnum.管理员,
                    GameType = gameType
                };
                await collection.InsertOneAsync(insert);
            }
            if (userConns.IsNull()) return;
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收管理员消息
        /// </summary>
        /// <param name="hubContext"></param>
        /// <returns></returns>
        public static void ReceiveAdminMessage(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendAdminMessage";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendAdminMessageClass>(result);

                WebUserSendMessage userSendMessage = new WebUserSendMessage()
                {
                    Avatar = data.Url,
                    MerchantID = data.MerchantID,
                    Message = data.Message,
                    NickName = "管理员",
                    UserType = UserEnum.管理员,
                    GameType = data.GameType
                };
                if (data.ConnectionIds.IsNull()) return;
                await hubContext.Clients.Clients(data.ConnectionIds).SendAsync("SendMessage", JsonConvert.SerializeObject(userSendMessage));
            });
        }

        private class SendAdminMessageClass
        {
            public string Message { get; set; }

            public string MerchantID { get; set; }

            public GameOfType GameType { get; set; }

            public List<string> ConnectionIds { get; set; }
            public string ConnectionId { get; set; }

            public bool BeOverdue { get; set; }

            public string Url { get; set; }
        }
        #endregion

        #region 玩家积分
        /// <summary>
        /// 发送玩家积分
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="merchantID"></param>
        public static async Task SendUserPointChange(string userID, string merchantID)
        {
            string methodName = "SendUserPointChange";
            var connid = Utils.GetConnID(userID, merchantID);
            if (string.IsNullOrEmpty(connid))
                connid = await Utils.GetBaccaratConnIDByUserID(merchantID, userID, null);
            var data = new SendUserPointChangeClass()
            {
                MerchantID = merchantID,
                UserID = userID,
                ConnectionId = connid
            };
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收玩家积分
        /// </summary>
        /// <param name="hubContext"></param>
        public static void ReceiveUserPointChange(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendUserPointChange";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendUserPointChangeClass>(result);
                //发送signalr消息
                var user = await new UserOperation().GetModelAsync(t => t._id == data.UserID && t.MerchantID == data.MerchantID);
                if (user == null) return;

                if (!string.IsNullOrEmpty(data.ConnectionId))
                {
                    await hubContext.Clients.Clients(data.ConnectionId).SendAsync("PointChange", user.UserMoney.ToString());
                    return;
                }
            });
        }

        private class SendUserPointChangeClass
        {
            public string MerchantID { get; set; }
            public string UserID { get; set; }

            public string ConnectionId { get; set; }
        }
        #endregion

        #region 用户消息
        /// <summary>
        /// 发送用户消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="userID"></param>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        public static async Task SendUserMessage(string message, string userID, string merchantID, GameOfType gameType)
        {
            string methodName = "SendUserMessage";
            //先将数据保存至数据库
            var address = await Utils.GetAddress(merchantID);
            UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
            var collection = userSendMessageOperation.GetCollection(merchantID);
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID);
            var list = await Utils.GetRoomGameConnIDs(merchantID, gameType, userID);
            UserSendMessage insert = new UserSendMessage()
            {
                Avatar = user.Avatar,
                Message = message,
                MerchantID = merchantID,
                NickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                UserID = user._id,
                UserType = UserEnum.普通用户,
                GameType = gameType
            };
            await collection.InsertOneAsync(insert);
            var data = new SendUserMessageClass()
            {
                MerchantID = merchantID,
                GameType = gameType,
                Message = message,
                UserID = userID,
                Avatar = user.Avatar,
                UserType = UserEnum.普通用户,
                NickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                ConnectionIds = list
            };
            if (list.IsNull()) return;
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收玩家信息
        /// </summary>
        /// <param name="hubContext"></param>
        public static void ReceiveSendUserMessage(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendUserMessage";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendUserMessageClass>(result);
                if (data.ConnectionIds.IsNull()) return;
                await hubContext.Clients.Clients(data.ConnectionIds).SendAsync("SendMessage", JsonConvert.SerializeObject(data));
            });
        }

        private class SendUserMessageClass
        {
            public string Message { get; set; }

            public string MerchantID { get; set; }

            public GameOfType GameType { get; set; }

            public string Avatar { get; set; }

            public string UserID { get; set; }

            public string NickName { get; set; }

            public UserEnum UserType { get; set; }

            public List<string> ConnectionIds { get; set; }
        }

        /// <summary>
        /// 发送用户消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="userID">用户id</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="znum">桌号</param>
        public static async Task SendVideoUserMessage(string message, string userID, string merchantID, BaccaratGameType gameType, int znum)
        {
            string methodName = "SendUserVideoMessage";
            if (string.IsNullOrEmpty(message)) return;
            //先将数据保存至数据库
            //var address = await Utils.GetAddress(merchantID);
            //UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
            //var collection = userSendMessageOperation.GetCollection(merchantID);
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID);
            //UserSendMessage insert = new UserSendMessage()
            //{
            //    Avatar = user.Avatar,
            //    Message = message,
            //    MerchantID = merchantID,
            //    NickName = user.NickName,
            //    UserID = user._id,
            //    UserType = UserEnum.普通用户,
            //    VGameType = gameType,
            //    ZNum = znum
            //};
            //await collection.InsertOneAsync(insert);
            var data = new SendBaccaratMessageClass()
            {
                Avatar = user.Avatar,
                Message = message,
                MerchantID = merchantID,
                NickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                UserID = user._id,
                UserType = UserEnum.普通用户,
                GameType = gameType,
                Znum = znum
            };
            //发送signalr消息
            var list = await Utils.GetBaccaratConnIDs(data.MerchantID, data.Znum, data.UserID);
            if (list.IsNull()) return;
            var result = new SendAdminMessageClass()
            {
                ConnectionIds = list,
                Message = JsonConvert.SerializeObject(data)
            };
            SendMould(methodName, JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 接收玩家信息
        /// </summary>
        /// <param name="hubContext"></param>
        public static void ReceiveVideoSendUserMessage(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendUserVideoMessage";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendAdminMessageClass>(result);
                if (!data.ConnectionIds.IsNull())
                    await hubContext.Clients.Clients(data.ConnectionIds).SendAsync("SendVideoMessage", data.Message);
            });
        }
        #endregion

        #region 全局
        /// <summary>
        /// 发送大厅消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="merchantID"></param>
        /// <param name="methodModelName"></param>
        public static async Task SendOverallMessage(string message, string merchantID, string methodModelName)
        {
            string methodName = "SendOverallMessage";
            var roomlist = await Utils.GetRoomListConnIDs(merchantID);
            var data = new SendOverallMessageClass()
            {
                MerchantID = merchantID,
                Message = message,
                MethodName = methodModelName,
                ConnectionIds = roomlist
            };
            if (roomlist.IsNull()) return;
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收大厅消息
        /// </summary>
        /// <param name="hubContext"></param>
        public static void ReceiveOverallMessage(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendOverallMessage";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendOverallMessageClass>(result);
                if (data.ConnectionIds.IsNull()) return;
                await hubContext.Clients.Clients(data.ConnectionIds).SendAsync(data.MethodName, data.Message);
            });
        }

        private class SendOverallMessageClass
        {
            public string MerchantID { get; set; }
            public string Message { get; set; }

            public string MethodName { get; set; }

            public List<string> ConnectionIds { get; set; }
        }
        #endregion

        #region 相关游戏或游戏房间
        /// <summary>
        /// 发送游戏消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="merchantID"></param>
        /// <param name="methodModelName"></param>
        /// <param name="gameType"></param>
        /// <param name="userID"></param>
        /// <param name="userConns"></param>
        /// <returns></returns>
        public static async Task SendGameMessage(string message, string merchantID, string methodModelName, GameOfType? gameType, string userID = null, List<string> userConns = null)
        {
            string methodName = "SendGameMessage";
            if (userConns.IsNull())
            {
                userConns = new List<string>();
                //发送signalr消息
                if (gameType != null)
                    userConns = await Utils.GetRoomGameConnIDs(merchantID, gameType.Value);
                else
                {
                    var userConnID = Utils.GetConnID(userID, merchantID);
                    userConns.Add(userConnID);
                }
            }
            var data = new SendGameMessageClass()
            {
                MerchantID = merchantID,
                Message = message,
                MethodName = methodModelName,
                GameType = gameType,
                ConnectionIds = userConns
            };
            if (userConns.IsNull()) return;
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收游戏消息
        /// </summary>
        /// <param name="hubContext"></param>
        public static void ReceiveGameMessage(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendGameMessage";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendGameMessageClass>(result);

                if (data.ConnectionIds.IsNull()) return;
                await hubContext.Clients.Clients(data.ConnectionIds).SendAsync(data.MethodName, data.Message);
            });
        }

        private class SendGameMessageClass : SendOverallMessageClass
        {
            public GameOfType? GameType { get; set; }
        }
        #endregion

        #region 发送游戏内所有人
        /// <summary>
        /// 发送所有用户消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="merchantID"></param>
        /// <param name="methodModelName"></param>
        public static void SendAllinMessage(string message, string merchantID, string methodModelName)
        {
            string methodName = "SendAllinMessage";
            //发送signalr消息
            BsonOperation bsonOperation = new BsonOperation("SignalR");
            var bsonList = bsonOperation.Collection.Find(bsonOperation.Builder.Eq("MerchantID", merchantID)).ToList();
            var list = new List<string>();
            if (!bsonList.IsNull())
            {
                foreach (var bson in bsonList)
                {
                    list.Add(bson["ConnectionId"].ToString());
                }
            }
            var data = new SendOverallMessageClass()
            {
                MerchantID = merchantID,
                Message = message,
                MethodName = methodModelName,
                ConnectionIds = list
            };
            if (list.IsNull()) return;
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收所有用户消息
        /// </summary>
        /// <param name="hubContext"></param>
        public static void ReceiveAllinMessage(IHubContext<ChatHub> hubContext)
        {
            {
                string methodName = "SendAllinMessage";
                ReceiveMould(methodName, async result =>
                {
                    var data = JsonConvert.DeserializeObject<SendOverallMessageClass>(result);
                    if (!data.ConnectionIds.IsNull())
                        await hubContext.Clients.Clients(data.ConnectionIds).SendAsync(data.MethodName, data.Message);
                });
            }
        }
        #endregion

        #region 飞单
        /// <summary>
        /// 发送飞单
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="nper">期号</param>
        /// <param name="result"></param>
        /// <param name="retry">重试次数</param>
        /// <returns></returns>
        public static async Task SendFlyingSheet(string merchantID, GameOfType gameType, string nper, SendFlying result, int retry = 0)
        {
            string methodName = "SendFlyingSheet";
            if (result == null) return;
            SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
            if (retry >= 10)
            {
                result.Status = SendFlyEnum.未接收;
                await sendFlyingOperation.UpdateModelAsync(result);
                return;
            }
            if (retry > 0)
            {
                //查询是否存在
                var model = await sendFlyingOperation.GetModelAsync(t => t.uuid == result.uuid);
                if (model.Status == SendFlyEnum.预发送)
                {
                    model.Status = SendFlyEnum.等待发送;
                    await sendFlyingOperation.UpdateModelAsync(model);
                }
                //不存在则表示发送成功
                if (model == null) return;
                if (model.Status == SendFlyEnum.已接收) return;
            }
            var conn = Utils.GetFlySheet(merchantID);
            if (string.IsNullOrEmpty(conn))
            {
                //重试
                ++retry;
                await CentralProcess.Replacement(result, merchantID, gameType, nper, retry);
                return;
            }
            var status = Utils.FlySheetJudgementExpires(merchantID);
            var data = new SendAdminMessageClass()
            {
                MerchantID = merchantID,
                GameType = gameType,
                ConnectionId = conn,
                BeOverdue = status
            };
            if (!string.IsNullOrEmpty(conn))
            {
                var obj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(result));
                obj.Remove("_id");
                obj.Remove("CreatedTime");
                obj.Remove("LastUpdateTime");
                obj.Remove("MerchantID");
                obj.Remove("Status");
                data.Message = JsonConvert.SerializeObject(obj);
            }
            if (string.IsNullOrEmpty(data.Message)) return;
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收飞单消息
        /// </summary>
        /// <param name="hubContext"></param>
        public static void ReceiveFlyingSheet(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendFlyingSheet";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendAdminMessageClass>(result);

                if (!data.BeOverdue)
                {
                    await hubContext.Clients.Client(data.ConnectionId).SendAsync("LoginStatus", 3);
                    await Utils.DeleteFlySheetConn(data.ConnectionId);
                    return;
                }
                //发送signalr消息

                if (!string.IsNullOrEmpty(data.ConnectionId))
                {
                    await hubContext.Clients.Client(data.ConnectionId).SendAsync("FlySheetSend", data.Message);
                }
            });
        }

        /// <summary>
        /// 发送用户盘口信息
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static async Task SendMerchantHandicap(string merchantID, string userID)
        {
            string methodName = "SendMerchantHandicap";
            var connIDs = await Utils.GetMerchantFlySheetConns(merchantID, userID);
            if (connIDs.IsNull()) return;
            var infos = await CentralProcess.GetMerchantHandicap(merchantID, userID);
            var data = new SendOverallMessageClass()
            {
                Message = JsonConvert.SerializeObject(infos),
                ConnectionIds = connIDs
            };
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收用户盘口信息
        /// </summary>
        /// <param name="hubContext"></param>
        public static void ReceiveMerchantHandicap(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendMerchantHandicap";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendOverallMessageClass>(result);

                //发送signalr消息
                await hubContext.Clients.Clients(data.ConnectionIds).SendAsync("TargetInfo", data.Message);
            });
        }

        #region 不使用
        /// <summary>
        /// 发送商户飞单用户下注信息
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="targetID">目标id</param>
        /// <param name="userID">用户id</param>
        /// <param name="message">信息</param>
        /// <returns></returns>
        [Obsolete("不使用", true)]
        public static async Task SendMerchantBetInfo(string merchantID, string targetID, string userID, string message)
        {
            string methodName = "SendMerchantBetInfo";
            string connID = await Utils.GetMerchantFlySheetConn(merchantID, targetID, userID);
            if (string.IsNullOrEmpty(connID)) return;
            JObject jObject = new JObject();
            jObject.Add("ConnectionId", connID);
            jObject.Add("Info", message);
            SendMould(methodName, JsonConvert.SerializeObject(jObject));
        }

        /// <summary>
        /// 接收商户飞单用户下注信息
        /// </summary>
        /// <param name="hubContext"></param>
        [Obsolete("不使用", true)]
        public static void ReceiveMerchantBetInfo(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendMerchantBetInfo";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<JObject>(result);

                //发送signalr消息
                if (!string.IsNullOrEmpty(data["ConnectionId"].ToString()))
                {
                    await hubContext.Clients.Client(data["ConnectionId"].ToString()).SendAsync("MerchantBetSheet", data["Info"].ToString());
                }
            });
        }
        #endregion
        #endregion

        #region 任务分发
        /// <summary>
        /// 发送任务分发
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static void SendTaskDistribution(TaskDistributionModel data)
        {
            string methodName = "SendTaskDistribution";
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收任务分发
        /// </summary>
        public static void ReceiveTaskDistribution()
        {
            string methodName = "SendTaskDistribution";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<TaskDistributionModel>(result);

                //关闭状态才接收
                var gameStatus = DistributionLow.GameTaskStatus[data.GameType];
                if (gameStatus == DistributionLow.TaskStatusEnum.关闭)
                {
                    await DistributionLow.DistributionAsync(data);
                }
            });
        }

        /// <summary>
        /// 发送任务分发
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static void SendVideoTaskDistribution(VideoTaskDistributionModel data)
        {
            string methodName = "SendBaccaratTaskDistribution";
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收任务分发
        /// </summary>
        public static void ReceiveVideoTaskDistribution()
        {
            string methodName = "SendBaccaratTaskDistribution";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<VideoTaskDistributionModel>(result);

                //关闭状态才接收
                var gameStatus = VideoDistributionLow.TaskStatusEnum.关闭;
                if (VideoDistributionLow.GameTaskStatus.ContainsKey(data.ZNum))
                {
                    gameStatus = VideoDistributionLow.GameTaskStatus[data.ZNum];
                }
                else
                {
                    gameStatus = VideoDistributionLow.TaskStatusEnum.关闭;
                    VideoDistributionLow.GameTaskStatus.Add(data.ZNum, VideoDistributionLow.TaskStatusEnum.关闭);
                }
                if (gameStatus == VideoDistributionLow.TaskStatusEnum.关闭)
                {
                    await VideoDistributionLow.DistributionAsync(data);
                }
            });
        }
        #endregion

        #region 发送视讯游戏消息
        /// <summary>
        /// 发送管理员消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="merchantID"></param>
        /// <param name="znum"></param>
        /// <param name="gameType"></param>
        /// <param name="list"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public static async Task SendBaccaratAdminMessage(string message, string merchantID, int? znum, BaccaratGameType gameType, List<string> list = null, string userID = null)
        {
            string methodName = "SendBaccaratAdminMessage";
            if (string.IsNullOrEmpty(message)) return;
            if (znum == null)
            {
                var info = await Utils.GetBaccaratUserConn(merchantID, userID, gameType);
                if (info == null) return;
                znum = info.Item2;
            }
            if (list.IsNull())
                list = await Utils.GetBaccaratConnIDs(merchantID, znum.Value);
            if (list.IsNull()) return;
            var url = RedisOperation.GetAdminPortrait(merchantID);
            var result = new SendBaccaratMessageClass()
            {
                Avatar = url,
                Message = message,
                MerchantID = merchantID,
                NickName = "管理员",
                UserType = UserEnum.管理员,
                GameType = gameType,
                Znum = znum.Value
            };
            var data = new SendAdminMessageClass()
            {
                ConnectionIds = list,
                Message = JsonConvert.SerializeObject(result)
            };
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 发送管理员消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        public static async Task SendBaccaratUserMessage(string message, string merchantID, string userID, BaccaratGameType gameType)
        {
            if (string.IsNullOrEmpty(message)) return;
            //查询用户是否在游戏内
            var result = await Utils.GetBaccaratUserConn(merchantID, userID, gameType);
            if (result == null) return;
            await SendBaccaratAdminMessage(message, merchantID, result.Item2, gameType);
        }

        /// <summary>
        /// 接收管理员消息
        /// </summary>
        /// <param name="hubContext"></param>
        /// <returns></returns>
        public static void ReceiveBaccaratAdminMessage(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendBaccaratAdminMessage";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendAdminMessageClass>(result);
                if (data.ConnectionIds.IsNull()) return;
                await hubContext.Clients.Clients(data.ConnectionIds).SendAsync("SendVideoMessage", data.Message);
            });
        }
        #endregion

        #region 刷新视讯游戏状态
        /// <summary>
        /// 发送游戏状态
        /// </summary>
        /// <param name="message"></param>
        public static async Task SendGameStatus(string message)
        {
            var methodName = "SendGameStatus";
            var list = await Utils.GetBaccaratConnIDsByTid();
            var result = new SendAdminMessageClass()
            {
                ConnectionIds = list,
                Message = message
            };
            if (list.IsNull()) return;
            SendMould(methodName, JsonConvert.SerializeObject(result));
        }

        /// <summary>
        /// 接收游戏状态
        /// </summary>
        /// <param name="hubContext"></param>
        public static void ReceiveGameStatus(IHubContext<ChatHub> hubContext)
        {
            var methodName = "SendGameStatus";
            ReceiveMould(methodName, async result =>
            {
                //发送游戏状态
                var data = JsonConvert.DeserializeObject<SendAdminMessageClass>(result);
                if (data.ConnectionIds.IsNull()) return;
                await hubContext.Clients.Clients(data.ConnectionIds).SendAsync("GameStatusInfo", data);
            });
        }
        #endregion

        #region 百家乐分发任务 
        /// <summary>
        /// 发送各个商户信息
        /// </summary>
        /// <param name="merchantIDs"></param>
        /// <param name="znum"></param>
        /// <param name="gameType"></param>
        /// <param name="nper"></param>
        /// <param name="type"></param>
        public static void SendBaccaratStartBet(List<string> merchantIDs, int znum, BaccaratGameType gameType, string nper, int type)
        {
            var methodName = "SendBaccaratStartBet";
            SendJobMould(methodName, JsonConvert.SerializeObject(new BaccaratJobClass()
            {
                Merchants = merchantIDs,
                ZNum = znum,
                GameType = BaccaratGameType.百家乐,
                Type = type,
                Nper = nper
            }));
        }

        /// <summary>
        /// 接收开始下注信息
        /// </summary>
        /// <param name="hubContext"></param>
        /// <returns></returns>
        public static void ReceiveBaccaratStartBet(IHubContext<ChatHub> hubContext)
        {
            var methodName = "SendBaccaratStartBet";
            ReceiveJobMould(methodName, result =>
            {
                //发送游戏状态
                var data = JsonConvert.DeserializeObject<BaccaratJobClass>(result);
                var tasks = new List<Task>();
                foreach (var merchantID in data.Merchants)
                {
                    var task = Task.Run(async()=>
                    {
                        var conns = await Utils.GetBaccaratConnIDs(merchantID, data.ZNum);
                        if (conns.IsNull()) return;
                        if (data.Type == 1)
                        {
                            await GameCollection.BaccaratMsgHandle(merchantID, data.Nper, data.ZNum, data.GameType, conns);
                        }
                        else if (data.Type == 2)
                        {
                            var msg = await WinPrize.GameBaccaratOnlineInfo(merchantID, data.GameType);
                            await SendBaccaratAdminMessage(msg, merchantID, data.ZNum, BaccaratGameType.百家乐, conns);
                        }
                    });
                    tasks.Add(task);
                }
                Task.WaitAll(tasks.ToArray());
                //await hubContext.Clients.Clients(data.ConnectionIds).SendAsync("GameStatusInfo", data);
            });
        }
        #endregion

        #region 后台游戏状态
        ///// <summary>
        ///// 发送后台消息
        ///// </summary>
        ///// <param name="merchantID"></param>
        ///// <param name="msg"></param>
        ///// <param name="sendmethodName"></param>
        ///// <returns></returns>
        //public static async Task SendBackstageMessage(string merchantID, string msg, string sendmethodName)
        //{
        //    string methodName = "SendBackstageMessage";
        //    var conn = await RedisOperation.GetValueAsync("Backstage", merchantID);
        //    if (string.IsNullOrEmpty(conn)) return;
        //    var data = new BackstageMessage()
        //    {
        //        MerchantID = merchantID,
        //        Message = msg,
        //        ConnectionId = conn,
        //        MethodName = sendmethodName
        //    };
        //    SendMould(methodName, JsonConvert.SerializeObject(data));
        //}

        ///// <summary>
        ///// 接收后台消息
        ///// </summary>
        ///// <param name="hubContext"></param>
        //public static void ReceiveBackstageMessage(IHubContext<ChatHub> hubContext)
        //{
        //    string methodName = "SendBackstageMessage";
        //    ReceiveMould(methodName, async result =>
        //    {
        //        var data = JsonConvert.DeserializeObject<BackstageMessage>(result);
        //        await hubContext.Clients.Clients(data.ConnectionId).SendAsync(data.MethodName, data.Message);
        //    });
        //}

        //private class BackstageMessage
        //{ 
        //    public string MerchantID { get; set; }

        //    public string Message { get; set; }

        //    public string ConnectionId { get; set; }

        //    public string MethodName { get; set; }
        //}
        #endregion
    }

    internal class SendBaccaratMessageClass
    {
        public SendBaccaratMessageClass()
        {
        }

        public string Avatar { get; set; }
        public string Message { get; set; }
        public string MerchantID { get; set; }
        public string NickName { get; set; }
        public UserEnum UserType { get; set; }
        public BaccaratGameType GameType { get; set; }
        public int Znum { get; set; }
        public string UserID { get; set; }
    }

    public class BaccaratJobClass
    {
        public List<string> Merchants { get; set; }

        public int ZNum { get; set; }

        public BaccaratGameType GameType { get; set; }

        public int Type { get; set; }

        public string Nper { get; set; }
    }
}
