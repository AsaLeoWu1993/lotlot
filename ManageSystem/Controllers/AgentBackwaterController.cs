using Entity;
using Entity.WebModel;
using ManageSystem.Manipulate;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Operation.Abutment;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ManageSystem.Controllers
{
    /// <summary>
    /// 回水
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    [MerchantAuthentication]
    public class AgentBackwaterController : ControllerBase
    {
        /// <summary>
        /// 获取代理用户表
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
        ///             ID：主要id
        ///             UserID：用户id
        ///             Avatar：头像 （需要前端拼接）api地址+返回地址
        ///             IsAgent：是否为代理
        ///             NickName：昵称
        ///             LoginName：登录名称
        ///             UserMoney：余额
        ///             RoomNum：房间号
        ///             Racing：赛车回水率
        ///             Airship：飞艇回水率
        ///             TimeHonored：时时彩回水率
        ///             ExtremeSpeed：极速回水率
        ///             Aus10：澳10回水率
        ///             Aus5：澳5回水率
        ///             Xyft168：幸运飞艇168
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAgentUserList(string keyword, int start = 1, int pageSize = 10)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserOperation userOperation = new UserOperation();
            FilterDefinition<User> filter = userOperation.Builder.Where(t => t.MerchantID == merchantID && t.IsAgent == true);
            if (!string.IsNullOrEmpty(keyword))
                filter &= userOperation.Builder.Regex(t => t.NickName, keyword) | userOperation.Builder.Regex(t => t.LoginName, keyword) | userOperation.Builder.Regex(t => t.OnlyCode, keyword) | userOperation.Builder.Regex(t => t.MemoName, keyword);
            var userList = userOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var total = await userOperation.GetCountAsync(filter);
            var result = new List<dynamic>();
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            foreach (var user in userList)
            {
                var agent = agentBackwaterOperation.GetAgentByUserIDAndNo(user._id, merchantID);
                if (agent == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到代理数据！"));
                var data = new
                {
                    UserID = user._id,
                    user.LoginName,
                    NickName = string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                    user.Avatar,
                    user.UserMoney,
                    RoomNum = agent.ExtensionCode,
                    agent.Pk10,
                    agent.Xyft,
                    agent.Cqssc,
                    agent.Jssc,
                    agent.Azxy10,
                    agent.Azxy5,
                    agent.Ireland10,
                    agent.Ireland5,
                    agent.XYft168,
                    agent.Jsssc,
                    agent.Baccarat,
                    ID = agent._id,
                    user.IsAgent,
                    user.OnlyCode
                };
                result.Add(data);
            }
            return Ok(new RecoverListModel<dynamic>() { Data = result, Message = "查询成功！", Status = RecoverEnum.成功, Total = total });
        }

        /// <summary>
        /// 获取回水率
        /// </summary>
        /// <param name="id">id</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model:
        ///         {
        ///             ID：主要id
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetAgentInfo(string id)
        {
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var model = agentBackwaterOperation.GetAgentByIDAndNo(id, merchantID);
            if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到代理数据！"));
            var result = new WebAgentBackwater()
            {
                ID = model._id,
                Pk10 = model.Pk10,
                Azxy10 = model.Azxy10,
                Cqssc = model.Cqssc,
                Ireland10 = model.Ireland10,
                Azxy5 = model.Azxy5,
                Ireland5 = model.Ireland5,
                Jssc = model.Jssc,
                Xyft = model.Xyft,
                Xyft168 = model.XYft168,
                Jsssc = model.Jsssc,
                Baccarat = model.Baccarat
            };
            return Ok(new RecoverClassModel<WebAgentBackwater>() { Message = "查询成功！", Model = result, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 修改回水率
        /// </summary>
        /// <param name="webAgentBackwater"></param>
        /// <remarks>
        ///##  参数说明
        ///     ID：主要id
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAgentInfo([FromBody]WebAgentBackwater webAgentBackwater)
        {
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            if (webAgentBackwater == null) return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var model = agentBackwaterOperation.GetAgentByIDAndNo(webAgentBackwater.ID, merchantID);
            if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到代理数据！"));
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == model.UserID && t.MerchantID == merchantID);
            var nickName = string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID,
                OpLocation = OpLocationEnum.代理回水设置,
                OpType = OpTypeEnum.修改,
                OpBcontent = string.Format("[{0}({1})]  北京赛车:{2},幸运飞艇:{3},重庆时时彩:{4},极速赛车:{5},澳10:{6},澳5:{7},爱尔兰10:{8},爱尔兰5:{9},幸运飞艇168:{10},极速时时彩:{11}",
                nickName, user.OnlyCode, model.Pk10, model.Xyft, model.Cqssc, model.Jssc, model.Azxy10, model.Azxy5, model.Ireland10, model.Ireland5, model.XYft168, model.Jsssc),
                OpAcontent = string.Format("[{0}({1})]  北京赛车:{2},幸运飞艇:{3},重庆时时彩:{4},极速赛车:{5},澳10:{6},澳5:{7},爱尔兰10:{8},爱尔兰5:{9},幸运飞艇168:{10},极速时时彩:{11}",
                nickName, user.OnlyCode, webAgentBackwater.Pk10, webAgentBackwater.Xyft, webAgentBackwater.Cqssc, webAgentBackwater.Jssc, webAgentBackwater.Azxy10, webAgentBackwater.Azxy5, webAgentBackwater.Ireland10, webAgentBackwater.Ireland5, webAgentBackwater.Xyft168, webAgentBackwater.Jsssc)
            };
            await sensitiveOperation.InsertModelAsync(sensitive);

            model.Pk10 = webAgentBackwater.Pk10;
            model.Xyft = webAgentBackwater.Xyft;
            model.Cqssc = webAgentBackwater.Cqssc;
            model.Jssc = webAgentBackwater.Jssc;
            model.Azxy10 = webAgentBackwater.Azxy10;
            model.Azxy5 = webAgentBackwater.Azxy5;
            model.Ireland10 = webAgentBackwater.Ireland10;
            model.Ireland5 = webAgentBackwater.Ireland5;
            model.XYft168 = webAgentBackwater.Xyft168;
            model.Jsssc = webAgentBackwater.Jsssc;
            model.Baccarat = webAgentBackwater.Baccarat;
            await agentBackwaterOperation.UpdateModelAsync(model);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 获取下线列表
        /// </summary>
        /// <param name="id">id</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Total：数据总数量
        ///         Data
        ///         {
        ///             ID：主要id
        ///             UserID：用户id
        ///             NickName：昵称
        ///             LoginName：登录名称
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOfflineUsers(string id)
        {
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var model = agentBackwaterOperation.GetAgentByIDAndNo(id, merchantID);
            if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到代理数据！"));
            var userList = await userOperation.GetModelListAsync(userOperation.Builder.In(t => t._id, model.Offline.Select(t => t.UserID).ToList()));
            var result = new List<dynamic>();
            foreach (var user in userList)
            {
                var data = new
                {
                    ID = id,
                    UserID = user._id,
                    NickName = string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                    user.LoginName
                };
                result.Add(data);
            }
            return Ok(new RecoverListModel<dynamic>() { Data = result, Message = "查询成功！", Status = RecoverEnum.成功, Total = result.Count });
        }

        /// <summary>
        /// 添加下线
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="userID">下级用户id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddOfflineUser(string id, string userID)
        {
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var model = agentBackwaterOperation.GetAgentByIDAndNo(id, merchantID);
            if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到代理数据！"));
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户数据！"));
            if (!model.Offline.Exists(t => t.UserID == user._id))
            {
                model.Offline.Add(new OfflineUser()
                {
                    UserID = user._id
                });
                await agentBackwaterOperation.UpdateModelAsync(model);
            }
            user.RoomNum = model.ExtensionCode;
            await userOperation.UpdateModelAsync(user);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }

        /// <summary>
        /// 取消下线
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userID">用户id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteOfflineUser(string id, string userID)
        {
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var model = agentBackwaterOperation.GetAgentByIDAndNo(id, merchantID);
            if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到代理数据！"));
            model.Offline.RemoveAll(t => t.UserID == userID);
            await agentBackwaterOperation.UpdateModelAsync(model);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }
    }
}