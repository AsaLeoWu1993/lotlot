using Entity;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Operation.Common.WinPrize;
using static Operation.Common.Utils;
using Entity.BaccaratModel;
using Operation.Baccarat;

namespace Operation.Common
{
    /// <summary>
    /// 封盘核对消息及中奖消息
    /// </summary>
    public static class SealupMessage
    {
        /// <summary>
        /// 获取商户当期已下注注单
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="nper"></param>
        /// <returns></returns>
        public static async Task<List<UserBetInfo>> GetMerchantBetList(string merchantID, GameOfType gameType, string nper)
        {
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var list = await collection.FindListAsync(t => t.MerchantID == merchantID && t.GameType == gameType && t.Nper == nper
            && t.BetStatus == BetStatus.未开奖);
            return list;
        }

        /// <summary>
        /// 封盘消息转换
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="message"></param>
        /// <param name="nper"></param>
        /// <returns></returns>
        public static async Task<string> GetAllBetsAsync(string merchantID, GameOfType gameType, string message, string nper)
        {
            var personList = await GetPersonInfos(merchantID);
            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            //玩家数量
            message = Utils.SealedTransformation(gameType, message);
            message = message
                .Replace("{玩家数量}", (personList.Count + room.Online).ToString())
                .Replace("{玩家总分}", personList.Sum(t => t.UserMoney).ToString("#0.00"));
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var list = await collection.FindListAsync(t => t.GameType == gameType && t.Nper == nper && t.BetStatus == BetStatus.未开奖);
            if (list.IsNull())
                return message.Replace("{核对}", "");
            var userIDList = list.Select(t => t.UserID).Distinct().ToList();
            var userOperation = new UserOperation();
            var userList = await userOperation.GetModelListAsync(userOperation.Builder.Eq(t => t.MerchantID, merchantID)
                & userOperation.Builder.In(t => t._id, userIDList));
            var messages = new List<string>();
            foreach (var data in list)
            {
                var user = userList.Find(t => t._id == data.UserID);
                var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                var msg = data.BetRemarks.Select(t => t.Remark).ToList();
                var item = string.Format("[{0}][{1}]", nickName, string.Join("#", msg));
                messages.Add(item);
            }
            //Utils.Logger.Error(string.Format("核对 商户：{0} 游戏：{1}  期号：{2} 下注人数：{3}", merchantID, gameType, nper, list.Count));
            return message.Replace("{核对}", string.Join("\r\n", messages));
        }

        /// <summary>
        /// 封盘消息转换
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="message"></param>
        /// <param name="nper"></param>
        /// <param name="znum"></param>
        /// <returns></returns>
        public static async Task<string> GetBaccaratBetsAsync(string merchantID, BaccaratGameType gameType, string message, string nper, int znum)
        {
            var personList = await GetPersonInfos(merchantID);
            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            //玩家数量
            message = message.Replace("{期号}", nper)
                .Replace("{游戏}", Enum.GetName(typeof(BaccaratGameType), gameType))
                .Replace("{玩家数量}", (personList.Count + room.Online).ToString())
                .Replace("{玩家总分}", personList.Sum(t => t.UserMoney).ToString("#0.00"));
            var address = await Utils.GetAddress(merchantID);
            BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
            var collection = baccaratBetOperation.GetCollection(merchantID);
            var list = await collection.FindListAsync(t => t.GameType == gameType && t.Nper == nper && t.ZNum == znum && t.BetStatus == BetStatus.未开奖);
            if (list.IsNull())
                return message.Replace("{核对}", "");
            var userIDList = list.Select(t => t.UserID).Distinct().ToList();
            var userOperation = new UserOperation();
            var userList = await userOperation.GetModelListAsync(userOperation.Builder.Eq(t => t.MerchantID, merchantID)
                & userOperation.Builder.In(t => t._id, userIDList));
            var messages = new List<string>();
            foreach (var data in list)
            {
                var user = userList.Find(t => t._id == data.UserID);
                var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                var msg = data.BetRemarks.Select(t => t.Remark).ToList();
                var item = string.Format("[{0}][{1}]", nickName, string.Join("#", msg));
                messages.Add(item);
            }
            //Utils.Logger.Error(string.Format("核对 商户：{0} 游戏：{1}  期号：{2} 下注人数：{3}", merchantID, gameType, nper, list.Count));
            return message.Replace("{核对}", string.Join("\r\n", messages));
        }

