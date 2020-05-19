using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using MongoDB.Driver;
using Operation.Abutment;
using Operation.Baccarat;
using Operation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 回水模块
    /// </summary>
    public static class BackwaterKind
    {
        /// <summary>
        /// 回水
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="userIDList"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="gameType"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static async Task<bool> GameBackwater(string merchantID, List<string> userIDList, DateTime startTime, DateTime endTime, GameOfType gameType, PatternEnum pattern)
        {
            UserOperation userOperation = new UserOperation();
            BackwaterSetupOperation backwaterSetupOperation = new BackwaterSetupOperation();
            List<Backwaterencaps> msgList = new List<Backwaterencaps>();
            foreach (var userID in userIDList)
            {
                //查询回水设置
                var user = await userOperation.GetModelAsync(t => t._id == userID
                && t.MerchantID == merchantID && !string.IsNullOrEmpty(t.ProgrammeID));
                if (user == null)
                    continue;
                var setup = await backwaterSetupOperation.GetModelAsync(t => t._id == user.ProgrammeID && t.MerchantID == merchantID && t.Pattern == pattern);
                if (setup == null) continue;
                if (setup.GameType != null && setup.GameType.Value != gameType)
                    continue;
                var data = await BackwaterSetup(merchantID, userID, startTime, endTime, gameType, pattern, setup);
                if (!string.IsNullOrEmpty(data.UserID))
                    msgList.Add(data);
            }
            #region 是否有回水数据
            if (!msgList.IsNull())
            {
                var roomInfo = await Utils.GetRoomInfosAsync(merchantID, gameType);
                if (roomInfo == null) return false;
                ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
                var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
                msgList = (from data in msgList
                           group data by
                           new
                           {
                               data.UserID
                           } into grp
                           select new Backwaterencaps
                           {
                               UserID = grp.Key.UserID,
                               Account = grp.Sum(t => t.Account),
                               Money = grp.Sum(t => t.Money)
                           }).ToList();
                //是否开启回水通知
                if (roomInfo.Back)
                {
                    foreach (var data in msgList)
                    {
                        var user = await userOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == data.UserID);
                        string message = reply.Backwater.Replace("{可返流水}", data.Account.ToString("#0.00"))
                            .Replace("{返水金额}", data.Money.ToString("#0.00")).Replace("{剩余}", user.UserMoney.ToString("#0.00"))
                            .Replace("{昵称}", user.NickName);
                        //await SignalRSendMessage.SendAdminMessage(message, merchantID, gameType, hub);
                        await RabbitMQHelper.SendAdminMessage(message, merchantID, gameType);
                    }
                }
                return true;
            }
            #endregion
            #region
            //foreach (var user in userList)
            //{
            //    var userBet = await userBetInfoOperation.GetModelListAsync(t => t.MerchantID == merchantID && (t.BetStatus == BetStatusEnum.已中奖 || t.BetStatus == BetStatusEnum.未中奖)
            //        && t.UserID == user._id && t.GameType == gameType && t.Notes == NotesEnum.正常 && t.CreatedTime >= searchTime.AddHours(6) && t.CreatedTime <= searchTime.AddDays(1));
            //    if (userBet.IsNull()) continue;
            //    foreach (var item in dic)
            //    {
            //        var pattern = GameBetsMessage.GetEnumByStatus<PatternEnum>(item.Value);
            //        var flowBetList = userBet.FindAll(t => t.IsBackwater == false);
            //        if (pattern == PatternEnum.流水模式 && flowBetList.IsNull())
            //            continue;
            //        //流水
            //        var flowPattern = backwaterList.Find(t => t. == user.Level && t.Pattern == pattern);
            //        BackwaterJournal flowModel = await backwaterJournalOperation.GetModelAsync(t => t.AddDataTime == searchTime
            //        && t.UserID == user._id && t.Pattern == pattern && t.MerchantID == merchantID && t.GameType == gameType && t.BackStatus == BackStatusEnum.已回水);

            //        bool flag = false;
            //        decimal inputAmount = pattern == PatternEnum.流水模式 ? flowBetList.Sum(t => t.BetMoney) : userBet.Sum(t => t.BetMoney);
            //        decimal proLoss = pattern == PatternEnum.流水模式 ? flowBetList.Sum(t => t.MediumBonus) - flowBetList.Sum(t => t.BetMoney)
            //            : userBet.Sum(t => t.MediumBonus) - userBet.Sum(t => t.BetMoney);
            //        //输赢模式
            //        if (pattern == PatternEnum.输赢模式 && flowModel != null)
            //            continue;
            //        if (flowModel == null)
            //        {
            //            flowModel = new BackwaterJournal()
            //            {
            //                Ascent = 0,
            //                MerchantID = merchantID,
            //                InputAmount = inputAmount,
            //                UserID = user._id,
            //                BackStatus = BackStatusEnum.未回水,
            //                GameType = gameType,
            //                Pattern = pattern,
            //                ProLoss = proLoss,
            //                UserStatus = user.Status,
            //                AddDataTime = searchTime
            //            };
            //        }
            //        else
            //        {
            //            flag = true;
            //            flowModel.BackStatus = BackStatusEnum.未回水;
            //        }
            //        //新添加数据
            //        if (flowModel.BackStatus == BackStatusEnum.未回水)
            //        {
            //            if (flowPattern == null)
            //            {
            //                flowModel.BackStatus = BackStatusEnum.未回水;
            //                continue;
            //            }
            //            else
            //            {
            //                if (pattern == PatternEnum.流水模式)
            //                {
            //                    //回水
            //                    if (flowPattern.Minrecord <= inputAmount && flowPattern.Maxrecord >= inputAmount)
            //                    {
            //                        //回水金额
            //                        var amount = inputAmount * flowPattern.Odds / 100;
            //                        amount = Math.Round(amount, 2);
            //                        flowModel.Ascent += amount;

            //                        //添加用户金额和日志
            //                        await userOperation.UpperScore(user._id, merchantID, amount, ChangeTargetEnum.回水, "系统回水金额" + amount,
            //                            "系统回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, gameType: gameType);
            //                        flowModel.BackStatus = BackStatusEnum.已回水;
            //                        msgList.Add(new Backwaterencaps()
            //                        {
            //                            UserID = user._id,
            //                            Account = inputAmount,
            //                            Money = amount
            //                        });
            //                        //回水数据
            //                        foreach (var bet in userBet)
            //                        {
            //                            bet.IsBackwater = true;
            //                            await userBetInfoOperation.UpdateModelAsync(bet);
            //                        }
            //                    }
            //                    else
            //                        continue;
            //                }
            //                else
            //                {
            //                    //赢则不回水
            //                    if (flowModel.ProLoss > 0)
            //                    {
            //                        flowModel.Ascent = 0;
            //                        flowModel.BackStatus = BackStatusEnum.未回水;
            //                    }
            //                    else if (flowPattern.Minrecord <= -proLoss && flowPattern.Maxrecord >= -proLoss)
            //                    {
            //                        //回水金额
            //                        var amount = -flowModel.ProLoss * flowPattern.Odds / 100;
            //                        amount = Math.Round(amount, 2);
            //                        flowModel.Ascent += amount;

            //                        //添加用户金额和日志
            //                        await userOperation.UpperScore(user._id, merchantID, amount, ChangeTargetEnum.回水, "系统回水金额" + amount,
            //                            "系统回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, gameType: gameType);
            //                        flowModel.BackStatus = BackStatusEnum.已回水;

            //                        msgList.Add(new Backwaterencaps()
            //                        {
            //                            UserID = user._id,
            //                            Account = inputAmount,
            //                            Money = amount
            //                        });
            //                    }
            //                    else
            //                        continue;
            //                }
            //            }
            //            if (flag)
            //            {
            //                flowModel.InputAmount += inputAmount;
            //                flowModel.ProLoss += proLoss;
            //                await backwaterJournalOperation.UpdateModelAsync(flowModel);
            //            }
            //            else
            //                await backwaterJournalOperation.InsertModelAsync(flowModel);
            //        }
            //    }
            //    RabbitMQHelper.SendUserPointChange(user._id, merchantID);
            //    //await SignalRSendMessage.SendUserPointChange(user._id, merchantID, hub);
            //}
            //if (!msgList.IsNull())
            //{
            //    var room = new RoomOperation(baseMongo).GetModel(t => t.MerchantID == merchantID);
            //    var collName = gameType == GameOfType.北京赛车 ? "RoomGameOfRacing" :
            //            gameType == GameOfType.幸运飞艇 ? "RoomGameOfAirship" :
            //            gameType == GameOfType.极速赛车 ? "RoomGameOfExtremeSpeed" :
            //            gameType == GameOfType.澳州10 ? "RoomGameOfAus10" :
            //            gameType == GameOfType.澳州5 ? "RoomGameOfAus5" : "RoomGameOfTimeHonored";
            //    BsonOperation bsonOperation = new BsonOperation(collName);
            //    var bsonData = bsonOperation.Collection.Find(bsonOperation.Builder.Eq("RoomID", room._id)).FirstOrDefault();
            //    if (bsonData == null) return true;
            //    ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            //    var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            //    msgList = (from data in msgList
            //               group data by
            //               new
            //               {
            //                   data.UserID
            //               } into grp
            //               select new Backwaterencaps
            //               {
            //                   UserID = grp.Key.UserID,
            //                   Account = grp.Sum(t => t.Account),
            //                   Money = grp.Sum(t => t.Money)
            //               }).ToList();
            //    //是否开启回水通知
            //    if (bsonData["Che"].AsBoolean)
            //    {
            //        foreach (var data in msgList)
            //        {
            //            var user = await userOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == data.UserID);
            //            string message = reply.Backwater.Replace("{可返流水}", data.Account.ToString("#0.00"))
            //                .Replace("{返水金额}", data.Money.ToString("#0.00")).Replace("{剩余}", user.UserMoney.ToString("#0.00"))
            //                .Replace("{昵称}", user.NickName);
            //            //await SignalRSendMessage.SendAdminMessage(message, merchantID, gameType, hub);
            //            await RabbitMQHelper.SendAdminMessage(message, merchantID, gameType);
            //        }
            //    }
            //    return true;
            //}
            #endregion
            return false;
        }

        /// <summary>
        /// 回水
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="userIDList"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="gameType"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static async Task<bool> VideoGameBackwater(string merchantID, List<string> userIDList, DateTime startTime, DateTime endTime, BaccaratGameType gameType, PatternEnum pattern)
        {
            UserOperation userOperation = new UserOperation();
            VideoBackwaterSetupOperation backwaterSetupOperation = new VideoBackwaterSetupOperation();
            List<Backwaterencaps> msgList = new List<Backwaterencaps>();
            foreach (var userID in userIDList)
            {
                //查询回水设置
                var user = await userOperation.GetModelAsync(t => t._id == userID
                && t.MerchantID == merchantID && !string.IsNullOrEmpty(t.VideoProgrammeID));
                if (user == null)
                    continue;
                var setup = await backwaterSetupOperation.GetModelAsync(t => t._id == user.VideoProgrammeID && t.MerchantID == merchantID && t.Pattern == pattern);
                if (setup == null) continue;
                if (setup.GameType != null && setup.GameType.Value != gameType)
                    continue;
                var data = await VideoBackwaterSetup(merchantID, userID, startTime, endTime, gameType, pattern, setup);
                if (!string.IsNullOrEmpty(data.UserID))
                    msgList.Add(data);
            }
            #region 是否有回水数据
            if (!msgList.IsNull())
            {
                var roomInfo = await Utils.GetVideoRoomInfosAsync(merchantID, gameType);
                if (roomInfo == null) return false;
                ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
                var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
                msgList = (from data in msgList
                           group data by
                           new
                           {
                               data.UserID
                           } into grp
                           select new Backwaterencaps
                           {
                               UserID = grp.Key.UserID,
                               Account = grp.Sum(t => t.Account),
                               Money = grp.Sum(t => t.Money)
                           }).ToList();
                //是否开启回水通知
                if (roomInfo.Back)
                {
                    foreach (var data in msgList)
                    {
                        var user = await userOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == data.UserID);
                        string message = reply.Backwater.Replace("{可返流水}", data.Account.ToString("#0.00"))
                            .Replace("{返水金额}", data.Money.ToString("#0.00")).Replace("{剩余}", user.UserMoney.ToString("#0.00"))
                            .Replace("{昵称}", user.NickName);
                        await RabbitMQHelper.SendBaccaratAdminMessage(message, merchantID, null, gameType, null, user._id);
                    }
                }
                return true;
            }
            #endregion
            return false;
        }

        /// <summary>
        /// 流水
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="userID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="gameType"></param>
        /// <param name="pattern"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        private static async Task<Backwaterencaps> BackwaterSetup(string merchantID, string userID, DateTime startTime, DateTime endTime, GameOfType gameType, PatternEnum pattern, BackwaterSetup setup)
        {
            var result = new Backwaterencaps();
            UserOperation userOperation = new UserOperation();
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            BackwaterJournalOperation backwaterJournalOperation = new BackwaterJournalOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            //查找用户正常注单
            var userBet = await collection.FindListAsync(t => t.MerchantID == merchantID && t.BetStatus == BetStatus.已开奖
                    && t.UserID == userID && t.GameType == gameType && (t.Notes == NotesEnum.正常 || t.Notes == NotesEnum.虚拟) && t.CreatedTime >= startTime && t.CreatedTime <= endTime
                    && t.CreatedTime >= user.ProgrammeAddTime.Value);
            if (userBet.IsNull()) return result;
            var flowBetList = userBet.FindAll(t => t.IsBackwater == false);
            if (pattern == PatternEnum.流水模式 && flowBetList.IsNull())
                return result;
            #region 正常流水
            if (flowBetList.FindIndex(t => t.Notes == NotesEnum.正常) > -1)
            {
                var trueBetList = flowBetList.FindAll(t => t.Notes == NotesEnum.正常);
                //流水
                BackwaterJournal flowModel = await backwaterJournalOperation.GetModelAsync(t => t.AddDataTime == startTime.ToString("yyyy-MM-dd")
                && t.UserID == userID && t.Pattern == pattern && t.MerchantID == merchantID && t.GameType == gameType && t.BackStatus == BackStatusEnum.已回水 && t.UserStatus == UserStatusEnum.正常);

                bool flag = false;
                decimal inputAmount = pattern == PatternEnum.流水模式 ? trueBetList.Sum(t => t.AllUseMoney) : userBet.Sum(t => t.AllUseMoney);
                decimal proLoss = pattern == PatternEnum.流水模式 ? trueBetList.Sum(t => t.AllMediumBonus) - trueBetList.Sum(t => t.AllUseMoney)
                    : userBet.Sum(t => t.AllMediumBonus) - userBet.Sum(t => t.AllUseMoney);
                //输赢模式
                if (pattern == PatternEnum.输赢模式 && flowModel != null)
                    return result;
                if (flowModel == null)
                {
                    flowModel = new BackwaterJournal()
                    {
                        Ascent = 0,
                        MerchantID = merchantID,
                        InputAmount = inputAmount,
                        UserID = userID,
                        BackStatus = BackStatusEnum.未回水,
                        GameType = gameType,
                        Pattern = pattern,
                        ProLoss = proLoss,
                        UserStatus = UserStatusEnum.正常,
                        AddDataTime = startTime.ToString("yyyy-MM-dd")
                    };
                }
                else
                {
                    flag = true;
                    flowModel.BackStatus = BackStatusEnum.未回水;
                }
                //新添加数据
                if (flowModel.BackStatus == BackStatusEnum.未回水)
                {
                    if (setup == null)
                    {
                        flowModel.BackStatus = BackStatusEnum.未回水;
                        return result;
                    }
                    else
                    {
                        if (pattern == PatternEnum.流水模式)
                        {
                            //回水
                            if (setup.Minrecord <= inputAmount && setup.Maxrecord >= inputAmount)
                            {
                                //回水金额
                                var amount = inputAmount * setup.Odds / 100;
                                amount = Math.Round(amount, 2);
                                flowModel.Ascent += amount;

                                //添加用户金额和日志
                                await userOperation.UpperScore(userID, merchantID, amount, ChangeTargetEnum.回水, "系统回水金额" + amount,
                                    "系统回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, gameType: gameType);
                                flowModel.BackStatus = BackStatusEnum.已回水;
                                result = new Backwaterencaps()
                                {
                                    UserID = userID,
                                    Account = inputAmount,
                                    Money = amount
                                };
                                //回水数据
                                foreach (var bet in userBet)
                                {
                                    bet.IsBackwater = true;
                                    UpdateDefinition<UserBetInfo> update = Builders<UserBetInfo>.Update
                                    .Set(t => t.IsBackwater, true)
                                    .Set(t => t.LastUpdateTime, DateTime.Now);
                                    collection.UpdateOne(t => t._id == bet._id, update, cancellationToken: new CancellationToken());
                                }
                            }
                            else
                                return result;
                        }
                        else
                        {
                            //赢则不回水
                            if (flowModel.ProLoss > 0)
                            {
                                flowModel.Ascent = 0;
                                flowModel.BackStatus = BackStatusEnum.未回水;
                            }
                            else if (setup.Minrecord <= -proLoss && setup.Maxrecord >= -proLoss)
                            {
                                //回水金额
                                var amount = -flowModel.ProLoss * setup.Odds / 100;
                                amount = Math.Round(amount, 2);
                                flowModel.Ascent += amount;

                                //添加用户金额和日志
                                await userOperation.UpperScore(userID, merchantID, amount, ChangeTargetEnum.回水, "系统回水金额" + amount,
                                    "系统回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, gameType: gameType);
                                flowModel.BackStatus = BackStatusEnum.已回水;

                                result = new Backwaterencaps()
                                {
                                    UserID = userID,
                                    Account = inputAmount,
                                    Money = amount
                                };
                            }
                            else
                                return result;
                        }
                    }
                    if (flag)
                    {
                        flowModel.InputAmount += inputAmount;
                        flowModel.ProLoss += proLoss;
                        await backwaterJournalOperation.UpdateModelAsync(flowModel);
                    }
                    else
                        await backwaterJournalOperation.InsertModelAsync(flowModel);

                    await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                }
            }
            #endregion
            #region 虚拟流水
            if (flowBetList.FindIndex(t => t.Notes == NotesEnum.虚拟) > -1)
            {
                var trueBetList = flowBetList.FindAll(t => t.Notes == NotesEnum.虚拟);
                //流水
                BackwaterJournal flowModel = await backwaterJournalOperation.GetModelAsync(t => t.AddDataTime == startTime.ToString("yyyy-MM-dd")
                && t.UserID == userID && t.Pattern == pattern && t.MerchantID == merchantID && t.GameType == gameType && t.BackStatus == BackStatusEnum.已回水 && t.UserStatus == UserStatusEnum.假人);

                bool flag = false;
                decimal inputAmount = pattern == PatternEnum.流水模式 ? trueBetList.Sum(t => t.AllUseMoney) : userBet.Sum(t => t.AllUseMoney);
                decimal proLoss = pattern == PatternEnum.流水模式 ? trueBetList.Sum(t => t.AllMediumBonus) - trueBetList.Sum(t => t.AllUseMoney)
                    : userBet.Sum(t => t.AllMediumBonus) - userBet.Sum(t => t.AllUseMoney);
                //输赢模式
                if (pattern == PatternEnum.输赢模式 && flowModel != null)
                    return result;
                if (flowModel == null)
                {
                    flowModel = new BackwaterJournal()
                    {
                        Ascent = 0,
                        MerchantID = merchantID,
                        InputAmount = inputAmount,
                        UserID = userID,
                        BackStatus = BackStatusEnum.未回水,
                        GameType = gameType,
                        Pattern = pattern,
                        ProLoss = proLoss,
                        UserStatus = UserStatusEnum.假人,
                        AddDataTime = startTime.ToString("yyyy-MM-dd")
                    };
                }
                else
                {
                    flag = true;
                    flowModel.BackStatus = BackStatusEnum.未回水;
                }
                //新添加数据
                if (flowModel.BackStatus == BackStatusEnum.未回水)
                {
                    if (setup == null)
                    {
                        flowModel.BackStatus = BackStatusEnum.未回水;
                        return result;
                    }
                    else
                    {
                        if (pattern == PatternEnum.流水模式)
                        {
                            //回水
                            if (setup.Minrecord <= inputAmount && setup.Maxrecord >= inputAmount)
                            {
                                //回水金额
                                var amount = inputAmount * setup.Odds / 100;
                                amount = Math.Round(amount, 2);
                                flowModel.Ascent += amount;

                                //添加用户金额和日志
                                await userOperation.UpperScore(userID, merchantID, amount, ChangeTargetEnum.回水, "系统回水金额" + amount,
                                    "系统回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, gameType: gameType);
                                flowModel.BackStatus = BackStatusEnum.已回水;
                                result = new Backwaterencaps()
                                {
                                    UserID = userID,
                                    Account = inputAmount,
                                    Money = amount
                                };
                                //回水数据
                                foreach (var bet in userBet)
                                {
                                    bet.IsBackwater = true;
                                    UpdateDefinition<UserBetInfo> update = Builders<UserBetInfo>.Update
                                    .Set(t => t.IsBackwater, true)
                                    .Set(t => t.LastUpdateTime, DateTime.Now);
                                    collection.UpdateOne(t => t._id == bet._id, update, cancellationToken: new CancellationToken());
                                }
                            }
                            else
                                return result;
                        }
                        else
                        {
                            //赢则不回水
                            if (flowModel.ProLoss > 0)
                            {
                                flowModel.Ascent = 0;
                                flowModel.BackStatus = BackStatusEnum.未回水;
                            }
                            else if (setup.Minrecord <= -proLoss && setup.Maxrecord >= -proLoss)
                            {
                                //回水金额
                                var amount = -flowModel.ProLoss * setup.Odds / 100;
                                amount = Math.Round(amount, 2);
                                flowModel.Ascent += amount;

                                //添加用户金额和日志
                                await userOperation.UpperScore(userID, merchantID, amount, ChangeTargetEnum.回水, "系统回水金额" + amount,
                                    "系统回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, gameType: gameType);
                                flowModel.BackStatus = BackStatusEnum.已回水;

                                result = new Backwaterencaps()
                                {
                                    UserID = userID,
                                    Account = inputAmount,
                                    Money = amount
                                };
                            }
                            else
                                return result;
                        }
                    }
                    if (flag)
                    {
                        flowModel.InputAmount += inputAmount;
                        flowModel.ProLoss += proLoss;
                        await backwaterJournalOperation.UpdateModelAsync(flowModel);
                    }
                    else
                        await backwaterJournalOperation.InsertModelAsync(flowModel);

                    await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                }
            }
            #endregion
            return result;
        }

        /// <summary>
        /// 流水
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="userID"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="gameType"></param>
        /// <param name="pattern"></param>
        /// <param name="setup"></param>
        /// <returns></returns>
        private static async Task<Backwaterencaps> VideoBackwaterSetup(string merchantID, string userID, DateTime startTime, DateTime endTime, BaccaratGameType gameType, PatternEnum pattern, VideoBackwaterSetup setup)
        {
            var result = new Backwaterencaps();
            UserOperation userOperation = new UserOperation();
            var address = await Utils.GetAddress(merchantID);
            BaccaratBetOperation userBetInfoOperation = await BetManage.GetBaccaratBetOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            VideoBackwaterJournalOperation backwaterJournalOperation = new VideoBackwaterJournalOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            //查找用户正常注单
            var userBet = await collection.FindListAsync(t => t.MerchantID == merchantID && t.BetStatus == BetStatus.已开奖
                    && t.UserID == userID && t.GameType == gameType && t.CreatedTime >= startTime && t.CreatedTime <= endTime
                    && t.CreatedTime >= user.ProgrammeAddTime.Value);
            if (userBet.IsNull()) return result;
            var flowBetList = userBet.FindAll(t => t.IsBackwater == false);
            if (pattern == PatternEnum.流水模式 && flowBetList.IsNull())
                return result;
            #region 正常流水
            if (flowBetList.FindIndex(t => t.Notes == NotesEnum.正常) > -1)
            {
                var trueBetList = flowBetList.FindAll(t => t.Notes == NotesEnum.正常);
                //流水
                VideoBackwaterJournal flowModel = await backwaterJournalOperation.GetModelAsync(t => t.AddDataTime == startTime.ToString("yyyy-MM-dd")
                && t.UserID == userID && t.Pattern == pattern && t.MerchantID == merchantID && t.GameType == gameType && t.BackStatus == BackStatusEnum.已回水 && t.UserStatus == UserStatusEnum.正常);

                bool flag = false;
                decimal inputAmount = pattern == PatternEnum.流水模式 ? trueBetList.Sum(t => t.AllUseMoney) : userBet.Sum(t => t.AllUseMoney);
                decimal proLoss = pattern == PatternEnum.流水模式 ? trueBetList.Sum(t => t.AllMediumBonus) - trueBetList.Sum(t => t.AllUseMoney)
                    : userBet.Sum(t => t.AllMediumBonus) - userBet.Sum(t => t.AllUseMoney);
                //输赢模式
                if (pattern == PatternEnum.输赢模式 && flowModel != null)
                    return result;
                if (flowModel == null)
                {
                    flowModel = new VideoBackwaterJournal()
                    {
                        Ascent = 0,
                        MerchantID = merchantID,
                        InputAmount = inputAmount,
                        UserID = userID,
                        BackStatus = BackStatusEnum.未回水,
                        GameType = gameType,
                        Pattern = pattern,
                        ProLoss = proLoss,
                        UserStatus = UserStatusEnum.正常,
                        AddDataTime = startTime.ToString("yyyy-MM-dd")
                    };
                }
                else
                {
                    flag = true;
                    flowModel.BackStatus = BackStatusEnum.未回水;
                }
                //新添加数据
                if (flowModel.BackStatus == BackStatusEnum.未回水)
                {
                    if (setup == null)
                    {
                        flowModel.BackStatus = BackStatusEnum.未回水;
                        return result;
                    }
                    else
                    {
                        if (pattern == PatternEnum.流水模式)
                        {
                            //回水
                            if (setup.Minrecord <= inputAmount && setup.Maxrecord >= inputAmount)
                            {
                                //回水金额
                                var amount = inputAmount * setup.Odds / 100;
                                amount = Math.Round(amount, 2);
                                flowModel.Ascent += amount;

                                //添加用户金额和日志
                                await userOperation.UpperScore(userID, merchantID, amount, ChangeTargetEnum.回水, "系统回水金额" + amount,
                                    "系统回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, videoGameType: gameType);
                                flowModel.BackStatus = BackStatusEnum.已回水;
                                result = new Backwaterencaps()
                                {
                                    UserID = userID,
                                    Account = inputAmount,
                                    Money = amount
                                };
                                //回水数据
                                foreach (var bet in userBet)
                                {
                                    bet.IsBackwater = true;
                                    UpdateDefinition<BaccaratBet> update = Builders<BaccaratBet>.Update
                                    .Set(t => t.IsBackwater, true)
                                    .Set(t => t.LastUpdateTime, DateTime.Now);
                                    collection.UpdateOne(t => t._id == bet._id, update, cancellationToken: new CancellationToken());
                                }
                            }
                            else
                                return result;
                        }
                        else
                        {
                            //赢则不回水
                            if (flowModel.ProLoss > 0)
                            {
                                flowModel.Ascent = 0;
                                flowModel.BackStatus = BackStatusEnum.未回水;
                            }
                            else if (setup.Minrecord <= -proLoss && setup.Maxrecord >= -proLoss)
                            {
                                //回水金额
                                var amount = -flowModel.ProLoss * setup.Odds / 100;
                                amount = Math.Round(amount, 2);
                                flowModel.Ascent += amount;

                                //添加用户金额和日志
                                await userOperation.UpperScore(userID, merchantID, amount, ChangeTargetEnum.回水, "系统回水金额" + amount,
                                    "系统回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, videoGameType: gameType);
                                flowModel.BackStatus = BackStatusEnum.已回水;

                                result = new Backwaterencaps()
                                {
                                    UserID = userID,
                                    Account = inputAmount,
                                    Money = amount
                                };
                            }
                            else
                                return result;
                        }
                    }
                    if (flag)
                    {
                        flowModel.InputAmount += inputAmount;
                        flowModel.ProLoss += proLoss;
                        await backwaterJournalOperation.UpdateModelAsync(flowModel);
                    }
                    else
                        await backwaterJournalOperation.InsertModelAsync(flowModel);

                    await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                }
            }
            #endregion
            #region 虚拟流水
            if (flowBetList.FindIndex(t => t.Notes == NotesEnum.虚拟) > -1)
            {
                var trueBetList = flowBetList.FindAll(t => t.Notes == NotesEnum.虚拟);
                //流水
                var flowModel = await backwaterJournalOperation.GetModelAsync(t => t.AddDataTime == startTime.ToString("yyyy-MM-dd")
                && t.UserID == userID && t.Pattern == pattern && t.MerchantID == merchantID && t.GameType == gameType && t.BackStatus == BackStatusEnum.已回水 && t.UserStatus == UserStatusEnum.假人);

                bool flag = false;
                decimal inputAmount = pattern == PatternEnum.流水模式 ? trueBetList.Sum(t => t.AllUseMoney) : userBet.Sum(t => t.AllUseMoney);
                decimal proLoss = pattern == PatternEnum.流水模式 ? trueBetList.Sum(t => t.AllMediumBonus) - trueBetList.Sum(t => t.AllUseMoney)
                    : userBet.Sum(t => t.AllMediumBonus) - userBet.Sum(t => t.AllUseMoney);
                //输赢模式
                if (pattern == PatternEnum.输赢模式 && flowModel != null)
                    return result;
                if (flowModel == null)
                {
                    flowModel = new VideoBackwaterJournal()
                    {
                        Ascent = 0,
                        MerchantID = merchantID,
                        InputAmount = inputAmount,
                        UserID = userID,
                        BackStatus = BackStatusEnum.未回水,
                        GameType = gameType,
                        Pattern = pattern,
                        ProLoss = proLoss,
                        UserStatus = UserStatusEnum.假人,
                        AddDataTime = startTime.ToString("yyyy-MM-dd")
                    };
                }
                else
                {
                    flag = true;
                    flowModel.BackStatus = BackStatusEnum.未回水;
                }
                //新添加数据
                if (flowModel.BackStatus == BackStatusEnum.未回水)
                {
                    if (setup == null)
                    {
                        flowModel.BackStatus = BackStatusEnum.未回水;
                        return result;
                    }
                    else
                    {
                        if (pattern == PatternEnum.流水模式)
                        {
                            //回水
                            if (setup.Minrecord <= inputAmount && setup.Maxrecord >= inputAmount)
                            {
                                //回水金额
                                var amount = inputAmount * setup.Odds / 100;
                                amount = Math.Round(amount, 2);
                                flowModel.Ascent += amount;

                                //添加用户金额和日志
                                await userOperation.UpperScore(userID, merchantID, amount, ChangeTargetEnum.回水, "系统回水金额" + amount,
                                    "系统回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, videoGameType: gameType);
                                flowModel.BackStatus = BackStatusEnum.已回水;
                                result = new Backwaterencaps()
                                {
                                    UserID = userID,
                                    Account = inputAmount,
                                    Money = amount
                                };
                                //回水数据
                                foreach (var bet in userBet)
                                {
                                    bet.IsBackwater = true;
                                    UpdateDefinition<BaccaratBet> update = Builders<BaccaratBet>.Update
                                    .Set(t => t.IsBackwater, true)
                                    .Set(t => t.LastUpdateTime, DateTime.Now);
                                    collection.UpdateOne(t => t._id == bet._id, update, cancellationToken: new CancellationToken());
                                }
                            }
                            else
                                return result;
                        }
                        else
                        {
                            //赢则不回水
                            if (flowModel.ProLoss > 0)
                            {
                                flowModel.Ascent = 0;
                                flowModel.BackStatus = BackStatusEnum.未回水;
                            }
                            else if (setup.Minrecord <= -proLoss && setup.Maxrecord >= -proLoss)
                            {
                                //回水金额
                                var amount = -flowModel.ProLoss * setup.Odds / 100;
                                amount = Math.Round(amount, 2);
                                flowModel.Ascent += amount;

                                //添加用户金额和日志
                                await userOperation.UpperScore(userID, merchantID, amount, ChangeTargetEnum.回水, "系统回水金额" + amount,
                                    "系统回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, videoGameType: gameType);
                                flowModel.BackStatus = BackStatusEnum.已回水;

                                result = new Backwaterencaps()
                                {
                                    UserID = userID,
                                    Account = inputAmount,
                                    Money = amount
                                };
                            }
                            else
                                return result;
                        }
                    }
                    if (flag)
                    {
                        flowModel.InputAmount += inputAmount;
                        flowModel.ProLoss += proLoss;
                        await backwaterJournalOperation.UpdateModelAsync(flowModel);
                    }
                    else
                        await backwaterJournalOperation.InsertModelAsync(flowModel);

                    await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                }
            }
            #endregion
            return result;
        }

        /// <summary>
        /// 回水信息
        /// </summary>
        public class Backwaterencaps
        {
            /// <summary>
            /// 用户id
            /// </summary>
            public string UserID { get; set; }

            /// <summary>
            /// 使用金额
            /// </summary>
            public decimal Account { get; set; }

            /// <summary>
            /// 剩余
            /// </summary>
            public decimal Money { get; set; }
        }

        /// <summary>
        /// 代理回水
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="time"></param>
        /// <param name="gameType"></param>
        /// <param name="vgameType"></param>
        /// <returns></returns>
        public static async Task<bool> AgentBackwater(string merchantID, DateTime time, GameOfType? gameType, BaccaratGameType? vgameType)
        {
            var result = false;
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            UserOperation userOperation = new UserOperation();
            UserBackwaterJournalOperation userBackwaterJournalOperation = new UserBackwaterJournalOperation();
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
            var vcollection = baccaratBetOperation.GetCollection(merchantID);
            var userList = await userOperation.GetModelListAsync(t => t.IsAgent && t.Status ==
             UserStatusEnum.正常 && t.MerchantID == merchantID);
            var agentList = await agentBackwaterOperation.GetModelListAsync(t => t.MerchantID == merchantID);
            foreach (var user in userList)
            {
                var agent = await agentBackwaterOperation.GetModelAsync(t => t.MerchantID == merchantID && t.UserID == user._id);
                if (agent == null) continue;
                if (agent.Offline.IsNull()) continue;
                var offline = agent.Offline.FindAll(t => t.AddTime <= time.AddDays(1));
                foreach (var info in offline)
                {
                    var userID = info.UserID;
                    var offUser = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID && t.Status == UserStatusEnum.正常);
                    if (offUser == null) continue;
                    var model = await userBackwaterJournalOperation.GetModelAsync(t => t.MerchantID == merchantID && t.AgentUserID == agent.UserID && t.AddDataTime == time && t.UserID == userID && t.GameType == gameType && t.VGameType == vgameType);
                    decimal odd = 0;
                    if (gameType != null)
                    {
                        switch (gameType)
                        {
                            case GameOfType.北京赛车:
                                odd = agent.Pk10;
                                break;
                            case GameOfType.幸运飞艇:
                                odd = agent.Xyft;
                                break;
                            case GameOfType.极速赛车:
                                odd = agent.Jssc;
                                break;
                            case GameOfType.澳州10:
                                odd = agent.Azxy10;
                                break;
                            case GameOfType.澳州5:
                                odd = agent.Azxy5;
                                break;
                            case GameOfType.重庆时时彩:
                                odd = agent.Cqssc;
                                break;
                            case GameOfType.爱尔兰赛马:
                                odd = agent.Ireland10;
                                break;
                            case GameOfType.爱尔兰快5:
                                odd = agent.Ireland5;
                                break;
                            case GameOfType.幸运飞艇168:
                                odd = agent.XYft168;
                                break;
                            case GameOfType.极速时时彩:
                                odd = agent.Jsssc;
                                break;
                        }
                    }
                    else if (vgameType != null)
                    {
                        switch (vgameType)
                        {
                            case BaccaratGameType.百家乐:
                                odd = agent.Baccarat;
                                break;
                        }
                    }
                    if (model == null)
                    {
                        model = new UserBackwaterJournal()
                        {
                            UserID = userID,
                            AddDataTime = time,
                            AgentUserID = agent.UserID,
                            MerchantID = merchantID,
                            Ascent = 0,
                            BackStatus = BackStatusEnum.未回水,
                            GameType = gameType,
                            InputAmount = 0,
                            VGameType = vgameType
                        };
                    }
                    else
                        continue;
                    if (model.BackStatus == BackStatusEnum.未回水)
                    {
                        if (gameType != null)
                        {
                            var userBet = await collection.FindListAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.GameType == gameType
               && t.CreatedTime >= info.AddTime && t.CreatedTime >= time.AddHours(6) && t.CreatedTime < time.AddDays(1).AddHours(6) && t.BetStatus == BetStatus.已开奖);
                            if (userBet.IsNull()) continue;
                            model.InputAmount = userBet.Sum(t => t.AllUseMoney);
                        }
                        else if (vgameType != null)
                        {
                            var userBet = await vcollection.FindListAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.GameType == vgameType
              && t.CreatedTime >= info.AddTime && t.CreatedTime >= time.AddHours(6) && t.CreatedTime < time.AddDays(1).AddHours(6) && t.BetStatus == BetStatus.已开奖);
                            if (userBet.IsNull()) continue;
                            model.InputAmount = userBet.Sum(t => t.AllUseMoney);
                        }

                        //回水金额
                        var amount = model.InputAmount * odd / 100;
                        amount = Math.Round(amount, 2);
                        model.Ascent = amount;
                        model.BackStatus = BackStatusEnum.已回水;
                        //添加用户金额和日志
                        await userOperation.UpperScore(agent.UserID, merchantID, amount, ChangeTargetEnum.回水, "代理回水金额" + amount,
                            "代理回水金额" + amount, orderStatus: OrderStatusEnum.上分成功, gameType: gameType, videoGameType: vgameType);
                    }
                    await userBackwaterJournalOperation.InsertModelAsync(model);
                    result = true;
                }
                await RabbitMQHelper.SendUserPointChange(user._id, merchantID);
            }
            return result;
        }

        /// <summary>
        /// 用户回水
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户号</param>
        /// <returns></returns>
        public static async Task<RecoverModel> UserBackwaterAsync(string userID, GameOfType gameType, string merchantID)
        {
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            //ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            //var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);

            //获取对应房间设置
            var roomInfo = await Utils.GetRoomInfosAsync(merchantID, gameType);
            if (roomInfo == null) return new RecoverModel(RecoverEnum.失败, "未查询到房间信息！");
            if (roomInfo.SwitchBackwater)
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
                var userNotBackList = await collection.FindListAsync(t => t.MerchantID == merchantID
                && t.UserID == userID && t.GameType == gameType && t.BetStatus == BetStatus.已开奖 && t.CreatedTime >= startTime && t.CreatedTime <= endTime);
                if (userNotBackList.IsNull())
                    return new RecoverModel(RecoverEnum.失败, "暂无可返水流水");

                if (string.IsNullOrEmpty(user.ProgrammeID))
                    return new RecoverModel(RecoverEnum.失败, "未设置返水方案");
                //查询设置回水方案
                BackwaterSetupOperation backwaterSetupOperation = new BackwaterSetupOperation();
                var setup = await backwaterSetupOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == user.ProgrammeID);
                if (setup == null)
                    return new RecoverModel(RecoverEnum.失败, "未查询到返水方案");

                if (setup.Pattern == PatternEnum.输赢模式)
                    return new RecoverModel(RecoverEnum.失败, "您的账户不支持自助返水");
                var result = await GameBackwater(merchantID, new List<string>() { user._id },
                    startTime, endTime, gameType, setup.Pattern);
                if (result)
                {
                    await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                    return new RecoverModel(RecoverEnum.失败, "返水成功");
                }
                else
                    return new RecoverModel(RecoverEnum.失败, "暂无可返水流水");
            }
            else
                return new RecoverModel(RecoverEnum.失败, "当前房间未启用该功能！");
        }

        /// <summary>
        /// 用户回水
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户号</param>
        /// <returns></returns>
        public static async Task<RecoverModel> UserVideoBackwaterAsync(string userID, BaccaratGameType gameType, string merchantID)
        {
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            //ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            //var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            var address = await Utils.GetAddress(merchantID);
            BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
            var collection = baccaratBetOperation.GetCollection(merchantID);

            //获取对应房间设置
            var roomInfo = await Utils.GetVideoRoomInfosAsync(merchantID, gameType);
            if (roomInfo == null) return new RecoverModel(RecoverEnum.失败, "未查询到房间信息！");
            if (roomInfo.SwitchBackwater)
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
                var userNotBackList = await collection.FindListAsync(t => t.MerchantID == merchantID
                && t.UserID == userID && t.GameType == gameType && t.BetStatus == BetStatus.已开奖 && t.CreatedTime >= startTime && t.CreatedTime <= endTime);
                if (userNotBackList.IsNull())
                    return new RecoverModel(RecoverEnum.失败, "暂无可返水流水");

                if (string.IsNullOrEmpty(user.VideoProgrammeID))
                    return new RecoverModel(RecoverEnum.失败, "未设置返水方案");
                //查询设置返水方案
                VideoBackwaterSetupOperation videoBackwaterSetupOperation = new VideoBackwaterSetupOperation();
                var setup = await videoBackwaterSetupOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == user.VideoProgrammeID);
                if (setup == null)
                    return new RecoverModel(RecoverEnum.失败, "未查询到返水方案");

                if (setup.Pattern == PatternEnum.输赢模式)
                    return new RecoverModel(RecoverEnum.失败, "您的账户不支持自助返水");
                var result = await VideoGameBackwater(merchantID, new List<string>() { user._id },
                    startTime, endTime, gameType, setup.Pattern);
                if (result)
                {
                    await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                    return new RecoverModel(RecoverEnum.失败, "返水成功");
                }
                else
                    return new RecoverModel(RecoverEnum.失败, "暂无可返水流水");
            }
            else
                return new RecoverModel(RecoverEnum.失败, "当前房间未启用该功能！");
        }

        /// <summary>
        /// 获取当天用户下注信息
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="agent">代理id</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public static async Task<List<WebAgentReport>> SearchUserBet(string merchantID, AgentBackwater agent, DateTime time)
        {
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
            var vcollection = baccaratBetOperation.GetCollection(merchantID);
            UserBackwaterJournalOperation userBackwaterJournalOperation = new UserBackwaterJournalOperation();
            UserOperation userOperation = new UserOperation();
            var result = new List<WebAgentReport>();
            var offline = agent.Offline.FindAll(t => t.AddTime <= time.AddDays(1));
            foreach (var info in offline)
            {
                var userID = info.UserID;
                var user = await userOperation.GetModelAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.正常
                && t._id == userID);
                if (user == null) continue;
                //查询是否已有回水数据
                var journal = await userBackwaterJournalOperation.GetModelListAsync(t => t.MerchantID == merchantID
                 && t.UserID == userID && t.AgentUserID == agent.UserID && t.AddDataTime == time);
                if (journal.IsNull())
                {
                    var userBet = await collection.FindListAsync(t => t.MerchantID == merchantID && t.UserID == userID
               && t.CreatedTime >= info.AddTime && t.CreatedTime >= agent.AddTime && t.CreatedTime >= time.AddHours(6) && t.CreatedTime < time.AddDays(1).AddHours(6) && t.BetStatus == BetStatus.已开奖);
                    var vuserBet = await vcollection.FindListAsync(t => t.MerchantID == merchantID && t.UserID == userID
&& t.CreatedTime >= info.AddTime && t.CreatedTime >= agent.AddTime && t.CreatedTime >= time.AddHours(6) && t.CreatedTime < time.AddDays(1).AddHours(6) && t.BetStatus == BetStatus.已开奖);
                    var pk10 = userBet.FindAll(t => t.GameType == GameOfType.北京赛车).Sum(t => t.AllUseMoney);
                    var xyft = userBet.FindAll(t => t.GameType == GameOfType.幸运飞艇).Sum(t => t.AllUseMoney);
                    var jssc = userBet.FindAll(t => t.GameType == GameOfType.极速赛车).Sum(t => t.AllUseMoney);
                    var cqssc = userBet.FindAll(t => t.GameType == GameOfType.重庆时时彩).Sum(t => t.AllUseMoney);
                    var azxy10 = userBet.FindAll(t => t.GameType == GameOfType.澳州10).Sum(t => t.AllUseMoney);
                    var azxy5 = userBet.FindAll(t => t.GameType == GameOfType.澳州5).Sum(t => t.AllUseMoney);
                    var ireland10 = userBet.FindAll(t => t.GameType == GameOfType.爱尔兰赛马).Sum(t => t.AllUseMoney);
                    var ireland5 = userBet.FindAll(t => t.GameType == GameOfType.爱尔兰快5).Sum(t => t.AllUseMoney);
                    var xyft168 = userBet.FindAll(t => t.GameType == GameOfType.幸运飞艇168).Sum(t => t.AllUseMoney);
                    var jsssc = userBet.FindAll(t => t.GameType == GameOfType.极速时时彩).Sum(t => t.AllUseMoney);
                    var baccarat = vuserBet.FindAll(t => t.GameType == BaccaratGameType.百家乐).Sum(t => t.AllUseMoney);
                    var data = new WebAgentReport
                    {
                        AgentID = agent.UserID,
                        NickName = user.NickName,
                        OnlyCode = user.OnlyCode,
                        Pk10 = pk10,
                        Xyft = xyft,
                        Jssc = jssc,
                        Cqssc = cqssc,
                        Azxy10 = azxy10,
                        Azxy5 = azxy5,
                        Ireland10 = ireland10,
                        Ireland5 = ireland5,
                        Xyft168 = xyft168,
                        Jsssc = jsssc,
                        Baccarat = baccarat,
                        Ascent = 0,
                        BackStatus = BackStatusEnum.未回水,
                        Time = time.ToString("yyyy-MM-dd")
                    };
                    if (pk10 + xyft + jssc + cqssc + azxy10 + azxy5 + ireland10 + ireland5 + xyft168 + baccarat + jsssc > 0)
                        result.Add(data);
                }
                else
                {
                    var data = new WebAgentReport
                    {
                        AgentID = agent.UserID,
                        NickName = user.NickName,
                        OnlyCode = user.OnlyCode,
                        Pk10 = journal.FindAll(t => t.GameType == GameOfType.北京赛车).Sum(t => t.InputAmount),
                        Xyft = journal.FindAll(t => t.GameType == GameOfType.幸运飞艇).Sum(t => t.InputAmount),
                        Jssc = journal.FindAll(t => t.GameType == GameOfType.极速赛车).Sum(t => t.InputAmount),
                        Cqssc = journal.FindAll(t => t.GameType == GameOfType.重庆时时彩).Sum(t => t.InputAmount),
                        Azxy10 = journal.FindAll(t => t.GameType == GameOfType.澳州10).Sum(t => t.InputAmount),
                        Azxy5 = journal.FindAll(t => t.GameType == GameOfType.澳州5).Sum(t => t.InputAmount),
                        Ireland10 = journal.FindAll(t => t.GameType == GameOfType.爱尔兰赛马).Sum(t => t.InputAmount),
                        Ireland5 = journal.FindAll(t => t.GameType == GameOfType.爱尔兰快5).Sum(t => t.InputAmount),
                        Xyft168 = journal.FindAll(t => t.GameType == GameOfType.幸运飞艇168).Sum(t => t.InputAmount),
                        Jsssc = journal.FindAll(t => t.GameType == GameOfType.极速时时彩).Sum(t => t.InputAmount),
                        Ascent = journal.Sum(t => t.Ascent),
                        BackStatus = BackStatusEnum.已回水,
                        Time = time.ToString("yyyy-MM-dd")
                    };
                    result.Add(data);
                }
            }
            return result;
        }
    }
}
