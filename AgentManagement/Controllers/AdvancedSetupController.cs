using AgentManagement.Manipulate;
using Entity;
using Entity.AgentModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Agent;
using Operation.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgentManagement.Controllers
{
    /// <summary>
    /// 高级设置
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    public class AdvancedSetupController : ControllerBase
    {
        private readonly AgentUserOperation agentUserOperation = null;
        private readonly AgentMainLogOperation agentMainLogOperation = null;
        private readonly AdvancedSetupOperation advancedSetupOperation = null;
        private readonly IHostingEnvironment _hostingEnvironment = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public AdvancedSetupController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            agentUserOperation = new AgentUserOperation();
            agentMainLogOperation = new AgentMainLogOperation();
            advancedSetupOperation = new AdvancedSetupOperation();
        }
        /// <summary>
        /// 获取高级代理下所有代理
        /// </summary>
        /// <param name="loginName">查询登录名</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             AgentID：代理Id
        ///             UserName：用户名
        ///             UserBalance：余额
        ///             SubAgentPrice：代理价格
        ///             BankNum：银行账户
        ///             Account：支付宝名称
        ///             IP：ip
        ///             LastLoginTime：最后登录时间
        ///             CreateTime：创建时间
        ///             Status：锁定  1：正常 2：锁定
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAgentInfo(string loginName, int start = 1, int pageSize = 10)
        {
            if (!HttpContext.IsAdmin())
                return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            FilterDefinition<AgentUser> filter = agentUserOperation.Builder.Eq(t => t.HighestAgentID, agentID);
            if (!string.IsNullOrEmpty(loginName))
                filter &= agentUserOperation.Builder.Regex(t => t.LoginName, loginName);
            var list = agentUserOperation.GetModelListByPaging(filter, t => t.LastLoginTime, false, start, pageSize);
            var total = await agentUserOperation.GetCountAsync(filter);
            BankInfoOperation bankInfoOperation = new BankInfoOperation();
            AlipayInfoOperation alipayInfoOperation = new AlipayInfoOperation();
            var result = new List<dynamic>();
            foreach (var agent in list)
            {
                var userName = await CentralProcess.GetPerAgentName(agent._id);
                var bankInfo = await bankInfoOperation.GetModelAsync(t => t.AgentID == agent._id);
                var alipayInfo = await alipayInfoOperation.GetModelAsync(t => t.AgentID == agent._id);
                var data = new
                {
                    AgentID = agent._id,
                    UserName = userName,
                    agent.UserBalance,
                    agent.SubAgentPrice,
                    bankInfo?.BankNum,
                    alipayInfo?.Account,
                    IP = agent.LastLoginIP,
                    agent.Status,
                    LastLoginTime = agent.LastLoginTime?.ToString("yyyy/MM/dd HH:mm"),
                    CreateTime = agent.CreatedTime.ToString("yyyy/MM/dd HH:mm")
                };
                result.Add(data);
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        /// <summary>
        /// 联系方式
        /// </summary>
        /// <param name="agentID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetContactInfo(string agentID)
        {
            if (!HttpContext.IsAdmin())
                return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            ContactOperation contactOperation = new ContactOperation();
            var contact = await contactOperation.GetModelAsync(t => t.AgentID == agentID);
            dynamic result = null;
            if (contact != null)
            {
                result = new
                {
                    contact.Phone,
                    contact.QQ,
                    contact.Email
                };
            }
            else
            {
                result = new
                {
                    Phone = "",
                    QQ = "",
                    Email = ""
                };
            }
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功",
                Model = result,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 锁定/解锁
        /// </summary>
        /// <param name="agentID">下级代理id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> LockAgent(string agentID)
        {
            if (!HttpContext.IsAdmin())
                return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            var log = new AgentMainLog()
            {
                AgentID = mainAgentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "锁定/解锁代理"
            };
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID
            && t.HighestAgentID == mainAgentID);
            if (agent == null)
            {
                log.Status = OperationStatusEnum.失败;
                log.Remark = "锁定/解锁代理失败";
                await agentMainLogOperation.InsertModelAsync(log);
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息"));
            }
            if (agent.Status == AgentStatusEnum.正常)
            {
                agent.Status = AgentStatusEnum.锁定;
                log.Remark = string.Format("[{0}]锁定", agent.LoginName);
            }
            else if (agent.Status == AgentStatusEnum.锁定)
            {
                agent.Status = AgentStatusEnum.正常;
                log.Remark = string.Format("[{0}]被解锁", agent.LoginName);
            }
            await agentMainLogOperation.InsertModelAsync(log);
            await agentUserOperation.UpdateModelAsync(agent);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 修改代理价格
        /// </summary>
        /// <param name="agentID">下级代理id</param>
        /// <param name="price">代理价格</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePrice(string agentID, decimal price)
        {
            if (!HttpContext.IsAdmin())
                return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID
            && t.HighestAgentID == mainAgentID);
            var log = new AgentMainLog()
            {
                AgentID = mainAgentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "调整代理价格"
            };
            if (agent == null)
            {
                log.Status = OperationStatusEnum.失败;
                log.Remark = "调整代理价格失败";
                await agentMainLogOperation.InsertModelAsync(log);
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息"));
            }
            log.Remark = string.Format("[{0}]价格调整为{1}", agent.LoginName, price);
            agent.SubAgentPrice = price;
            await agentUserOperation.UpdateModelAsync(agent);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="agentID">下级代理id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetPwd(string agentID)
        {
            if (!HttpContext.IsAdmin())
                return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID
            && t.HighestAgentID == mainAgentID);
            var log = new AgentMainLog()
            {
                AgentID = mainAgentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "调整代理价格"
            };
            if (agent == null)
            {
                log.Status = OperationStatusEnum.失败;
                log.Remark = "重置密码失败";
                await agentMainLogOperation.InsertModelAsync(log);
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息"));
            }
            //agent.Password = Utils.MD5("88888888");
            agent.Password = "88888888";
            await agentUserOperation.UpdateModelAsync(agent);
            log.Remark = string.Format("[{0}]重置密码", agent.LoginName);
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "重置成功"));
        }

        /// <summary>
        /// 代理加扣款
        /// </summary>
        /// <param name="agentID">代理id</param>
        /// <param name="status">操作</param>
        /// <param name="type">类型</param>
        /// <param name="amount">金额</param>
        /// <param name="reason">理由</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgentDeduction(string agentID, DeductionStatusEnum status, DeductionTypeEnum type, decimal amount, string reason)
        {
            if (!HttpContext.IsAdmin())
                return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID
            && t.HighestAgentID == mainAgentID);
            var log = new AgentMainLog()
            {
                AgentID = mainAgentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "加扣款"
            };
            if (agent == null)
            {
                log.Status = OperationStatusEnum.失败;
                log.Remark = "加扣款失败";
                await agentMainLogOperation.InsertModelAsync(log);
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息"));
            }
            if (status == DeductionStatusEnum.扣款)
            {
                if (agent.UserBalance < amount)
                {
                    log.Status = OperationStatusEnum.失败;
                    log.Remark = string.Format("扣款失败，[{0}]余额不足", agent.LoginName);
                    await agentMainLogOperation.InsertModelAsync(log);
                    return Ok(new RecoverModel(RecoverEnum.失败, string.Format("扣款失败，[{0}]余额不足", agent.LoginName)));
                }
                await agentUserOperation.DownScore(agent._id, amount, AccountTypeEnum.其他, "管理员扣款\r\n" + reason, false, true);
            }
            else
            {
                await agentUserOperation.UpScore(agent._id, amount, AccountTypeEnum.其他, "管理员加款\r\n" + reason, true);
                //添加充值记录
                RechargeOperation rechargeOperation = new RechargeOperation();
                var recharge = new Recharge()
                {
                    OrderType = "商家加款",
                    ApplyAgentID = agent._id,
                    AcceptanceAgentID = mainAgentID,
                    ActualAmount = amount,
                    Amount = amount,
                    ApplyAgentName = agent.LoginName,
                    IsHandle = true
                };
                await rechargeOperation.InsertModelAsync(recharge);
            }
            log.Remark = string.Format("[{0}]{1}{2}{3}\r\n{4}", agent.LoginName,
                    Enum.GetName(typeof(DeductionTypeEnum), (int)type),
                    Enum.GetName(typeof(DeductionStatusEnum), (int)status),
                    amount, reason);
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取代理免费时长和超出时长费用
        /// </summary>
        /// <param name="agenName"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFreeAdmission(string agenName)
        {
            if (!HttpContext.IsAdmin())
                return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            var agent = await agentUserOperation.GetModelAsync(t => t.LoginName == agenName
            && t.HighestAgentID == mainAgentID);
            if (agent == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息"));
            var result = new
            {
                agent.FreeDuration,
                agent.ExcessExpenses
            };
            return Ok(new RecoverClassModel<object>()
            {
                Message = "获取成功",
                Model = result,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 修改代理免费时长和超出时长费用
        /// </summary>
        /// <param name="agentName">代理id</param>
        /// <param name="freeDuration">免费时长</param>
        /// <param name="excessExpenses">超出时长费用</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateFreeAdmission(string agentName, int freeDuration, decimal excessExpenses)
        {
            if (!HttpContext.IsAdmin())
                return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            var agent = await agentUserOperation.GetModelAsync(t => t.LoginName == agentName
            && t.HighestAgentID == mainAgentID);
            var log = new AgentMainLog()
            {
                AgentID = mainAgentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "调整免费时长和费用"
            };
            if (agent == null)
            {
                log.Status = OperationStatusEnum.失败;
                log.Remark = "调整免费时长和费用失败";
                await agentMainLogOperation.InsertModelAsync(log);
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息"));
            }
            agent.FreeDuration = freeDuration;
            agent.ExcessExpenses = excessExpenses;
            await agentUserOperation.UpdateModelAsync(agent);
            log.Remark = string.Format("[{0}]调整时长[{1}]和费用[{2}]", agent.LoginName, freeDuration, excessExpenses);
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 提现请求查询
        /// </summary>
        /// <param name="loginName">用户名</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             ID：数据id
        ///             Name：用户名
        ///             Amount：提现金额
        ///             Balance：当前余额
        ///             ApplyTime：申请时间
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RequestCash(string loginName, int start = 1, int pageSize = 10)
        {
            var type = Convert.ToInt32(HttpContext.User.FindFirstValue("Type"));
            var mainAgentID = string.Empty;
            if (type != (int)ManagementTypeEnum.财务类型)
            {
                if (!HttpContext.IsAdmin())
                    return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
                mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            }
            else
            {
                //查询超级管理员
                var agent = await agentUserOperation.GetModelAsync(t => t.IsHighest == true);
                mainAgentID = agent._id;
            }
            CashWithdrawalOperation cashWithdrawalOperation = new CashWithdrawalOperation();
            FilterDefinition<CashWithdrawal> filter = cashWithdrawalOperation.Builder.Where(t => t.AcceptanceAgentID == mainAgentID
            && t.Status == WithdrawalStatusEnum.申请);
            if (!string.IsNullOrEmpty(loginName))
                filter &= cashWithdrawalOperation.Builder.Regex(t => t.ApplyAgentName, loginName);
            var list = cashWithdrawalOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var total = await cashWithdrawalOperation.GetCountAsync(filter);
            var result = (from data in list
                          select new WebRequestCash
                          {
                              ID = data._id,
                              Name = data.ApplyAgentName,
                              Amount = data.Amount,
                              Balance = agentUserOperation.GetModel(t => t._id == data.ApplyAgentID && t.HighestAgentID == data.AcceptanceAgentID)?.UserBalance,
                              ApplyTime = data.CreatedTime.ToString("yyyy/MM/dd HH:mm")
                          }).ToList();
            return Ok(new RecoverListModel<WebRequestCash>()
            {
                Data = result,
                Total = total,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }


        private class WebRequestCash
        {
            public string ID { get; set; }
            public string Name { get; set; }
            public decimal Amount { get; set; }
            public decimal? Balance { get; set; }
            public string ApplyTime { get; set; }
        }

        /// <summary>
        /// 获取提现申请明细
        /// </summary>
        /// <param name="id">数据id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRequestCashInfo(string id)
        {
            var type = Convert.ToInt32(HttpContext.User.FindFirstValue("Type"));
            var mainAgentID = string.Empty;
            if (type != (int)ManagementTypeEnum.财务类型)
            {
                if (!HttpContext.IsAdmin())
                    return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
                mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            }
            else
            {
                //查询超级管理员
                var agent = await agentUserOperation.GetModelAsync(t => t.IsHighest == true);
                mainAgentID = agent._id;
            }
            CashWithdrawalOperation cashWithdrawalOperation = new CashWithdrawalOperation();
            var data = await cashWithdrawalOperation.GetModelAsync(t => t._id == id && t.AcceptanceAgentID == mainAgentID
            && t.Status == WithdrawalStatusEnum.申请);
            if (data == null)
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到提现申请明细！"));
            dynamic result = null;
            if (data.ApplyType == ApplyTypeEnum.支付宝)
            {
                AlipayInfoOperation alipayInfoOperation = new AlipayInfoOperation();
                var alipay = await alipayInfoOperation.GetModelAsync(t => t.AgentID == data.ApplyAgentID);
                if (alipay == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到申请代理绑定支付宝！"));
                result = new
                {
                    AgentName = data.ApplyAgentName,
                    Transfer = "支付宝",
                    PayName = alipay.Account,
                    QRCode = alipay.QRPath,
                    data.Amount,
                    data.ApplyType
                };
            }
            else if (data.ApplyType == ApplyTypeEnum.银行卡)
            {
                BankInfoOperation bankInfoOperation = new BankInfoOperation();
                var bank = await bankInfoOperation.GetModelAsync(t => t.AgentID == data.ApplyAgentID);
                if (bank == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到申请代理绑定支付宝！"));
                result = new
                {
                    AgentName = data.ApplyAgentName,
                    Transfer = Enum.GetName(typeof(BankEnum), (int)bank.BankType),
                    PayName = bank.Name,
                    bank.BankNum,
                    data.Amount,
                    data.ApplyType
                };
            }
            else if (data.ApplyType == ApplyTypeEnum.泰达币)
            {
                VirtualCurrencyOperation virtualCurrencyOperation = new VirtualCurrencyOperation();
                var model = await virtualCurrencyOperation.GetModelAsync(t => t.AgentID == data.ApplyAgentID);
                if (model == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到申请代理绑定泰达币账号！"));
                result = new
                {
                    AgentName = data.ApplyAgentName,
                    Transfer = "泰达币",
                    data.Amount,
                    data.ApplyType,
                    Path = model
                };
            }
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功",
                Model = result,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 确认放款
        /// </summary>
        /// <param name="id">数据id</param>
        /// <param name="opinion">备注</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ConfirmationRequest(string id, string opinion)
        {
            var type = Convert.ToInt32(HttpContext.User.FindFirstValue("Type"));
            var mainAgentID = string.Empty;
            if (type != (int)ManagementTypeEnum.财务类型)
            {
                if (!HttpContext.IsAdmin())
                    return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
                mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            }
            else
            {
                //查询超级管理员
                var agent = await agentUserOperation.GetModelAsync(t => t.IsHighest == true);
                mainAgentID = agent._id;
            }
            CashWithdrawalOperation cashWithdrawalOperation = new CashWithdrawalOperation();
            var data = await cashWithdrawalOperation.GetModelAsync(t => t._id == id && t.AcceptanceAgentID == mainAgentID
            && t.Status == WithdrawalStatusEnum.申请);
            var log = new AgentMainLog()
            {
                AgentID = mainAgentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "提现放款"
            };
            if (data == null)
            {
                log.Remark = "未查询到提现申请明细";
                log.Status = OperationStatusEnum.失败;
                await agentMainLogOperation.InsertModelAsync(log);
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到提现申请明细！"));
            }
            data.Status = WithdrawalStatusEnum.已放款;
            await cashWithdrawalOperation.UpdateModelAsync(data);
            log.Remark = string.Format("确认放款[{0}]{1}\r\n{2}", data.ApplyAgentName, data.Amount, opinion);
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 拒绝放款
        /// </summary>
        /// <param name="id">数据id</param>
        /// <param name="opinion">备注</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RefuseRequest(string id, string opinion)
        {
            var type = Convert.ToInt32(HttpContext.User.FindFirstValue("Type"));
            var mainAgentID = string.Empty;
            if (type != (int)ManagementTypeEnum.财务类型)
            {
                if (!HttpContext.IsAdmin())
                    return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
                mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            }
            else
            {
                //查询超级管理员
                var agent = await agentUserOperation.GetModelAsync(t => t.IsHighest == true);
                mainAgentID = agent._id;
            }
            CashWithdrawalOperation cashWithdrawalOperation = new CashWithdrawalOperation();
            var data = await cashWithdrawalOperation.GetModelAsync(t => t._id == id && t.AcceptanceAgentID == mainAgentID
            && t.Status == WithdrawalStatusEnum.申请);
            var log = new AgentMainLog()
            {
                AgentID = mainAgentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "拒绝提现放款"
            };
            if (data == null)
            {
                log.Remark = "未查询到提现申请明细";
                log.Status = OperationStatusEnum.失败;
                await agentMainLogOperation.InsertModelAsync(log);
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到提现申请明细！"));
            }
            data.Status = WithdrawalStatusEnum.已拒绝;
            await cashWithdrawalOperation.UpdateModelAsync(data);
            log.Remark = string.Format("拒绝放款[{0}]{1}\r\n{2}", data.ApplyAgentName, data.Amount, opinion);
            await agentMainLogOperation.InsertModelAsync(log);
            //退还
            await agentUserOperation.UpScore(data.ApplyAgentID, data.Amount, AccountTypeEnum.提现退回,
                string.Format("商家拒绝提现请求\r\n{0}", opinion), false);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 提现记录查询
        /// </summary>
        /// <param name="loginName">用户名</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             Name：用户名
        ///             Amount：提现金额
        ///             Balance：当前余额
        ///             ApplyTime：申请时间
        ///             HandleTime：处理时间
        ///             Type：提现方式
        ///         }
        ///         Summary：放款汇总
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RequestRecord(string loginName, DateTime startTime, DateTime endTime, int start = 1, int pageSize = 10)
        {
            var type = Convert.ToInt32(HttpContext.User.FindFirstValue("Type"));
            var mainAgentID = string.Empty;
            if (type != (int)ManagementTypeEnum.财务类型)
            {
                if (!HttpContext.IsAdmin())
                    return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
                mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            }
            else
            {
                //查询超级管理员
                var agent = await agentUserOperation.GetModelAsync(t => t.IsHighest == true);
                mainAgentID = agent._id;
            }
            CashWithdrawalOperation cashWithdrawalOperation = new CashWithdrawalOperation();
            FilterDefinition<CashWithdrawal> filter = cashWithdrawalOperation.Builder.Where(t => t.AcceptanceAgentID == mainAgentID
            && t.Status != WithdrawalStatusEnum.申请 && t.CreatedTime >= startTime && t.CreatedTime <= endTime);
            if (!string.IsNullOrEmpty(loginName))
                filter &= cashWithdrawalOperation.Builder.Regex(t => t.ApplyAgentName, loginName);
            var list = cashWithdrawalOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var allList = await cashWithdrawalOperation.GetModelListAsync(filter);
            var total = allList.Count;
            //AlipayInfoOperation alipayInfoOperation = new AlipayInfoOperation();
            //BankInfoOperation bankInfoOperation = new BankInfoOperation();
            var result = (from data in list
                          select new WebRequestRecord
                          {
                              Name = data.ApplyAgentName,
                              Amount = data.Amount,
                              Balance = agentUserOperation.GetModel(t => t._id == data.ApplyAgentID && t.HighestAgentID == data.AcceptanceAgentID)?.UserBalance,
                              ApplyTime = data.CreatedTime.ToString("yyyy/MM/dd HH:mm"),
                              HandleTime = data.LastUpdateTime.ToString("yyyy/MM/dd HH:mm"),
                              Type = Enum.GetName(typeof(ApplyTypeEnum), (int)data.ApplyType),
                              Url = data.Acceptance,
                              Status = Enum.GetName(typeof(WithdrawalStatusEnum), (int)data.Status)
                          }).ToList();
            return Ok(new
            {
                Data = result,
                Total = total,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Summary = allList.FindAll(t => t.Status == WithdrawalStatusEnum.已放款).Sum(t => t.Amount)
            });
        }

        private class WebRequestRecord
        {
            public string Name { get; set; }
            public decimal Amount { get; set; }
            public decimal? Balance { get; set; }
            public string ApplyTime { get; set; }
            public string HandleTime { get; set; }
            public string Type { get; set; }

            public string Url { get; set; }
            public string Status { get; set; }
        }

        /// <summary>
        /// 获取商户充值记录
        /// </summary>
        /// <param name="keyword">关键字  商家名称   代理名称</param>
        /// <param name="salesType">销售方式  1：余额 2：库存  3：临时</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchMerchantRecharge(string keyword, SalesTypeEnum? salesType, DateTime? startTime, DateTime? endTime, int start = 1, int pageSize = 10)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            FilterDefinition<AgentUser> filter = agentUserOperation.Builder.Eq(t => t.HighestAgentID, agentID);
            //全部代理
            var list = agentUserOperation.GetModelList(filter);
            var agentIDList = list.Select(t => t._id).ToList();

            //查询充值定单
            SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            FilterDefinition<SalesRecords> salesRecordsfilter = salesRecordsOperation.Builder.Where(t => t.CreatedTime >= startTime && t.CreatedTime <= endTime) & salesRecordsOperation.Builder.In(t => t.AgentID, agentIDList);
            if (!string.IsNullOrEmpty(keyword))
                salesRecordsfilter &= salesRecordsOperation.Builder.Regex(t => t.MerchantName, keyword);
            if (salesType != null)
                salesRecordsfilter &= salesRecordsOperation.Builder.Eq(t => t.SalesType, salesType.Value);
            var recoreds = salesRecordsOperation.GetModelListByPaging(salesRecordsfilter, t => t.CreatedTime, false, start, pageSize);
            var total = await salesRecordsOperation.GetCountAsync(salesRecordsfilter);
            var result = new List<dynamic>();
            foreach (var data in recoreds)
            {
                var agent = list.Find(t => t._id == data.AgentID);
                result.Add(new
                {
                    data.MerchantName,
                    AgentName = agent?.LoginName,
                    data.Amount,
                    PayType = Enum.GetName(typeof(SalesTypeEnum), (int)data.SalesType) + "直充",
                    MaturityTime = data?.MaturityTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    OperationTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss")
                });
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Message = "获取成功",
                Data = result,
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        /// <summary>
        /// 获取充值订单列表
        /// </summary>
        /// <param name="loginName">用户名称</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             ID：数据id
        ///             RechargeName：充值对象
        ///             DirectName：直接代理
        ///             Amount：金额
        ///             ActualAmount：实际余额
        ///             OrderType：订单方式
        ///             Status：订单状态
        ///             ApplyTime：申请时间
        ///             IsHandle：是否处理
        ///             HandleTime：处理时间
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchRecharge(string loginName, DateTime? startTime, DateTime? endTime, int start = 1, int pageSize = 10)
        {
            var type = Convert.ToInt32(HttpContext.User.FindFirstValue("Type"));
            var mainAgentID = string.Empty;
            if (type != (int)ManagementTypeEnum.财务类型)
            {
                if (!HttpContext.IsAdmin())
                    return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
                mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            }
            else
            {
                //查询超级管理员
                var agent = await agentUserOperation.GetModelAsync(t => t.IsHighest == true);
                mainAgentID = agent._id;
            }
            RechargeOperation rechargeOperation = new RechargeOperation();
            FilterDefinition<Recharge> filter = rechargeOperation.Builder.Eq(t => t.AcceptanceAgentID, mainAgentID);
            if (!string.IsNullOrEmpty(loginName))
                filter &= rechargeOperation.Builder.Regex(t => t.ApplyAgentName, loginName);
            if (startTime != null)
                filter &= rechargeOperation.Builder.Where(t => t.CreatedTime >= startTime);
            if (endTime != null)
                filter &= rechargeOperation.Builder.Where(t => t.CreatedTime <= endTime);
            var list = rechargeOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var total = await rechargeOperation.GetCountAsync(filter);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                var agent = await agentUserOperation.GetModelAsync(t => t._id == data.ApplyAgentID);
                var perAgent = string.IsNullOrEmpty(agent.SupAgentID) ? null :
                    agentUserOperation.GetModel(t => t._id == agent.SupAgentID)?.LoginName;
                result.Add(new
                {
                    ID = data._id,
                    RechargeName = data.ApplyAgentName,
                    DirectName = perAgent,
                    data.Amount,
                    data.ActualAmount,
                    data.OrderType,
                    Status = !data.IsHandle ? "待确认" : Enum.GetName(typeof(ROrderStatusEnum), (int)data.OrderStatus),
                    ApplyTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm"),
                    data.IsHandle,
                    HandleTime = data.LastUpdateTime.ToString("yyyy-MM-dd HH:mm")
                });
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        /// <summary>
        /// 确认到账
        /// </summary>
        /// <param name="id">数据id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ConfirmationArrival(string id)
        {
            var type = Convert.ToInt32(HttpContext.User.FindFirstValue("Type"));
            var mainAgentID = string.Empty;
            if (type != (int)ManagementTypeEnum.财务类型)
            {
                if (!HttpContext.IsAdmin())
                    return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
                mainAgentID = HttpContext.User.FindFirstValue("AgentID");
            }
            else
            {
                //查询超级管理员
                var agentInfo = await agentUserOperation.GetModelAsync(t => t.IsHighest == true);
                mainAgentID = agentInfo._id;
            }
            RechargeOperation rechargeOperation = new RechargeOperation();
            var model = await rechargeOperation.GetModelAsync(t => t._id == id
            && t.AcceptanceAgentID == mainAgentID && t.IsHandle == false);
            if (model == null)
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到充值订单明细！"));
            model.IsHandle = true;
            await rechargeOperation.UpdateModelAsync(model);
            var agent = await agentUserOperation.GetModelAsync(t => t._id == model.ApplyAgentID && t.HighestAgentID == mainAgentID);
            var log = new AgentMainLog()
            {
                AgentID = mainAgentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "确认充值到账",
                Remark = string.Format("[{0}]充值确认到账{1}", agent.LoginName, model.Amount)
            };
            await agentMainLogOperation.InsertModelAsync(log);
            //加款
            await agentUserOperation.UpScore(agent._id, model.Amount, AccountTypeEnum.余额充值, "充值成功", true);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取统一出款时间
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSetTime()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = string.Format("{0}-{1}", data.PayStartTime.ToString("HH:mm"),
                data.PayEndTime.ToString("HH:mm")),
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 设置统一出款时间
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetPayTime(DateTime startTime, DateTime endTime)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            data.PayStartTime = startTime;
            data.PayEndTime = endTime;
            await advancedSetupOperation.UpdateModelAsync(data);
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "配置出款时间",
                Remark = "配置出款时间"
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }

        /// <summary>
        /// 加扣客户有效时间
        /// </summary>
        /// <param name="cusName">客户名称</param>
        /// <param name="num">数量</param>
        /// <param name="type">类型 1：月  2：天</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetDeductionTime(string cusName, int num, int type)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            if (num == 0) return Ok(new RecoverModel(RecoverEnum.失败, "数量有误！"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchat = await merchantOperation.GetModelAsync(t => t.MeName == cusName);
            if (merchat == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到客户信息"));
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchat.AgentID
            && t.HighestAgentID == agentID);
            if (agent == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到客户信息"));
            if (type == 1)
            {
                merchat.MaturityTime = merchat.MaturityTime.AddMonths(num);
                //merchat.MaturityTime = merchat.MaturityTime > DateTime.Now ?
                //    merchat.MaturityTime.AddMonths(num) : DateTime.Now.AddMonths(num);
            }
            else if (type == 2)
                merchat.MaturityTime = merchat.MaturityTime.AddDays(num);
            //merchat.MaturityTime = merchat.MaturityTime > DateTime.Now ?
            //    merchat.MaturityTime.AddDays(num) : DateTime.Now.AddDays(num);
            merchat.RechargeTime = DateTime.Now;
            //添加日志
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "加扣客户有效时间",
                Remark = string.Format("[{0}]修改到期时间{1}{2}", cusName,
                num, type == 1 ? "月" : "天")
            };
            await agentMainLogOperation.InsertModelAsync(log);
            await merchantOperation.UpdateModelAsync(merchat);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }

        /// <summary>
        /// 手动处理客户充值
        /// </summary>
        /// <param name="agentName">代理名称</param>
        /// <param name="merchantName">商户名称</param>
        /// <param name="amount">扣除金额</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ManualRecharge(string agentName, string merchantName, decimal amount)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var agent = await agentUserOperation.GetModelAsync(t => t.HighestAgentID == agentID
             && t.LoginName == agentName);
            if (agent == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息"));
            //查询运营状态
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            if (!setup.Formal) return Ok(new RecoverModel(RecoverEnum.失败, "当前系统处于测试模式，不能为商户充值！"));
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t.MeName == merchantName && t.AgentID == agent._id);
            if (merchant == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到客户信息"));
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "手动客户充值"
            };
            if (agent.UserBalance < amount)
            {
                log.Status = OperationStatusEnum.失败;
                log.Remark = string.Format("[{0}]代理余额不足", agent.LoginName);
                await agentMainLogOperation.InsertModelAsync(log);
                return Ok(new RecoverModel(RecoverEnum.失败, "代理余额不足"));
            }
            #region 扣费  商家时间延长
            await agentUserOperation.DownScore(agent._id, amount, AccountTypeEnum.直充扣费,
                   string.Format("[{0}]充值月卡 商家代理扣费", merchant.MeName), true, true);
            SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            var saleCount = await salesRecordsOperation.GetCountAsync(t => t.AgentID == agent._id
            && t.MerchantID == merchant._id && t.SalesType != SalesTypeEnum.临时);
            var sales = new SalesRecords()
            {
                AgentID = agent._id,
                MerchantID = merchant._id,
                SalesType = SalesTypeEnum.余额,
                Amount = agent.SubAgentPrice,
                MerchantName = merchant.MeName
            };
            await salesRecordsOperation.InsertModelAsync(sales);

            RechargeOperation rechargeOperation = new RechargeOperation();
            //添加充值记录
            var recharge = new Recharge()
            {
                OrderType = "余额直充",
                ApplyAgentID = agent._id,
                AcceptanceAgentID = agent.HighestAgentID,
                ActualAmount = agent.SubAgentPrice,
                Amount = agent.SubAgentPrice,
                ApplyAgentName = agent.LoginName,
                IsHandle = true
            };
            await rechargeOperation.InsertModelAsync(recharge);

            //添加时长
            //查询是否充值过
            if (saleCount > 0)
                merchant.MaturityTime = merchant.MaturityTime.AddMonths(1);
            else
                merchant.MaturityTime = DateTime.Now.AddMonths(1);
            merchant.RechargeTime = DateTime.Now;
            await merchantOperation.UpdateModelAsync(merchant);
            log.Remark = string.Format("[{0}]充值商户[{1}]，扣费{2}", agent.LoginName, merchant.MeName, amount);
            await agentMainLogOperation.InsertModelAsync(log);
            #endregion

            return Ok(new RecoverModel(RecoverEnum.成功, "充值成功"));
        }

        /// <summary>
        /// 配置人工收款二维码
        /// </summary>
        /// <param name="fileinput">图片文件</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetReceivables(IFormFile fileinput)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            if (fileinput == null) return Ok(new RecoverModel(RecoverEnum.失败, "未选择图片！"));
            var url = await BlobHelper.UploadImageToBlob(fileinput, "AdminImages");
            if (string.IsNullOrEmpty(url)) return Ok(new RecoverModel(RecoverEnum.失败, "图片格式错误！"));
            if (url == "1") return Ok(new RecoverModel(RecoverEnum.失败, "图片大小最大为20M！"));
            data.PaymentCode = url;

            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "配置收款码",
                Remark = "配置人工收款码"
            };
            await agentMainLogOperation.InsertModelAsync(log);
            await advancedSetupOperation.UpdateModelAsync(data);
            return Ok(new RecoverModel(RecoverEnum.成功, "配置成功"));
        }

        /// <summary>
        /// 预览二维码 地址
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> PreviewFile()
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = data.PaymentCode,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 重置客户密码
        /// </summary>
        /// <param name="cusName">客户名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ResetCustomerPwd(string cusName)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchat = await merchantOperation.GetModelAsync(t => t.MeName == cusName);
            if (merchat == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到客户信息"));
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchat.AgentID
            && t.HighestAgentID == agentID);
            if (agent == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到客户信息"));
            merchat.Password = Utils.MD5("123456a");

            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "重置客户密码",
                Remark = string.Format("[{0}]重置了密码", cusName)
            };
            await agentMainLogOperation.InsertModelAsync(log);
            await merchantOperation.UpdateModelAsync(merchat);

            return Ok(new RecoverModel(RecoverEnum.成功, "重置成功"));
        }

        /// <summary>
        /// 增减代理库存月卡
        /// </summary>
        /// <param name="agentName">代理名称</param>
        /// <param name="num">数量  不为0</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> OperationMonthly(string agentName, int num)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var agent = await agentUserOperation.GetModelAsync(t => t.HighestAgentID == agentID
             && t.LoginName == agentName);
            if (agent == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息"));
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "加扣代理库存"
            };
            if (num <= 0)
            {
                num = -num;
                if (agent.Stock < num)
                    return Ok(new RecoverModel(RecoverEnum.失败, "该代理库存不足"));
                agent.Stock -= num;
                log.Remark = string.Format("[{0}]减少库存{1}张", agent.LoginName, -num);
            }
            else
            {
                agent.Stock += num;
                log.Remark = string.Format("[{0}]增加库存{1}张", agent.LoginName, num);


                //添加充值记录
                RechargeOperation rechargeOperation = new RechargeOperation();
                var recharge = new Recharge()
                {
                    OrderType = "库存直充",
                    ApplyAgentID = agent._id,
                    AcceptanceAgentID = agentID,
                    ActualAmount = 0,
                    Amount = agent.SubAgentPrice * num,
                    ApplyAgentName = agent.LoginName,
                    IsHandle = true
                };
                await rechargeOperation.InsertModelAsync(recharge);
            }

            await agentUserOperation.UpdateModelAsync(agent);
            await agentMainLogOperation.InsertModelAsync(log);

            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取滚动公告
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoll()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = data.Notice,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 修改滚动公告
        /// </summary>
        /// <param name="notice">公告内容</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateRoll(string notice)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            data.Notice = notice;
            await advancedSetupOperation.UpdateModelAsync(data);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 获取操作日志
        /// </summary>
        /// <param name="type">操作类型</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             OperationTime：操作时间
        ///             OperationName：操作用户
        ///             IP：登录ip
        ///             OperationType：操作类型
        ///             Status：状态
        ///             Remark：备注
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchOperationLogs(string type, int start = 1, int pageSize = 10)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            FilterDefinition<AgentMainLog> filter = agentMainLogOperation.Builder.Eq(t => t.AgentID, agentID);
            if (!string.IsNullOrEmpty(type))
                filter &= agentMainLogOperation.Builder.Regex(t => t.OperationMsg, type);
            var list = agentMainLogOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var total = await agentMainLogOperation.GetCountAsync(filter);
            var result = (from data in list
                          select new WebOperationLogs
                          {
                              OperationTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                              OperationName = agent.LoginName,
                              IP = data.LoginIP,
                              OperationType = data.OperationMsg,
                              Status = Enum.GetName(typeof(OperationStatusEnum), (int)data.Status),
                              Remark = data.Remark
                          }).ToList();
            return Ok(new RecoverListModel<WebOperationLogs>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        private class WebOperationLogs
        {
            public string OperationTime { get; set; }
            public string OperationName { get; set; }
            public string IP { get; set; }
            public string OperationType { get; set; }
            public string Status { get; set; }
            public string Remark { get; set; }
        }

        /// <summary>
        /// 获取高级管理公告
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetNotice()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            var result = new
            {
                Title = data.TitleBulletin,
                data.Content,
                Time = data.LastUpdateTime.ToString("yyyy-MM-dd HH:mm:ss")
            };
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功",
                Model = result,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 公告内容
        /// </summary>
        public class NoticeClass
        {
            /// <summary>
            /// 标题
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 内容
            /// </summary>
            public string Content { get; set; }
        }

        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="model">公告信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateNotice([FromBody]NoticeClass model)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            data.TitleBulletin = model.Title;
            data.Content = model.Content;
            await advancedSetupOperation.UpdateModelAsync(data);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 获取连接地址列表
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             ID：数据id
        ///             Address：连接地址
        ///             Count：现有人数
        ///             MaxCount：最大人数
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDatabaseList()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
            var addressList = await databaseAddressOperation.GetModelListAsync(t => t._id != null);
            MerchantOperation merchantOperation = new MerchantOperation();
            var result = new List<DBAddress>();
            foreach (var data in addressList)
            {
                var count = await merchantOperation.GetCountAsync(t => t.AddressID == data._id);
                result.Add(new DBAddress()
                {
                    ID = data._id,
                    Address = data.Address,
                    Count = (int)count,
                    MaxCount = data.MaxCount,
                    DBName = data.DBName
                });
            }
            return Ok(new RecoverListModel<DBAddress>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 添加连接地址
        /// </summary>
        /// <param name="path">连接地址</param>
        /// <param name="dbName">数据库名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddDatabas(string path, string dbName)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
            var address = new DatabaseAddress()
            {
                Address = path,
                DBName = dbName
            };
            //测试填写地址是否可用
            try
            {
                MongoUrl url = new MongoUrl(path); // url
                MongoClientSettings settings = MongoClientSettings.FromUrl(url);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
                settings.ConnectTimeout = TimeSpan.FromSeconds(10);
                MongoClient server = new MongoClient(settings);
                server.ListDatabaseNames();
            }
            catch (Exception e)
            {
                return Ok(new RecoverModel(RecoverEnum.失败, "连接地址不可用！" + "\r\n" + JsonConvert.SerializeObject(e)));
            }
            await databaseAddressOperation.InsertModelAsync(address);
            return Ok(new RecoverModel(RecoverEnum.成功, "添加成功"));
        }

        /// <summary>
        /// 修改数据库地址
        /// </summary>
        /// <param name="id"></param>
        /// <param name="path">连接地址</param>
        /// <param name="dbName">数据库名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateDatabas(string id, string path, string dbName)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
            var address = await databaseAddressOperation.GetModelAsync(t => t._id == id);
            if (address == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到数据信息！"));
            address.Address = path;
            address.DBName = dbName;
            //测试填写地址是否可用
            try
            {
                MongoUrl url = new MongoUrl(path); // url
                MongoClientSettings settings = MongoClientSettings.FromUrl(url);
                settings.ServerSelectionTimeout = TimeSpan.FromSeconds(10);
                settings.ConnectTimeout = TimeSpan.FromSeconds(10);
                MongoClient server = new MongoClient(settings);
                server.ListDatabaseNames();
            }
            catch (Exception e)
            {
                return Ok(new RecoverModel(RecoverEnum.失败, "连接地址不可用！" + "\r\n" + JsonConvert.SerializeObject(e)));
            }
            await databaseAddressOperation.UpdateModelAsync(address);
            return Ok(new RecoverModel(RecoverEnum.成功, "添加成功"));
        }

        /// <summary>
        /// 数据库信息
        /// </summary>
        public class DBAddress
        {
            /// <summary>
            /// id
            /// </summary>
            public string ID { get; set; }
            /// <summary>
            /// 地址
            /// </summary>
            public string Address { get; set; }
            /// <summary>
            /// 已使用数量
            /// </summary>
            public int Count { get; set; }
            /// <summary>
            /// 最大使用数量
            /// </summary>
            public int MaxCount { get; set; }

            /// <summary>
            /// 数据库名称
            /// </summary>
            public string DBName { get; set; }
        }

        /// <summary>
        /// 添加兑换码
        /// </summary>
        /// <param name="count">添加数量</param>
        /// <param name="amount">金额</param>
        /// <param name="time">到期时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddRedeem(int count, decimal amount, DateTime time)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            if (count <= 0) return Ok(new RecoverModel(RecoverEnum.参数错误, "添加数量有误"));
            if (time < DateTime.Now) return Ok(new RecoverModel(RecoverEnum.失败, "到期时间有误"));
            RedeemOperation redeemOperation = new RedeemOperation();
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            for (var i = 0; i < count; i++)
            {
                Redeem redeem = new Redeem()
                {
                    AgentID = agentID,
                    Status = RedeemEnum.未使用,
                    Amount = amount,
                    EffectiveTime = time,
                    RedeemCode = Guid.NewGuid().ToString().Replace("-", "")
                };
                await redeemOperation.InsertModelAsync(redeem);
            }
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "添加兑换码",
                Status = OperationStatusEnum.成功,
                Remark = string.Format("添加{0}张金额{1}的兑换码", count, amount)
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "添加成功"));
        }

        /// <summary>
        /// 获取所有兑换码信息
        /// </summary>
        /// <param name="keyword">使用代理名称</param>
        /// <param name="status">状态  1：未使用  2：已使用</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             ID：数据Id
        ///             UseName：使用代理名称
        ///             UseTime：使用时间
        ///             EffectiveTime：到期时间
        ///             Status：状态,
        ///             Amount：金额
        ///             RedeemCode：兑换码
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public IActionResult SearchRedeemInfo(string keyword, RedeemEnum? status)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            RedeemOperation redeemOperation = new RedeemOperation();
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            FilterDefinition<Redeem> filter = redeemOperation.Builder.Where(t => t.AgentID == agentID);
            if (!string.IsNullOrEmpty(keyword))
                filter &= redeemOperation.Builder.Regex(t => t.UseAgentName, keyword);
            if (status != null)
                filter &= redeemOperation.Builder.Eq(t => t.Status, status.Value);
            var list = redeemOperation.GetModelList(filter, t => t.EffectiveTime, false);
            var result = new List<dynamic>();
            foreach (var lt in list)
            {
                var data = new
                {
                    ID = lt._id,
                    UseName = lt.UseAgentName,
                    UseTime = lt.UseTime?.ToString("yyyy-MM-dd HH:mm"),
                    EffectiveTime = lt.EffectiveTime.ToString("yyyy-MM-dd HH:mm"),
                    Status = Enum.GetName(typeof(RedeemEnum), (int)lt.Status),
                    lt.Amount,
                    lt.RedeemCode
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
        /// 删除兑换码
        /// </summary>
        /// <param name="reIDList"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteRedeem(List<string> reIDList)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            if (reIDList.IsNull()) return Ok(new RecoverModel(RecoverEnum.失败, "未选择兑换码"));
            RedeemOperation redeemOperation = new RedeemOperation();
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            FilterDefinition<Redeem> filter = redeemOperation.Builder.Where(t => t.AgentID == agentID);
            filter &= redeemOperation.Builder.In(t => t._id, reIDList);
            await redeemOperation.DeleteModelManyAsync(filter);
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "删除兑换码",
                Status = OperationStatusEnum.成功,
                Remark = string.Format("删除{0}张兑换码", reIDList.Count)
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "删除成功"));
        }

        /// <summary>
        /// 获取兑换码链接
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         RedeemUrl:1500
        ///         MeRedeemUrl:2000
        ///         HiRedeemUrl:3000
        ///         ThousandUrl:1000
        ///         ThoushunUrl:1100
        ///         ThoustwohUrl:1200
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRedeemUrl()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            return Ok(new RecoverClassModel<dynamic>()
            {
                Model = new
                {
                    data.RedeemUrl,
                    data.MeRedeemUrl,
                    data.HiRedeemUrl,
                    data.ThousandUrl,
                    data.ThoushunUrl,
                    data.ThoustwohUrl
                },
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 修改兑换码地址
        /// </summary>
        /// <param name="lowurl">1500地址</param>
        /// <param name="meurl">2000地址</param>
        /// <param name="hiurl">3000地址</param>
        /// <param name="taurl">1000地址</param>
        /// <param name="thurl">1100地址</param>
        /// <param name="tturl">1200地址</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateRedeemUrl(string lowurl, string meurl, string hiurl, string taurl, string thurl, string tturl)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            data.RedeemUrl = lowurl;
            data.MeRedeemUrl = meurl;
            data.HiRedeemUrl = hiurl;
            data.ThousandUrl = taurl;
            data.ThoushunUrl = thurl;
            data.ThoustwohUrl = tturl;
            await advancedSetupOperation.UpdateModelAsync(data);
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改兑换码地址",
                Status = OperationStatusEnum.成功,
                Remark = "修改兑换码地址"
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取h5域名设置说明
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetH5Description()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = data.H5DomainDescription,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 设置h5域名设置说明
        /// </summary>
        /// <param name="description"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateH5Description(string description)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            data.H5DomainDescription = description;
            await advancedSetupOperation.UpdateModelAsync(data);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功"));
        }

        /// <summary>
        /// 修改商户滚动公告
        /// </summary>
        /// <param name="notice">公告内容</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateMerchantRoll(string notice)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            data.MerchantNotice = notice;
            await advancedSetupOperation.UpdateModelAsync(data);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 获取商户滚动公告
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMerchantRoll()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = data.MerchantNotice,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取汇总数据
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSummary()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var agentList = await agentUserOperation.GetModelListAsync(t => t.HighestAgentID == agentID);
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchantList = await merchantOperation.GetModelListAsync(merchantOperation.Builder.In(t => t.AgentID, agentList.Select(t => t._id).ToList()));
            UserOperation userOperation = new UserOperation();
            var userCount = await userOperation.GetCountAsync(userOperation.Builder.Where(t => t.Status == UserStatusEnum.正常)
                & userOperation.Builder.In(t => t.MerchantID, merchantList.Select(x => x._id).ToList()));
            return Ok(new RecoverClassModel<dynamic>()
            {
                Model = new
                {
                    AgentCount = agentList.Count,
                    MerchantCount = merchantList.Count,
                    OverdueCount = merchantList.FindAll(t => t.MaturityTime <= DateTime.Now).Count,
                    UserCount = userCount
                },
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取商户信息列表
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             ID：商户id
        ///             MerchantName：商户名称
        ///             DirectAgency：直接代理
        ///             LastRechargeTime：最后充值时间
        ///             ExpireTime：到期时间
        ///             SeurityNo：安全码
        ///             SuperStatus：超级商户
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMerchantInfoList(string keyword, int start = 1, int pageSize = 10)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var agentList = await agentUserOperation.GetModelListAsync(t => t.HighestAgentID == agentID);
            MerchantOperation merchantOperation = new MerchantOperation();
            var filter = merchantOperation.Builder.In(t => t.AgentID, agentList.Select(t => t._id).ToList());
            if (!string.IsNullOrEmpty(keyword))
                filter &= merchantOperation.Builder.Regex(t => t.MeName, keyword);
            var merchantList = merchantOperation.GetModelListByPaging(filter, t => t.CreatedTime, true, start, pageSize);
            var total = await merchantOperation.GetCountAsync(filter);
            var result = new List<dynamic>();
            foreach (var merchant in merchantList)
            {
                var agent = agentList.Find(t => t._id == merchant.AgentID);
                var data = new
                {
                    ID = merchant._id,
                    MerchantName = merchant.MeName,
                    DirectAgency = agent.LoginName,
                    LastRechargeTime = merchant.RechargeTime?.ToString("yyyy-MM-dd HH:mm"),
                    ExpireTime = merchant.MaturityTime.ToString("yyyy-MM-dd HH:mm"),
                    merchant.SeurityNo,
                    merchant.SuperStatus
                };
                result.Add(data);
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Status = RecoverEnum.成功,
                Message = "获取成功",
                Total = total
            });
        }

        /// <summary>
        /// 修改商户是否为超级商户
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <param name="status">状态</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateMerchantStatus(string merchantID, bool status)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            if (merchant == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到客户信息"));
            merchant.SuperStatus = status;
            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }

        /// <summary>
        /// 刷新安全码
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RefreshNo(string merchantID)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchat = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            if (merchat == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到客户信息"));
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchat.AgentID
            && t.HighestAgentID == agentID);
            if (agent == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到客户信息"));
            var seurityNo = await Utils.GetMerchantSeurityNo();

            merchat.SeurityNo = seurityNo;
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "重置客户安全码",
                Remark = string.Format("[{0}]重置了安全码", merchat.MeName)
            };
            await agentMainLogOperation.InsertModelAsync(log);
            await merchantOperation.UpdateModelAsync(merchat);

            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 重置商户登录密码
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RefreshMerchantPwd(string merchantID)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            if (merchant == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到客户信息"));
            merchant.Password = Utils.MD5("888888");
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "重置客户登录密码",
                Remark = string.Format("[{0}]重置了登录密码", merchant.MeName)
            };
            await agentMainLogOperation.InsertModelAsync(log);
            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverModel(RecoverEnum.成功, "重置登录密码：888888"));
        }

        /// <summary>
        /// 重置商户锁屏密码
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RefreshMerchantSecurityPwd(string merchantID)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            if (merchant == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到客户信息"));
            if (string.IsNullOrEmpty(merchant.SecurityPwd))
                return Ok(new RecoverModel(RecoverEnum.失败, "该客户未设置锁屏密码，不能重置！"));
            merchant.SecurityPwd = Utils.MD5("888888");
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "重置客户锁屏密码",
                Remark = string.Format("[{0}]重置了锁屏密码", merchant.MeName)
            };
            await agentMainLogOperation.InsertModelAsync(log);
            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverModel(RecoverEnum.成功, "重置锁屏密码：888888"));
        }

        /// <summary>
        /// 重置代理安全密码
        /// </summary>
        /// <param name="targetAgentID">目标代理id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> RefreshAgentSafePwd(string targetAgentID)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var agent = await agentUserOperation.GetModelAsync(t => t._id == targetAgentID && t.HighestAgentID == agentID);
            if (agent == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息！"));
            agent.SafePassWord = Utils.MD5("888888");
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "重置代理安全密码",
                Remark = string.Format("代理[{0}]重置了安全密码", agent.LoginName)
            };
            await agentMainLogOperation.InsertModelAsync(log);
            await agentUserOperation.UpdateModelAsync(agent);
            return Ok(new RecoverModel(RecoverEnum.成功, "重置安全密码：888888"));
        }

        /// <summary>
        /// 获取游戏信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetGameList()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var result = Utils.GetGameList();
            return Ok(new RecoverListModel<Operation.Common.Utils.GameListItem>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 设置代理极差
        /// </summary>
        /// <param name="num">极差值</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAgencyMargin(decimal num)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            if (num < 0) return Ok(new RecoverModel(RecoverEnum.失败, "极差不能小于0！"));
            setup.AgencyMargin = num;
            await advancedSetupOperation.UpdateModelAsync(setup);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 获取代理极差
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAgencyMargin()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = setup.AgencyMargin.ToString(),
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        #region 修改绑定商户上级代理
        /// <summary>
        /// 验证目标商户是否存在
        /// </summary>
        /// <param name="merchantName">商户名称</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTargetMerchant(string merchantName)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t.MeName == merchantName);
            if (merchant == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到商户信息！"));
            //查询上级代理是否为此高级代理
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchant.AgentID);
            if (agent == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到该商户的上级代理！"));
            if (agent.HighestAgentID != agentID)
                return Ok(new RecoverModel(RecoverEnum.失败, "验证失败，请确认商户名称！"));
            return Ok(new RecoverKeywordModel()
            {
                Keyword = agent.LoginName,
                Message = "验证成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取所有代理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAgent()
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var agentList = await agentUserOperation.GetModelListAsync(t => t.HighestAgentID == agentID);
            return Ok(new RecoverListModel<string>()
            {
                Data = agentList.Select(t => t.LoginName).ToList(),
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = agentList.Count
            });
        }

        /// <summary>
        /// 修改商户代理
        /// </summary>
        /// <param name="merchantName">商户名称</param>
        /// <param name="agentName">代理名称</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateTargetMerchant(string merchantName, string agentName)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t.MeName == merchantName);
            if (merchant == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到商户信息！"));
            //查询上级代理是否为此高级代理
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchant.AgentID);
            if (agent == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到该商户的上级代理！"));
            if (agent.HighestAgentID != agentID)
                return Ok(new RecoverModel(RecoverEnum.失败, "验证失败，请确认商户名称！"));
            //目标代理
            var tarMerchant = await agentUserOperation.GetModelAsync(t => t.LoginName == agentName && t.HighestAgentID == agentID);
            if (tarMerchant == null) return Ok(new RecoverModel(RecoverEnum.失败, "目标代理不存在！"));
            merchant.AgentID = tarMerchant._id;
            await merchantOperation.UpdateModelAsync(merchant);
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改商户目标代理",
                Remark = string.Format("[{0}]修改目标代理为[{1}]", merchant.MeName, tarMerchant.LoginName)
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.失败, "修改成功！"));
        }
        #endregion

        #region 系统测试运营
        /// <summary>
        /// 查看运营模式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFormal()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            return Ok(new
            {
                Keyword = setup.Formal,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 修改运营模式
        /// </summary>
        /// <param name="status">true:正式  false:测试</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateFormal(bool status)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            setup.Formal = status;
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改运营模式",
                Remark = string.Format("修改运营模式为[{0}]", status ? "正式" : "测试")
            };
            await agentMainLogOperation.InsertModelAsync(log);
            await advancedSetupOperation.UpdateModelAsync(setup);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }
        #endregion

        /// <summary>
        /// 在线商户信息
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="start">页数</param>
        /// <param name="pageSize">页码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMerchatOnlineInfo(string keyword, int start = 1, int pageSize = 10)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var agentList = await agentUserOperation.GetModelListAsync(t => t.HighestAgentID == agentID);
            MerchantOperation merchantOperation = new MerchantOperation();
            FilterDefinition<Merchant> filter = merchantOperation.Builder.In(t => t.AgentID, agentList.Select(t => t._id).ToList());
            if (!string.IsNullOrEmpty(keyword))
                filter &= merchantOperation.Builder.Regex(t => t.MeName, keyword);
            var merchantList = merchantOperation.GetModelList(filter, t => t.LoginTime, false);
            var result = new List<dynamic>();
            foreach (var merchant in merchantList)
            {
                if (merchant.OnLineTime == null) continue;
                if ((DateTime.Now - merchant.OnLineTime.Value).TotalMinutes > 10) continue;
                result.Add(new
                {
                    MerchantName = merchant.MeName,
                    LoginTime = merchant.LoginTime.Value.ToString("yyyy-MM-dd HH:mm")
                });
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result.Skip((start - 1) * pageSize).Take(pageSize).ToList(),
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 验证超级管理密码
        /// </summary>
        /// <param name="pwd">管理密码</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SuperValidation(string pwd)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agentID);
            if (setup.SuperPwd != pwd)
                return Ok(new RecoverModel(RecoverEnum.失败, "超级管理密码验证失败！"));
            return Ok(new RecoverModel(RecoverEnum.成功, "验证成功！"));
        }

        /// <summary>
        /// 清理两天未登录且未充值过的商户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ClearMerchant()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            MerchantOperation merchantOperation = new MerchantOperation();
            //删除未充值过的商户  添加时间和登录时间超过两天的
            await merchantOperation.DeleteModelManyAsync(t => t.RechargeTime == null
            && (t.LoginTime == null || t.LoginTime <= DateTime.Now.AddDays(-2))
            && t.CreatedTime <= DateTime.Now.AddDays(-2));
            SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            //删除后查看所有添加时间和登录时间超过两天的商户   判断是否充值过
            var list = await merchantOperation.GetModelListAsync(t => (t.LoginTime == null || t.LoginTime <= DateTime.Now.AddDays(-2) && t.MaturityTime < DateTime.Now)
            && t.CreatedTime <= DateTime.Now.AddDays(-2));
            foreach (var merchant in list)
            {
                var count = await salesRecordsOperation.GetCountAsync(t => t.MerchantID == merchant._id && t.SalesType != SalesTypeEnum.临时);
                if (count == 0)
                    await merchantOperation.DeleteModelOneAsync(t => t._id == merchant._id);
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "清理成功！"));
        }

        #region
        ///// <summary>
        ///// 删除所有测试数据（只保存销售admin）
        ///// </summary>
        ///// <returns></returns>
        //[HttpGet]
        //public async Task<IActionResult> DeleteTestData()
        //{
        //    if (!HttpContext.IsAdmin())return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));

        //    var agentID = HttpContext.User.FindFirstValue("AgentID");
        //    //删除商户
        //    MerchantOperation merchantOperation = new MerchantOperation();
        //    List<string> baseNames = new List<string>
        //    {
        //        "Merchant",
        //        "AgentBackwater",
        //        "Article",
        //        "BackwaterJournal",
        //        "BackwaterSetup",
        //        "BetLimitOrdinary",
        //        "BetLimitSpecial",
        //        "BetLimitBaccarat",
        //        "FoundationSetup",
        //        "OddsOrdinary",
        //        "OddsSpecial",
        //        "OddsBaccarat",
        //        "ReplySetUp",
        //        "Room",
        //        "RoomGameDetailed",
        //        "VideoRoom",
        //        "Sensitive",
        //        "ShamRobotmanage",
        //        "User",
        //        "UserIntegration"
        //    };
        //    //下注表
        //    var merchantList = await merchantOperation.GetModelListAsync(t => t._id != "");
        //    foreach (var merchant in merchantList)
        //    {
        //        baseNames.Add(string.Format("{0}{1}", "UserBetInfo", merchant._id));
        //    }
        //    //销售
        //    //删除除admin外的销售信息
        //    await agentUserOperation.DeleteModelManyAsync(t => t._id != agentID && t.IsHighest == false);
        //    AlipayInfoOperation alipayInfoOperation = new AlipayInfoOperation();
        //    BankInfoOperation bankInfoOperation = new BankInfoOperation();
        //    ContactOperation contactOperation = new ContactOperation();
        //    //删除相关信息
        //    await alipayInfoOperation.DeleteModelManyAsync(t => t.AgentID != agentID);
        //    await bankInfoOperation.DeleteModelManyAsync(t => t.AgentID != agentID);
        //    await contactOperation.DeleteModelManyAsync(t => t.AgentID != agentID);
        //    DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
        //    var databaseList = await databaseAddressOperation.GetModelListAsync(t => t._id != "");
        //    foreach (var database in databaseList)
        //    {
        //        database.Count = 0;
        //        await databaseAddressOperation.UpdateModelAsync(database);
        //    }
        //    baseNames.Add("CashWithdrawal");
        //    baseNames.Add("MerchantTestRecord");
        //    baseNames.Add("Recharge");
        //    baseNames.Add("SalesRecords");
        //    baseNames.Add("AccountingRecord");
        //    foreach (var name in baseNames)
        //    {
        //        await baseMongo.Database.DropCollectionAsync(name);
        //    }
        //    var log = new AgentMainLog()
        //    {
        //        AgentID = agentID,
        //        LoginIP = HttpContext.GetIP(),
        //        OperationMsg = "删除销售商户所有数据",
        //        Remark = "删除销售商户所有数据"
        //    };
        //    await agentMainLogOperation.InsertModelAsync(log);
        //    return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        //}
        #endregion
    }
}