        /// <summary>
        /// 获取房间内人数（包括机器人）
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task<List<OnlinePersonInfo>> GetPersonInfos(string merchantID)
        {
            UserOperation userOperation = new UserOperation();
            //var items = await Utils.GetRoomGameConnInfos(merchantID, gameType);
            //var result = new List<OnlinePersonInfo>();
            //var tasks = new List<Task>();
            //foreach (var item in items)
            //{
            //    var task = Task.Run(async () =>
            //    {
            //        var user = await userOperation.GetModelAsync(t => t._id == item.UserID && t.MerchantID == merchantID);
            //        if (user == null) return;
            //        var slr = new OnlinePersonInfo
            //        {
            //            UserID = user._id,
            //            NickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
            //            UserMoney = user.UserMoney
            //        };
            //        result.Add(slr);
            //    });
            //    tasks.Add(task);
            //}
            //await Task.WhenAll(tasks.ToArray());

            ////添加机器人
            //var robotList = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.假人);
            ////ShamUserOperation shamUserOperation = new ShamUserOperation();
            //ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            //foreach (var user in robotList)
            //{
            //    //var sham = await shamUserOperation.GetModelAsync(t => t.MerchantID == merchantID && t.UserID == user._id);
            //    var sham = await shamRobotOperation.GetModelAsync(t => t.MerchantID == merchantID && t.UserID == user._id);
            //    if (sham == null) continue;
            //    var item = sham.GameCheckInfo.Find(t => t.GameType == gameType);
            //    if (item == null) continue;
            //    if (!item.Check) continue;
            //    var slr = new OnlinePersonInfo
            //    {
            //        UserID = user._id,
            //        NickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
            //        UserMoney = user.UserMoney
            //    };
            //    result.Add(slr);
            //}
            var userList = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID && (t.Status == UserStatusEnum.正常 || t.Status == UserStatusEnum.假人));
            var result = new List<OnlinePersonInfo>();
            foreach (var user in userList)
            {
                var slr = new OnlinePersonInfo
                {
                    UserID = user._id,
                    NickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                    UserMoney = user.UserMoney
                };
                result.Add(slr);
            }
            return result;
        }

        ///// <summary>
        ///// 查询所有人数（机器人 真人  不在线真人）
        ///// </summary>
        ///// <param name="merchantID"></param>
        ///// <param name="gameType"></param>
        ///// <returns></returns>
        //public static async Task<int> GetAllPersonInfos(string merchantID, GameOfType gameType)
        //{
        //    UserOperation userOperation = new UserOperation();
        //    //添加真人
        //    var result = await userOperation.GetCountAsync(t => t.MerchantID == merchantID
        //    && t.Status == UserStatusEnum.正常);
        //    //添加机器人
        //    var robotList = userOperation.GetModelList(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.假人);
        //    //ShamUserOperation shamUserOperation = new ShamUserOperation();
        //    ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
        //    foreach (var user in robotList)
        //    {
        //        //var sham = await shamUserOperation.GetModelAsync(t => t.MerchantID == merchantID && t.UserID == user._id);
        //        var sham = await shamRobotOperation.GetModelAsync(t => t.MerchantID == merchantID && t.UserID == user._id);
        //        if (sham == null) continue;
        //        var item = sham.GameCheckInfo.Find(t => t.GameType == gameType);
        //        if (item == null) continue;
        //        if (!item.Check) continue;
        //        ++result;
        //    }
        //    return Convert.ToInt32(result);
        //}

        private class HandleGameMessage
        {
            public string UserID { get; set; }

            public string Message { get; set; }

            public string OddNum { get; set; }
        }

        private class HandleAllBets
        {
            public string UserID { get; set; }
            public string MerchantID { get; set; }
            public string Nper { get; set; }
            public string BetRule { get; set; }
            public string Message { get; set; }
        }

        /// <summary>
        /// 获取游戏历史数据
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public static string GetGameHistoryTop(GameOfType gameType)
        {
            var top = 30;
            string msg = string.Empty;
            if (Utils.GameTypeItemize(gameType))
            {
                Lottery10Operation lottery10Operation = new Lottery10Operation();
                var list = lottery10Operation.GetModelListByPaging(t => t.GameType == gameType && t.MerchantID == null, t => t.IssueNum, false, 1, top);
                msg = JsonConvert.SerializeObject(Utils.GetGameHistories10(list));
            }
            else
            {
                Lottery5Operation lottery5Operation = new Lottery5Operation();
                var list = lottery5Operation.GetModelListByPaging(t => t.GameType == gameType && t.MerchantID == null, t => t.IssueNum, false, 1, top);
                msg = JsonConvert.SerializeObject(Utils.GetGameHistories5(list));
            }
            var result = new { IsArray = true, Data = msg };
            return JsonConvert.SerializeObject(result);
        }
    }
}
