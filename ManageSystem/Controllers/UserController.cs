using Entity;
using Entity.WebModel;
using ManageSystem.Hubs;
using ManageSystem.Manipulate;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using Operation.Abutment;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManageSystem.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    [MerchantAuthentication]
    public class UserController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment = null;
        private readonly IHubContext<ChatHub> context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        /// <param name="hub"></param>
        public UserController(IHostingEnvironment hostingEnvironment, IHubContext<ChatHub> hub)
        {
            _hostingEnvironment = hostingEnvironment;
            context = hub;
        }

        /// <summary>
        /// 添加用户
        /// </summary>
        /// <remarks>
        ///##  参数说明
        ///    model
        ///    {
        ///         LoginName:用户登录名称
        ///         Password:登录密码
        ///         Level:用户级别  1：黄铜  2：白银  3：黄金  4：铂金  5：钻石  6：银皇冠  7：金皇冠
        ///         NickName:昵称
        ///    }
        ///    其它不传
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddUserInfo([FromBody]WebAddUser addUser)
        {
            UserOperation userOperation = new UserOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            //查询商户正式用户
            var count = await userOperation.GetCountAsync(t => t.MerchantID == merchantID
            && (t.Status == UserStatusEnum.冻结 || t.Status == UserStatusEnum.正常));
            if (count > 200)
                return Ok(new RecoverModel(RecoverEnum.失败, "账号数达到最高限制！"));
            if (addUser == null) return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
            var exis = await userOperation.GetModelAsync(t => t.LoginName == addUser.LoginName && t.MerchantID == merchantID);
            if (exis != null) return Ok(new RecoverModel(RecoverEnum.参数错误, "已存在相同登录名称用户！"));
            var user = new User()
            {
                LoginName = addUser.LoginName,
                Password = Utils.MD5(addUser.Password),
                Level = addUser.Level,
                Avatar = "UserImages/default.png",
                NickName = addUser.NickName,
                MerchantID = merchantID,
                Status = UserStatusEnum.正常,
                OnlyCode = userOperation.GetNewUserOnlyCode()
            };
            await userOperation.InsertModelAsync(user);
            return Ok(new RecoverModel(RecoverEnum.成功, string.Format("添加{0}成功！", addUser.LoginName)));
        }

        /// <summary>
        /// 查询用户列表
        /// </summary>
        /// <param name="loginName">登录名称</param>
        /// <param name="status">用户状态 1：正常 2：冻结 4：假人</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <remarks>
        ///##  参数说明
        ///     loginName：登录名称
        ///     status：用户状态 1：正常 2：冻结 4：假人
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             ID：用户id  不显示
        ///             Avatar：头像 （需要前端拼接）api地址+返回地址
        ///             IsAgent：是否为代理
        ///             NickName：昵称
        ///             Record：飞单状态
        ///             UserMoney：余额
        ///             Level：用户级别
        ///             LoginName：登录名称
        ///             OnlyCode：唯一码
        ///             BetGameName：下注游戏名称
        ///             ProLoss：盈亏
        ///             IsSupport：人托
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchUserInfo(string loginName, UserStatusEnum? status, int start = 1, int pageSize = 10)
        {
            UserOperation userOperation = new UserOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            FilterDefinition<User> filter = userOperation.Builder.Ne(t => t.Status, UserStatusEnum.删除) & userOperation.Builder.Eq(t => t.MerchantID, merchantID);
            if (!string.IsNullOrEmpty(loginName))
                filter &= (userOperation.Builder.Regex(t => t.LoginName, loginName) | userOperation.Builder.Regex(t => t.OnlyCode, loginName) | userOperation.Builder.Regex(t => t.NickName, loginName) | userOperation.Builder.Regex(t => t.MemoName, loginName));
            if (status != null)
                filter &= userOperation.Builder.Eq(t => t.Status, status.Value);
            var data = userOperation.GetModelListByPaging(filter, t => t.OnlyCode, true, start, pageSize);
            var result = (await TransformationData(data)).OrderBy(t => t.OnlyCode).ToList();
            var total = await userOperation.GetCountAsync(filter);
            return Ok(new RecoverListModel<WebSearchUser>()
            {
                Data = result,
                Message = "查询成功！",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        /// <summary>
        /// 数据转换
        /// </summary>
        /// <param name="users"></param>
        /// <returns></returns>
        private async Task<List<WebSearchUser>> TransformationData(List<User> users)
        {
            var result = new List<WebSearchUser>();
            if (users == null || users.Count == 0) return result;
            var tasks = new List<Task>();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var allBet = await collection.FindListAsync(t => t.BetStatus == BetStatus.已开奖
                && t.CreatedTime >= DateTime.Today && t.CreatedTime <= DateTime.Now);
            foreach (var user in users)
            {
                var userAllBet = allBet.FindAll(t => t.UserID == user._id);
                var betGameName = userAllBet.Select(t => new { Name = Enum.GetName(typeof(GameOfType), t.GameType) }).Select(t => t.Name)
                .Distinct().ToList();
                var seUser = new WebSearchUser()
                {
                    ID = user._id,
                    Avatar = user.Avatar,
                    IsAgent = user.IsAgent,
                    NickName = string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                    Record = user.Record,
                    UserMoney = user.UserMoney,
                    Level = Enum.GetName(typeof(UserStatusEnum), user.Level),
                    Status = Enum.GetName(typeof(UserStatusEnum), user.Status),
                    LoginName = user.LoginName,
                    OnlyCode = user.OnlyCode,
                    BetGameName = string.Join(",", betGameName),
                    ProLoss = userAllBet.Sum(t => t.AllMediumBonus) - userAllBet.Sum(t => t.AllUseMoney),
                    IsSupport = user.IsSupport
                };
                result.Add(seUser);
            }
            return result;
        }

        /// <summary>
        /// 飞单操作
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RecordOperation(string userID)
        {
            UserOperation userOperation = new UserOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户相关信息！"));
            user.Record = user.Record ? false : true;
            await userOperation.UpdateModelAsync(user);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 代理操作
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AvatarOperation(string userID)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.Status == UserStatusEnum.正常 && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户相关信息！"));
            user.IsAgent = user.IsAgent ? false : true;
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            if (user.IsAgent == false)
            {
                //取消代理操作   查询是否有未回水数据
                var agentBackInfo = await agentBackwaterOperation.GetModelAsync(t => t.MerchantID == merchantID && t.UserID == user._id);
                if (agentBackInfo != null)
                {
                    var addTime = agentBackInfo.AddTime.Date;
                    var now = DateTime.Today;
                    var diff = (now - addTime).Days;
                    for (var i = 0; i <= diff; i++)
                    {
                        var time = addTime.AddDays(i);
                        var userBetAgentList = await BackwaterKind.SearchUserBet(merchantID, agentBackInfo, time);
                        var data = new WebAgentReport()
                        {
                            AgentID = user._id,
                            NickName = string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
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
                            Jsssc = userBetAgentList.Sum(t => t.Jsssc),
                            BackStatus = userBetAgentList.IsNull() ? BackStatusEnum.未回水 : userBetAgentList.Exists(t => t.BackStatus == BackStatusEnum.未回水) ? BackStatusEnum.未回水
                    : BackStatusEnum.已回水
                        };
                        if (data.BackStatus == BackStatusEnum.未回水 && (data.Pk10 + data.Xyft + data.Cqssc + data.Jssc
                            + data.Azxy10 + data.Azxy5 + data.Ireland10 + data.Ireland5) > 0)
                        {
                            return Ok(new RecoverModel(RecoverEnum.失败, "还存在未返水代理注单，时间：" + data.Time));
                        }
                    }
                }
            }
            await userOperation.UpdateModelAsync(user);
            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID,
                OpLocation = OpLocationEnum.切换代理,
                OpType = OpTypeEnum.修改
            };

            //添加代理数据
            if (user.IsAgent)
            {
                sensitive.OpBcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "非代理");
                sensitive.OpAcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "代理");

                var agent = agentBackwaterOperation.GetModel(t => t.UserID == user._id && t.MerchantID == merchantID);
                if (agent == null)
                {
                    var code = CentralProcess.SetExtensionCode();
                    var flag = await agentBackwaterOperation.GetModelAsync(t => t.ExtensionCode == code);
                    if (flag != null) code = CentralProcess.SetExtensionCode();
                    agent = new AgentBackwater()
                    {
                        UserID = user._id,
                        MerchantID = merchantID,
                        ExtensionCode = code,
                        AddTime = DateTime.Now
                    };
                    await agentBackwaterOperation.InsertModelAsync(agent);
                }
                else
                {
                    agent.AddTime = DateTime.Now;
                    await agentBackwaterOperation.UpdateModelAsync(agent);
                }
            }
            else
            {
                var agent = agentBackwaterOperation.GetModel(t => t.UserID == user._id && t.MerchantID == merchantID);
                agent.Offline = new List<OfflineUser>();
                await agentBackwaterOperation.UpdateModelAsync(agent);
                sensitive.OpAcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "非代理");
                sensitive.OpBcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "代理");
            }
            await sensitiveOperation.InsertModelAsync(sensitive);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 假人操作
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DummyOperation(string userID)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户相关信息！"));
            user.Record = false;
            var statusValue = Enum.GetName(typeof(UserStatusEnum), (int)user.Status);
            //判断是否为直接创建的假人
            if (user.IsSupport)
                return Ok(new RecoverModel(RecoverEnum.失败, "该用户不能切换状态！"));
            if (user.IsAgent)
                return Ok(new RecoverModel(RecoverEnum.失败, "该用户为代理帐户，不能切换状态！"));
            if (user.UserMoney != 0)
                return Ok(new RecoverModel(RecoverEnum.失败, "该用户还有余额，不能切换状态！"));
            //查询下注
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var status = user.Status == UserStatusEnum.正常 ? NotesEnum.正常 : NotesEnum.虚拟;
            //查询是否存在未结算注单
            var betList = await collection.FindListAsync(t => t.MerchantID == merchantID && t.UserID == userID && t.Notes == status);
            if (betList.FindIndex(t => t.BetStatus == BetStatus.未开奖) > -1)
                return Ok(new RecoverModel(RecoverEnum.失败, "存在未结算注单，不能切换状态！"));
            //查询是否存在未返水注单
            if (!string.IsNullOrEmpty(user.ProgrammeID))
            {
                if (betList.FindIndex(t => t.CreatedTime >= user.ProgrammeAddTime && t.BetStatus == BetStatus.已开奖 && t.IsBackwater == false) > -1)
                    return Ok(new RecoverModel(RecoverEnum.失败, "存在未返水注单，不能切换状态！"));
            }
            //查询是否为代理下级
            if (user.Status == UserStatusEnum.正常)
            {
                AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
                var agentBack = await agentBackwaterOperation.GetModelAsync(t => t.MerchantID == merchantID && t.ExtensionCode == user.RoomNum);
                if (agentBack != null)
                {
                    var infos = agentBack.Offline.Find(t => t.UserID == userID);
                    if (infos != null)
                    {
                        //查询添加代理时间
                        betList = betList.FindAll(t => t.CreatedTime >= agentBack.AddTime &&
                        t.CreatedTime >= infos.AddTime);
                        var times = betList.Select(t => new { Time = t.CreatedTime.ToString("yyyy-MM-dd") })
                            .GroupBy(t => new { t.Time }).Select(t => t.Key.Time).ToList();
                        foreach (var time in times)
                        {
                            //查询当天是否有回水
                            var userBackwaterJournalOperation = new UserBackwaterJournalOperation();
                            var journal = await userBackwaterJournalOperation.GetModelListAsync(t => t.MerchantID == merchantID
                 && t.UserID == userID && t.AgentUserID == agentBack.UserID && t.AddDataTime == Convert.ToDateTime(time));
                            if (journal == null)
                                return Ok(new RecoverModel(RecoverEnum.失败, "存在上级代理未返水，时间：" + time));
                        }
                    }
                }
            }
            user.Status = (user.Status == UserStatusEnum.正常 || user.Status == UserStatusEnum.冻结) ? UserStatusEnum.假人 : UserStatusEnum.正常;
            await userOperation.UpdateModelAsync(user);
            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID
            };
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation(); ;
            if (user.Status == UserStatusEnum.假人)
            {
                sensitive.OpLocation = OpLocationEnum.用户状态切换;
                sensitive.OpType = OpTypeEnum.修改;
                sensitive.OpBcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, statusValue);
                sensitive.OpAcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "假人");
                var model = shamRobotOperation.GetModel(t => t.UserID == user._id && t.MerchantID == merchantID);
                if (model == null)
                {
                    model = new ShamRobotmanage()
                    {
                        UserID = user._id,
                        MerchantID = merchantID
                    };
                    await shamRobotOperation.InsertModelAsync(model);
                }
            }
            else
            {
                sensitive.OpLocation = OpLocationEnum.用户状态切换;
                sensitive.OpType = OpTypeEnum.修改;
                sensitive.OpAcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "正常");
                sensitive.OpBcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "假人");
            }
            await sensitiveOperation.InsertModelAsync(sensitive);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 冻结操作
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FrozenOperation(string userID)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户相关信息！"));
            user.IsAgent = false;
            user.Record = false;
            user.Status = user.Status == UserStatusEnum.正常 ? UserStatusEnum.冻结 : UserStatusEnum.正常;
            await userOperation.UpdateModelAsync(user);
            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID
            };
            if (user.Status == UserStatusEnum.冻结)
            {
                sensitive.OpLocation = OpLocationEnum.用户状态切换;
                sensitive.OpType = OpTypeEnum.修改;
                sensitive.OpBcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "正常");
                sensitive.OpAcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "冻结");
                RedisOperation.DeleteKey(user._id);
                //删除长连接
                //强制下线通知
                await RabbitMQHelper.SendGameMessage("2", merchantID, "Subordinate", null, user._id);
                RedisOperation.DeleteKey(user._id.Replace("-", ""));
                BsonOperation bsonOperation = new BsonOperation("SignalR");
                await bsonOperation.Collection.DeleteManyAsync(bsonOperation.Builder.Eq("UserID", user._id));
            }
            else
            {
                sensitive.OpLocation = OpLocationEnum.用户状态切换;
                sensitive.OpType = OpTypeEnum.修改;
                sensitive.OpAcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "正常");
                sensitive.OpBcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, "冻结");
            }
            await sensitiveOperation.InsertModelAsync(sensitive);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="userID">用户ID</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleleUser(string userID)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户相关信息！"));

            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID,
                OpLocation = OpLocationEnum.删除用户,
                OpType = OpTypeEnum.删除,
                OpBcontent = string.Format("用户[{0}][{1}]为{2}状态", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode, Enum.GetName(typeof(UserStatusEnum), (int)user.Status)),
                OpAcontent = string.Format("删除用户[{0}][{1}]", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.OnlyCode)
            };
            await sensitiveOperation.InsertModelAsync(sensitive);

            user.IsAgent = false;
            user.Record = false;
            user.Status = UserStatusEnum.删除;
            await userOperation.UpdateModelAsync(user);

            //删除机器人配置
            if (user.Status == UserStatusEnum.假人 && user.IsSupport == true)
            {
                ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
                await shamRobotOperation.DeleteModelOneAsync(t => t.UserID == user._id && t.MerchantID == merchantID);
            }
            //强制下线通知
            await RabbitMQHelper.SendGameMessage("2", merchantID, "Subordinate", null, user._id);
            RedisOperation.DeleteKey(user._id.Replace("-", ""));
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <remarks>
        ///##  参数说明
        ///     userID：用户id
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             ID：id
        ///             LoginName：登录名称 
        ///             Level：等级 1：黄铜  2：白银  3：黄金  4：铂金  5：钻石  6：银皇冠  7：金皇冠
        ///             LoginTime：最近登录时间
        ///             NickName：昵称
        ///             Remark：备注
        ///             Talking：用户发言
        ///             UserMoney：余额
        ///             OnlyCode：唯一码
        ///             ProgrammeID：回水id
        ///             IsSupport：机器人托
        ///             MemoName：备注名
        ///             ShowType：显示名称方式 true:昵称  false:备注名
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserInfo(string userID)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户相关信息！"));
            var result = new WebGetUserInfo
            {
                ID = user._id,
                LoginName = user.LoginName,
                Level = user.Level,
                LoginTime = user.LoginTime?.ToString("yyyy-MM-dd HH:mm"),
                NickName = user.NickName,
                Remark = user.Remark,
                Talking = user.Talking,
                UserMoney = user.UserMoney,
                OnlyCode = user.OnlyCode.ToString(),
                ProgrammeID = user.ProgrammeID,
                IsSupport = user.IsSupport,
                MemoName = user.MemoName,
                ShowType = user.ShowType,
                VideoProgrammeID  = user.VideoProgrammeID,
                OpenID = user.Unionid
            };
            return Ok(new RecoverClassModel<WebGetUserInfo>() { Message = "获取成功！", Model = result, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 修改用户信息及上分下分
        /// </summary>
        /// <param name="webGetUserInfo">用户信息</param>
        /// <param name="up">上分</param>
        /// <param name="down">下分</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUserInfo([FromBody] WebGetUserInfo webGetUserInfo, decimal up = 0, decimal down = 0)
        {
            if (webGetUserInfo == null) return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == webGetUserInfo.ID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户信息！"));
            StringBuilder bstringBuilder = new StringBuilder();
            StringBuilder astringBuilder = new StringBuilder();
            if (user.Talking != webGetUserInfo.Talking)
            {
                bstringBuilder.AppendFormat("用户[{0}][{1}]为{2}状态\r\n", user.NickName, user.OnlyCode, user.Talking ? "可发言" : "禁止发言");
                astringBuilder.AppendFormat("用户[{0}][{1}]为{2}状态\r\n", user.NickName, user.OnlyCode, webGetUserInfo.Talking ? "可发言" : "禁止发言");
            }
            user.Talking = webGetUserInfo.Talking;
            user.NickName = webGetUserInfo.NickName;
            user.Level = webGetUserInfo.Level;
            user.Remark = webGetUserInfo.Remark;
            if (user.ProgrammeID != webGetUserInfo.ProgrammeID)
            {
                BackwaterSetupOperation backwaterSetupOperation = new BackwaterSetupOperation(); ;
                if (!string.IsNullOrEmpty(webGetUserInfo.ProgrammeID))
                {
                    var programme = await backwaterSetupOperation.GetModelAsync(t => t._id == webGetUserInfo.ProgrammeID
                    && t.MerchantID == merchantID);
                    astringBuilder.AppendFormat("切换用户[{0}][{1}]回水方案{2}\r\n", user.NickName, user.OnlyCode, programme.Name);
                }
                string userPrevName = "";
                if (!string.IsNullOrEmpty(user.ProgrammeID))
                {
                    var programme = await backwaterSetupOperation.GetModelAsync(t => t._id == user.ProgrammeID
                    && t.MerchantID == merchantID);
                    if (programme != null)
                        userPrevName = programme.Name;
                }
                bstringBuilder.AppendFormat("用户[{0}][{1}]回水方案为{2}\r\n", user.NickName, user.OnlyCode, userPrevName);

                user.ProgrammeAddTime = DateTime.Now;
            }
            user.ProgrammeID = webGetUserInfo.ProgrammeID;
            if (user.VideoProgrammeID != webGetUserInfo.VideoProgrammeID)
            {
                VideoBackwaterSetupOperation videoBackwaterSetupOperation = new VideoBackwaterSetupOperation();
                if (!string.IsNullOrEmpty(webGetUserInfo.VideoProgrammeID))
                {
                    var programme = await videoBackwaterSetupOperation.GetModelAsync(t => t._id == webGetUserInfo.VideoProgrammeID
                    && t.MerchantID == merchantID);
                    astringBuilder.AppendFormat("切换用户[{0}][{1}]视讯回水方案{2}\r\n", user.NickName, user.OnlyCode, programme.Name);
                }
                string userPrevName = "";
                if (!string.IsNullOrEmpty(user.ProgrammeID))
                {
                    var programme = await videoBackwaterSetupOperation.GetModelAsync(t => t._id == user.VideoProgrammeID
                    && t.MerchantID == merchantID);
                    if (programme != null)
                        userPrevName = programme.Name;
                }
                bstringBuilder.AppendFormat("用户[{0}][{1}]视讯回水方案为{2}\r\n", user.NickName, user.OnlyCode, userPrevName);

                user.ProgrammeAddTime = DateTime.Now;
            }
            user.VideoProgrammeID = webGetUserInfo.VideoProgrammeID;
            if (!string.IsNullOrEmpty(webGetUserInfo.Password))
            {
                if (user.IsSupport)
                    return Ok(new RecoverModel(RecoverEnum.失败, "该用户不能修改密码！"));
                user.Password = Utils.MD5(webGetUserInfo.Password);
                astringBuilder.AppendFormat("修改用户[{0}][{1}]密码\r\n", user.NickName, user.OnlyCode);
            }
            if (up > 0)
                await userOperation.UpperScore(user._id, merchantID, up, ChangeTargetEnum.系统, "管理员加款（用户设置）", "管理员加款（用户设置）");
            if (down > 0)
            {
                if (user.UserMoney < down) return Ok(new RecoverModel(RecoverEnum.失败, "用户余额不足！"));
                await userOperation.LowerScore(user._id, merchantID, down, ChangeTargetEnum.系统, "管理员扣款（用户设置）", "管理员扣款（用户设置）", orderStatus: OrderStatusEnum.充值成功);
            }
            //其它信息
            user.MemoName = webGetUserInfo.MemoName;
            if (string.IsNullOrEmpty(webGetUserInfo.MemoName))
            {
                user.ShowType = true;
            }
            else
            {
                user.ShowType = webGetUserInfo.ShowType;
            }
            await userOperation.UpdateModelAsync(user);
            if (astringBuilder.Length > 0 || bstringBuilder.Length > 0)
            {
                SensitiveOperation sensitiveOperation = new SensitiveOperation();
                var sensitive = new Sensitive()
                {
                    MerchantID = merchantID,
                    OpLocation = OpLocationEnum.修改用户信息,
                    OpType = OpTypeEnum.修改,
                    OpBcontent = bstringBuilder.ToString(),
                    OpAcontent = astringBuilder.ToString()
                };
                await sensitiveOperation.InsertModelAsync(sensitive);
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 获取方案下拉
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGrogrammeList()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            BackwaterSetupOperation backwaterSetupOperation = new BackwaterSetupOperation(); ;
            var list = await backwaterSetupOperation.GetModelListAsync(t => t.MerchantID == merchantID);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                result.Add(
                    new
                    {
                        ID = data._id,
                        data.Name,
                        data.GameType
                    });
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
        /// 获取视讯方案下拉
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoGrogrammeList()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            VideoBackwaterSetupOperation videoBackwaterSetupOperation = new VideoBackwaterSetupOperation(); ;
            var list = await videoBackwaterSetupOperation.GetModelListAsync(t => t.MerchantID == merchantID);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                result.Add(
                    new
                    {
                        ID = data._id,
                        data.Name,
                        data.GameType
                    });
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
        /// 上传用户头像
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="fileinput"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUserImages(string userID, IFormFile fileinput)
        {
            UserOperation userOperation = new UserOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户相关信息！"));

            if (fileinput == null) return Ok(new RecoverModel(RecoverEnum.失败, "未选择图片！"));
            var url = await BlobHelper.UploadImageToBlob(fileinput, "UserImages");
            if (string.IsNullOrEmpty(url)) return Ok(new RecoverModel(RecoverEnum.失败, "图片格式错误！"));
            if (url == "1") return Ok(new RecoverModel(RecoverEnum.失败, "图片大小最大为20M！"));
            user.Avatar = url;
            await userOperation.UpdateModelAsync(user);
            //通知用户
            await RabbitMQHelper.SendGameMessage(url, merchantID, "UserAvatarChange", null, userID);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
            #region 上传图片
            //// 原文件名（包括路径）
            //var filename = fileinput.FileName;
            //// 扩展名
            //var extName = filename.Substring(filename.LastIndexOf('.')).Replace("\"", "");
            //string filepath = _hostingEnvironment.WebRootPath;
            //filepath = Path.Combine(filepath, "Images", "UserImages", userID);
            ////判断路径是否存在
            //if (!Directory.Exists(filepath))
            //    Directory.CreateDirectory(filepath);

            //byte[] buffer = new byte[1024 * 1024 * 20];
            //var newFilename = DateTime.Now.ToString("yyyyMMddHHmmssffffff") + extName;
            //if (filename != "")
            //{
            //    var path = Path.Combine(filepath, newFilename);
            //    var file = new FileStream(path, FileMode.OpenOrCreate);
            //    var stre = fileinput.OpenReadStream();
            //    int read;
            //    do
            //    {
            //        read = stre.Read(buffer, 0, 1024 * 1024 * 20);
            //        file.Write(buffer, 0, read);
            //    } while (read == 1024 * 1024 * 20);
            //    file.Dispose();
            //    stre.Dispose();
            //    var filePath = "Images/UserImages/" + userID + "/" + newFilename;
            //    user.Avatar = filePath;
            //    await userOperation.UpdateModelAsync(user);

            //    //通知用户
            //    await RabbitMQHelper.SendGameMessage(filePath, merchantID, "UserAvatarChange", null, userID);
            //    return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
            //}
            //return Ok(new RecoverModel(RecoverEnum.失败, "上传图片失败！"));
            #endregion
        }

        /// <summary>
        /// 获取用户余额
        /// </summary>
        /// <param name="loginName">登录名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserBalance(string loginName)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t.LoginName == loginName && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户相关信息！"));
            return Ok(new RecoverKeywordModel()
            {
                Message = "查询成功！",
                Status = RecoverEnum.成功,
                Keyword = string.Format("昵称：{0} 余额：{1}", string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.UserMoney)
            });
        }

        #region 用户帐变 下注  充值等
        /// <summary>
        /// 获取用户上下分记录
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserBrancHis(string userID, DateTime startTime, DateTime endTime)
        {
            UserOperation userOperation = new UserOperation();
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID);
            var list = userIntegrationOperation.GetModelList(t => t.UserID == userID && t.MerchantID == merchantID &&
            t.ChangeTarget != ChangeTargetEnum.中奖 && t.ChangeTarget != ChangeTargetEnum.投注 && t.ChangeTarget != ChangeTargetEnum.回水 &&
            t.Management != ManagementEnum.未审批 && t.CreatedTime >= startTime && t.CreatedTime <= endTime, t => t.CreatedTime, false);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                var home = new
                {
                    UserID = user._id,
                    Amount = (data.ChangeType == ChangeTypeEnum.上分 ? "" : "-") + (data.Amount < 0 ? -data.Amount : data.Amount).ToString("#0.00"),
                    ApplyTime = data.CreatedTime.ToString("MM-dd HH:mm"),
                    data.ChangeType,
                    NickName = string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                    data.Message,
                    OnlyCode = user.OnlyCode.ToString()
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
        /// 获取用户帐变记录
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页面数量</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserAccountChange(string userID, DateTime startTime, DateTime endTime, int start = 1, int pageSize = 10)
        {
            UserOperation userOperation = new UserOperation();
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID);
            var list = userIntegrationOperation.GetModelListByPaging(t => t.UserID == userID && t.MerchantID == merchantID &&
            t.CreatedTime >= startTime && t.CreatedTime <= endTime && t.Management == ManagementEnum.已同意,
            t => t.CreatedTime, false, start, pageSize);
            var total = userIntegrationOperation.GetCount(t => t.UserID == userID && t.MerchantID == merchantID &&
            t.CreatedTime >= startTime && t.CreatedTime <= endTime && t.Management == ManagementEnum.已同意);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                var home = new
                {
                    UserID = user._id,
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
                Total = total
            });
        }

        ///// <summary>
        ///// 获取用户相关游戏下注中奖详细
        ///// </summary>
        ///// <param name="userID"></param>
        ///// <param name="gameType"></param>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <param name="nper"></param>
        ///// <param name="start"></param>
        ///// <param name="pageSize"></param>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<IActionResult> GetUserGameInfos(string userID, GameOfType gameType, DateTime startTime, DateTime endTime, string nper, int start = 1, int pageSize = 10)
        //{
        //    UserOperation userOperation = new UserOperation();

        //    var merchantID = HttpContext.Items["MerchantID"].ToString();
        //    var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
        //    //获取对应游戏下注信息
        //    var address = await Utils.GetAddress(merchantID);
        //    UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
        //    var collection = userBetInfoOperation.GetCollection(merchantID);
        //    var betList = await collection.FindListAsync(t => t.UserID == userID
        //    && t.MerchantID == merchantID && t.GameType == gameType
        //    && t.BetStatus == BetStatus.已开奖 &&
        //    t.CreatedTime >= startTime && t.CreatedTime <= endTime);
        //    if (!string.IsNullOrEmpty(nper))
        //        betList = betList.FindAll(t => t.Nper == nper);
        //    var winBetList = new List<OddBetInfo>();
        //    foreach (var bet in betList)
        //    {
        //        foreach (var remark in bet.BetRemarks)
        //        {
        //            winBetList.AddRange(remark.OddBets.FindAll(t => t.BetStatus == BetStatusEnum.已中奖));
        //        }
        //    }
        //    var datas = (from data in winBetList
        //                 select new
        //                 {
        //                     data.Nper,
        //                     data.OddNum,
        //                     data.MediumBonus,
        //                     Message = string.Format("{0}{1}/{2}={3}", Enum.GetName(typeof(BetTypeEnum), (int)data.BetRule),
        //                     data.BetNo, data.BetMoney.ToString(), data.MediumBonus.ToString())
        //                 }).ToList();
        //    var final = (from data in datas
        //                 group data by
        //                 new
        //                 {
        //                     data.Nper,
        //                     data.OddNum
        //                 } into grp
        //                 select new
        //                 {
        //                     grp.Key.OddNum,
        //                     grp.Key.Nper,
        //                     AllMediumBonus = grp.Sum(t => t.MediumBonus),
        //                     Message = string.Join(",", grp.Select(t => t.Message).ToList())
        //                 }).ToList();
        //    var result = new List<dynamic>();
        //    foreach (var data in oddNums)
        //    {
        //        var infos = betList.FindAll(t => t.OddNum == data.OddNum && t.Nper == data.Nper);
        //        string lotTime = string.Empty;
        //        var nums = CancelAnnouncement.GetGameNums(gameType, data.Nper, ref lotTime, ',', merchantID);
        //        //查询下注信息
        //        var userIntegra = await userIntegrationOperation.GetModelAsync(t => t.UserID == userID && t.MerchantID == merchantID && t.GameType == gameType &&
        //        t.OddNum == data.OddNum);
        //        var win = final.Find(t => t.OddNum == data.OddNum && t.Nper == data.Nper);
        //        var item = new
        //        {
        //            data.Nper,
        //            AllBet = infos.Sum(t => t.BetMoney),
        //            BetType = userIntegra.Remark,
        //            Loss = (win == null ? 0 : win.AllMediumBonus) - userIntegra.Amount,
        //            WinNums = win == null ? "" : win.Message,
        //            LotNums = nums,
        //            LotTime = lotTime
        //        };
        //        result.Add(item);
        //    }
        //    return Ok(new RecoverListModel<dynamic>()
        //    {
        //        Data = result,
        //        Message = "获取成功！",
        //        Status = RecoverEnum.成功,
        //        Total = total
        //    });
        //}

        /// <summary>
        /// 获取用户收款帐户
        /// </summary>
        /// <param name="userID">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserReceivables(string userID)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关用户！"));
            var result = new
            {
                user.WeChat,
                user.WeChatUrl,
                user.Alipay,
                user.AlipayUrl
            };
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功！",
                Model = result,
                Status = RecoverEnum.成功
            });
        }
        #endregion

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
        /// <param name="userID">用户id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUserHead(string pagName, string userID)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询用户信息！"));
            user.Avatar = pagName;
            await userOperation.UpdateModelAsync(user);
            await RabbitMQHelper.SendGameMessage(pagName, merchantID, "UserAvatarChange", null, userID);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 获取现有正式玩家人数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetExistingCount()
        {
            UserOperation userOperation = new UserOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            //查询商户正式用户
            var count = await userOperation.GetCountAsync(t => t.MerchantID == merchantID
            && (t.Status == UserStatusEnum.冻结 || t.Status == UserStatusEnum.正常));
            return Ok(new RecoverKeywordModel()
            {
                Keyword = count.ToString(),
                Status = RecoverEnum.成功,
                Message = "获取成功！"
            });
        }

        /// <summary>
        /// 生成登录账号
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="loginName"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GenerateAccount(string userID, string loginName)
        {
            UserOperation userOperation = new UserOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t=>t._id == userID && t.MerchantID == merchantID);
            if (!string.IsNullOrEmpty(user.LoginName))
                return Ok(new RecoverModel(RecoverEnum.失败, "该用户已绑定登录号！"));
            var count = await userOperation.GetCountAsync(t => t.MerchantID == merchantID && t.LoginName == loginName);
            if (count > 0)
                return Ok(new RecoverModel(RecoverEnum.失败, "已有该登录名用户，请重新设置！"));
            user.LoginName = loginName;
            user.Password = Utils.MD5("888888");
            await userOperation.UpdateModelAsync(user);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }
    }
}