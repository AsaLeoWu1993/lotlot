using AgentManagement.Manipulate;
using Entity;
using Entity.AgentModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Agent;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgentManagement.Controllers
{
    /// <summary>
    /// 帐户安全
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    public class AccountSafeController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment = null;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public AccountSafeController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        [HttpGet]
        [NotAuthenticationAttribute]
        public IActionResult GetHtml(string path)
        {
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
            return Ok("ok");
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateUserPwd(string oldPwd, string newPwd)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            var count = HttpContext.Session.GetInt32("ErrorCount") == null ? 0 : HttpContext.Session.GetInt32("ErrorCount").Value;
            if (agent.Password != oldPwd)
            {
                HttpContext.Session.SetInt32("ErrorCount", count + 1);
                if (count + 1 == 3)
                {
                    agent.LockTime = DateTime.Now.AddMinutes(10);
                    await agentUserOperation.UpdateModelAsync(agent);
                    var loginName = HttpContext.User.FindFirstValue("LoginName");
                    var key = loginName + ":" + agentID;
                    RedisOperation.DeleteKey(key);
                    HttpContext.Session.Clear();
                    await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                    return Ok(new RecoverModel(RecoverEnum.失败, "输入错误三次，锁定10分钟！"));
                }
                return Ok(new RecoverModel(RecoverEnum.失败,
                        "当前登录密码错误！"));
            }
            if (oldPwd == newPwd)
                return Ok(new RecoverModel(RecoverEnum.失败,
                        "新密码不能与原密码相同！"));
            agent.Password = newPwd;
            await agentUserOperation.UpdateModelAsync(agent);
            HttpContext.Session.SetInt32("ErrorCount", 0);
            return Ok(new RecoverModel(RecoverEnum.成功,
                       "修改成功！"));
        }

        /// <summary>
        /// 修改安全密码
        /// </summary>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSafePwd(string oldPwd, string newPwd)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
            var count = HttpContext.Session.GetInt32("ErrorCount") == null ? 0 : HttpContext.Session.GetInt32("ErrorCount").Value;
            if (agent.SafePassWord != Utils.MD5(oldPwd))
            {
                HttpContext.Session.SetInt32("ErrorCount", count + 1);
                if (count + 1 == 3)
                {
                    agent.LockTime = DateTime.Now.AddMinutes(10);
                    await agentUserOperation.UpdateModelAsync(agent);
                    var loginName = HttpContext.User.FindFirstValue("LoginName");
                    var key = loginName + ":" + agentID;
                    RedisOperation.DeleteKey(key);
                    HttpContext.Session.Clear();
                    await HttpContext.SignOutAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme);
                    return Ok(new RecoverModel(RecoverEnum.失败, "输入错误三次，锁定10分钟！"));
                }
                return Ok(new RecoverModel(RecoverEnum.失败,
                        "当前安全密码错误！"));
            }
            if (oldPwd == newPwd)
                return Ok(new RecoverModel(RecoverEnum.失败,
                        "新密码不能与原密码相同！"));
            agent.SafePassWord = Utils.MD5(newPwd);
            await agentUserOperation.UpdateModelAsync(agent);
            HttpContext.Session.SetInt32("ErrorCount", 0);
            return Ok(new RecoverModel(RecoverEnum.成功,
                       "修改成功！"));
        }

        /// <summary>
        /// 获取联系方式
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetContactInfo()
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
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
        /// 修改联系方式
        /// </summary>
        /// <param name="phone"></param>
        /// <param name="qq"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SetContactInfo(string phone, string qq, string email)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            ContactOperation contactOperation = new ContactOperation();
            var contact = await contactOperation.GetModelAsync(t => t.AgentID == agentID);
            if (contact == null)
            {
                contact = new Contact()
                {
                    Phone = phone,
                    Email = email,
                    QQ = qq,
                    AgentID = agentID
                };
                await contactOperation.InsertModelAsync(contact);
            }
            else
            {
                contact.QQ = qq;
                contact.Email = email;
                contact.Phone = phone;
                await contactOperation.UpdateModelAsync(contact);
            }
            return Ok(new RecoverModel(RecoverEnum.成功,
                       "修改成功！"));
        }

        /// <summary>
        /// 获取银行列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetBankList()
        {
            var dic = GameBetsMessage.EnumToDictionary(typeof(BankEnum));
            return Ok(new RecoverClassModel<Dictionary<string, int>>
            {
                Message = "获取成功",
                Model = dic,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取银行信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBankInfo()
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            BankInfoOperation bankInfoOperation = new BankInfoOperation();
            var bankInfo = await bankInfoOperation.GetModelAsync(t => t.AgentID == agentID);
            dynamic result = null;
            if (bankInfo == null)
            {
                result = new
                {
                    Name = "",
                    BankName = "",
                    BankNum = ""
                };
            }
            else
            {
                result = new
                {
                    bankInfo.Name,
                    BankName = Enum.GetName(typeof(BankEnum), (int)bankInfo.BankType),
                    bankInfo.BankNum,
                    bankInfo.BankType
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
        /// 修改银行卡信息
        /// </summary>
        /// <param name="currentInfo">当前收款账号</param>
        /// <param name="accountName">姓名</param>
        /// <param name="bankType">新的收款银行</param>
        /// <param name="bankInfo">新的收款账号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateBankInfo(string currentInfo, string accountName, BankEnum bankType, string bankInfo)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            BankInfoOperation bankInfoOperation = new BankInfoOperation();
            var bank = await bankInfoOperation.GetModelAsync(t => t.AgentID == agentID);
            if (bank == null)
            {
                bank = new BankInfo()
                {
                    AgentID = agentID,
                    BankNum = bankInfo,
                    BankType = bankType,
                    Name = accountName
                };
                await bankInfoOperation.InsertModelAsync(bank);
                return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
            }
            else
            {
                var count = HttpContext.Session.GetInt32("ErrorCount") == null ? 0 : HttpContext.Session.GetInt32("ErrorCount").Value;
                if (bank.BankNum != currentInfo)
                {
                    HttpContext.Session.SetInt32("ErrorCount", count + 1);
                    if (count + 1 == 3)
                    {
                        AgentUserOperation agentUserOperation = new AgentUserOperation();
                        var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
                        agent.LockTime = DateTime.Now.AddMinutes(10);
                        await agentUserOperation.UpdateModelAsync(agent);
                        var loginName = HttpContext.User.FindFirstValue("LoginName");
                        var key = loginName + ":" + agentID;
                        RedisOperation.DeleteKey(key);
                        HttpContext.Session.Clear();
                        await HttpContext.SignOutAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme);
                        return Ok(new RecoverModel(RecoverEnum.失败, "输入错误三次，锁定10分钟！"));
                    }
                    return Ok(new RecoverModel(RecoverEnum.失败, "当前收款账号错误！"));
                }
                bank.Name = accountName;
                bank.BankType = bankType;
                bank.BankNum = bankInfo;
                await bankInfoOperation.UpdateModelAsync(bank);
                HttpContext.Session.SetInt32("ErrorCount", 0);
                return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
            }
        }

        /// <summary>
        /// 获取绑定支付宝信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAlipayInfo()
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            AlipayInfoOperation alipayInfoOperation = new AlipayInfoOperation();
            var data = await alipayInfoOperation.GetModelAsync(t => t.AgentID == agentID);
            dynamic result = null;
            if (data == null)
            {
                result = new
                {
                    Name = "",
                    Path = ""
                };
            }
            else
            {
                result = new
                {
                    Name = data.Account,
                    Path = data.QRPath
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
        /// 修改支付宝信息
        /// </summary>
        /// <param name="currentName">当前支付宝账号</param>
        /// <param name="newName">新支付宝账号</param>
        /// <param name="fileinput">二维码文件</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAlipayInfo(string currentName, string newName, IFormFile fileinput)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            AlipayInfoOperation alipayInfoOperation = new AlipayInfoOperation();
            var data = await alipayInfoOperation.GetModelAsync(t => t.AgentID == agentID);
            if (fileinput == null) return Ok(new RecoverModel(RecoverEnum.失败, "未选择图片！"));
            var url = await BlobHelper.UploadImageToBlob(fileinput, "AlipayImages");
            if (string.IsNullOrEmpty(url)) return Ok(new RecoverModel(RecoverEnum.失败, "图片格式错误！"));
            if (url == "1") return Ok(new RecoverModel(RecoverEnum.失败, "图片大小最大为20M！"));
            if (data == null)
            {
                data = new AlipayInfo()
                {
                    AgentID = agentID,
                    Account = newName,
                    QRPath = url
                };
                await alipayInfoOperation.InsertModelAsync(data);
                return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
            }
            else
            {
                var count = HttpContext.Session.GetInt32("ErrorCount") == null ? 0 : HttpContext.Session.GetInt32("ErrorCount").Value;
                if (data.Account != currentName)
                {
                    HttpContext.Session.SetInt32("ErrorCount", count + 1);
                    if (count + 1 == 3)
                    {
                        AgentUserOperation agentUserOperation = new AgentUserOperation();
                        var agent = await agentUserOperation.GetModelAsync(t => t._id == agentID);
                        agent.LockTime = DateTime.Now.AddMinutes(10);
                        await agentUserOperation.UpdateModelAsync(agent);
                        var loginName = HttpContext.User.FindFirstValue("LoginName");
                        var key = loginName + ":" + agentID;
                        RedisOperation.DeleteKey(key);
                        HttpContext.Session.Clear();
                        await HttpContext.SignOutAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme);
                        return Ok(new RecoverModel(RecoverEnum.失败, "输入错误三次，锁定10分钟！"));
                    }
                    return Ok(new RecoverModel(RecoverEnum.失败, "当前支付宝账号错误！"));
                }
                data.Account = newName;
                data.QRPath = url;
                await alipayInfoOperation.UpdateModelAsync(data);
                HttpContext.Session.SetInt32("ErrorCount", 0);
                return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
            }
        }

        /// <summary>
        /// 解绑支付宝帐户
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UnboundAccount()
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            AlipayInfoOperation alipayInfoOperation = new AlipayInfoOperation();
            var data = await alipayInfoOperation.GetModelAsync(t => t.AgentID == agentID);
            if (data == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未绑定支付宝帐户！"));
            else
                alipayInfoOperation.DeleteModelOne(t => t.AgentID == agentID);
            return Ok(new RecoverModel(RecoverEnum.成功, "解绑成功！"));
        }

        /// <summary>
        /// 设置虚假货币信息
        /// </summary>
        /// <param name="path">地址</param>
        /// <param name="type">货币类型  1:泰达币</param>
        /// <param name="oldPath">旧地址</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateVirtual(string path, CurrencyEnum type, string oldPath)
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            VirtualCurrencyOperation virtualCurrencyOperation = new VirtualCurrencyOperation();
            var data = await virtualCurrencyOperation.GetModelAsync(t => t.AgentID == agentID);
            if (data == null)
            {
                data = new VirtualCurrency()
                {
                    AgentID = agentID
                };
                data.CurrencyInfo.Add(new CurrencyInfo
                {
                    CurrencyPath = path,
                    CurrencyType = type
                });
                await virtualCurrencyOperation.InsertModelAsync(data);
            }
            else
            {
                //查询是否已经存在
                if (data.CurrencyInfo.Exists(t => t.CurrencyType == type))
                {
                    var info = data.CurrencyInfo.Find(t => t.CurrencyType == type);
                    if (info.CurrencyPath != oldPath)
                        return Ok(new RecoverModel(RecoverEnum.失败, "原地址错误，修改失败！"));
                    data.CurrencyInfo.RemoveAll(t => t.CurrencyType == type);
                }
                data.CurrencyInfo.Add(new CurrencyInfo
                {
                    CurrencyPath = path,
                    CurrencyType = type
                });
                await virtualCurrencyOperation.UpdateModelAsync(data);
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }

        /// <summary>
        /// 获取虚假货币信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVirtual()
        {
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            VirtualCurrencyOperation virtualCurrencyOperation = new VirtualCurrencyOperation();
            var data = await virtualCurrencyOperation.GetModelAsync(t => t.AgentID == agentID);
            if (data == null)
                return Ok(new RecoverListModel<CurrencyInfo>()
                { 
                    Data = null,
                    Total = 0,
                    Message = "获取成功！",
                    Status = RecoverEnum.成功
                });
            else
                return Ok(new RecoverListModel<CurrencyInfo>()
                {
                    Data = data.CurrencyInfo,
                    Total = data.CurrencyInfo.Count,
                    Message = "获取成功！",
                    Status = RecoverEnum.成功
                });
        }
     }
}