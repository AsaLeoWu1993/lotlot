using Entity;
using Entity.WebModel;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageSystem.Manipulate
{
    /// <summary>
    /// 飞单
    /// </summary>
    public class FlyingSheet
    {
        /// <summary>
        /// 飞单
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        /// <param name="gameType">游戏高达</param>
        /// <returns></returns>
        public static async Task<SendFlying> FlyingSheetMethod(string merchantID, string nper, GameOfType gameType)
        {
            //房间设置
            var roomSetup = await Utils.GetRoomInfosAsync(merchantID, gameType);
            if (roomSetup.LotteryRecord == RecordType.不飞单) return null;
            if (roomSetup.LotteryRecord == RecordType.飞单到外部网盘)
            {
                SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
                //var result = await sendFlyingOperation.GetModelAsync(t => t.MerchantID == merchantID
                //&& t.IssueCode == nper && t.game == Enum.GetName(typeof(GameOfType), gameType)
                //&& t.Status == SendFlyEnum.预发送);
                var key = merchantID + Enum.GetName(typeof(GameOfType), gameType);
                var datas = RedisOperation.GetValues(key);
                if (datas == null || datas.Count == 0) return null;
                var result = new SendFlying()
                {
                    MerchantID = merchantID,
                    game = Enum.GetName(typeof(GameOfType), gameType),
                    IssueCode = nper,
                    Status = SendFlyEnum.等待发送,
                    uuid = Guid.NewGuid().ToString().Replace("-", "")
                };
                foreach (var data in datas)
                {
                    result.orders.AddRange(JsonConvert.DeserializeObject<List<FlyingBet>>(data.Value));
                }
                //删除记录
                RedisOperation.DeleteKey(key);
                await sendFlyingOperation.InsertModelAsync(result);
                return result;
            }
            else
            {
                var address = await Utils.GetAddress(merchantID);
                UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
                var collection = userBetInfoOperation.GetCollection(merchantID);
                var list = await collection.FindListAsync(t => t.MerchantID == merchantID && t.GameType == gameType && t.Nper == nper
                && t.BetStatus == BetStatus.未开奖 && t.SendFly == true);
                //查看飞单是否开启
                MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
                var setup = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);
                if (setup == null || setup.OpenSheet == false) return null;
                //查看是否登录
                var dicInfo = await Utils.GetMerchantFlySheetInfo(merchantID);
                if (dicInfo == null) return null;
                //飞单到外部商户
                foreach (var data in list)
                {
                    foreach (var remark in data.BetRemarks)
                    {
                        //发送数据
                        await MerchantInternalSheet(merchantID, dicInfo["TargetID"].ToString(), data.UserID, dicInfo["UserID"].ToString(), gameType, remark.Remark, data.Nper);
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// 发送并删除与这期不相当数据
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="issueNum">期号</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static async Task SendAndDeleteBeoverdue(string merchantID, string issueNum, GameOfType gameType)
        {
            SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
            var typeName = Enum.GetName(typeof(GameOfType), (int)gameType);
            var result = await sendFlyingOperation.GetModelAsync(t => t.MerchantID == merchantID
            && t.game == typeName && t.IssueCode == issueNum);
            await RabbitMQHelper.SendFlyingSheet(merchantID, gameType, issueNum, result);
            //if (result != null)
            //{
            //    var conn = await Utils.GetFlySheet(merchantID);
            //    if (!string.IsNullOrEmpty(conn))
            //    {
            //        var obj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(result));
            //        obj.Remove("_id");
            //        obj.Remove("CreatedTime");
            //        obj.Remove("LastUpdateTime");
            //        obj.Remove("MerchantID");
            //        await _hubContext.Clients.Client(conn).SendAsync("FlySheetSend", JsonConvert.SerializeObject(obj));
            //    }
            //}
            //删除
            await sendFlyingOperation.DeleteModelManyAsync(t => t.MerchantID == merchantID
            && t.game == typeName && t.IssueCode != issueNum);
        }

        /// <summary>
        /// 禁撤回时发送内容
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="oddNum"></param>
        /// <param name="nper"></param>
        /// <returns></returns>
        public static async Task ProhibitionWithdrawal(string merchantID, GameOfType gameType, BetRemarkInfo oddNum, string nper)
        {
            var result = new SendFlying()
            {
                MerchantID = merchantID,
                uuid = Guid.NewGuid().ToString().Replace("-", ""),
                IssueCode = nper,
                game = Enum.GetName(typeof(GameOfType), (int)gameType),
                Status = SendFlyEnum.等待发送
            };
            result.orders.AddRange(Utils.GetFlyingBet(gameType, oddNum));
            SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
            await sendFlyingOperation.InsertModelAsync(result);
            await RabbitMQHelper.SendFlyingSheet(merchantID, gameType, nper, result);
        }

        /// <summary>
        /// 外部商户飞单下注
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="targetID">目标商户id</param>
        /// <param name="userID">用户id</param>
        /// <param name="targetUserID">目标用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="betInfo">下注内容</param>
        /// <param name="nper">期号</param>
        /// <returns></returns>
        public static async Task MerchantInternalSheet(string merchantID, string targetID, string userID, string targetUserID, GameOfType gameType, string betInfo, string nper)
        {
            var result = new GameBetsMessage.GameBetStatus();
            if (Utils.GameTypeItemize(gameType))
                result = await GameBetsMessage.General(targetUserID, gameType, betInfo, targetID, nper, NotesEnum.正常);
            else
                result = await GameBetsMessage.Special(targetUserID, gameType, betInfo, targetID, nper, NotesEnum.正常);

            MerchantInternalOperation merchantInternalOperation = new MerchantInternalOperation();
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == targetUserID && t.MerchantID == targetID);
            var userName = string.IsNullOrEmpty(user.MemoName) ? user.LoginName : user.MemoName;
            //添加数据
            var data = new MerchantInternal()
            {
                MerchantID = merchantID,
                TargetMerchantID = targetID,
                TargetUserID = targetUserID,
                UserID = userID,
                GameType = gameType,
                Remark = betInfo,
                UserName = user.NickName + "(" + userName + ")",
                Nper = nper
            };
            if (result.Status == GameBetsMessage.BetStatuEnum.正常)
            {
                data.Status = FlyEnum.飞单成功;
                data.UseMoney = result.UseMoney;
            }
            else
            {
                data.Status = FlyEnum.飞单失败;
            }
            await merchantInternalOperation.InsertModelAsync(data);

            //发送信息
            //await RabbitMQHelper.SendMerchantBetInfo(data.MerchantID, data.TargetMerchantID, data.TargetUserID, JsonConvert.SerializeObject(MerchantInternalTransformation(data)));
        }

        public static WebMerchantInternal MerchantInternalTransformation(MerchantInternal model)
        {
            var result = new WebMerchantInternal()
            {
                GameType = Enum.GetName(typeof(GameOfType), model.GameType),
                Remark = model.Remark,
                Status = Enum.GetName(typeof(FlyEnum), model.Status),
                Time = model.CreatedTime.ToString("yyyy/MM/dd HH:mm"),
                UseMoney = model.UseMoney,
                UserName = model.UserName,
                Nper = model.Nper
            };
            return result;
        }
    }
}
