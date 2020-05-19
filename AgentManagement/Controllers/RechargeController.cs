using AgentManagement.Manipulate;
using Entity;
using Entity.AgentModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Operation.Abutment;
using Operation.Agent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgentManagement.Controllers
{
    /// <summary>
    /// 充值
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    public class RechargeController : ControllerBase
    {
        /// <summary>
        /// 获取帐户剩余可赠送时长和使用金额
        /// </summary>
        /// <param name="duration"></param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             Count：剩余时长
        ///             Price：使用金额
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAccountDuration(int duration)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            MerchantTestRecordOperation merchantTestRecordOperation = new MerchantTestRecordOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            var list = await merchantTestRecordOperation.GetModelListAsync(t => t.AgentID == agentID
            && t.CreatedTime >= DateTime.Today && t.CreatedTime <= DateTime.Now);
            //已使用时长
            var count = list.Sum(t => t.Duration);
            decimal price = 0;
            if ((count + duration) > agent.FreeDuration)
            {
                var recharge = (count + duration) - agent.FreeDuration;
                //代理价格
                price = agent.ExcessExpenses * recharge;
            }
            var result = new
            {
                Count = agent.FreeDuration - count,
                Price = price.ToString("#0.00")
            };
            return Ok(new RecoverClassModel<dynamic>()
            {
                Model = result,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 测试用户充值
        /// </summary>
        /// <param name="account">商户帐号</param>
        /// <param name="duration">时长</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RechargeTestMerchant(string account, int duration)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            MerchantOperation merchantOperation = new MerchantOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            //查询运营状态
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agent.HighestAgentID);
            if (!setup.Formal) return Ok(new RecoverModel(RecoverEnum.失败, "当前系统处于测试模式，不能为商户充值！"));
            var merchant = await merchantOperation.GetModelAsync(t => t.MeName == account);
            if (merchant == null)
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关客户信息!"));
            if (merchant.AgentID != agentID)
                return Ok(new RecoverModel(RecoverEnum.失败, "该商户不是你的下级，不能为其充值！"));
            //查看是否充值过月卡
            SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            var flag = await salesRecordsOperation.GetModelAsync(t => t.MerchantID == merchant._id
            && t.AgentID == agentID && t.SalesType != SalesTypeEnum.临时);
            if (flag != null)
                return Ok(new RecoverModel(RecoverEnum.失败, "该用户已充值过月卡！"));
            //查看累计充值的测试时间
            MerchantTestRecordOperation merchantTestRecordOperation = new MerchantTestRecordOperation();
            var merchantTestRecordList = await merchantTestRecordOperation.GetModelListAsync(t => t.AgentID == agentID
            && t.MerchantID == merchant._id);
            if (merchantTestRecordList.Sum(t => t.Duration) >= 20)
                return Ok(new RecoverModel(RecoverEnum.失败, "该用户累计充值测试时长已超过20小时！"));

            var list = await merchantTestRecordOperation.GetModelListAsync(t => t.AgentID == agentID
           && t.CreatedTime >= DateTime.Today && t.CreatedTime <= DateTime.Now);
            //已使用时长
            var count = list.Sum(t => t.Duration);
            //使用金额
            decimal price = 0;
            if ((count + duration) > agent.FreeDuration)
            {
                var recharge = (count + duration) - agent.FreeDuration;
                //代理价格
                price = agent.ExcessExpenses * recharge;
                if (price > agent.UserBalance)
                    return Ok(new RecoverModel(RecoverEnum.失败, "用户余额不足！"));

                await agentUserOperation.DownScore(agentID, price, AccountTypeEnum.其他,
                    string.Format("[{0}]充值测试时长{1}小时", account, recharge), false, true);
            }
            //添加测试日志
            var testRecord = new MerchantTestRecord()
            {
                Duration = duration,
                AgentID = agentID,
                MerchantID = merchant._id
            };
            await merchantTestRecordOperation.InsertModelAsync(testRecord);
            //添加销售记录
            //添加时长
            merchant.MaturityTime = merchant.MaturityTime > DateTime.Now ? merchant.MaturityTime.AddHours(duration)
                : DateTime.Now.AddHours(duration);
            var sales = new SalesRecords()
            {
                AgentID = agentID,
                MerchantID = merchant._id,
                Amount = price,
                MerchantName = merchant.MeName,
                SalesType = SalesTypeEnum.临时,
                MaturityTime = merchant.MaturityTime
            };
            await salesRecordsOperation.InsertModelAsync(sales);
            merchant.RechargeTime = DateTime.Now;
            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverModel(RecoverEnum.成功, "充值成功"));
        }

        /// <summary>
        /// 获取库存和使用金额
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             Count：剩余时长
        ///             Price：使用金额
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAccountMonthly()
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            var result = new
            {
                Count = agent.Stock,
                Price = agent.Stock > 0 ? 0 : agent.SubAgentPrice
            };
            return Ok(new RecoverClassModel<dynamic>()
            {
                Model = result,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 充值月卡
        /// </summary>
        /// <param name="account">客户帐号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RechargeMonthly(string account)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t.MeName == account);
            if (merchant == null)
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关客户信息!"));
            if (merchant.AgentID != agentID)
                return Ok(new RecoverModel(RecoverEnum.失败, "该商户不是你的下级，不能为其充值！"));
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            //查询运营状态
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agent.HighestAgentID);
            if (!setup.Formal) return Ok(new RecoverModel(RecoverEnum.失败, "当前系统处于测试模式，不能为商户充值！"));
            SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            RechargeOperation rechargeOperation = new RechargeOperation();
            var saleCount = await salesRecordsOperation.GetCountAsync(t => t.MerchantID == merchant._id
            && t.AgentID == agentID && t.SalesType != SalesTypeEnum.临时);
            //添加时长
            merchant.MaturityTime = saleCount > 0 ? merchant.MaturityTime.AddMonths(1)
                : DateTime.Now.AddMonths(1);
            //使用库存
            if (agent.Stock > 0)
            {
                var sales = new SalesRecords()
                {
                    AgentID = agentID,
                    MerchantID = merchant._id,
                    SalesType = SalesTypeEnum.库存,
                    MerchantName = merchant.MeName,
                    MaturityTime = merchant.MaturityTime
                };
                await salesRecordsOperation.InsertModelAsync(sales);

                //添加充值记录
                var recharge = new Recharge()
                {
                    OrderType = "库存直充",
                    ApplyAgentID = agent._id,
                    AcceptanceAgentID = agent.HighestAgentID,
                    ActualAmount = 0,
                    Amount = agent.SubAgentPrice,
                    ApplyAgentName = agent.LoginName,
                    IsHandle = true
                };
                await rechargeOperation.InsertModelAsync(recharge);
                agent.Stock -= 1;
                await agentUserOperation.UpdateModelAsync(agent);

                //添加账变日志
                AccountingRecordOperation accountingRecordOperation = new AccountingRecordOperation();
                var result = new AccountingRecord()
                {
                    Balance = agent.UserBalance,
                    Remark = string.Format("[{0}]充值月卡", merchant.MeName),
                    VariableAmount = 0,
                    Type = AccountTypeEnum.库存充值,
                    AgentID = agentID
                };
                await accountingRecordOperation.InsertModelAsync(result);
            }
            else
            {
                if (agent.SubAgentPrice > agent.UserBalance)
                    return Ok(new RecoverModel(RecoverEnum.失败, "账户余额不足！"));
                await agentUserOperation.DownScore(agentID, agent.SubAgentPrice, AccountTypeEnum.直充扣费,
                    string.Format("[{0}]充值月卡", merchant.MeName), true, true);
                var sales = new SalesRecords()
                {
                    AgentID = agentID,
                    MerchantID = merchant._id,
                    SalesType = SalesTypeEnum.余额,
                    Amount = agent.SubAgentPrice,
                    MerchantName = merchant.MeName,
                    MaturityTime = merchant.MaturityTime
                };
                await salesRecordsOperation.InsertModelAsync(sales);

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
            }
            merchant.RechargeTime = DateTime.Now;
            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverModel(RecoverEnum.成功, "充值成功"));
        }

        /// <summary>
        /// 商户余额充值
        /// </summary>
        /// <param name="merchantName">商户名称</param>
        /// <param name="amount">金额</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> MerchantRecharge(string merchantName, decimal amount)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t.MeName == merchantName);
            if (merchant == null)
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关客户信息!"));
            if (merchant.AgentID != agentID)
                return Ok(new RecoverModel(RecoverEnum.失败, "该商户不是你的下级，不能为其充值！"));
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            if (agent.UserBalance < amount * 0.99m)
                return Ok(new RecoverModel(RecoverEnum.失败, "代理余额不足！"));
            merchant.MarsCurrency += amount;
            await agentUserOperation.DownScore(agentID, amount * 0.99m, AccountTypeEnum.其他, string.Format("商户[{0}]充值火星币[{1}]，扣除余额[{2}]", merchantName, amount * 0.99m, amount), false, true);
            await merchantOperation.UpdateModelAsync(merchant);

            return Ok(new RecoverModel(RecoverEnum.成功, "充值成功！"));
        }

        /// <summary>
        /// 查询销售列表
        /// </summary>
        /// <param name="userName">名称</param>
        /// <param name="salesType">销售方式  1：余额 2：库存  3：临时</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <remarks>
        ///##  参数说明
        ///     userName：用户名
        ///     startTime：开始时间
        ///     endTime：结束时间
        ///     start：页码
        ///     pageSize：页数
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             RechargeTime：充值时间
        ///             Direct：直接代理
        ///             RechargeType：充值类型
        ///             UseAmount：使用金额
        ///             Account：购卡账户
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchSalesRecords(string userName, SalesTypeEnum? salesType, DateTime? startTime, DateTime? endTime, int start = 1, int pageSize = 10)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            //下级代理
            var agentList = await agentUserOperation.GetAgentList(agentID);
            var agentIDList = agentList.FindAll(t => t.AgentID != agentID).Select(t => t.AgentID).ToList();
            FilterDefinition<SalesRecords> filter = salesRecordsOperation.Builder.In(t => t.AgentID, agentIDList);
            if (!string.IsNullOrEmpty(userName))
                filter &= salesRecordsOperation.Builder.Regex(t => t.MerchantName, userName);
            if (salesType != null)
                filter &= salesRecordsOperation.Builder.Eq(t => t.SalesType, salesType.Value);
            if (startTime != null)
                filter &= salesRecordsOperation.Builder.Where(t => t.CreatedTime >= startTime);
            if (endTime != null)
                filter &= salesRecordsOperation.Builder.Where(t => t.CreatedTime <= endTime);
            var list = salesRecordsOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var total = await salesRecordsOperation.GetCountAsync(filter);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                var infos = agentList.Find(t => t.AgentID == data.AgentID);
                result.Add(new
                {
                    RechargeTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direct = infos.Relation,
                    RechargeType = Enum.GetName(typeof(SalesTypeEnum), (int)data.SalesType) + "直充",
                    UseAmount = data.Amount.ToString("#0.00"),
                    Account = data.MerchantName
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
        /// 查询个人销售报表
        /// </summary>
        /// <param name="userName">购卡用户</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchPersonalSalesRecords(string userName, DateTime? startTime, DateTime? endTime, int start = 1, int pageSize = 10)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            FilterDefinition<SalesRecords> filter = salesRecordsOperation.Builder.Eq(t => t.AgentID, agentID);
            if (!string.IsNullOrEmpty(userName))
                filter &= salesRecordsOperation.Builder.Regex(t => t.MerchantName, userName);
            if (startTime != null)
                filter &= salesRecordsOperation.Builder.Where(t => t.CreatedTime >= startTime);
            if (endTime != null)
                filter &= salesRecordsOperation.Builder.Where(t => t.CreatedTime <= endTime);
            var list = salesRecordsOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var total = await salesRecordsOperation.GetCountAsync(filter);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                result.Add(new
                {
                    RechargeTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Direct = agent.LoginName,
                    RechargeType = Enum.GetName(typeof(SalesTypeEnum), (int)data.SalesType) + "直充",
                    UseAmount = data.Amount.ToString("#0.00"),
                    Account = data.MerchantName
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
        /// 查询账变记录
        /// </summary>
        /// <param name="status">类型</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <param name="agentName">代理名称   仅能管理员使用</param>
        /// <remarks>
        ///##  参数说明
        ///     status：类型
        ///     余额充值 = 1,
        ///     直充扣费 = 2,
        ///     余额提现 = 3,
        ///     团队返利 = 4,
        ///     提现退回 = 5,
        ///     活动 = 6,
        ///     其他 = 7
        ///     startTime：开始时间
        ///     endTime：结束时间
        ///     start：页码
        ///     pageSize：页数
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             SerialNum：流水号
        ///             AddTime：添加时间
        ///             Change：变动金额
        ///             Balance：余额
        ///             Type：类型
        ///             Remark：摘要
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchAccountChange(AccountTypeEnum? status, DateTime? startTime, DateTime? endTime, int start = 1, int pageSize = 10, string agentName = null)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            AccountingRecordOperation accountingRecordOperation = new AccountingRecordOperation();
            FilterDefinition<AccountingRecord> filter = null;
            if (!string.IsNullOrEmpty(agentName))
            {
                if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "此用户无权利"));
                AgentUserOperation agentUserOperation = new AgentUserOperation();
                var agent = await agentUserOperation.GetModelAsync(t => t.LoginName == agentName);
                if (agent == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到对应代理！"));
                filter = accountingRecordOperation.Builder.Eq(t => t.AgentID, agent._id);
            }
            else
                filter = accountingRecordOperation.Builder.Eq(t => t.AgentID, agentID);
            if (status != null)
                filter &= accountingRecordOperation.Builder.Where(t => t.Type == status);
            if (startTime != null)
                filter &= accountingRecordOperation.Builder.Where(t => t.CreatedTime >= startTime);
            if (endTime != null)
                filter &= accountingRecordOperation.Builder.Where(t => t.CreatedTime <= endTime);

            var list = accountingRecordOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var total = await accountingRecordOperation.GetCountAsync(filter);
            var result = new List<dynamic>();
            foreach (var data in list)
            {
                result.Add(new
                {
                    data.SerialNum,
                    AddTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Change = data.VariableAmount.ToString("#0.00"),
                    Balance = data.Balance.ToString("#0.00"),
                    Type = Enum.GetName(typeof(AccountTypeEnum), (int)data.Type),
                    data.Remark
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
        /// 个人报表
        /// </summary>
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
        ///             Date：日期
        ///             Aecharge：充值
        ///             Sales：直充客户
        ///             Rebate返利：余额
        ///             Cash：提现
        ///             Activity：活动
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchPersonalState(DateTime startTime, DateTime endTime, int start = 1, int pageSize = 10)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            CashWithdrawalOperation cashWithdrawalOperation = new CashWithdrawalOperation();
            RechargeOperation rechargeOperation = new RechargeOperation();
            AccountingRecordOperation accountingRecordOperation = new AccountingRecordOperation();
            SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            var diff = (endTime - startTime).Days;
            var result = new List<dynamic>();
            for (int i = 0; i <= diff; i++)
            {
                var rechargeList = await rechargeOperation.GetModelListAsync(t => t.ApplyAgentID == agentID
                && t.OrderStatus == ROrderStatusEnum.成功 && t.IsHandle == true && t.OrderType == "余额直充"
                && t.LastUpdateTime >= startTime.AddDays(i) && t.LastUpdateTime <= startTime.AddDays(i + 1));
                //余额充值
                var allAecharge = rechargeList.Sum(t => t.Amount);
                //直充客户
                var salesCount = await salesRecordsOperation.GetCountAsync(t => t.AgentID == agentID && (t.SalesType == SalesTypeEnum.库存 || t.SalesType == SalesTypeEnum.余额)
                && t.CreatedTime >= startTime.AddDays(i) && t.CreatedTime <= startTime.AddDays(i + 1));
                //返利 活动
                var accountList = await accountingRecordOperation.GetModelListAsync(t => t.AgentID == agentID
                && (t.Type == AccountTypeEnum.团队返利 || t.Type == AccountTypeEnum.活动)
                && t.CreatedTime >= startTime.AddDays(i) && t.CreatedTime <= startTime.AddDays(i + 1));
                //返利
                var rebate = accountList.FindAll(t => t.Type == AccountTypeEnum.团队返利).Sum(t => t.VariableAmount);
                //活动
                var activity = accountList.FindAll(t => t.Type == AccountTypeEnum.活动).Sum(t => t.VariableAmount);

                //提现
                var cashList = await cashWithdrawalOperation.GetModelListAsync(t => t.ApplyAgentID == agentID
                && t.Status == WithdrawalStatusEnum.已放款 &&
                t.LastUpdateTime >= startTime.AddDays(i) && t.LastUpdateTime <= startTime.AddDays(i + 1));
                var cashCount = cashList.Sum(t => t.Amount);

                var data = new
                {
                    Date = startTime.AddDays(i).ToString("yyyy-MM-dd"),
                    Aecharge = allAecharge,
                    Sales = salesCount,
                    Rebate = rebate,
                    Cash = cashCount,
                    Activity = activity
                };
                if (data.Aecharge + data.Sales + data.Rebate + data.Cash + data.Activity > 0)
                    result.Add(data);
            }
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result.Skip((start - 1) * pageSize).Take(pageSize).ToList(),
                Total = result.Count,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取直充合计和返利合计
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetHeadData(DateTime startTime, DateTime endTime)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            SalesRecordsOperation salesRecordsOperation = new SalesRecordsOperation();
            AccountingRecordOperation accountingRecordOperation = new AccountingRecordOperation();

            var rchargeCount = await salesRecordsOperation.GetCountAsync(t => t.AgentID == agentID && (t.SalesType == SalesTypeEnum.余额 || t.SalesType == SalesTypeEnum.库存)
            && t.CreatedTime >= startTime && t.CreatedTime <= endTime);

            var allAccount = await accountingRecordOperation.GetModelListAsync(t => t.AgentID == agentID &&
             t.Type == AccountTypeEnum.团队返利 && t.CreatedTime >= startTime & t.CreatedTime <= endTime);

            dynamic result = new
            {
                Recharge = rchargeCount,
                Account = allAccount.Sum(t => t.VariableAmount)
            };
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功",
                Model = result,
                Status = RecoverEnum.成功
            });
        }
    }
}