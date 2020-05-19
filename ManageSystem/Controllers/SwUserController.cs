using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using ManageSystem.Hubs;
using ManageSystem.Manipulate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
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
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static Operation.Common.GameBetsMessage;

namespace ManageSystem.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    [UserAuthentication]
    public class SwUserController : ControllerBase
    {
        readonly MerchantOperation merchantOperation = null;
        readonly UserOperation userOperation = null;
        private readonly IHostingEnvironment _hostingEnvironment = null;

        private readonly IHubContext<ChatHub> context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hub"></param>
        /// <param name="hostingEnvironment"></param>
        public SwUserController(IHubContext<ChatHub> hub, IHostingEnvironment hostingEnvironment)
        {
            context = hub;
            _hostingEnvironment = hostingEnvironment;
            userOperation = new UserOperation();
            merchantOperation = new MerchantOperation();
        }

        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="loginName">登录名</param>
        /// <param name="password">密码</param>
        /// <param name="nickName">昵称</param>
        /// <param name="code">验证码</param>
        /// <param name="seurityNo">安全码</param>
        /// <returns></returns>
        [HttpGet]
        [NotUserAuthentication]
        public async Task<IActionResult> UserRegister(string loginName, string password, string nickName, string code, string seurityNo)
        {
            loginName = loginName.Trim();
            password = password.Trim();
            var session = RedisOperation.GetString(HttpContext.GetIP());
            if (string.IsNullOrEmpty(session)) return Ok(new RecoverModel(RecoverEnum.参数错误, "请重新获取验证码！"));
            if (session.ToLower() != code.ToLower())
                return Ok(new RecoverModel(RecoverEnum.参数错误, "验证码错误！"));
            if (string.IsNullOrEmpty(seurityNo)) return Ok(new RecoverModel(RecoverEnum.失败, "请先设置安全码！"));
            var merchant = await new MerchantOperation().GetModelAsync(t => t.SeurityNo == seurityNo);
            if (merchant == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到商户信息！"));
            //查询商户正式用户
            var count = await userOperation.GetCountAsync(t => t.MerchantID == merchant._id
            && (t.Status == UserStatusEnum.冻结 || t.Status == UserStatusEnum.正常));
            if (count >= 200)
                return Ok(new RecoverModel(RecoverEnum.失败, "账号数达到最高限制！"));
            var user = await userOperation.GetModelAsync(t => t.LoginName == loginName && t.MerchantID == merchant._id);
            if (user != null) return Ok(new RecoverModel(RecoverEnum.参数错误, "已存在相同登录名称用户！"));
            string[] headList = { "default", "1", "2", "3", "4", "5", "6", "7", "8" };
            Random random = new Random();
            user = new User()
            {
                LoginName = loginName,
                Password = Utils.MD5(password),
                Level = UserLevelEnum.黄铜,
                Avatar = string.Format("UserImages/{0}.png", headList[random.Next(0, headList.Length)]),
                NickName = nickName,
                MerchantID = merchant._id,
                Status = UserStatusEnum.正常,
                OnlyCode = userOperation.GetNewUserOnlyCode()
            };
            //添加默认方案
            var backwaterSetupOperation = new BackwaterSetupOperation();
            var backwaterSetup = backwaterSetupOperation.GetModel(t => t.MerchantID == merchant._id, t => t.CreatedTime, true);
            if (backwaterSetup != null)
                user.ProgrammeID = backwaterSetup._id;
            VideoBackwaterSetupOperation videoBackwaterSetupOperation = new VideoBackwaterSetupOperation();
            var videoSetup = videoBackwaterSetupOperation.GetModel(t => t.MerchantID == merchant._id, t => t.CreatedTime, true);
            if (videoSetup != null)
                user.VideoProgrammeID = videoSetup._id;
            await userOperation.InsertModelAsync(user);
            return Ok(new RecoverModel(RecoverEnum.成功, "注册成功！"));
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NotUserAuthentication]
        public FileContentResult MixVerifyCode()
        {
            //string code = VerifyCodeHelper.GetSingleObj().CreateVerifyCode(VerifyCodeHelper.VerifyCodeType.MixVerifyCode);
            string code = VerifyCodeHelper.GetSingleObj().CreateVerifyCode(VerifyCodeHelper.VerifyCodeType.NumberVerifyCode);
            RedisOperation.UpdateString(HttpContext.GetIP(), code, 10);
            var bitmap = VerifyCodeHelper.GetSingleObj().CreateBitmapByImgVerifyCode(code, 100, 40);
            MemoryStream stream = new MemoryStream();
            bitmap.Save(stream, ImageFormat.Gif);
            return File(stream.ToArray(), "image/gif");
        }

        /// <summary>
        /// 验证安全码
        /// </summary>
        /// <param name="code">安全码</param>
        /// <returns></returns>
        [HttpGet]
        [NotUserAuthentication]
        public async Task<IActionResult> SetUpCode(string code)
        {
            var merchant = await merchantOperation.GetModelAsync(t => t.SeurityNo == code);
            if (merchant == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "安全码错误，请确认！"));
            return Ok(new RecoverModel(RecoverEnum.成功, "验证成功！"));
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="loginName">登录名称</param>
        /// <param name="password">登录密码</param>
        /// <param name="seurityNo">安全码</param>
        /// <remarks>
        ///##  参数说明
        ///     loginName：登录名称
        ///     password：登录密码
        ///     seurityNo：安全码
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             UserID：用户id
        ///             MerchantID：商户id
        ///             LoginName：用户登录名
        ///             NickName：用户昵称
        ///             UserMoney：用户积分
        ///             Avatar：头像
        ///             OnlyCode：唯一码
        ///             RoomNum：房间码
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        [NotUserAuthentication]
        public async Task<IActionResult> UserLogin(string loginName, string password, string seurityNo)
        {
            if (string.IsNullOrEmpty(seurityNo)) return Ok(new RecoverModel(RecoverEnum.失败, "请先验证安全码！"));
            var merchant = await new MerchantOperation().GetModelAsync(t => t.SeurityNo == seurityNo);
            if (merchant == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到商户信息！"));
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchant.AgentID);
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agent.HighestAgentID);
            if (merchant.MaturityTime <= DateTime.Now && setup.Formal)
                return Ok(new RecoverModel(RecoverEnum.身份过期, "商户已到期，请联系管理员！"));
            var user = await userOperation.GetModelAsync(t => t.LoginName == loginName
            && t.MerchantID == merchant._id && t.Password == Utils.MD5(password) && t.Status != UserStatusEnum.删除);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "用户帐号或密码错误！"));
            if (!(user.Status == UserStatusEnum.正常 || (user.Status == UserStatusEnum.假人 && user.IsSupport == false)))
            {
                if (user.Status == UserStatusEnum.冻结)
                    return Ok(new RecoverModel(RecoverEnum.身份冻结, "账号被冻结，无法登录！"));
            }
            var conn = Utils.GetConnID(user._id, merchant._id);
            if (conn != null)
            {
                //强制下线通知
                await RabbitMQHelper.SendGameMessage("1", merchant._id, "Subordinate", null, user._id);
            }
            #region 添加cookie
            var key = user._id.Replace("-", "");
            var token = Guid.NewGuid().ToString().Replace("-", "");
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "UserID", user._id },
                { "SeurityNo", merchant.SeurityNo },
                { "OnlyCode", user.OnlyCode.ToString() },
                { "MerchantID", merchant._id },
                { "Status", ((int)user.Status).ToString() },
                { "MaturityTime", merchant.MaturityTime.ToString("yyyy-MM-dd HH:mm:ss") },
                { "AgentID", agent.HighestAgentID },
                { "Token", token}
            };
            RedisOperation.UpdateCacheKey(key, dic, 20);
            #endregion
            user.LoginTime = DateTime.Now;
            await userOperation.UpdateModelAsync(user);
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "登录成功！",
                Status = RecoverEnum.成功,
                Model = new
                {
                    UserID = user._id,
                    MerchantID = merchant._id,
                    user.LoginName,
                    NickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                    user.UserMoney,
                    user.Avatar,
                    user.OnlyCode,
                    user.RoomNum,
                    user.IsAgent,
                    Key = key,
                    Token = token
                }
            });
        }

        /// <summary>
        /// 微信端登录
        /// </summary>
        /// <param name="unionid">unionid</param>
        /// <param name="seurityNo">安全码</param>
        /// <returns></returns> 
        [HttpGet]
        [NotUserAuthentication]
        public async Task<IActionResult> UserWeChatLogin(string unionid, string seurityNo)
        {
            if (string.IsNullOrEmpty(seurityNo)) return Ok(new RecoverModel(RecoverEnum.失败, "请先验证安全码！"));
            var merchant = await new MerchantOperation().GetModelAsync(t => t.SeurityNo == seurityNo);
            if (merchant == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到商户信息！"));
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchant.AgentID);
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agent.HighestAgentID);
            if (merchant.MaturityTime <= DateTime.Now && setup.Formal)
                return Ok(new RecoverModel(RecoverEnum.身份过期, "商户已到期，请联系管理员！"));
            var user = await userOperation.GetModelAsync(t => t.Unionid == unionid &&
            t.MerchantID == merchant._id && t.Status != UserStatusEnum.删除);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.失败, "请重新微信授权！"));
            if (!(user.Status == UserStatusEnum.正常 || (user.Status == UserStatusEnum.假人 && user.IsSupport == false)))
            {
                if (user.Status == UserStatusEnum.冻结)
                    return Ok(new RecoverModel(RecoverEnum.身份冻结, "账号被冻结，无法登录！"));
            }
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "用户帐号或密码错误！"));
            var conn = Utils.GetConnID(user._id, merchant._id);
            if (conn != null)
            {
                //强制下线通知
                await RabbitMQHelper.SendGameMessage("1", merchant._id, "Subordinate", null, user._id);
            }
            #region 添加cookie
            var key = user._id.Replace("-", "");
            var token = Guid.NewGuid().ToString().Replace("-", "");
            Dictionary<string, string> dic = new Dictionary<string, string>
            {
                { "UserID", user._id },
                { "SeurityNo", merchant.SeurityNo },
                { "OnlyCode", user.OnlyCode.ToString() },
                { "MerchantID", merchant._id },
                { "Status", ((int)user.Status).ToString() },
                { "MaturityTime", merchant.MaturityTime.ToString("yyyy-MM-dd HH:mm:ss") },
                { "AgentID", agent.HighestAgentID },
                { "Token", token}
            };
            RedisOperation.UpdateCacheKey(key, dic, 20);
            #endregion

            user.LoginTime = DateTime.Now;
            await userOperation.UpdateModelAsync(user);
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "登录成功！",
                Status = RecoverEnum.成功,
                Model = new
                {
                    UserID = user._id,
                    MerchantID = merchant._id,
                    user.LoginName,
                    NickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                    user.UserMoney,
                    user.Avatar,
                    user.OnlyCode,
                    user.RoomNum,
                    user.Unionid,
                    user.IsAgent,
                    Key = key,
                    Token = token
                }
            });
        }

        /// <summary>
        /// 获取游戏状态
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             IssueNum：期号
        ///             Number：号码
        ///             Surplus：剩余时间 秒
        ///             Status：游戏状态  等待中 = 1,封盘中 = 2,开奖中 = 3,已停售 = 4,未开奖 = 5,已关闭 = 6
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RoomListInit(GameOfType gameType)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            WebAppGameInfos gameInfos = await Utils.GetGameStatus(merchantID, gameType);
            return Ok(new RecoverClassModel<WebAppGameInfos>() { Message = "获取成功！", Model = gameInfos, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 用户发送消息
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="message">消息</param>
        /// <param name="uuid">标识</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UserSendMessage(GameOfType gameType, string message, string uuid)
        {

            var userID = HttpContext.Items["UserID"].ToString();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var conn = Utils.GetRoomGameConnID(userID, gameType, merchantID);
            if (!string.IsNullOrEmpty(conn))
                await context.Clients.Clients(conn).SendAsync("Feedback", uuid);
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            //Utils.Logger.Error(string.Format("{0}:发送消息 {1}", user.NickName, message));
            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            var gameStatus = RedisOperation.GetValue<Utils.GameNextLottery>("GameStatus", Enum.GetName(typeof(GameOfType), gameType));
            var nper = gameStatus.NextNper;
            var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
            #region 特殊操作
            if (message.Contains("上分") || message.Contains("下分") || message == "取消" || message == "全部取消" || message == "查" || message == "返水")
            {
                await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType);
                string output = string.Empty;
                try
                {
                    #region 上下分
                    if (message.Contains("上分"))
                    {
                        var amount = Convert.ToDecimal(message.Replace("上分", ""));
                        if (amount <= 0)
                        {
                            var result = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, gameType, nper);
                            await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                            return Ok(new RecoverModel(RecoverEnum.失败, "发送成功！"));
                        }

                        //判断用户类型
                        var userStatus = HttpContext.Items["Status"].ToString();
                        if (Convert.ToInt32(userStatus) == (int)UserStatusEnum.正常)
                        {
                            await CancelAnnouncement.UpperScore(userID, gameType, merchantID, amount);

                            if (reply.NoticeCheckRequest)
                            {
                                var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, gameType, nper);
                                await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                            }
                        }
                        else
                        {
                            var recordID = await CancelAnnouncement.UpperScore(userID, gameType, merchantID, amount, NotesEnum.虚拟);

                            if (reply.NoticeCheckRequest)
                            {
                                var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, gameType, nper);
                                await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                            }

                            //查询定时
                            RoomOperation roomOperation = new RoomOperation();
                            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
                            if (room == null)
                                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到房间信息！"));
                            if (room.ShamOnfirm)
                            {
                                await Common.SendShamUserApply(recordID, merchantID, room.ShamOnfirmTime);
                            }
                        }

                        return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                    }
                    else if (message.Contains("下分"))
                    {
                        var amount = Convert.ToDecimal(message.Replace("下分", ""));
                        if (amount <= 0)
                        {
                            var result = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, gameType, nper);
                            await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                            return Ok(new RecoverModel(RecoverEnum.失败, "发送成功！"));
                        }
                        if (user.UserMoney < amount)
                        {
                            await RabbitMQHelper.SendAdminMessage(string.Format("@{0} 积分不足，下分失败", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName), merchantID, gameType);
                            return Ok(new RecoverModel(RecoverEnum.失败, "发送成功！"));
                        }
                        //判断用户类型
                        var userStatus = HttpContext.Items["Status"].ToString();
                        if (Convert.ToInt32(userStatus) == (int)UserStatusEnum.正常)
                        {
                            await CancelAnnouncement.LowerScore(userID, gameType, merchantID, amount);
                            if (reply.NoticeCheckRequest)
                            {
                                var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, gameType, nper);
                                await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                                await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                            }
                        }
                        else
                        {
                            var recordID = await CancelAnnouncement.LowerScore(userID, gameType, merchantID, amount, NotesEnum.虚拟);
                            if (reply.NoticeCheckRequest)
                            {
                                var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, gameType, nper);
                                await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                                await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                            }
                            //查询定时
                            RoomOperation roomOperation = new RoomOperation();
                            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
                            if (room == null)
                                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到房间信息！"));
                            if (room.ShamOnfirm)
                            {
                                await Common.SendShamUserApply(recordID, merchantID, room.ShamOnfirmTime);
                            }
                        }
                        return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                    }
                    #endregion

                    #region 取消操作
                    if (message == "取消" || message == "全部取消")
                    {
                        //房间信息  禁止撤单
                        var roomSetup = await Utils.GetRoomInfosAsync(merchantID, gameType);
                        if (roomSetup.Revoke)
                        {
                            var result = await Utils.InstructionConversion(reply.ProhibitionCancel, userID, merchantID, gameType, nper);
                            await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                            return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                        }
                        var lottery = await Utils.GetGameStatus(merchantID, gameType);
                        if (lottery.Status != GameStatusEnum.等待中)
                        {
                            var result = await Utils.InstructionConversion(reply.ProhibitionCancel, userID, merchantID, gameType, nper);
                            await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                            return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                        }
                        //是否开启禁止撤单
                        var foundationSetup = RedisOperation.GetFoundationSetup(merchantID);
                        if (foundationSetup.ProhibitChe && lottery.Surplus <= foundationSetup.EntertainedFrontTime)
                        {
                            if (lottery.Surplus <= foundationSetup.EntertainedFrontTime)
                            {
                                var msg = await Utils.InstructionConversion(reply.ProhibitionCancel, userID, merchantID, gameType, nper);
                                await RabbitMQHelper.SendAdminMessage(msg, merchantID, gameType);
                                return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                            }
                        }
                        Tuple<string, decimal> qxresult;
                        if (message == "取消")
                            qxresult = await CancelAnnouncement.CancelOne(userID, gameType, merchantID, nper, reply);
                        else
                            qxresult = await CancelAnnouncement.CancelAll(userID, gameType, merchantID, nper, reply);
                        if (!string.IsNullOrEmpty(qxresult.Item1) && reply.NoticeCancel)
                        {
                            await RabbitMQHelper.SendAdminMessage(qxresult.Item1, merchantID, gameType);
                        }
                        if (qxresult.Item2 != 0)
                        {

                        }
                        await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                        return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                    }
                    #endregion

                    #region 查
                    if (message == "查")
                    {
                        output = await CancelAnnouncement.CheckStream(userID, gameType, merchantID, nper, reply);
                        await RabbitMQHelper.SendAdminMessage(output, merchantID, gameType);
                        return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                    }
                    #endregion

                    #region 返水
                    if (message == "返水")
                    {
                        var result = await BackwaterKind.UserBackwaterAsync(userID, gameType, merchantID);
                        if (result.Status != RecoverEnum.成功)
                        {
                            await RabbitMQHelper.SendAdminMessage(string.Format("@{0}{1}", nickName, result.Message), merchantID, gameType);
                        }
                        return Ok(result);
                    }
                    #endregion
                }
                catch
                {
                    if (reply.NoticeInvalidSub)
                    {
                        var result = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, gameType, nper);
                        await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                    }
                    return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                }
            }
            #endregion
            #region 下注
            string[] strChar = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "大", "小", "单", "双", "和", "龙",
            "虎", " ", "前三", "后三", "中三", "和", "通买", "豹子", "对子", "顺子","半顺", "杂六",
            "万个"};
            if (message.Contains(strChar))
            {
                var lottery = await Utils.GetGameStatus(merchantID, gameType);
                nper = lottery.NextIssueNum;
                if (lottery.Status == GameStatusEnum.封盘中)
                {
                    await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType);
                    if (reply.NoticeSealing)
                    {
                        var result = await Utils.InstructionConversion(reply.Sealing, userID, merchantID, gameType, nper);
                        await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                    }
                    return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                }
                else if (lottery.Status != GameStatusEnum.等待中)
                {
                    await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType);
                    var result = string.Format("@{0}{1}，禁止投注", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, Enum.GetName(typeof(GameStatusEnum), (int)lottery.Status));
                    await RabbitMQHelper.SendAdminMessage(result, merchantID, gameType);
                    return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                }
                else if (lottery.Status == GameStatusEnum.等待中)
                {
                    var charItem = message.Split(' ');
                    foreach (var str in charItem)
                    {
                        if (string.IsNullOrEmpty(str)) continue;
                        await RabbitMQHelper.SendUserMessage(str, userID, merchantID, gameType);
                        var result = new GameBetStatus();
                        var status = user.Status == UserStatusEnum.正常 ?
                            NotesEnum.正常 : NotesEnum.虚拟;
                        //Utils.Logger.Error(string.Format("{0}:开始下注{1}", user.NickName, str));
                        if (Utils.GameTypeItemize(gameType))
                            result = await General(userID, gameType, str, merchantID, nper, status);
                        else
                            result = await Special(userID, gameType, str, merchantID, nper, status);
                        //Utils.Logger.Error(string.Format("{0}:开始完成{1}", user.NickName, str));
                        if (result.Status == BetStatuEnum.正常)
                        {
                            await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                            if (reply.NoticeBetSuccess)
                            {
                                var msgResult = await Utils.InstructionConversion(reply.GameSuccess, userID, merchantID, gameType, nper, result.OddNum);
                                await RabbitMQHelper.SendAdminMessage(msgResult, merchantID, gameType);
                            }

                            if (user.Record)
                            {
                                //发送自己盘口
                                await RabbitMQHelper.SendMerchantHandicap(merchantID, userID);

                                var foundationSetup = RedisOperation.GetFoundationSetup(merchantID);
                                //房间设置
                                var roomSetup = await Utils.GetRoomInfosAsync(merchantID, gameType);
                                if (roomSetup.LotteryRecord == RecordType.飞单到外部网盘)
                                {
                                    //直接飞单
                                    if (roomSetup.Revoke || (foundationSetup.ProhibitChe && lottery.Surplus <= foundationSetup.EntertainedFrontTime))
                                    {
                                        await FlyingSheet.ProhibitionWithdrawal(merchantID, gameType, result.BetInfos, nper);
                                    }
                                    else
                                    {
                                        var orders = Utils.GetFlyingBet(gameType, result.BetInfos);
                                        var key = merchantID + Enum.GetName(typeof(GameOfType), gameType);
                                        RedisOperation.SetHash(key, result.OddNum, JsonConvert.SerializeObject(orders));
                                    }
                                }
                                else if (roomSetup.LotteryRecord == RecordType.飞单到高级商户)
                                {
                                    //飞单设置
                                    MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
                                    var merchantSheet = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);

                                    if (lottery.Surplus < foundationSetup.EntertainedFrontTime && merchantSheet != null && merchantSheet.OpenSheet)
                                    {
                                        var dicInfo = await Utils.GetMerchantFlySheetInfo(merchantID);
                                        if (dicInfo != null)
                                        {
                                            await FlyingSheet.MerchantInternalSheet(merchantID, dicInfo["TargetID"].ToString(), userID, dicInfo["UserID"].ToString(), gameType, str, nper);
                                        }
                                    }
                                }
                            }
                        }
                        else if (result.Status == BetStatuEnum.积分不足 && reply.NoticeInsufficientIntegral)
                        {
                            var msgResult = await Utils.InstructionConversion(reply.NotEnough, userID, merchantID, gameType, nper);
                            await RabbitMQHelper.SendAdminMessage(msgResult, merchantID, gameType);
                        }
                        else if (result.Status == BetStatuEnum.限额 && reply.NoticeQuota)
                        {
                            await RabbitMQHelper.SendAdminMessage(string.Format("@{0} {1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, result.OutPut), merchantID, gameType);
                        }
                        else if (result.Status == BetStatuEnum.格式错误 && reply.NoticeInvalidSub)
                        {
                            var msgResult = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, gameType, nper);
                            await RabbitMQHelper.SendAdminMessage(msgResult, merchantID, gameType);
                        }
                    }
                    //Utils.Logger.Error(string.Format("{0}:发送消息流程结束{1}", user.NickName, message));
                    return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                }
            }
            #endregion
            if (!user.Talking)
            {
                await RabbitMQHelper.SendAdminMessage(string.Format("@{0}禁止发送聊天消息", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName), merchantID, gameType);
                return Ok(new RecoverModel(RecoverEnum.失败, "发送失败！"));
            }
            else
            {
                await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType);
                return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
            }
        }

        /// <summary>
        /// 视讯游戏发送消息
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="message">消息</param>
        /// <param name="znum">桌号</param>
        /// <param name="uuid"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> VideoUserSendMessage(BaccaratGameType gameType, string message, int znum, string uuid)
        {
            var userID = HttpContext.Items["UserID"].ToString();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var conn = await Utils.GetBaccarat(merchantID, userID, znum.ToString(), gameType);
            if (!string.IsNullOrEmpty(conn))
                await context.Clients.Clients(conn).SendAsync("Feedback", uuid);
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            var infos = RedisOperation.GetValue<GameStatic>("Baccarat", znum.ToString());
            var nper = infos.Scene;
            var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
            var status = await Common.GetVideoGameStatus(merchantID, gameType);
            if (!status)
            {
                await RabbitMQHelper.SendBaccaratAdminMessage(string.Format("@{0} 游戏房间已关闭，禁止下注！", nickName), merchantID, znum, gameType);
                return Ok(new RecoverModel(RecoverEnum.失败, "游戏房间已关闭！"));
            }
            #region 特殊操作
            if (message.Contains("上分") || message.Contains("下分") || message == "取消" || message == "全部取消" || message == "查" || message == "返水")
            {
                await RabbitMQHelper.SendVideoUserMessage(message, userID, merchantID, gameType, znum);
                string output = string.Empty;
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
                            return Ok(new RecoverModel(RecoverEnum.失败, "发送成功！"));
                        }

                        //判断用户类型
                        var userStatus = HttpContext.Items["Status"].ToString();
                        if (Convert.ToInt32(userStatus) == (int)UserStatusEnum.正常)
                        {
                            await CancelAnnouncement.UpperScore(userID, null, merchantID, amount, baccaratGameType: gameType, znum: znum);

                            if (reply.NoticeCheckRequest)
                            {
                                var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, null, nper, null, gameType);
                                await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                            }
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
                                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到房间信息！"));
                            if (room.ShamOnfirm)
                            {
                                await Common.SendShamUserApply(recordID, merchantID, room.ShamOnfirmTime);
                            }
                        }
                        return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                    }
                    else if (message.Contains("下分"))
                    {
                        var amount = Convert.ToDecimal(message.Replace("下分", ""));
                        if (amount <= 0)
                        {
                            var result = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, null, nper, null, gameType);
                            await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                            return Ok(new RecoverModel(RecoverEnum.失败, "发送成功！"));
                        }
                        if (user.UserMoney < amount)
                        {
                            await RabbitMQHelper.SendBaccaratAdminMessage(string.Format("@{0} 积分不足，下分失败", nickName), merchantID, znum, gameType);
                            return Ok(new RecoverModel(RecoverEnum.失败, "发送成功！"));
                        }
                        //判断用户类型
                        var userStatus = HttpContext.Items["Status"].ToString();
                        if (Convert.ToInt32(userStatus) == (int)UserStatusEnum.正常)
                        {
                            await CancelAnnouncement.LowerScore(userID, null, merchantID, amount, NotesEnum.正常, gameType, znum);
                            if (reply.NoticeCheckRequest)
                            {
                                var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, null, nper, null, gameType);
                                await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                            }
                        }
                        else
                        {
                            var recordID = await CancelAnnouncement.LowerScore(userID, null, merchantID, amount, NotesEnum.虚拟, gameType, znum);
                            if (reply.NoticeCheckRequest)
                            {
                                var result = await Utils.InstructionConversion(reply.ReceivingRequests, userID, merchantID, null, nper, null, gameType);
                                await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                                await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                            }
                            //查询定时
                            RoomOperation roomOperation = new RoomOperation();
                            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
                            if (room == null)
                                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到房间信息！"));
                            if (room.ShamOnfirm)
                            {
                                await Common.SendShamUserApply(recordID, merchantID, room.ShamOnfirmTime);
                            }
                        }
                        return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                    }
                    #endregion

                    #region 取消操作
                    if (message == "取消" || message == "全部取消")
                    {
                        //房间信息  禁止撤单
                        var roomSetup = await Utils.GetVideoRoomInfosAsync(merchantID, gameType);
                        //是否开启禁止撤单
                        var foundationSetup = RedisOperation.GetVideoFoundationSetup(merchantID);
                        if (roomSetup.Revoke || infos.Cstate != "init" || foundationSetup.ProhibitChe)
                        {
                            var result = await Utils.InstructionConversion(reply.ProhibitionCancel, userID, merchantID, null, nper, null, gameType);
                            await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                            return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                        }
                        Tuple<string, decimal> qxresult;
                        if (message == "取消")
                            qxresult = await CancelAnnouncement.CancelVideoOne(userID, gameType, merchantID, nper, reply, znum);
                        else
                            qxresult = await CancelAnnouncement.CancelVideoAll(userID, gameType, merchantID, nper, reply, znum);
                        if (!string.IsNullOrEmpty(qxresult.Item1) && reply.NoticeCancel)
                        {
                            await RabbitMQHelper.SendBaccaratAdminMessage(qxresult.Item1, merchantID, znum, gameType);
                        }
                        if (qxresult.Item2 != 0)
                        {

                        }
                        await RabbitMQHelper.SendUserPointChange(userID, merchantID);
                        return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                    }
                    #endregion

                    #region 查
                    if (message == "查")
                    {
                        var result = await Utils.InstructionConversion(reply.CheckScore + "\r\n" + reply.CheckStream, userID, merchantID, null, nper, null, gameType);
                        await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                        return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                    }
                    #endregion

                    #region 返水
                    if (message == "返水")
                    {
                        var result = await BackwaterKind.UserVideoBackwaterAsync(userID, gameType, merchantID);
                        if (result.Status != RecoverEnum.成功)
                        {
                            await RabbitMQHelper.SendBaccaratAdminMessage(string.Format("@{0}{1}", nickName, result.Message), merchantID, znum, gameType);
                        }
                        return Ok(result);
                    }
                    #endregion
                }
                catch
                {
                    if (reply.NoticeInvalidSub)
                    {
                        var result = await Utils.InstructionConversion(reply.CommandError, userID, merchantID, null, nper, null, gameType);
                        await RabbitMQHelper.SendBaccaratAdminMessage(result, merchantID, znum, gameType);
                    }
                    return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
                }
            }
            #endregion
            #region 下注
            var strChar = new List<string>()
                {
                    "庄","闲","和","庄对","闲对", "任意对子"
                };
            if (message.Contains(strChar))
            {
                await RabbitMQHelper.SendVideoUserMessage(message, userID, merchantID, gameType, znum);

                //获取当前房间状态
                if (infos.Cstate != "init")
                {
                    await RabbitMQHelper.SendBaccaratAdminMessage(string.Format("@{0} 未到下注时间，禁止下注！", nickName), merchantID, znum, gameType);
                    return Ok(new RecoverModel(RecoverEnum.失败, "未到下注时间！"));
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
                            await RabbitMQHelper.SendUserPointChange(userID, merchantID);
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
                return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
            }
            #endregion
            if (!user.Talking)
            {
                await RabbitMQHelper.SendBaccaratAdminMessage(string.Format("@{0}禁止发送聊天消息", nickName), merchantID, znum, gameType);
                return Ok(new RecoverModel(RecoverEnum.失败, "发送失败！"));
            }
            else
            {
                await RabbitMQHelper.SendVideoUserMessage(message, userID, merchantID, gameType, znum);
                return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
            }
        }

        /// <summary>
        /// 获取用户积分
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserIntegral()
        {

            var userID = HttpContext.Items["UserID"].ToString();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到用户！"));
            return Ok(new RecoverClassModel<dynamic>() { Message = "获取成功", Model = user.UserMoney, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 获取游戏历史记录
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="start">开始页</param>
        /// <param name="pageSize">每页数量</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGameHistory(GameOfType gameType, int start = 1, int pageSize = 10)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var result = new List<WebGameHistory>();
            long total = 0;
            if (Utils.GameTypeItemize(gameType))
            {
                Lottery10Operation lottery10Operation = new Lottery10Operation();
                var list = lottery10Operation.GetModelListByPaging(t => t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID),
                        t => t.IssueNum, false, start, pageSize);
                total = await lottery10Operation.GetCountAsync(t => t.GameType == gameType);
                result = Utils.GetGameHistories10(list);
            }
            else
            {
                Lottery5Operation lottery5Operation = new Lottery5Operation();
                var list = lottery5Operation.GetModelListByPaging(t => t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID),
                        t => t.IssueNum, false, start, pageSize);
                total = await lottery5Operation.GetCountAsync(t => t.GameType == gameType);
                result = Utils.GetGameHistories5(list);
            }
            return Ok(new RecoverListModel<WebGameHistory>() { Data = result, Message = "获取成功！", Status = RecoverEnum.成功, Total = total });
        }

        /// <summary>
        /// 获取上分二维码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetQRCode()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var room = await new RoomOperation().GetModelAsync(t => t.MerchantID == merchantID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到房间信息！"));
            return Ok(new RecoverClassModel<SubAccount>()
            {
                Message = "获取成功！",
                Model = room.SubAccount,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取各个游戏赔率
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGameOdds(GameOfType gameType)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (!Utils.GameTypeItemize(gameType))
            {
                var oddsSpecialOperation = new OddsSpecialOperation();
                var model = await oddsSpecialOperation.GetModelAsync(merchantID, gameType);
                if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到赔率数据！"));
                var result = Utils.GetWebOddsData<OddsSpecial, WebOddsSpecial>(model);
                return Ok(new RecoverClassModel<WebOddsSpecial>() { Message = "获取成功！", Model = result, Status = RecoverEnum.成功 });
            }
            else
            {
                var oddsOrdinaryOperation = new OddsOrdinaryOperation();
                var model = await oddsOrdinaryOperation.GetModelAsync(merchantID, gameType);
                if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到赔率数据！"));
                var result = Utils.GetWebOddsData<OddsOrdinary, WebOddsOrdinary>(model);
                return Ok(new RecoverClassModel<WebOddsOrdinary>() { Message = "获取成功！", Model = result, Status = RecoverEnum.成功 });
            }
        }

        /// <summary>
        /// 获取视讯游戏赔率
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoGameOdds(BaccaratGameType gameType)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (gameType == BaccaratGameType.百家乐)
            {
                OddsBaccaratOperation oddsBaccaratOperation = new OddsBaccaratOperation();
                var data = await oddsBaccaratOperation.GetModelAsync(t => t.MerchantID == merchantID);
                var result = new
                {
                    data.AnyPair,
                    data.Banker,
                    data.BankerPair,
                    data.He,
                    data.Player,
                    data.PlayerPair
                };
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Message = "获取成功！",
                    Model = result,
                    Status = RecoverEnum.成功
                });
            }
            return Ok();
        }

        /// <summary>
        /// 获取用户帐变记录
        /// </summary>
        /// <param name="time">查询时间</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             Amount：变动分数
        ///             Balance：变动后分数
        ///             Time：时间
        ///             Type：类型
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserAccountChange(DateTime time)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID);
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            var startTime = time.Date;
            var endTime = time.Date.AddDays(1);
            var list = userIntegrationOperation.GetModelList(t => t.MerchantID == merchantID
            && t.UserID == userID && t.CreatedTime >= startTime && t.CreatedTime <= endTime
            && t.Management == ManagementEnum.已同意, t => t.CreatedTime, false);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                string message = string.Empty;
                if (data.ChangeTarget == ChangeTargetEnum.中奖
                    || data.ChangeTarget == ChangeTargetEnum.投注)
                {
                    message = data.Message + "\r\n" + data.Remark;
                }
                else
                    message = data.Message;
                var home = new
                {
                    data.Amount,
                    data.Balance,
                    Time = data.CreatedTime.ToString("yyyy-MM-dd HH:mm"),
                    Type = message
                };
                result.Add(home);
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "获取成功！",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 获取用户充值记录
        /// </summary>
        /// <param name="history">true:今日  false:历史</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserImpulse(bool history)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var list = history ?
                new UserIntegrationOperation().GetModelList(t => t.UserID == userID && t.MerchantID == merchantID &&
            t.ChangeTarget == ChangeTargetEnum.申请 && t.ChangeType == ChangeTypeEnum.上分 && t.CreatedTime >= DateTime.Today && t.CreatedTime <= DateTime.Now,
            t => t.CreatedTime, false) :
            new UserIntegrationOperation().GetModelList(t => t.UserID == userID && t.MerchantID == merchantID &&
            t.ChangeTarget == ChangeTargetEnum.申请 && t.ChangeType == ChangeTypeEnum.上分,
            t => t.CreatedTime, false);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                var home = new
                {
                    UserID = userID,
                    data.Amount,
                    data.Balance,
                    ApplyTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    data.Message,
                    data.Remark
                };
                result.Add(home);
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "获取成功！",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 获取用户提现记录
        /// </summary>
        /// <param name="history">true:今日  false:历史</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetUserWithdrawal(bool history)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var list = history ?
                new UserIntegrationOperation().GetModelList(t => t.UserID == userID && t.MerchantID == merchantID &&
            t.ChangeTarget == ChangeTargetEnum.申请 && t.ChangeType == ChangeTypeEnum.下分 && t.CreatedTime >= DateTime.Today && t.CreatedTime <= DateTime.Now,
            t => t.CreatedTime, false) :
            new UserIntegrationOperation().GetModelList(t => t.UserID == userID && t.MerchantID == merchantID &&
            t.ChangeTarget == ChangeTargetEnum.申请 && t.ChangeType == ChangeTypeEnum.下分,
            t => t.CreatedTime, false);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                var home = new
                {
                    UserID = userID,
                    data.Amount,
                    data.Balance,
                    ApplyTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    data.Message,
                    data.Remark
                };
                result.Add(home);
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "获取成功！",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 获取用户绑定帐户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserAccountInfo()
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到用户信息！"));
            var result = new
            {
                user.WeChat,
                user.WeChatUrl,
                user.Alipay,
                user.AlipayUrl
            };
            return Ok(new RecoverClassModel<dynamic>() { Message = "查询成功！", Model = result, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 修改用户微信帐户
        /// </summary>
        /// <param name="weChat">微信帐户</param>
        /// <param name="fileinput">图片</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUserWeChat(string weChat, IFormFile fileinput)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到用户信息！"));
            user.WeChat = weChat;
            var url = await BlobHelper.UploadImageToBlob(fileinput, "WeChat");
            if (!string.IsNullOrEmpty(url))
            {
                if (url == "1") return Ok(new RecoverModel(RecoverEnum.失败, "图片大小最大为20M！"));
                user.WeChatUrl = url;
            }
            await userOperation.UpdateModelAsync(user);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 修改用户支付宝帐户
        /// </summary>
        /// <param name="alipay">支付宝帐户</param>
        /// <param name="fileinput">图片</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUserAlipay(string alipay, IFormFile fileinput)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到用户信息！"));
            user.Alipay = alipay;
            var url = await BlobHelper.UploadImageToBlob(fileinput, "Alipay");
            if (!string.IsNullOrEmpty(url))
            {
                if (url == "1") return Ok(new RecoverModel(RecoverEnum.失败, "图片大小最大为20M！"));
                user.AlipayUrl = url;
            }
            await userOperation.UpdateModelAsync(user);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 用户退出
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult UserExit()
        {
            var key = HttpContext.Request.Headers["UserAuthorization"].ToString();
            RedisOperation.DeleteKey(key);
            HttpContext.Session.Clear();
            return Ok(new RecoverModel(RecoverEnum.成功, "退出成功！"));
        }

        /// <summary>
        /// 设置房间号（推广码）
        /// </summary>
        /// <param name="roomNum"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SetUserRoomNum(string roomNum)
        {
            var userID = HttpContext.Items["UserID"].ToString();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var room = await new RoomOperation().GetModelAsync(t => t.MerchantID == merchantID);
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            if (room.RoomNum == roomNum)
            {
                //取消代理
                var userAgent = await agentBackwaterOperation.GetModelAsync(t => t.ExtensionCode == user.RoomNum && t.MerchantID == merchantID);
                if (userAgent != null)
                {
                    userAgent.Offline.RemoveAll(t => t.UserID == userID);
                    await agentBackwaterOperation.UpdateModelAsync(userAgent);
                }
                user.RoomNum = roomNum;
                await userOperation.UpdateModelAsync(user);
                return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
            }
            if (user.Status == UserStatusEnum.假人)
                return Ok(new RecoverModel(RecoverEnum.失败, "游客不能设置推广码！"));
            var agent = await agentBackwaterOperation.GetModelAsync(t => t.ExtensionCode == roomNum && t.MerchantID == merchantID);
            if (agent == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到此房间！"));
            if (userID == agent.UserID) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到此房间！"));

            if (!agent.Offline.Exists(t => t.UserID == userID))
            {
                agent.Offline.Add(new OfflineUser()
                {
                    UserID = userID
                });
                await agentBackwaterOperation.UpdateModelAsync(agent);
            }
            user.RoomNum = roomNum;
            await userOperation.UpdateModelAsync(user);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }

        /// <summary>
        /// 获取客服信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetCustomerInfo()
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var room = await new RoomOperation().GetModelAsync(t => t.MerchantID == merchantID);
            var result = new
            {
                room.QQ,
                room.WeChat,
                room.CustomerUrl,
                room.CustomerOpen
            };
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功！",
                Model = result,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 进入游戏房间
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> JoinGameRoom(GameOfType gameType)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var bsonData = await Utils.GetRoomInfosAsync(merchantID, gameType);
            if (bsonData.Status == RoomStatus.关闭)
                return Ok(new RecoverModel(RecoverEnum.失败, "游戏房间已关闭！"));
            var user = await userOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == userID);
            if (bsonData.Minin > user.UserMoney)
                return Ok(new RecoverModel(RecoverEnum.失败, string.Format("该房间游戏资金不能低于{0}！", bsonData.Minin)));
            var room = await new RoomOperation().GetModelAsync(t => t.MerchantID == merchantID);
            if (user.RoomNum != room.RoomNum)
            {
                var agentList = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.正常 && t.IsAgent == true);
                var userIDList = agentList.Select(t => t._id).ToList();
                AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
                FilterDefinition<AgentBackwater> filter = agentBackwaterOperation.Builder.Where(t => t.MerchantID == merchantID && t.ExtensionCode == user.RoomNum)
                    & agentBackwaterOperation.Builder.In(t => t.UserID, userIDList);
                var agentInfo = await agentBackwaterOperation.GetModelListAsync(filter);
                if (agentInfo.IsNull())
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "请重新设置房间号"));
            }
            var roomSetup = await Utils.GetRoomInfosAsync(merchantID, gameType);
            return Ok(new RecoverKeywordModel(RecoverEnum.成功, "进入成功!", roomSetup.Instructions.ToString()));
        }

        /// <summary>
        /// 用户聊天 
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ChatMessage(string message, GameOfType gameType)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID && t.Status == UserStatusEnum.正常);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到用户信息！"));
            if (!user.Talking)
            {
                await RabbitMQHelper.SendAdminMessage(string.Format("@{0}禁止发送聊天消息", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName), merchantID, gameType);
                return Ok(new RecoverModel(RecoverEnum.失败, "发送失败！"));
            }
            else
            {
                await RabbitMQHelper.SendUserMessage(message, userID, merchantID, gameType);
                //await SignalRSendMessage.SendUserMessage(message, userID, merchantID, gameType, context);
                return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
            }
        }

        /// <summary>
        /// 获取头像列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetHeadList()
        {
            List<string> result = new List<string>
            {
                "UserImages/default.png",
                "UserImages/1.png",
                "UserImages/2.png",
                "UserImages/3.png",
                "UserImages/4.png",
                "UserImages/5.png",
                "UserImages/6.png",
                "UserImages/7.png",
                "UserImages/8.png"
            };
            return Ok(new RecoverListModel<string>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 修改用户头像
        /// </summary>
        /// <param name="pagName">图片名称(只传名称和格式  1.png)</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUserHead(string pagName)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询用户信息！"));
            user.Avatar = pagName;
            await userOperation.UpdateModelAsync(user);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 用户申请回水
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> UserApplyBack(GameOfType gameType)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            await RabbitMQHelper.SendUserMessage("返水", userID, merchantID, gameType);
            var result = await BackwaterKind.UserBackwaterAsync(userID, gameType, merchantID);
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (result.Status != RecoverEnum.成功)
            {
                await RabbitMQHelper.SendAdminMessage(string.Format("@{0}{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, result.Message), merchantID, gameType);
            }
            return Ok(result);
        }

        /// <summary>
        /// 获取积分  盈亏  流水
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             Integral：积分
        ///             Flow：流水
        ///             ProLoss：盈亏
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAccountChange()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
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
            var betList = await collection.FindListAsync(t => t.UserID == userID
            && t.CreatedTime >= startTime && t.CreatedTime <= endTime && t.BetStatus == BetStatus.已开奖);

            //流水
            var water = betList.Sum(t => t.AllUseMoney);
            //盈亏
            var proLoss = betList.Sum(t => t.AllMediumBonus) - betList.Sum(t => t.AllUseMoney);

            var data = new
            {
                Integral = user.UserMoney,
                Flow = water,
                ProLoss = proLoss
            };
            return Ok(new RecoverClassModel<dynamic>
            {
                Message = "获取成功",
                Model = data,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取下注信息
        /// </summary>
        /// <param name="betTime">时间</param>
        /// <param name="gameType">游戏类型  全部传空</param>
        /// <param name="vgameType">视讯游戏类型  全部传空</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             GameType：游戏类型
        ///             Nper：期号
        ///             Remark：投注内容
        ///             BetStatus：状态
        ///             ProLoss：盈亏,
        ///             Flow：流水,
        ///             VGameType：视讯游戏类型
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBetInfo(DateTime betTime, GameOfType? gameType, BaccaratGameType? vgameType)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var startTime = betTime.Date.AddHours(6);
            var endTime = betTime.Date.AddHours(6).AddDays(1);
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            FilterDefinition<UserBetInfo> filter = userBetInfoOperation.Builder.Where(t => t.UserID == userID
            && t.CreatedTime >= startTime && t.CreatedTime <= endTime);

            BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
            var vcollection = baccaratBetOperation.GetCollection(merchantID);
            FilterDefinition<BaccaratBet> vfilter = baccaratBetOperation.Builder.Where(t => t.UserID == userID
            && t.CreatedTime >= startTime && t.CreatedTime <= endTime);

            var data = new List<GetBetInfoClass>();
            if (gameType != null)
            {
                filter &= userBetInfoOperation.Builder.Eq(t => t.GameType, gameType.Value);
                vfilter = null;
            }
            else if (vgameType != null)
            {
                filter = null;
                vfilter &= baccaratBetOperation.Builder.Eq(t => t.GameType, vgameType.Value);
            }

            //查询数据
            if (filter != null)
            {
                try
                {
                    var betList = await collection.FindListAsync(filter);
                    foreach (var info in betList)
                    {
                        foreach (var remarks in info.BetRemarks)
                        {
                            data.Add(new GetBetInfoClass
                            {
                                GameType = gameType,
                                Nper = info.Nper,
                                Remark = remarks.Remark,
                                BetStatus = Enum.GetName(typeof(BetStatus), info.BetStatus),
                                ProLoss = info.BetStatus == BetStatus.未开奖 ? 0 : remarks.OddBets.Sum(t => t.MediumBonus) - remarks.OddBets.Sum(t => t.BetMoney),
                                Flow = remarks.OddBets.Sum(t => t.BetMoney),
                                CreateTime = remarks.BetTime
                            });
                        }
                    }
                }
                catch (FormatException)
                { 
                    
                }
            }
            //查询视讯数据
            if (vfilter != null)
            {
                var betList = await vcollection.FindListAsync(vfilter);
                foreach (var info in betList)
                {
                    foreach (var remarks in info.BetRemarks)
                    {
                        data.Add(new GetBetInfoClass
                        {
                            GameType = gameType,
                            Nper = info.Nper,
                            Remark = remarks.Remark,
                            BetStatus = Enum.GetName(typeof(BetStatus), info.BetStatus),
                            ProLoss = info.BetStatus == BetStatus.未开奖 ? 0 : remarks.MediumBonus - remarks.BetMoney,
                            Flow = remarks.BetMoney,
                            CreateTime = remarks.BetTime
                        });
                    }
                }
            }

            //排序
            return Ok(new RecoverListModel<GetBetInfoClass>()
            {
                Data = data.OrderByDescending(t => t.CreateTime).ToList(),
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = data.Count
            });
        }

        private class GetBetInfoClass
        {
            public GameOfType? GameType { get; set; }

            public BaccaratGameType? VGameType { get; set; }

            public string Nper { get; set; }
            public string Remark { get; set; }
            public string BetStatus { get; set; }
            public decimal ProLoss { get; set; }

            public decimal Flow { get; set; }

            public DateTime CreateTime { get; set; }
        }

        /// <summary>
        /// 申请上下分
        /// </summary>
        /// <param name="type">上下分  1：上分  2：下分</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ApplyIntegral(ChangeTypeEnum type, decimal amount)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到用户信息！"));
            UserIntegration userIntegration = new UserIntegration();
            //下分
            if (type == ChangeTypeEnum.下分)
            {
                if (amount <= 0)
                    return Ok(new RecoverModel(RecoverEnum.失败, "金额错误！"));
                if (user.UserMoney < amount) return Ok(new RecoverModel(RecoverEnum.失败, "下分失败，用户积分不足！"));
                //userIntegration = new UserIntegration()
                //{
                //    UserID = userID,
                //    MerchantID = merchantID,
                //    OrderStatus = OrderStatusEnum.申请下分,
                //    Amount = amount,
                //    Balance = user.UserMoney,
                //    ChangeTarget = ChangeTargetEnum.申请,
                //    ChangeType = ChangeTypeEnum.下分,
                //    Management = ManagementEnum.未审批,
                //    Message = "申请下分",
                //    Remark = "申请下分",
                //    Notes = user.Status == UserStatusEnum.正常 ? NotesEnum.正常 : NotesEnum.虚拟
                //};
                await CancelAnnouncement.LowerScore(userID, null, merchantID, amount);
                await RabbitMQHelper.SendUserPointChange(userID, merchantID);
            }
            //上分
            else
            {
                userIntegration = new UserIntegration()
                {
                    UserID = userID,
                    MerchantID = merchantID,
                    OrderStatus = OrderStatusEnum.申请上分,
                    Amount = amount,
                    Balance = user.UserMoney,
                    ChangeTarget = ChangeTargetEnum.申请,
                    ChangeType = ChangeTypeEnum.上分,
                    Management = ManagementEnum.未审批,
                    Message = "申请上分",
                    Remark = "申请上分",
                    Notes = user.Status == UserStatusEnum.正常 ? NotesEnum.正常 : NotesEnum.虚拟
                };
            };
            await userIntegrationOperation.InsertModelAsync(userIntegration);

            if (user.Status == UserStatusEnum.假人)
            {
                //自动上下分
                //查询定时
                RoomOperation roomOperation = new RoomOperation();
                var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
                if (room == null)
                    return Ok(new RecoverModel(RecoverEnum.失败, "未查询到房间信息！"));
                if (room.ShamOnfirm)
                {
                    await Common.SendShamUserApply(userIntegration._id, merchantID, room.ShamOnfirmTime);
                }
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "申请成功！"));
        }

        /// <summary>
        /// 获取上下分记录
        /// </summary>
        /// <param name="type">上下分  1：上分  2：下分</param>
        /// <param name="time">申请时间</param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SearchRechargeList(ChangeTypeEnum type, DateTime time)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var startTime = time.Date;
            var endTime = time.Date.AddDays(1);
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            var list = userIntegrationOperation.GetModelList(t => t.MerchantID == merchantID && t.UserID == userID
            && t.CreatedTime >= startTime && t.CreatedTime <= endTime && t.ChangeType == type && (t.Management == ManagementEnum.已同意 || t.Management == ManagementEnum.已拒绝)
            && (t.ChangeTarget == ChangeTargetEnum.手动 || t.ChangeTarget == ChangeTargetEnum.申请), t => t.CreatedTime, false);
            var result = new List<dynamic>();
            foreach (var lt in list)
            {
                var data = new
                {
                    Time = lt.LastUpdateTime.ToString("yyyy-MM-dd HH:mm"),
                    Amount = lt.Amount.ToString("#0.00"),
                    Status = Enum.GetName(typeof(ManagementEnum), (int)lt.Management)
                };
                result.Add(data);
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 修改用户密码
        /// </summary>
        /// <param name="loginName">登录名称</param>
        /// <param name="nickName">昵称</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUesrPwdInfo(string loginName, string nickName, string password)
        {
            loginName = loginName.Trim();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到用户信息！"));
            //是否存在
            var exic = await userOperation.GetModelAsync(t => t.LoginName == loginName && t.MerchantID == merchantID && t._id != userID && t.Status != UserStatusEnum.删除);
            if (exic == null)
            {
                user.NickName = nickName;
                if (string.IsNullOrEmpty(user.LoginName))
                    user.LoginName = loginName;
                if (!string.IsNullOrEmpty(password))
                {
                    user.Password = Utils.MD5(password);

                    //删除缓存
                    RedisOperation.DeleteKey(userID);
                    HttpContext.Session.Clear();
                    await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                }
                await userOperation.UpdateModelAsync(user);
            }
            else
            {
                if (exic.Status == UserStatusEnum.假人)
                    return Ok(new RecoverModel(RecoverEnum.失败, "该帐户不能绑定！"));

                if (!string.IsNullOrEmpty(user.LoginName))
                    return Ok(new RecoverModel(RecoverEnum.失败, "参数错误"));

                //判断密码是否相同
                if (Utils.MD5(password) != exic.Password)
                    return Ok(new RecoverModel(RecoverEnum.失败, "绑定帐号失败！"));

                exic.NickName = nickName;
                exic.Unionid = user.Unionid;
                user.Unionid = null;
                exic.Avatar = user.Avatar;
                await userOperation.UpdateModelAsync(exic);
                await userOperation.UpdateModelAsync(user);
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 验证帐号
        /// </summary>
        /// <param name="loginName">登录名称</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> VerifyAccount(string loginName, string password)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到用户信息！"));
            //是否存在
            var exic = await userOperation.GetModelAsync(t => t.LoginName == loginName && t.MerchantID == merchantID && t._id != userID && t.Status != UserStatusEnum.删除 && t.Password == Utils.MD5(password));
            if (exic == null)
            {
                return Ok(new RecoverModel(RecoverEnum.失败, "帐号验证失败！"));
            }
            else
            {
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        LoginName = loginName,
                        Balance = exic.UserMoney
                    },
                    Message = "验证成功",
                    Status = RecoverEnum.成功
                });
            }
        }

        /// <summary>
        /// 获取微信开关
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NotUserAuthentication]
        public async Task<IActionResult> GetSwitch()
        {
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new
                {
                    Keyword = true,
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
                return Ok(new
                {
                    Keyword = model.WeChatSwitch,
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
        }

        /// <summary>
        /// 获取房间名称
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoomName(GameOfType gameType)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetRoomByMerchantID(merchantID);
            var roomInfo = await Utils.GetRoomInfosAsync(merchantID, gameType);
            if (roomInfo == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到房间信息！"));
            var roomName = roomInfo.GameRoomName;
            return Ok(new RecoverKeywordModel()
            {
                Keyword = roomName,
                Status = RecoverEnum.成功,
                Message = "获取成功"
            });
        }

        /// <summary>
        /// 获取游戏视频地址
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGameVideoUrl(GameOfType gameType)
        {
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new
                {
                    Keyword = "",
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
            {
                var gameVideo = model.GameVideos.Find(t => t.GameType == gameType);
                var url = gameVideo == null ? "" : gameVideo.Url;
                return Ok(new
                {
                    Keyword = url,
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            }
        }

        /// <summary>
        /// 获取游戏信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NotUserAuthentication]
        public IActionResult GetGameList()
        {
            var result = Utils.GetGameList();
            return Ok(new RecoverListModel<Utils.GameListItem>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 随机获取一个可使用域名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NotUserAuthentication]
        public async Task<IActionResult> GetAppUrls()
        {
            var platformSetUpOperation = new PlatformSetUpOperation();
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new RecoverKeywordModel()
                {
                    Keyword = null,
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            if (model.AppUrls.IsNull())
                return Ok(new RecoverKeywordModel()
                {
                    Keyword = null,
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            var list = model.AppUrls.FindAll(t => t.Status);
            if (list.IsNull())
                return Ok(new RecoverKeywordModel()
                {
                    Keyword = null,
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            Random random = new Random();
            var info = list[random.Next(0, list.Count)];
            return Ok(new RecoverKeywordModel()
            {
                Keyword = info.Url,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取用户代理报表
        /// </summary>
        /// <param name="dateTime">时间  日期</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserAgentInfo(DateTime dateTime, GameOfType? gameType)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userID = HttpContext.Items["UserID"].ToString();
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t.MerchantID == merchantID
            && t._id == userID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到用户信息！"));
            if (!user.IsAgent)
                return Ok(new RecoverModel(RecoverEnum.失败, "此用户不为代理！"));
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            var agentBackInfo = await agentBackwaterOperation.GetModelAsync(t => t.MerchantID == merchantID && t.UserID == user._id);
            if (agentBackInfo == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到用户信息！")); ;
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var result = new List<WebAgentReport>();
            //var offline = agentBackInfo.Offline.FindAll(t => t.AddTime <= dateTime.AddDays(1));
            //下级数量
            var subCount = agentBackInfo.Offline.Count;
            var list = new List<AppAgentReport>();
            var tasks = new List<Task>();
            foreach (var info in agentBackInfo.Offline)
            {
                var targetUserID = info.UserID;
                var targetUser = await userOperation.GetModelAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.正常
                && t._id == targetUserID);
                if (targetUser == null) continue;
                FilterDefinition<UserBetInfo> betfilter = userBetInfoOperation.Builder.Where(t => t.MerchantID == merchantID && t.UserID == targetUserID
                && t.CreatedTime >= info.AddTime && t.CreatedTime >= agentBackInfo.AddTime && t.CreatedTime >= dateTime.Date.AddHours(6) && t.CreatedTime < dateTime.Date.AddDays(1).AddHours(6) && t.BetStatus == BetStatus.已开奖);
                if (gameType != null)
                    betfilter &= userBetInfoOperation.Builder.Eq(t => t.GameType, gameType.Value);
                var userBetList = await collection.FindListAsync(betfilter);
                //if (userBetList.IsNull()) return;
                var userBetInfo = new AppAgentReport
                {
                    NickName = string.Format("{0}[{1}]", targetUser.ShowType ? targetUser.NickName : string.IsNullOrEmpty(targetUser.MemoName) ? targetUser.NickName : targetUser.MemoName, targetUser.LoginName),
                    Turnover = userBetList.Sum(t => t.AllUseMoney),
                    ProLoss = userBetList.Sum(t => t.AllMediumBonus) - userBetList.Sum(t => t.AllUseMoney)
                };
                list.Add(userBetInfo);
            }
            var data = new AgentInfoData();
            data.SubCount = subCount;
            data.Result = list;
            data.AllTurnover = list.Sum(t => t.Turnover);
            data.AllProLoss = list.Sum(t => t.ProLoss);
            //查询是否回水
            UserBackwaterJournalOperation userBackwaterJournalOperation = new UserBackwaterJournalOperation();
            FilterDefinition<UserBackwaterJournal> filter = userBackwaterJournalOperation.Builder.Where(t => t.MerchantID == merchantID && t.AgentUserID == userID && t.AddDataTime == dateTime);
            if (gameType != null)
                filter &= userBackwaterJournalOperation.Builder.Eq(t => t.GameType, gameType.Value);
            var journal = await userBackwaterJournalOperation.GetModelListAsync(filter);
            data.Ascent = journal.Sum(t => t.Ascent);
            return Ok(new RecoverClassModel<AgentInfoData>()
            {
                Message = "获取成功！",
                Model = data,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取版本号信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NotUserAuthentication]
        public async Task<IActionResult> GetVersionInfo()
        {
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var setup = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            var result = new
            {
                setup.VersionFileUrl,
                setup.VersionNum
            };
            return Ok(new RecoverClassModel<dynamic>
            {
                Message = "获取成功！",
                Model = result,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取开启公告
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetArticle()
        {
            ArticleOperation articleOperation = new ArticleOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = await articleOperation.GetModelAsync(t => t.MerchantID == merchantID && t.ArticleType == ArticleTypeEnum.公告 && t.Open == true);
            var content = data == null ? "" : data.Content;
            return Ok(new RecoverKeywordModel(RecoverEnum.成功, "获取成功！", content));
        }

        /// <summary>
        /// 获取域名信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NotUserAuthentication]
        public async Task<IActionResult> GetDomainInfo()
        {
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var setup = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = setup.GrantUrl,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取视讯游戏桌子数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoZnumCount()
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var dic = new Dictionary<BaccaratGameType, int>();
            var roomInfo = await Utils.GetVideoRoomInfosAsync(merchantID, BaccaratGameType.百家乐);
            if (roomInfo.Status == RoomStatus.开启)
            {
                var blist = RedisOperation.GetHashValue<GameStatic>("Baccarat");
                dic.Add(BaccaratGameType.百家乐, blist.Count);
            }
            else
                dic.Add(BaccaratGameType.百家乐, 0);
            return Ok(new RecoverClassModel<Dictionary<BaccaratGameType, int>>
            { 
                Message = "获取成功！",
                Model = dic,
                Status = RecoverEnum.成功
            });
        }
    }
}