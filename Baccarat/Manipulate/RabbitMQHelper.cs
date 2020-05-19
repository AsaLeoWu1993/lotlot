using Baccarat.Hubs;
using Baccarat.Interactive;
using Baccarat.RedisModel;
using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Common;
using Operation.RedisAggregate;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Baccarat.Manipulate
{
    /// <summary>
    /// 消息队列
    /// </summary>
    public class RabbitMQHelper
    {
        static readonly ConnectionFactory factory;
        static readonly IConnection connection;
        static readonly IModel channel;
        static RabbitMQHelper()
        {
            try
            {
                factory = new ConnectionFactory()
                {
                    //HostName = "148.70.115.163",
                    //Port = 5672
                    HostName = "rabbitmq-0.rabbitmq.default.svc.cluster.local",
                    Port = 5672,
                    UserName = "rabbit",
                    Password = "123456"
                };
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
                //声明交换机
                channel.ExchangeDeclare(exchange: methodName, type: "fanout");
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
                //声明交换机
                channel.ExchangeDeclare(exchange: methodName, type: "fanout");
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
                        byte[] message = ea.Body;//接收到的消息
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
        #region 游戏房间对应所有人消息
        /// <summary>
        /// 发送管理员消息
        /// </summary>
        /// <param name="message">消息内容</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="znum">桌号</param>
        /// <param name="gameType">游戏类型</param>
        public static async Task SendBaccaratAdminMessage(string message, string merchantID, int znum, BaccaratGameType gameType)
        {
            string methodName = "SendBaccaratAdminMessage";
            if (string.IsNullOrEmpty(message)) return;
            //先将数据保存至数据库
            var address = Utils.GetAddress(merchantID);
            UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
            var collection = userSendMessageOperation.GetCollection(merchantID);
            UserSendMessage insert = new UserSendMessage()
            {
                Avatar = "",
                MerchantID = merchantID,
                Message = message,
                NickName = "管理员",
                UserType = UserEnum.管理员,
                VGameType = gameType,
                ZNum = znum
            };
            await collection.InsertOneAsync(insert);
            var result = new SendBaccaratMessageClass()
            {
                Avatar = "",
                Message = message,
                MerchantID = merchantID,
                NickName = "管理员",
                UserType = UserEnum.管理员,
                GameType = gameType,
                Znum = znum
            };
            var list = await Utils.GetBaccaratConnIDs(merchantID, znum);
            if (list.IsNull()) return;
            var data = new SendAdminMessageClass()
            {
                ConnectionIds = list,
                Message = JsonConvert.SerializeObject(result)
            };
            SendMould(methodName, JsonConvert.SerializeObject(data));
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

        private class SendAdminMessageClass
        {
            public string Message { get; set; }
            public List<string> ConnectionIds { get; set; }
            public string ConnectionId { get; set; }
        }
        #endregion
        #region 刷新游戏状态
        /// <summary>
        /// 发送游戏状态
        /// </summary>
        /// <param name="message"></param>
        public static async Task SendGameStatus(string message)
        {
            var methodName = "SendGameStatus";
            var data = JsonConvert.DeserializeObject<GameStatic>(message);
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
        #region 用户消息
        /// <summary>
        /// 发送用户消息
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="userID">用户id</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="znum">桌号</param>
        public static async Task SendUserMessage(string message, string userID, string merchantID, BaccaratGameType gameType, int znum)
        {
            string methodName = "SendUserVideoMessage";
            //先将数据保存至数据库
            var address = Utils.GetAddress(merchantID);
            UserSendMessageOperation userSendMessageOperation = await BetManage.GetMessageOperation(address);
            var collection = userSendMessageOperation.GetCollection(merchantID);
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID);
            UserSendMessage insert = new UserSendMessage()
            {
                Avatar = user.Avatar,
                Message = message,
                MerchantID = merchantID,
                NickName = user.NickName,
                UserID = user._id,
                UserType = UserEnum.普通用户,
                VGameType = gameType,
                ZNum = znum
            };
            await collection.InsertOneAsync(insert);
            var data = new SendBaccaratMessageClass()
            {
                Avatar = user.Avatar,
                Message = message,
                MerchantID = merchantID,
                NickName = user.NickName,
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
        public static void ReceiveSendUserMessage(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendUserVideoMessage";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendAdminMessageClass>(result);
                if (!data.ConnectionIds.IsNull())
                    await hubContext.Clients.Clients(data.ConnectionIds).SendAsync("SendVideoMessage", result);
            });
        }
        #endregion
        #region 玩家积分
        /// <summary>
        /// 发送玩家积分
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="merchantID"></param>
        public static void SendUserPointChange(string userID, string merchantID)
        {
            string methodName = "SendUserPointChange";
            var data = new SendUserPointChangeClass()
            {
                MerchantID = merchantID,
                UserID = userID
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
                //视讯游戏
                var connid = await Utils.GetBaccaratConnIDByUserID(data.MerchantID, data.UserID, null);
                if (!string.IsNullOrEmpty(connid))
                {
                    await hubContext.Clients.Clients(connid).SendAsync("PointChange", user.UserMoney.ToString());
                    return;
                }
                connid = Utils.GetConnID(data.UserID, data.MerchantID);
                if (!string.IsNullOrEmpty(connid))
                {
                    await hubContext.Clients.Clients(connid).SendAsync("PointChange", user.UserMoney.ToString());
                    return;
                }
            });
        }

        private class SendUserPointChangeClass
        {
            public string MerchantID { get; set; }
            public string UserID { get; set; }
        }
        #endregion
        #region 下发任务
        /// <summary>
        /// 发送任务分发
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public static void SendTaskDistribution(TaskDistributionModel data)
        {
            string methodName = "SendBaccaratTaskDistribution";
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收任务分发
        /// </summary>
        public static void ReceiveTaskDistribution()
        {
            string methodName = "SendBaccaratTaskDistribution";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<TaskDistributionModel>(result);

                //关闭状态才接收
                var gameStatus = DistributionLow.TaskStatusEnum.关闭;
                if (DistributionLow.GameTaskStatus.ContainsKey(data.ZNum))
                {
                    gameStatus = DistributionLow.GameTaskStatus[data.ZNum];
                }
                else
                {
                    gameStatus = DistributionLow.TaskStatusEnum.关闭;
                    DistributionLow.GameTaskStatus.Add(data.ZNum, DistributionLow.TaskStatusEnum.关闭);
                }
                if (gameStatus == DistributionLow.TaskStatusEnum.关闭)
                {
                    await DistributionLow.DistributionAsync(data);
                }
            });
        }
        #endregion
        #region 大厅及其它发送消息通用方法
        /// <summary>
        /// 发送大厅消息
        /// </summary>
        /// <param name="message"></param>
        /// <param name="merchantID"></param>
        /// <param name="methodModelName"></param>
        /// <param name="znum"></param>
        public static async Task SendOverallMessage(string message, string merchantID, string methodModelName, int znum)
        {
            string methodName = "SendVOverallMessage";
            var list = await Utils.GetBaccaratConnIDs(merchantID, znum);
            var data = new SendOverallMessageClass()
            {
                Message = message,
                MethodName = methodModelName,
                ConnectionIds = list
            };
            if (list.IsNull()) return;
            SendMould(methodName, JsonConvert.SerializeObject(data));
        }

        /// <summary>
        /// 接收大厅消息
        /// </summary>
        /// <param name="hubContext"></param>
        public static void ReceiveOverallMessage(IHubContext<ChatHub> hubContext)
        {
            string methodName = "SendVOverallMessage";
            ReceiveMould(methodName, async result =>
            {
                var data = JsonConvert.DeserializeObject<SendOverallMessageClass>(result);
                if (data.ConnectionIds.IsNull()) return;
                await hubContext.Clients.Clients(data.ConnectionIds).SendAsync(data.MethodName, data.Message);
            });
        }
        #endregion
        #region 发送游戏信息  用户
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
        #endregion
    }

    internal class SendOverallMessageClass
    {
        public SendOverallMessageClass()
        {
        }

        public string Message { get; set; }
        public string MethodName { get; set; }
        public List<string> ConnectionIds { get; set; }
    }

    internal class SendAdminMessageClass
    {
        public SendAdminMessageClass()
        {
        }
        public BaccaratGameType GameType { get; set; }
        public int ZNum { get; set; }
        public string Message { get; set; }
        public string MerchantID { get; set; }
    }
}
