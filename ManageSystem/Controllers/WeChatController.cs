using Entity;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Operation.Abutment;
using Operation.Agent;
using Operation.Common;
using System;
using System.Threading.Tasks;

namespace ManageSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    public class WeChatController : ControllerBase
    {
        /// <summary>
        /// code传输
        /// </summary>
        /// <param name="code"></param>
        /// <param name="seurityNo">安全码</param>
        /// <param name="type">类型 1:app 2:web</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> WeChatAppAction(string code, string seurityNo, int type = 1)
        {
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null) return Ok(new RecoverModel(RecoverEnum.失败, "管理员未配置微信登录功能！"));
            var url = type == 1 ? string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", model.App_AppID, model.App_AppSecret, code)
                : string.Format("https://api.weixin.qq.com/sns/oauth2/access_token?appid={0}&secret={1}&code={2}&grant_type=authorization_code", model.Web_AppID, model.Web_AppSecret, code);
            var result = await Utils.GetAsync(url);
            if (string.IsNullOrEmpty(result)) return Ok(new RecoverModel(RecoverEnum.失败, "未获取到微信信息！"));
            var suObj = JsonConvert.DeserializeObject<JObject>(result);
            if (suObj.ContainsKey("errcode"))
            {
                return Ok(new RecoverModel(RecoverEnum.失败, result));
            }
            var access_token = suObj["access_token"].ToString();
            var openid = suObj["openid"].ToString();
            //拉取用户个人信息
            var userUrl = string.Format("https://api.weixin.qq.com/sns/userinfo?access_token={0}&openid={1}&lang=zh_CN", access_token, openid);
            var userResult = await Utils.GetAsync(userUrl);
            var userSuObj = JsonConvert.DeserializeObject<JObject>(userResult);
            if (userSuObj.ContainsKey("errcode"))
            {
                return Ok(new RecoverModel(RecoverEnum.失败, userResult));
            }
            var unionid = userSuObj["unionid"].ToString();

            //查询用户信息
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t.SeurityNo == seurityNo);
            if (merchant == null) return Ok(new RecoverModel(RecoverEnum.失败, "安全码错误！"));
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchant.AgentID);
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agent.HighestAgentID);
            if (merchant.MaturityTime <= DateTime.Now && setup.Formal)
                return Ok(new RecoverModel(RecoverEnum.失败, "该商户已过期！"));
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t.Unionid == unionid && t.MerchantID == merchant._id && t.Status != UserStatusEnum.删除);
            if (user == null)
            {
                //查询商户正式用户
                var count = await userOperation.GetCountAsync(t => t.MerchantID == merchant._id
                && (t.Status == UserStatusEnum.冻结 || t.Status == UserStatusEnum.正常));
                if (count >= 200)
                    return Ok(new RecoverModel(RecoverEnum.失败, "账号数达到最高限制，不能注册，请联系客服！"));
                var nickName = string.Empty;
                if (userSuObj["nickname"].ToString().Length > 7)
                    nickName = userSuObj["nickname"].ToString().Substring(0, 7);
                else
                    nickName = userSuObj["nickname"].ToString();
                user = new User()
                {
                    Unionid = unionid,
                    Password = Utils.MD5("123456"),
                    Level = UserLevelEnum.黄铜,
                    Avatar = userSuObj["headimgurl"].ToString(),
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
            } 
            return Ok(new RecoverKeywordModel()
            {
                Keyword = unionid,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取app微信端appid
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAppInfos()
        {
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null) return Ok(new RecoverModel(RecoverEnum.失败, "管理员未配置微信登录功能！"));
            var data = new
            {
                model.App_AppID,
                model.App_AppSecret,
                model.Web_AppID,
                model.Web_AppSecret,
                model.MerchantAppUrl
            };
            return Ok(new RecoverClassModel<dynamic>
            {
                Message = "获取成功",
                Model = data,
                Status = RecoverEnum.成功
            });
        }
    }
}