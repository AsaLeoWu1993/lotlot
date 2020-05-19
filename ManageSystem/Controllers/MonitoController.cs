using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using ManageSystem.Manipulate;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Operation.Abutment;
using Operation.Baccarat;
using Operation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageSystem.Controllers
{
    /// <summary>
    /// 记录
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    [MerchantAuthentication]
    public class MonitoController : ControllerBase
    {
        #region
        /// <summary>
        /// 现场监控
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="betType">注单 1：所有  2：未开奖 3：已开奖</param>
        /// <param name="userType">用户类型 1：正式 4：虚拟</param>
        /// <param name="userKeyword">查询用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="nper">期号</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProfitLossByType(GameOfType? gameType, int betType, UserStatusEnum? userType, string userKeyword, DateTime startTime, DateTime endTime, string nper, int start = 1, int pageSize = 10)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除);
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            var userIDList = userList.Select(t => t._id).ToList();
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            FilterDefinition<UserBetInfo> filterDefinition = userBetInfoOperation.Builder.Where(t => t.CreatedTime >= startTime && t.LastUpdateTime <= endTime)
                & userBetInfoOperation.Builder.In(t => t.UserID, userIDList);
            if (gameType != null)
                filterDefinition &= userBetInfoOperation.Builder.Eq(t => t.GameType, gameType.Value);
            if (!string.IsNullOrEmpty(nper))
                filterDefinition &= userBetInfoOperation.Builder.Eq(t => t.Nper, nper);
            if (betType == 2)
                filterDefinition &= userBetInfoOperation.Builder.Where(t => t.BetStatus == BetStatus.未开奖);
            else if (betType == 3)
                filterDefinition &= userBetInfoOperation.Builder.Where(t => t.BetStatus == BetStatus.已开奖);
            var userBetList = collection.Find(filterDefinition).SortByDescending(t => t.LastUpdateTime).Skip((start - 1) * pageSize).Limit(pageSize).ToList();
            var total = await collection.CountDocumentsAsync(filterDefinition);

            var result = new List<ProfitLossClass>();
            foreach (var oddInfo in userBetList)
            {
                var user = userList.Find(t => t._id == oddInfo.UserID && t.MerchantID == merchantID);

                var data = new ProfitLossClass
                {
                    NickName = string.IsNullOrEmpty(user.MemoName) && user.ShowType ? user.NickName : user.MemoName,
                    OnlyCode = user.OnlyCode,
                    GameName = Enum.GetName(typeof(GameOfType), oddInfo.GameType),
                    Nper = oddInfo.Nper,
                    BetContent = string.Join(",", oddInfo.BetRemarks.Select(t => t.Remark).ToList()),
                    UserBetMoney = oddInfo.AllUseMoney,
                    BetTime = oddInfo.CreatedTime.ToString("yyyy-MM-dd HH:mm"),
                    Status = Enum.GetName(typeof(BetStatus), oddInfo.BetStatus),
                    ProLoss = oddInfo.BetStatus == BetStatus.未开奖 ? 0 : oddInfo.AllMediumBonus - oddInfo.AllUseMoney
                };
                result.Add(data);
            }
            return Ok(new RecoverListModel<ProfitLossClass>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        /// <summary>
        /// 视讯现场监控
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="betType">注单 1：所有  2：未开奖 3：已开奖</param>
        /// <param name="userType">用户类型 1：正式 4：虚拟</param>
        /// <param name="userKeyword">查询用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="nper">期号</param>
        /// <param name="znum">桌号</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoProfitLossByType(BaccaratGameType? gameType, int betType, UserStatusEnum? userType, string userKeyword, DateTime startTime, DateTime endTime, string nper, int? znum, int start = 1, int pageSize = 10)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除);
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            var userIDList = userList.Select(t => t._id).ToList();
            var address = await Utils.GetAddress(merchantID);
            BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
            var collection = baccaratBetOperation.GetCollection(merchantID);
            FilterDefinition<BaccaratBet> filterDefinition = baccaratBetOperation.Builder.Where(t => t.MerchantID == merchantID && t.CreatedTime >= startTime && t.CreatedTime <= endTime)
                & baccaratBetOperation.Builder.In(t => t.UserID, userIDList);
            if (gameType != null)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.GameType, gameType.Value);
            if (!string.IsNullOrEmpty(nper))
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.Nper, nper);
            if (znum != null)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.ZNum, znum.Value);
            if (betType == 2)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.BetStatus, BetStatus.未开奖);
            else if (betType == 3)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.BetStatus, BetStatus.已开奖);

            var userBetList = collection.Find(filterDefinition).SortByDescending(t => t.LastUpdateTime).Skip((start - 1) * pageSize).Limit(pageSize).ToList();
            var total = await collection.CountDocumentsAsync(filterDefinition);

            var result = new List<VideoProfitLossClass>();
            foreach (var betInfo in userBetList)
            {
                var user = userList.Find(t => t._id == betInfo.UserID && t.MerchantID == merchantID);

                var remarks = betInfo.BetRemarks.Select(t => t.Remark).ToList();
                //相关下注信息
                var data = new VideoProfitLossClass
                {
                    NickName = string.IsNullOrEmpty(user.MemoName) && user.ShowType ? user.NickName : user.MemoName,
                    OnlyCode = user.OnlyCode,
                    GameName = Enum.GetName(typeof(BaccaratGameType), betInfo.GameType),
                    Nper = betInfo.Nper,
                    BetContent = string.Join(",", remarks),
                    UserBetMoney = betInfo.AllUseMoney,
                    BetTime = betInfo.LastUpdateTime.ToString("yyyy-MM-dd HH:mm"),
                    Status = Enum.GetName(typeof(BetStatus), betInfo.BetStatus),
                    ProLoss = betInfo.BetStatus == BetStatus.未开奖 ? 0 : betInfo.AllMediumBonus - betInfo.AllUseMoney,
                    ZNum = betInfo.ZNum
                };
                result.Add(data);
            }

            return Ok(new RecoverListModel<VideoProfitLossClass>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        private class ProfitLossClass
        {
            public string NickName { get; set; }
            public string OnlyCode { get; set; }
            public string GameName { get; set; }
            public string Nper { get; set; }
            public string BetContent { get; set; }
            public decimal UserBetMoney { get; set; }
            public string BetTime { get; set; }
            public string Status { get; set; }
            public decimal ProLoss { get; set; }
        }

        private class VideoProfitLossClass
        {
            public string NickName { get; set; }
            public string OnlyCode { get; set; }
            public string GameName { get; set; }
            public string Nper { get; set; }
            public string BetContent { get; set; }
            public decimal UserBetMoney { get; set; }
            public string BetTime { get; set; }
            public string Status { get; set; }
            public decimal ProLoss { get; set; }
            public int ZNum { get; set; }
        }

        /// <summary>
        /// 获取现场监控流水和盈亏  游戏即将开奖期号
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="betType">注单 1：所有  2：未开奖 3：已开奖</param>
        /// <param name="userType">用户类型 1：正式 4：虚拟</param>
        /// <param name="userKeyword">查询用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="nper">期号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProfitLossInfo(GameOfType? gameType, int betType, UserStatusEnum? userType, string userKeyword, DateTime startTime, DateTime endTime, string nper)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除);
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            var userIDList = userList.Select(t => t._id).ToList();
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);

            FilterDefinition<UserBetInfo> filterDefinition = userBetInfoOperation.Builder.Where(t => t.MerchantID == merchantID && t.CreatedTime >= startTime && t.CreatedTime <= endTime)
                & userBetInfoOperation.Builder.In(t => t.UserID, userIDList);
            if (gameType != null)
                filterDefinition &= userBetInfoOperation.Builder.Eq(t => t.GameType, gameType.Value);
            if (!string.IsNullOrEmpty(nper))
                filterDefinition &= userBetInfoOperation.Builder.Eq(t => t.Nper, nper);
            if (betType == 2)
                filterDefinition &= userBetInfoOperation.Builder.Eq(t => t.BetStatus, BetStatus.未开奖);
            else if (betType == 3)
                filterDefinition &= userBetInfoOperation.Builder.Eq(t => t.BetStatus, BetStatus.已开奖);
            var userBetList = collection.Find(filterDefinition).Sort("{BetStatus:1, CreatedTime:-1}").ToList();
            //Lottery10Operation lottery10Operation = new Lottery10Operation();
            //Lottery5Operation lottery5Operation = new Lottery5Operation();
            ////游戏一直在采集中
            //var pk10 = lottery10Operation.GetModel(t => t._id != null && t.GameType == GameOfType.北京赛车 && (t.MerchantID == null || t.MerchantID == merchantID), t => t.IssueNum, false);

            //var xyft = lottery10Operation.GetModel(t => t._id != null && t.GameType == GameOfType.幸运飞艇 && (t.MerchantID == null || t.MerchantID == merchantID), t => t.IssueNum, false);

            //var cqssc = lottery5Operation.GetModel(t => t._id != null && t.GameType == GameOfType.重庆时时彩 && (t.MerchantID == null || t.MerchantID == merchantID), t => t.IssueNum, false);

            //var jssc = lottery10Operation.GetModel(t => t._id != null && t.GameType == GameOfType.极速赛车 && (t.MerchantID == null || t.MerchantID == merchantID), t => t.IssueNum, false);

            //var azxy10 = lottery10Operation.GetModel(t => t._id != null && t.GameType == GameOfType.澳州10 && (t.MerchantID == null || t.MerchantID == merchantID), t => t.IssueNum, false);

            //var azxy5 = lottery5Operation.GetModel(t => t._id != null && t.GameType == GameOfType.澳州5 && (t.MerchantID == null || t.MerchantID == merchantID), t => t.IssueNum, false);

            //var ireland10 = lottery10Operation.GetModel(t => t._id != null && t.GameType == GameOfType.爱尔兰赛马 && (t.MerchantID == null || t.MerchantID == merchantID), t => t.IssueNum, false);

            //var ireland5 = lottery5Operation.GetModel(t => t._id != null && t.GameType == GameOfType.爱尔兰快5 && (t.MerchantID == null || t.MerchantID == merchantID), t => t.IssueNum, false);

            //var xyft168 = lottery10Operation.GetModel(t => t._id != null && t.GameType == GameOfType.幸运飞艇168 && (t.MerchantID == null || t.MerchantID == merchantID), t => t.IssueNum, false);
            //查询流水和盈亏
            var data = new
            {
                Flow = userBetList.Sum(t => t.AllUseMoney),
                ProLoss = userBetList.FindAll(t => t.BetStatus != BetStatus.未开奖).Sum(t => t.AllMediumBonus) - userBetList.FindAll(t => t.BetStatus != BetStatus.未开奖).Sum(t => t.AllUseMoney),
                Unfinished = userBetList.FindAll(t => t.Notes == NotesEnum.正常 && t.BetStatus == BetStatus.未开奖).Sum(t => t.AllUseMoney),
                //Pk10 = pk10 == null ? "" : pk10.IssueNum,
                //Xyft = xyft == null ? "" : xyft.IssueNum,
                //Cqssc = cqssc == null ? "" : cqssc.IssueNum,
                //Jssc = jssc == null ? "" : jssc.IssueNum,
                //Azxy10 = azxy10 == null ? "" : azxy10.IssueNum,
                //Azxy5 = azxy5 == null ? "" : azxy5.IssueNum,
                //Ireland10 = ireland10 == null ? "" : ireland10.IssueNum,
                //Ireland5 = ireland5 == null ? "" : ireland5.IssueNum,
                //Xyft168 = xyft168 == null ? "" : xyft168.IssueNum
            };
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功",
                Model = data,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取视讯现场监控流水和盈亏
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="betType">注单 1：所有  2：未开奖 3：已开奖</param>
        /// <param name="userType">用户类型 1：正式 4：虚拟</param>
        /// <param name="userKeyword">查询用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="nper">期号</param>
        /// <param name="znum">桌号</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoProfitLossInfo(BaccaratGameType? gameType, int betType, UserStatusEnum? userType, string userKeyword, DateTime startTime, DateTime endTime, string nper, int? znum)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除);
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            var userIDList = userList.Select(t => t._id).ToList();
            var address = await Utils.GetAddress(merchantID);
            BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
            var collection = baccaratBetOperation.GetCollection(merchantID);

            FilterDefinition<BaccaratBet> filterDefinition = baccaratBetOperation.Builder.Where(t => t.MerchantID == merchantID && t.CreatedTime >= startTime && t.CreatedTime <= endTime && t.BetStatus == BetStatus.已开奖 && t.Notes == NotesEnum.正常)
                & baccaratBetOperation.Builder.In(t => t.UserID, userIDList);
            if (gameType != null)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.GameType, gameType.Value);
            if (!string.IsNullOrEmpty(nper))
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.Nper, nper);
            if (znum != null)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.ZNum, znum.Value);
            if (betType == 2)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.BetStatus, BetStatus.未开奖);
            else if (betType == 3)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.BetStatus, BetStatus.已开奖);
            var userBetList = collection.Find(filterDefinition).Sort("{BetStatus:1, CreatedTime:-1}").ToList();
            //查询流水和盈亏
            var data = new
            {
                Flow = userBetList.Sum(t => t.AllUseMoney),
                ProLoss = userBetList.FindAll(t => t.BetStatus != BetStatus.未开奖).Sum(t => t.AllMediumBonus) - userBetList.FindAll(t => t.BetStatus != BetStatus.未开奖).Sum(t => t.AllUseMoney),
                Unfinished = userBetList.FindAll(t => t.Notes == NotesEnum.正常 && t.BetStatus == BetStatus.未开奖).Sum(t => t.AllUseMoney),
            };
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功",
                Model = data,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 撤销注单
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="betType">注单 1：所有  2：未开奖 3：已开奖</param>
        /// <param name="userType">用户类型 1：正式 4：虚拟</param>
        /// <param name="userKeyword">查询用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="nper">期号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RevokeNoteList(GameOfType? gameType, int betType, UserStatusEnum? userType, string userKeyword, DateTime startTime, DateTime endTime, string nper)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除);
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            var userIDList = userList.Select(t => t._id).ToList();
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            FilterDefinition<UserBetInfo> filterDefinition = userBetInfoOperation.Builder.Where(t => t.MerchantID == merchantID && t.CreatedTime >= startTime && t.CreatedTime <= endTime)
                & userBetInfoOperation.Builder.In(t => t.UserID, userIDList);
            if (gameType != null)
                filterDefinition &= userBetInfoOperation.Builder.Eq(t => t.GameType, gameType.Value);
            if (!string.IsNullOrEmpty(nper))
                filterDefinition &= userBetInfoOperation.Builder.Eq(t => t.Nper, nper);
            else if (betType == 2)
                filterDefinition &= userBetInfoOperation.Builder.Eq(t => t.BetStatus, BetStatus.未开奖);
            else if (betType == 3)
                filterDefinition &= userBetInfoOperation.Builder.Eq(t => t.BetStatus, BetStatus.已开奖);

            var userBetList = await collection.FindListAsync(filterDefinition);
            //查询未结算
            var notSettlementList = userBetList.FindAll(t => t.BetStatus == BetStatus.未开奖);
            if (notSettlementList.IsNull()) return Ok(new RecoverModel(RecoverEnum.失败, "无可撤销注单"));
            StringBuilder builder = new StringBuilder();
            foreach (var userID in userIDList)
            {
                var userNotBetList = notSettlementList.FindAll(t => t.UserID == userID);
                if (userNotBetList.IsNull()) continue;
                decimal upScore = 0;
                for (var i = 0; i < userNotBetList.Count; i++)
                {
                    var notBet = userNotBetList[i];
                    for (var x = 0; x < notBet.BetRemarks.Count; x++)
                    {
                        var remark = notBet.BetRemarks[x];
                        for (var y = 0; y < remark.OddBets.Count; y++)
                        {
                            var data = remark.OddBets[y];
                            if (data.BetStatus == BetStatusEnum.已投注)
                            {
                                upScore += data.BetMoney;
                                remark.OddBets.RemoveAt(y);
                                y--;
                            }
                        }
                        if (remark.OddBets.IsNull())
                        {
                            notBet.BetRemarks.RemoveAt(x);
                            x--;
                        }
                    }
                    if (notBet.BetRemarks.IsNull())
                    {
                        //删除数据
                        await collection.DeleteOneAsync(t => t._id == notBet._id);
                        userNotBetList.RemoveAt(i);
                    }
                }
                //用户上分
                await userOperation.UpperScore(userID, merchantID, upScore, ChangeTargetEnum.系统,
                    "管理员撤销注单", "管理员撤销注单", orderStatus: OrderStatusEnum.上分成功);

                var user = userList.Find(t => t._id == userID);
                //foreach (var type in scoreGame)
                //{
                //    //上分提示
                //    await RabbitMQHelper.SendAdminMessage(string.Format("@{0}管理员撤销注单，撤销分数{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, type.BetMoney.ToString("#0.00")),
                //        merchantID, type.GameType);
                //}
                await RabbitMQHelper.SendUserPointChange(userID, merchantID);

                builder.AppendFormat("撤销[{0}({1})]注单，撤销分数{2}\r\n", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, upScore.ToString("#0.00"));
            }
            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID,
                OpLocation = OpLocationEnum.修改用户信息,
                OpType = OpTypeEnum.修改,
                OpAcontent = builder.ToString()
            };
            await sensitiveOperation.InsertModelAsync(sensitive);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 撤销视讯注单
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="betType">注单 1：所有  2：未开奖 3：已开奖</param>
        /// <param name="userType">用户类型 1：正式 4：虚拟</param>
        /// <param name="userKeyword">查询用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="nper">期号</param>
        /// <param name="znum">桌号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RevokeVideoNoteList(BaccaratGameType? gameType, int betType, UserStatusEnum? userType, string userKeyword, DateTime startTime, DateTime endTime, string nper, int? znum)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除);
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            var userIDList = userList.Select(t => t._id).ToList();
            var address = await Utils.GetAddress(merchantID);
            BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
            var collection = baccaratBetOperation.GetCollection(merchantID);
            FilterDefinition<BaccaratBet> filterDefinition = baccaratBetOperation.Builder.Where(t => t.MerchantID == merchantID && t.CreatedTime >= startTime && t.CreatedTime <= endTime)
                & baccaratBetOperation.Builder.In(t => t.UserID, userIDList);
            if (gameType != null)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.GameType, gameType.Value);
            if (!string.IsNullOrEmpty(nper))
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.Nper, nper);
            if (znum != null)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.ZNum, znum.Value);
            if (betType == 2)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.BetStatus, BetStatus.未开奖);
            else if (betType == 3)
                filterDefinition &= baccaratBetOperation.Builder.Eq(t => t.BetStatus, BetStatus.已开奖);

            var userBetList = collection.FindList(filterDefinition, t => t.Nper, false);
            //查询未结算
            var notSettlementList = userBetList.FindAll(t => t.BetStatus == BetStatus.未开奖);
            if (notSettlementList.IsNull()) return Ok(new RecoverModel(RecoverEnum.失败, "无可撤销注单"));
            StringBuilder builder = new StringBuilder();
            foreach (var userID in userIDList)
            {
                var userNotBetList = notSettlementList.FindAll(t => t.UserID == userID);
                if (userNotBetList.IsNull()) continue;
                decimal upScore = 0;
                for (var i = 0; i < userNotBetList.Count; i++)
                {
                    var notBet = userNotBetList[i];
                    for (var x = 0; x < notBet.BetRemarks.Count; x++)
                    {
                        var remark = notBet.BetRemarks[x];
                        if (remark.BetStatus == BaccaratBetEnum.已投注)
                        {
                            upScore += remark.BetMoney;
                            notBet.BetRemarks.RemoveAt(x);
                            x--;
                        }
                    }
                    if (notBet.BetRemarks.IsNull())
                    {
                        //删除数据
                        await collection.DeleteOneAsync(t => t._id == notBet._id);
                    }
                }
                //用户上分
                await userOperation.UpperScore(userID, merchantID, upScore, ChangeTargetEnum.系统,
                    "管理员撤销注单", "管理员撤销注单", orderStatus: OrderStatusEnum.上分成功);

                var user = userList.Find(t => t._id == userID);
                var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                await RabbitMQHelper.SendUserPointChange(userID, merchantID);

                builder.AppendFormat("撤销[{0}({1})]注单，撤销分数{2}\r\n", nickName, user.OnlyCode, upScore.ToString("#0.00"));
            }
            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID,
                OpLocation = OpLocationEnum.修改用户信息,
                OpType = OpTypeEnum.修改,
                OpAcontent = builder.ToString()
            };
            await sensitiveOperation.InsertModelAsync(sensitive);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }
        #endregion

        /// <summary>
        /// 获取回水报表数据
        /// </summary>
        /// <param name="gameType">游戏类型 所有传空</param>
        /// <param name="userType">用户类型  1：真人  4：假人  所有传空</param>
        /// <param name="pattern">回水模式 1：流水 2：输赢 所有传空</param>
        /// <param name="userKeyword">用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchReport(GameOfType? gameType, UserStatusEnum? userType, PatternEnum? pattern, string userKeyword, DateTime startTime, DateTime endTime)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除);
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            BackwaterSetupOperation backwaterSetupOperation = new BackwaterSetupOperation();
            var dic = GameBetsMessage.EnumToDictionary(typeof(GameOfType));
            var result = new List<ReportClass>();
            var tasks = new List<Task>();
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            foreach (var user in userList)
            {
                var task = Task.Run(async () =>
                {
                    //此用户的注单
                    FilterDefinitionBuilder<UserBetInfo> builder = Builders<UserBetInfo>.Filter;
                    FilterDefinition<UserBetInfo> filterBet = builder.Where(t => t.MerchantID == merchantID && t.UserID == user._id
                    && t.CreatedTime >= startTime && t.CreatedTime <= endTime && t.BetStatus == BetStatus.已开奖
                    //&& t.CreatedTime >= programmeAddTime.Value
                    );
                    var userBetList = await collection.FindListAsync(filterBet);
                    if (userBetList.IsNull()) return;
                    //查询回水设置
                    var backwater = await backwaterSetupOperation.GetModelAsync(t => t.MerchantID == merchantID
                    && t._id == user.ProgrammeID);
                    if (backwater == null)
                    {
                        backwater = new BackwaterSetup()
                        {
                            Pattern = PatternEnum.流水模式,
                            GameType = null
                        };
                    }
                    var reback = new List<ReportClass>();
                    //执行用户选择回水方案不为空
                    if (backwater.GameType != gameType && gameType == null)
                    {
                        if (pattern == null)
                        {
                            var callList = await GetSearchUserBet(merchantID, user._id, startTime, endTime, backwater.GameType.Value, backwater.Pattern, user.ProgrammeAddTime, userBetList);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else if (pattern.Value == backwater.Pattern)
                        {
                            //所有游戏
                            var callList = await GetSearchUserBet(merchantID, user._id, startTime, endTime, backwater.GameType.Value, pattern.Value, user.ProgrammeAddTime, userBetList);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else
                            return;
                    }
                    else if (backwater.GameType == gameType && gameType == null)
                    {
                        foreach (var item in dic)
                        {
                            var pargameType = GameBetsMessage.GetEnumByStatus<GameOfType>(item.Value);
                            if (pattern == null)
                            {
                                var callList = await GetSearchUserBet(merchantID, user._id, startTime, endTime, pargameType, backwater.Pattern, user.ProgrammeAddTime, userBetList);
                                if (!callList.IsNull())
                                    reback.AddRange(callList);
                            }
                            else if (pattern.Value == backwater.Pattern)
                            {
                                //所有游戏
                                var callList = await GetSearchUserBet(merchantID, user._id, startTime, endTime, pargameType, pattern.Value, user.ProgrammeAddTime, userBetList);
                                if (!callList.IsNull())
                                    reback.AddRange(callList);
                            }
                            else
                                return;
                        }
                    }
                    else if (backwater.GameType == gameType && gameType != null)
                    {
                        if (pattern == null)
                        {
                            var callList = await GetSearchUserBet(merchantID, user._id, startTime, endTime, backwater.GameType.Value, backwater.Pattern, user.ProgrammeAddTime, userBetList);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else if (pattern.Value == backwater.Pattern)
                        {
                            //所有游戏
                            var callList = await GetSearchUserBet(merchantID, user._id, startTime, endTime, backwater.GameType.Value, pattern.Value, user.ProgrammeAddTime, userBetList);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else
                            return;
                    }
                    else if (backwater.GameType == null && gameType != null)
                    {
                        if (pattern == null)
                        {
                            var callList = await GetSearchUserBet(merchantID, user._id, startTime, endTime, gameType.Value, backwater.Pattern, user.ProgrammeAddTime, userBetList);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else if (pattern.Value == backwater.Pattern)
                        {
                            //所有游戏
                            var callList = await GetSearchUserBet(merchantID, user._id, startTime, endTime, gameType.Value, pattern.Value, user.ProgrammeAddTime, userBetList);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else
                            return;
                    }
                    else
                        return;

                    reback.ForEach(t =>
                    {
                        t.NickName = string.IsNullOrEmpty(user.MemoName) && user.ShowType ? user.NickName : user.MemoName;
                        t.OnlyCode = user.OnlyCode;
                        t.SchemeName = backwater.Name;
                        t.BackStatus = string.IsNullOrEmpty(backwater.Name) ? BackStatusEnum.未回水 : t.BackStatus;
                        t.Reversible = string.IsNullOrEmpty(backwater.Name) ? 0 : t.Reversible;
                    });
                    if (!reback.IsNull())
                        result.AddRange(reback);
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks.ToArray()).ContinueWith(task =>
            {
                if (gameType != null)
                {
                    //分组集合
                    result = (from data in result
                              group data by
                              new
                              {
                                  data.BackStatus,
                                  data.NickName,
                                  data.OnlyCode,
                                  data.Pattern,
                                  data.SchemeName,
                                  data.UserStatus,
                                  data.GameType
                              } into grp
                              select new ReportClass
                              {
                                  Ascent = grp.Sum(t => t.Ascent),
                                  BackStatus = grp.Key.BackStatus,
                                  GameType = grp.Key.GameType,
                                  InputAmount = grp.Sum(t => t.InputAmount),
                                  NickName = grp.Key.NickName,
                                  OnlyCode = grp.Key.OnlyCode,
                                  Pattern = grp.Key.Pattern,
                                  ProLoss = grp.Sum(t => t.ProLoss),
                                  SchemeName = grp.Key.SchemeName,
                                  UserStatus = grp.Key.UserStatus,
                                  Reversible = (endTime - startTime).Days > 1 ? null : grp.Key.Pattern == PatternEnum.流水模式 ? grp.Sum(t => t.Reversible) : 0
                              }).ToList();
                }
                else
                {
                    result = (from data in result
                              group data by
                              new
                              {
                                  data.BackStatus,
                                  data.NickName,
                                  data.OnlyCode,
                                  data.Pattern,
                                  data.SchemeName,
                                  data.UserStatus
                              } into grp
                              select new ReportClass
                              {
                                  Ascent = grp.Sum(t => t.Ascent),
                                  BackStatus = grp.Key.BackStatus,
                                  GameType = null,
                                  InputAmount = grp.Sum(t => t.InputAmount),
                                  NickName = grp.Key.NickName,
                                  OnlyCode = grp.Key.OnlyCode,
                                  Pattern = grp.Key.Pattern,
                                  ProLoss = grp.Sum(t => t.ProLoss),
                                  SchemeName = grp.Key.SchemeName,
                                  UserStatus = grp.Key.UserStatus,
                                  Reversible = (endTime - startTime).Days > 1 ? null : grp.Key.Pattern == PatternEnum.流水模式 ? grp.Sum(t => t.Reversible) : 0
                              }).ToList();
                }
            });
            return Ok(new RecoverListModel<ReportClass>()
            {
                Data = result.OrderByDescending(t => t.BackStatus).ThenByDescending(t => t.NickName).ToList(),
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 获取回水报表数据
        /// </summary>
        /// <param name="gameType">游戏类型 所有传空</param>
        /// <param name="userType">用户类型  1：真人  4：假人  所有传空</param>
        /// <param name="pattern">回水模式 1：流水 2：输赢 所有传空</param>
        /// <param name="userKeyword">用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchVideoReport(BaccaratGameType? gameType, UserStatusEnum? userType, PatternEnum? pattern, string userKeyword, DateTime startTime, DateTime endTime)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除);
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            VideoBackwaterSetupOperation videoBackwaterSetupOperation = new VideoBackwaterSetupOperation();
            var dic = GameBetsMessage.EnumToDictionary(typeof(GameOfType));
            var result = new List<VideoReportClass>();
            var tasks = new List<Task>();
            foreach (var user in userList)
            {
                var task = Task.Run(async () =>
                {
                    //查询回水设置
                    var backwater = await videoBackwaterSetupOperation.GetModelAsync(t => t.MerchantID == merchantID
                    && t._id == user.VideoProgrammeID);
                    if (backwater == null)
                    {
                        backwater = new VideoBackwaterSetup()
                        {
                            Pattern = PatternEnum.流水模式,
                            GameType = null
                        };
                    }
                    var reback = new List<VideoReportClass>();
                    //执行用户选择回水方案不为空
                    if (backwater.GameType != gameType && gameType == null)
                    {
                        if (pattern == null)
                        {
                            var callList = await GetSearchVideoUserBet(merchantID, user._id, startTime, endTime, backwater.GameType.Value, backwater.Pattern, user.ProgrammeAddTime);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else if (pattern.Value == backwater.Pattern)
                        {
                            //所有游戏
                            var callList = await GetSearchVideoUserBet(merchantID, user._id, startTime, endTime, backwater.GameType.Value, pattern.Value, user.ProgrammeAddTime);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else
                            return;
                    }
                    else if (backwater.GameType == gameType && gameType == null)
                    {
                        foreach (var item in dic)
                        {
                            var pargameType = GameBetsMessage.GetEnumByStatus<BaccaratGameType>(item.Value);
                            if (pattern == null)
                            {
                                var callList = await GetSearchVideoUserBet(merchantID, user._id, startTime, endTime, pargameType, backwater.Pattern, user.ProgrammeAddTime);
                                if (!callList.IsNull())
                                    reback.AddRange(callList);
                            }
                            else if (pattern.Value == backwater.Pattern)
                            {
                                //所有游戏
                                var callList = await GetSearchVideoUserBet(merchantID, user._id, startTime, endTime, pargameType, pattern.Value, user.ProgrammeAddTime);
                                if (!callList.IsNull())
                                    reback.AddRange(callList);
                            }
                            else
                                return;
                        }
                    }
                    else if (backwater.GameType == gameType && gameType != null)
                    {
                        if (pattern == null)
                        {
                            var callList = await GetSearchVideoUserBet(merchantID, user._id, startTime, endTime, backwater.GameType.Value, backwater.Pattern, user.ProgrammeAddTime);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else if (pattern.Value == backwater.Pattern)
                        {
                            //所有游戏
                            var callList = await GetSearchVideoUserBet(merchantID, user._id, startTime, endTime, backwater.GameType.Value, pattern.Value, user.ProgrammeAddTime);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else
                            return;
                    }
                    else if (backwater.GameType == null && gameType != null)
                    {
                        if (pattern == null)
                        {
                            var callList = await GetSearchVideoUserBet(merchantID, user._id, startTime, endTime, gameType.Value, backwater.Pattern, user.ProgrammeAddTime);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else if (pattern.Value == backwater.Pattern)
                        {
                            //所有游戏
                            var callList = await GetSearchVideoUserBet(merchantID, user._id, startTime, endTime, gameType.Value, pattern.Value, user.ProgrammeAddTime);
                            if (!callList.IsNull())
                                reback.AddRange(callList);
                        }
                        else
                            return;
                    }
                    else
                        return;

                    reback.ForEach(t =>
                    {
                        t.NickName = string.IsNullOrEmpty(user.MemoName) && user.ShowType ? user.NickName : user.MemoName;
                        t.OnlyCode = user.OnlyCode;
                        t.SchemeName = backwater.Name;
                        t.BackStatus = string.IsNullOrEmpty(backwater.Name) ? BackStatusEnum.未回水 : t.BackStatus;
                        t.Reversible = string.IsNullOrEmpty(backwater.Name) ? 0 : t.Reversible;
                    });
                    if (!reback.IsNull())
                        result.AddRange(reback);
                });
                tasks.Add(task);
            }
            await Task.WhenAll(tasks.ToArray()).ContinueWith(task =>
            {
                //分组集合
                result = (from data in result
                          group data by
                          new
                          {
                              data.BackStatus,
                              data.NickName,
                              data.OnlyCode,
                              data.Pattern,
                              data.SchemeName,
                              data.UserStatus,
                              data.GameType
                          } into grp
                          select new VideoReportClass
                          {
                              Ascent = grp.Sum(t => t.Ascent),
                              BackStatus = grp.Key.BackStatus,
                              GameType = grp.Key.GameType,
                              InputAmount = grp.Sum(t => t.InputAmount),
                              NickName = grp.Key.NickName,
                              OnlyCode = grp.Key.OnlyCode,
                              Pattern = grp.Key.Pattern,
                              ProLoss = grp.Sum(t => t.ProLoss),
                              SchemeName = grp.Key.SchemeName,
                              UserStatus = grp.Key.UserStatus,
                              Reversible = (endTime - startTime).Days > 1 ? null : grp.Key.Pattern == PatternEnum.流水模式 ? grp.Sum(t => t.Reversible) : 0
                          }).ToList();
            });
            return Ok(new RecoverListModel<VideoReportClass>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        private async Task<List<ReportClass>> GetSearchUserBet(string merchantID, string userID, DateTime startTime, DateTime endTime, GameOfType gameType, PatternEnum pattern, DateTime? programmeAddTime, List<UserBetInfo> userAllBetList)
        {
            if (programmeAddTime == null) return null;
            BackwaterJournalOperation backwaterJournalOperation = new BackwaterJournalOperation();

            var result = new List<ReportClass>();
            var userBetList = userAllBetList.FindAll(t => t.GameType == gameType);
            if (userBetList.IsNull()) return result;

            //分天处理
            var dayDiff = (endTime - startTime).Days;
            for (int i = 0; i <= dayDiff; i++)
            {
                var dayBetList = userBetList.FindAll(t => t.CreatedTime >= startTime.AddDays(i)
                && t.CreatedTime <= startTime.AddDays(i + 1));
                if (dayBetList.IsNull()) continue;
                #region 已回水数据
                var backwaterList = await backwaterJournalOperation.GetModelListAsync(t => t.MerchantID == merchantID
                && t.UserID == userID && t.GameType == gameType && t.AddDataTime == startTime.AddDays(i).ToString("yyyy-MM-dd")
                && t.Pattern == pattern && t.BackStatus == BackStatusEnum.已回水);
                if (!backwaterList.IsNull())
                {
                    foreach (var backwater in backwaterList)
                    {
                        var backData = new ReportClass
                        {
                            GameType = backwater.GameType,
                            Ascent = backwater.Ascent,
                            BackStatus = backwater.BackStatus,
                            InputAmount = backwater.InputAmount,
                            ProLoss = backwater.ProLoss,
                            UserStatus = backwater.UserStatus,
                            Pattern = backwater.Pattern,
                            Reversible = 0
                        };
                        result.Add(backData);
                    }
                }
                #endregion
                if (pattern == PatternEnum.流水模式)
                {
                    //真人未回
                    var trueBetList = dayBetList.FindAll(t => t.Notes == NotesEnum.正常 && t.IsBackwater == false);
                    if (!trueBetList.IsNull())
                    {
                        //是否存在可回水数据
                        var flagList = trueBetList.FindAll(t => t.BetStatus == BetStatus.已开奖 && t.CreatedTime >= programmeAddTime.Value);
                        var data = new ReportClass
                        {
                            GameType = gameType,
                            InputAmount = trueBetList.Sum(t => t.AllUseMoney),
                            ProLoss = trueBetList.Sum(t => t.AllMediumBonus) - trueBetList.Sum(t => t.AllUseMoney),
                            Ascent = 0,
                            BackStatus = flagList.IsNull() ? BackStatusEnum.未回水 : BackStatusEnum.可回水,
                            UserStatus = UserStatusEnum.正常,
                            Pattern = PatternEnum.流水模式,
                            Reversible = flagList.Sum(t => t.AllUseMoney)
                        };
                        result.Add(data);
                    }

                    //假人未回
                    var falseBetList = dayBetList.FindAll(t => t.Notes == NotesEnum.虚拟 && t.IsBackwater == false);
                    if (!falseBetList.IsNull())
                    {
                        //是否存在可回水数据
                        var flagList = falseBetList.FindAll(t => t.BetStatus == BetStatus.已开奖 && t.CreatedTime >= programmeAddTime.Value);
                        var data = new ReportClass
                        {
                            GameType = gameType,
                            InputAmount = falseBetList.Sum(t => t.AllUseMoney),
                            ProLoss = flagList.Sum(t => t.AllMediumBonus) - flagList.Sum(t => t.AllUseMoney),
                            Ascent = 0,
                            BackStatus = flagList.IsNull() ? BackStatusEnum.未回水 : BackStatusEnum.可回水,
                            UserStatus = UserStatusEnum.假人,
                            Pattern = PatternEnum.流水模式,
                            Reversible = flagList.Sum(t => t.AllUseMoney)
                        };
                        result.Add(data);
                    }
                }
                else if (pattern == PatternEnum.输赢模式)
                {
                    //真人未回
                    var trueBetList = dayBetList.FindAll(t => t.Notes == NotesEnum.正常);
                    if (!trueBetList.IsNull())
                    {
                        //是否存在可回水数据
                        var flagList = trueBetList.FindAll(t => t.BetStatus == BetStatus.已开奖 && t.CreatedTime >= programmeAddTime.Value);
                        var data = new ReportClass
                        {
                            GameType = gameType,
                            InputAmount = trueBetList.Sum(t => t.AllUseMoney),
                            ProLoss = flagList.Sum(t => t.AllMediumBonus) - flagList.Sum(t => t.AllUseMoney),
                            Ascent = 0,
                            BackStatus = BackStatusEnum.未回水,
                            UserStatus = UserStatusEnum.正常,
                            Pattern = PatternEnum.输赢模式,
                            Reversible = 0
                        };
                        if (data.ProLoss < 0)
                            result.Add(data);
                    }

                    //假人未回
                    var falseBetList = dayBetList.FindAll(t => t.Notes == NotesEnum.虚拟);
                    if (!falseBetList.IsNull())
                    {
                        //是否存在可回水数据
                        var flagList = falseBetList.FindAll(t => t.BetStatus == BetStatus.已开奖 && t.CreatedTime >= programmeAddTime.Value);
                        var data = new ReportClass
                        {
                            GameType = gameType,
                            InputAmount = falseBetList.Sum(t => t.AllUseMoney),
                            ProLoss = flagList.Sum(t => t.AllMediumBonus) - flagList.Sum(t => t.AllUseMoney),
                            Ascent = 0,
                            BackStatus = BackStatusEnum.未回水,
                            UserStatus = UserStatusEnum.假人,
                            Pattern = PatternEnum.输赢模式,
                            Reversible = 0
                        };
                        if (data.ProLoss < 0)
                            result.Add(data);
                    }
                }
            }
            return result;
        }

        private async Task<List<VideoReportClass>> GetSearchVideoUserBet(string merchantID, string userID, DateTime startTime, DateTime endTime, BaccaratGameType gameType, PatternEnum pattern, DateTime? programmeAddTime)
        {
            if (programmeAddTime == null) return null;
            var address = await Utils.GetAddress(merchantID);
            BaccaratBetOperation userBetInfoOperation = await BetManage.GetBaccaratBetOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            VideoBackwaterJournalOperation backwaterJournalOperation = new VideoBackwaterJournalOperation();
            FilterDefinition<BaccaratBet> filter = userBetInfoOperation.Builder.Where(t => t.MerchantID == merchantID && t.UserID == userID
            && t.CreatedTime >= startTime && t.CreatedTime <= endTime && t.BetStatus == BetStatus.已开奖
            //&& t.CreatedTime >= programmeAddTime.Value
            );
            //游戏类型
            filter &= userBetInfoOperation.Builder.Eq(t => t.GameType, gameType);
            var userBetList = await collection.FindListAsync(filter);
            var result = new List<VideoReportClass>();
            if (userBetList.IsNull()) return result;

            //分天处理
            var dayDiff = (endTime - startTime).Days;
            for (int i = 0; i <= dayDiff; i++)
            {
                var dayBetList = userBetList.FindAll(t => t.CreatedTime >= startTime.AddDays(i)
                && t.CreatedTime <= startTime.AddDays(i + 1));
                if (dayBetList.IsNull()) continue;
                #region 已回水数据
                var backwaterList = await backwaterJournalOperation.GetModelListAsync(t => t.MerchantID == merchantID
                && t.UserID == userID && t.GameType == gameType && t.AddDataTime == startTime.AddDays(i).ToString("yyyy-MM-dd")
                && t.Pattern == pattern && t.BackStatus == BackStatusEnum.已回水);
                if (!backwaterList.IsNull())
                {
                    foreach (var backwater in backwaterList)
                    {
                        var backData = new VideoReportClass
                        {
                            GameType = backwater.GameType,
                            Ascent = backwater.Ascent,
                            BackStatus = backwater.BackStatus,
                            InputAmount = backwater.InputAmount,
                            ProLoss = backwater.ProLoss,
                            UserStatus = backwater.UserStatus,
                            Pattern = backwater.Pattern,
                            Reversible = 0
                        };
                        result.Add(backData);
                    }
                }
                #endregion
                if (pattern == PatternEnum.流水模式)
                {
                    //真人未回
                    var trueBetList = dayBetList.FindAll(t => t.Notes == NotesEnum.正常 && t.IsBackwater == false);
                    if (!trueBetList.IsNull())
                    {
                        //是否存在可回水数据
                        var flagList = trueBetList.FindAll(t => t.BetStatus == BetStatus.已开奖 && t.CreatedTime >= programmeAddTime.Value);
                        var data = new VideoReportClass
                        {
                            GameType = gameType,
                            InputAmount = trueBetList.Sum(t => t.AllUseMoney),
                            ProLoss = trueBetList.Sum(t => t.AllMediumBonus) - trueBetList.Sum(t => t.AllUseMoney),
                            Ascent = 0,
                            BackStatus = flagList.IsNull() ? BackStatusEnum.未回水 : BackStatusEnum.可回水,
                            UserStatus = UserStatusEnum.正常,
                            Pattern = PatternEnum.流水模式,
                            Reversible = flagList.Sum(t => t.AllUseMoney)
                        };
                        result.Add(data);
                    }

                    //假人未回
                    var falseBetList = dayBetList.FindAll(t => t.Notes == NotesEnum.虚拟 && t.IsBackwater == false);
                    if (!falseBetList.IsNull())
                    {
                        //是否存在可回水数据
                        var flagList = falseBetList.FindAll(t => t.BetStatus == BetStatus.已开奖 && t.CreatedTime >= programmeAddTime.Value);
                        var data = new VideoReportClass
                        {
                            GameType = gameType,
                            InputAmount = falseBetList.Sum(t => t.AllUseMoney),
                            ProLoss = flagList.Sum(t => t.AllMediumBonus) - flagList.Sum(t => t.AllUseMoney),
                            Ascent = 0,
                            BackStatus = flagList.IsNull() ? BackStatusEnum.未回水 : BackStatusEnum.可回水,
                            UserStatus = UserStatusEnum.假人,
                            Pattern = PatternEnum.流水模式,
                            Reversible = flagList.Sum(t => t.AllUseMoney)
                        };
                        result.Add(data);
                    }
                }
                else if (pattern == PatternEnum.输赢模式)
                {
                    //真人未回
                    var trueBetList = dayBetList.FindAll(t => t.Notes == NotesEnum.正常);
                    if (!trueBetList.IsNull())
                    {
                        //是否存在可回水数据
                        var flagList = trueBetList.FindAll(t => t.BetStatus == BetStatus.已开奖 && t.CreatedTime >= programmeAddTime.Value);
                        var data = new VideoReportClass
                        {
                            GameType = gameType,
                            InputAmount = trueBetList.Sum(t => t.AllUseMoney),
                            ProLoss = flagList.Sum(t => t.AllMediumBonus) - flagList.Sum(t => t.AllUseMoney),
                            Ascent = 0,
                            BackStatus = BackStatusEnum.未回水,
                            UserStatus = UserStatusEnum.正常,
                            Pattern = PatternEnum.输赢模式,
                            Reversible = 0
                        };
                        if (data.ProLoss < 0)
                            result.Add(data);
                    }

                    //假人未回
                    var falseBetList = dayBetList.FindAll(t => t.Notes == NotesEnum.虚拟);
                    if (!falseBetList.IsNull())
                    {
                        //是否存在可回水数据
                        var flagList = falseBetList.FindAll(t => t.BetStatus == BetStatus.已开奖 && t.CreatedTime >= programmeAddTime.Value);
                        var data = new VideoReportClass
                        {
                            GameType = gameType,
                            InputAmount = falseBetList.Sum(t => t.AllUseMoney),
                            ProLoss = flagList.Sum(t => t.AllMediumBonus) - flagList.Sum(t => t.AllUseMoney),
                            Ascent = 0,
                            BackStatus = BackStatusEnum.未回水,
                            UserStatus = UserStatusEnum.假人,
                            Pattern = PatternEnum.输赢模式,
                            Reversible = 0
                        };
                        if (data.ProLoss < 0)
                            result.Add(data);
                    }
                }
            }
            return result;
        }

        private class ReportClass
        {
            public GameOfType? GameType { get; set; }

            public string NickName { get; set; }

            public string OnlyCode { get; set; }

            public UserStatusEnum UserStatus { get; set; }

            public decimal InputAmount { get; set; }
            public decimal ProLoss { get; set; }

            public string SchemeName { get; set; }

            public decimal Ascent { get; set; }

            public BackStatusEnum BackStatus { get; set; }

            public PatternEnum Pattern { get; set; }

            public decimal? Reversible { get; set; } = 0;
        }

        private class VideoReportClass
        {
            public BaccaratGameType? GameType { get; set; }

            public string NickName { get; set; }

            public string OnlyCode { get; set; }

            public UserStatusEnum UserStatus { get; set; }

            public decimal InputAmount { get; set; }
            public decimal ProLoss { get; set; }

            public string SchemeName { get; set; }

            public decimal Ascent { get; set; }

            public BackStatusEnum BackStatus { get; set; }

            public PatternEnum Pattern { get; set; }

            public decimal? Reversible { get; set; } = 0;
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="gameType">游戏类型 所有传空</param>
        /// <param name="userType">用户类型  1：真人  4：假人  所有传空</param>
        /// <param name="pattern">回水模式 1：流水 2：输赢 所有传空</param>
        /// <param name="userKeyword">用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteRecords(GameOfType? gameType, UserStatusEnum? userType, PatternEnum? pattern, string userKeyword, DateTime startTime, DateTime endTime)
        {
            if ((endTime - startTime).Days > 1)
                return Ok(new RecoverModel(RecoverEnum.失败, "删除记录不能超过一天！"));

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除 && !string.IsNullOrEmpty(t.ProgrammeID));
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            BackwaterSetupOperation backwaterSetupOperation = new BackwaterSetupOperation();
            BackwaterJournalOperation backwaterJournalOperation = new BackwaterJournalOperation();
            foreach (var user in userList)
            {
                //查询用户回水方案类型  删除已回水数据
                var userbackPro = await backwaterSetupOperation.GetModelAsync(t => t._id == user.ProgrammeID);
                FilterDefinition<BackwaterJournal> backFilter = backwaterJournalOperation.Builder.Where(t => t.UserID == user._id && t.MerchantID == merchantID && t.AddDataTime == startTime.ToString("yyyy-MM-dd"));
                if (gameType != null)
                    backFilter &= backwaterJournalOperation.Builder.Eq(t => t.GameType, gameType.Value);
                if (pattern != null)
                    backFilter &= backwaterJournalOperation.Builder.Eq(t => t.Pattern, pattern.Value);

                await backwaterJournalOperation.DeleteModelManyAsync(backFilter);
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "删除成功！"));
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <param name="gameType">游戏类型 所有传空</param>
        /// <param name="userType">用户类型  1：真人  4：假人  所有传空</param>
        /// <param name="pattern">回水模式 1：流水 2：输赢 所有传空</param>
        /// <param name="userKeyword">用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteVideoRecords(BaccaratGameType? gameType, UserStatusEnum? userType, PatternEnum? pattern, string userKeyword, DateTime startTime, DateTime endTime)
        {
            if ((endTime - startTime).Days > 1)
                return Ok(new RecoverModel(RecoverEnum.失败, "删除记录不能超过一天！"));

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除 && !string.IsNullOrEmpty(t.VideoProgrammeID));
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            VideoBackwaterSetupOperation backwaterSetupOperation = new VideoBackwaterSetupOperation();
            VideoBackwaterJournalOperation backwaterJournalOperation = new VideoBackwaterJournalOperation();
            foreach (var user in userList)
            {
                //查询用户回水方案类型  删除已回水数据
                var userbackPro = await backwaterSetupOperation.GetModelAsync(t => t._id == user.VideoProgrammeID);
                FilterDefinition<VideoBackwaterJournal> backFilter = backwaterJournalOperation.Builder.Where(t => t.UserID == user._id && t.MerchantID == merchantID && t.AddDataTime == startTime.ToString("yyyy-MM-dd"));
                if (gameType != null)
                    backFilter &= backwaterJournalOperation.Builder.Eq(t => t.GameType, gameType.Value);
                if (pattern != null)
                    backFilter &= backwaterJournalOperation.Builder.Eq(t => t.Pattern, pattern.Value);

                await backwaterJournalOperation.DeleteModelManyAsync(backFilter);
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "删除成功！"));
        }

        /// <summary>
        /// 一键回水所有游戏
        /// </summary>
        /// <param name="gameType">游戏类型 所有传空</param>
        /// <param name="userType">用户类型  1：真人  4：假人  所有传空</param>
        /// <param name="pattern">回水模式 1：流水 2：输赢 所有传空</param>
        /// <param name="userKeyword">用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GhostReport(GameOfType? gameType, UserStatusEnum? userType, PatternEnum? pattern, string userKeyword, DateTime startTime, DateTime endTime)
        {
            if ((endTime - startTime).TotalDays > 1)
                return Ok(new RecoverModel(RecoverEnum.失败, "回水时间不能超过一天！"));
            if (TimeSpace(startTime) && TimeSpace(endTime))
            {
                if (pattern == PatternEnum.输赢模式)
                    return Ok(new RecoverModel(RecoverEnum.失败, "不能回水今日输赢模式！"));
                else if (pattern == null)
                    pattern = PatternEnum.流水模式;
            }

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除 && !string.IsNullOrEmpty(t.ProgrammeID));
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            var userIDList = userList.Select(t => t._id).ToList();
            var result = new List<bool>();
            var dic = GameBetsMessage.EnumToDictionary(typeof(GameOfType));
            if (gameType == null)
            {
                foreach (var item in dic)
                {
                    var pargameType = GameBetsMessage.GetEnumByStatus<GameOfType>(item.Value);
                    if (pattern == null)
                    {
                        result.Add(await BackwaterKind.GameBackwater(merchantID, userIDList,
                startTime, endTime, pargameType, PatternEnum.流水模式));

                        result.Add(await BackwaterKind.GameBackwater(merchantID, userIDList,
                startTime, endTime, pargameType, PatternEnum.输赢模式));
                    }
                    else
                        result.Add(await BackwaterKind.GameBackwater(merchantID, userIDList,
                startTime, endTime, pargameType, pattern.Value));
                }
            }
            else
            {
                //所有模式
                if (pattern == null)
                {
                    result.Add(await BackwaterKind.GameBackwater(merchantID, userIDList,
               startTime, endTime, gameType.Value, PatternEnum.流水模式));

                    result.Add(await BackwaterKind.GameBackwater(merchantID, userIDList,
            startTime, endTime, gameType.Value, PatternEnum.输赢模式));
                }
                else
                    result.Add(await BackwaterKind.GameBackwater(merchantID, userIDList,
            startTime, endTime, gameType.Value, pattern.Value));
            }

            if (result.Contains(true))
                return Ok(new RecoverModel(RecoverEnum.成功, "回水成功！"));
            else
                return Ok(new RecoverModel(RecoverEnum.失败, "当天无可返流水！"));
        }

        /// <summary>
        /// 一键回水所有游戏
        /// </summary>
        /// <param name="gameType">游戏类型 所有传空</param>
        /// <param name="userType">用户类型  1：真人  4：假人  所有传空</param>
        /// <param name="pattern">回水模式 1：流水 2：输赢 所有传空</param>
        /// <param name="userKeyword">用户关键字</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GhostVideoReport(BaccaratGameType? gameType, UserStatusEnum? userType, PatternEnum? pattern, string userKeyword, DateTime startTime, DateTime endTime)
        {
            if ((endTime - startTime).TotalDays > 1)
                return Ok(new RecoverModel(RecoverEnum.失败, "回水时间不能超过一天！"));
            if (TimeSpace(startTime) && TimeSpace(endTime))
            {
                if (pattern == PatternEnum.输赢模式)
                    return Ok(new RecoverModel(RecoverEnum.失败, "不能回水今日输赢模式！"));
                else if (pattern == null)
                    pattern = PatternEnum.流水模式;
            }

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            //查询用户
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.Status != UserStatusEnum.冻结 && t.Status != UserStatusEnum.删除 && !string.IsNullOrEmpty(t.VideoProgrammeID));
            if (!string.IsNullOrEmpty(userKeyword))
                filter &= userOperation.Builder.Regex(t => t.OnlyCode, userKeyword)
                    | userOperation.Builder.Regex(t => t.LoginName, userKeyword);
            if (userType != null)
                filter &= userOperation.Builder.Eq(t => t.Status, userType.Value);
            var userList = await userOperation.GetModelListAsync(filter);
            var userIDList = userList.Select(t => t._id).ToList();
            var result = new List<bool>();
            var dic = GameBetsMessage.EnumToDictionary(typeof(BaccaratGameType));
            if (gameType == null)
            {
                foreach (var item in dic)
                {
                    var pargameType = GameBetsMessage.GetEnumByStatus<BaccaratGameType>(item.Value);
                    if (pattern == null)
                    {
                        result.Add(await BackwaterKind.VideoGameBackwater(merchantID, userIDList,
                startTime, endTime, pargameType, PatternEnum.流水模式));

                        result.Add(await BackwaterKind.VideoGameBackwater(merchantID, userIDList,
                startTime, endTime, pargameType, PatternEnum.输赢模式));
                    }
                    else
                        result.Add(await BackwaterKind.VideoGameBackwater(merchantID, userIDList,
                startTime, endTime, pargameType, pattern.Value));
                }
            }
            else
            {
                //所有模式
                if (pattern == null)
                {
                    result.Add(await BackwaterKind.VideoGameBackwater(merchantID, userIDList,
               startTime, endTime, gameType.Value, PatternEnum.流水模式));

                    result.Add(await BackwaterKind.VideoGameBackwater(merchantID, userIDList,
            startTime, endTime, gameType.Value, PatternEnum.输赢模式));
                }
                else
                    result.Add(await BackwaterKind.VideoGameBackwater(merchantID, userIDList,
            startTime, endTime, gameType.Value, pattern.Value));
            }

            if (result.Contains(true))
                return Ok(new RecoverModel(RecoverEnum.成功, "回水成功！"));
            else
                return Ok(new RecoverModel(RecoverEnum.失败, "当天无可返流水！"));
        }

        private bool TimeSpace(DateTime time)
        {
            if (DateTime.Now >= DateTime.Today.AddHours(6))
            {
                if (time >= DateTime.Today.AddHours(6) && time <= DateTime.Today.AddDays(1).AddHours(6))
                    return true;
                else
                    return false;
            }
            else
            {
                if (time >= DateTime.Today.AddDays(-1).AddHours(6) && time <= DateTime.Today.AddHours(6))
                    return true;
                else
                    return false;
            }
        }

        /// <summary>
        /// 查询代理报表
        /// </summary>
        /// <param name="time">查询时间  日期</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchAgentReport(DateTime time, int start = 1, int pageSize = 10)
        {
            time = Convert.ToDateTime(time.ToString("yyyy-MM-dd"));
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            UserBackwaterJournalOperation userBackwaterJournalOperation = new UserBackwaterJournalOperation();
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var total = await userOperation.GetCountAsync(t => t.IsAgent && t.Status == UserStatusEnum.正常 && t.MerchantID == merchantID);
            var agentUserList = userOperation.GetModelListByPaging(t => t.IsAgent && t.Status == UserStatusEnum.正常 && t.MerchantID == merchantID,
                t => t.CreatedTime, false, start, pageSize);
            var result = new List<WebAgentReport>();
            foreach (var user in agentUserList)
            {
                var agentBackInfo = await agentBackwaterOperation.GetModelAsync(t => t.MerchantID == merchantID && t.UserID == user._id);
                if (agentBackInfo == null) continue;
                //if (agentBackInfo.AddTime > time) continue;
                var userBetAgentList = await BackwaterKind.SearchUserBet(merchantID, agentBackInfo, time);
                var data = new WebAgentReport()
                {
                    AgentID = user._id,
                    NickName = string.IsNullOrEmpty(user.MemoName) && user.ShowType ? user.NickName : user.MemoName,
                    Time = time.ToString("yyyy-MM-dd"),
                    OnlyCode = user.OnlyCode,
                    Ascent = userBetAgentList.Sum(t => t.Ascent),
                    Pk10 = userBetAgentList.Sum(t => t.Pk10),
                    Xyft = userBetAgentList.Sum(t => t.Xyft),
                    Cqssc = userBetAgentList.Sum(t => t.Cqssc),
                    Jssc = userBetAgentList.Sum(t => t.Jssc),
                    Azxy10 = userBetAgentList.Sum(t => t.Azxy10),
                    Azxy5 = userBetAgentList.Sum(t => t.Azxy5),
                    Ireland10 = userBetAgentList.Sum(t => t.Ireland10),
                    Ireland5 = userBetAgentList.Sum(t => t.Ireland5),
                    Xyft168 = userBetAgentList.Sum(t => t.Xyft168),
                    Jsssc = userBetAgentList.Sum(t=>t.Jsssc),
                    Baccarat = userBetAgentList.Sum(t => t.Baccarat),
                    BackStatus = userBetAgentList.IsNull() ? BackStatusEnum.未回水 : userBetAgentList.Exists(t => t.BackStatus == BackStatusEnum.未回水) ? BackStatusEnum.未回水
                    : BackStatusEnum.已回水
                };
                result.Add(data);
            }
            return Ok(new RecoverListModel<WebAgentReport>()
            {
                Data = result,
                Total = total,
                Status = RecoverEnum.成功,
                Message = "查询成功！"
            });
        }

        /// <summary>
        /// 获取下线数据
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="agentID">代理id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOfflineReport(DateTime time, string agentID)
        {
            time = Convert.ToDateTime(time.ToString("yyyy-MM-dd"));

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            var agent = await agentBackwaterOperation.GetModelAsync(t => t.UserID == agentID && t.MerchantID == merchantID);
            if (agent == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息！"));
            var result = await BackwaterKind.SearchUserBet(merchantID, agent, time);
            return Ok(new RecoverListModel<WebAgentReport>()
            {
                Data = result,
                Total = result.Count,
                Status = RecoverEnum.成功,
                Message = "查询成功！"
            });
        }

        /// <summary>
        /// 一键回水代理报表
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GhostUserReport(DateTime time)
        {
            time = Convert.ToDateTime(time.ToString("yyyy-MM-dd"));

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (time >= DateTime.Today)
                return Ok(new RecoverModel(RecoverEnum.参数错误, "不能回水今日数据！"));
            var dic = GameBetsMessage.EnumToDictionary(typeof(GameOfType));
            var tasks = new List<Task>();
            var result = new List<bool>();
            foreach (var item in dic)
            {
                var gameType = GameBetsMessage.GetEnumByStatus<GameOfType>(item.Value);
                var task = Task.Run(async () =>
                {
                    result.Add(await BackwaterKind.AgentBackwater(merchantID, time, gameType, null));
                });
                tasks.Add(task);
            }
            result.Add(await BackwaterKind.AgentBackwater(merchantID, time, null, BaccaratGameType.百家乐));
            await Task.WhenAll(tasks.ToArray());
            if (result.Contains(true))
                return Ok(new RecoverModel(RecoverEnum.成功, "回水成功！"));
            else
                return Ok(new RecoverModel(RecoverEnum.失败, "当天无可返流水！"));
        }
    }
}