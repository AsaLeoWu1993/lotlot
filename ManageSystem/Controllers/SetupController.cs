using Entity;
using Entity.GraspModel;
using Entity.WebModel;
using ManageSystem.Manipulate;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Agent;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
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
    [MerchantAuthentication]
    public class SetupController : ControllerBase
    {
        /// <summary>
        /// 
        /// </summary>
        public SetupController()
        {

        }

        /// <summary>
        /// 添加管理员消息
        /// </summary>
        /// <param name="webAdminMessage"></param>
        /// <remarks>
        ///##  参数说明
        ///    model
        ///    {
        ///         Racing:赛车
        ///         Airship:飞艇
        ///         TimeHonored:时时彩
        ///         ExtremeSpeed:极速
        ///         Aus10:澳10
        ///         Aus5:澳5
        ///         Content:内容
        ///    }
        ///    其它不传
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddMeaage([FromBody] WebAdminMessage webAdminMessage)
        {
            if (webAdminMessage == null) return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
            if (string.IsNullOrEmpty(webAdminMessage.Content)) return Ok(new RecoverModel(RecoverEnum.参数错误, "发送内容不能为空！"));

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (webAdminMessage.Pk10)
                await RabbitMQHelper.SendAdminMessage(webAdminMessage.Content, merchantID, GameOfType.北京赛车);
            if (webAdminMessage.Xyft)
                await RabbitMQHelper.SendAdminMessage(webAdminMessage.Content, merchantID, GameOfType.幸运飞艇);
            if (webAdminMessage.Cqssc)
                await RabbitMQHelper.SendAdminMessage(webAdminMessage.Content, merchantID, GameOfType.重庆时时彩);
            if (webAdminMessage.Jssc)
                await RabbitMQHelper.SendAdminMessage(webAdminMessage.Content, merchantID, GameOfType.极速赛车);
            if (webAdminMessage.Azxy10)
                await RabbitMQHelper.SendAdminMessage(webAdminMessage.Content, merchantID, GameOfType.澳州10);
            if (webAdminMessage.Azxy5)
                await RabbitMQHelper.SendAdminMessage(webAdminMessage.Content, merchantID, GameOfType.澳州5);
            if (webAdminMessage.Ireland10)
                await RabbitMQHelper.SendAdminMessage(webAdminMessage.Content, merchantID, GameOfType.爱尔兰赛马);
            if (webAdminMessage.Ireland5)
                await RabbitMQHelper.SendAdminMessage(webAdminMessage.Content, merchantID, GameOfType.爱尔兰快5);
            if (webAdminMessage.Xyft168)
                await RabbitMQHelper.SendAdminMessage(webAdminMessage.Content, merchantID, GameOfType.幸运飞艇168);
            if (webAdminMessage.Jsssc)
                await RabbitMQHelper.SendAdminMessage(webAdminMessage.Content, merchantID, GameOfType.极速时时彩);
            return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
        }

        /// <summary>
        /// 获取虚假用户列表信息
        /// </summary>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Data
        ///         {
        ///             ID：虚假用户id 
        ///             UserID：用户id
        ///             Avatar：头像
        ///             LoginName：登录名称
        ///             NickName：昵称
        ///             LowerScore：上分
        ///             UpperScore：下分
        ///             GameBetInfo：游戏信息
        ///             {
        ///                 GameType：游戏类型 1：赛车 2：飞艇 3：时时彩 4：极速 5：澳10  6：澳5  6种都要封装
        ///                 Check：是否选中
        ///                 BetInfo：投注内容
        ///             }
        ///             OnlyCode：唯一码
        ///             Derail：开关
        ///         }
        ///         Total：数据数量
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetShamUserListInfo(int start = 1, int pageSize = 10)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userList = userOperation.GetModelListByPaging(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.假人 && t.IsSupport == true, t => t.CreatedTime, true, start, pageSize);
            var total = await userOperation.GetCountAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.假人);
            var result = new List<WebShamUser>();
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            foreach (var user in userList)
            {
                var shamUser = await shamRobotOperation.GetModelAsync(t => t.UserID == user._id && t.MerchantID == merchantID);
                if (shamUser == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到虚假用户信息！"));
                var data = new WebShamUser()
                {
                    ID = shamUser._id,
                    UserID = user._id,
                    Avatar = user.Avatar,
                    LoginName = user.LoginName,
                    NickName = string.IsNullOrEmpty(user.MemoName) && user.ShowType ? user.NickName : user.MemoName,
                    GameBetInfo = shamUser.GameCheckInfo,
                    OnlyCode = user.OnlyCode,
                    UserMoney = user.UserMoney,
                    OpenGame = await OpenGameNames(merchantID, user._id),
                    Check = shamUser.Check
                };
                if (!data.GameBetInfo.Exists(t => t.GameType == GameOfType.极速时时彩))
                    data.GameBetInfo.Add(new GameBetType()
                    { 
                        GameType = GameOfType.极速时时彩
                    });
                result.Add(data);
            }
            return Ok(new RecoverListModel<WebShamUser>() { Data = result, Message = "查询成功！", Status = RecoverEnum.成功, Total = total });
        }

        /// <summary>
        /// 获取选择游戏
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        private async Task<string> OpenGameNames(string merchantID, string userID)
        {
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            var shamUser = await shamRobotOperation.GetModelAsync(t => t.UserID == userID && t.MerchantID == merchantID);
            if (shamUser == null) return null;
            var check = shamUser.GameCheckInfo.FindAll(t => t.Check);
            var data = check.Select(t => new { Name = Enum.GetName(typeof(GameOfType), (int)t.GameType) })
                .Select(t => t.Name).ToList();
            return string.Join("/", data);
        }

        /// <summary>
        /// 获取虚假用户信息
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetShamUserInfo(string id)
        {
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var shamUser = await shamRobotOperation.GetModelAsync(t => t._id == id && t.MerchantID == merchantID);
            if (shamUser == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到用户信息!"));
            var user = await userOperation.GetModelAsync(t => t._id == shamUser.UserID && t.MerchantID == merchantID);
            if (user == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到用户信息!"));
            var data = new
            {
                user.Avatar,
                NickName = string.IsNullOrEmpty(user.MemoName) && user.ShowType ? user.NickName : user.MemoName,
                UserID = user._id,
                user.OnlyCode,
                shamUser.GameCheckInfo
            };
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功",
                Model = data,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 修改虚假用户绑定行为
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="nickName">昵称</param>
        /// <param name="gameCheckInfo">行为绑定信息</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PutShamUserInfo(string userID, string nickName, [FromBody]WebShamRobot gameCheckInfo)
        {
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到相关用户"));
            user.NickName = nickName;
            await userOperation.UpdateModelAsync(user);
            var shamUser = await shamRobotOperation.GetModelAsync(t => t.UserID == userID && t.MerchantID == merchantID);
            if (shamUser == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到用户信息!"));
            shamUser.GameCheckInfo = gameCheckInfo.GameCheckInfo;
            shamUser.GameCheckInfo.ForEach(t =>
            {
                if (string.IsNullOrEmpty(t.BehaviorID))
                    t.Check = false;
            });
            await shamRobotOperation.UpdateModelAsync(shamUser);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 获取行为管理列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBehaviorManage()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotBehaviorOperation robotBehaviorOperation = new RobotBehaviorOperation();
            RobotProgramOperation robotProgramOperation = new RobotProgramOperation();
            var list = await robotBehaviorOperation.GetModelListAsync(t => t.MerchantID == merchantID);
            var result = new List<dynamic>();
            //查询行为
            foreach (var info in list)
            {
                //查询方案数量
                var count = await robotProgramOperation.GetCountAsync(t => t.BehaviorID == info._id
                && t.MerchantID == merchantID);
                var data = new
                {
                    BehaviorID = info._id,
                    info.BehaviorName,
                    Count = count
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
        /// 获取行为信息
        /// </summary>
        /// <param name="behaviorID">行为id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBehaviorInfo(string behaviorID)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotBehaviorOperation robotBehaviorOperation = new RobotBehaviorOperation();
            var behavior = await robotBehaviorOperation.GetModelAsync(t => t._id == behaviorID
                && t.MerchantID == merchantID);
            if (behavior == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到行为信息"));
            var data = new WebBehaviorInfo
            {
                BehaviorID = behaviorID,
                BehaviorName = behavior.BehaviorName,
                Attack = behavior.Attack,
                AttackQuery = behavior.AttackQuery,
                ArmisticeQuery = behavior.ArmisticeQuery,
                UpCmd = behavior.UpCmd,
                DownCmd = behavior.DownCmd,
                StopCmd = behavior.StopCmd,
                EndPoint = behavior.EndPoint
            };
            return Ok(new RecoverClassModel<WebBehaviorInfo>()
            {
                Message = "获取成功",
                Model = data,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取行为下拉列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBehaviorList()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotBehaviorOperation robotBehaviorOperation = new RobotBehaviorOperation();
            RobotProgramOperation robotProgramOperation = new RobotProgramOperation();
            var list = await robotBehaviorOperation.GetModelListAsync(t => t.MerchantID == merchantID);
            var result = new List<dynamic>();
            var data = (from dt in list
                        select new
                        {
                            BehaviorID = dt._id,
                            dt.BehaviorName,
                        }).ToList();
            result.Add(data);
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 修改行为信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PutBehaviorInfo([FromBody] WebBehaviorInfo model)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotBehaviorOperation robotBehaviorOperation = new RobotBehaviorOperation();
            var behavior = await robotBehaviorOperation.GetModelAsync(t => t._id == model.BehaviorID
                && t.MerchantID == merchantID);
            if (behavior == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到行为信息"));
            behavior.Attack = model.Attack;
            behavior.BehaviorName = model.BehaviorName;
            behavior.AttackQuery = model.AttackQuery;
            behavior.ArmisticeQuery = model.ArmisticeQuery;
            behavior.UpCmd = model.UpCmd;
            behavior.DownCmd = model.DownCmd;
            behavior.StopCmd = model.StopCmd;
            behavior.EndPoint = model.EndPoint;
            await robotBehaviorOperation.UpdateModelAsync(behavior);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 添加行为信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddBehaviorInfo([FromBody] WebBehaviorInfo model)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotBehaviorOperation robotBehaviorOperation = new RobotBehaviorOperation();
            var behavior = new RobotBehavior
            {
                MerchantID = merchantID,
                Attack = model.Attack,
                BehaviorName = model.BehaviorName,
                AttackQuery = model.AttackQuery,
                ArmisticeQuery = model.ArmisticeQuery,
                UpCmd = model.UpCmd,
                DownCmd = model.DownCmd,
                StopCmd = model.StopCmd,
                EndPoint = model.EndPoint
            };
            await robotBehaviorOperation.InsertModelAsync(behavior);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = behavior._id,
                Message = "添加成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 删除行为
        /// </summary>
        /// <param name="behaviorID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteBehavior(string behaviorID)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotBehaviorOperation robotBehaviorOperation = new RobotBehaviorOperation();
            var behavior = await robotBehaviorOperation.GetModelAsync(t => t._id == behaviorID
               && t.MerchantID == merchantID);
            if (behavior == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到行为信息"));
            await robotBehaviorOperation.DeleteModelOneAsync(t => t._id == behaviorID
              && t.MerchantID == merchantID);
            return Ok(new RecoverModel(RecoverEnum.成功, "删除成功"));
        }

        /// <summary>
        /// 获取方案列表
        /// </summary>
        /// <param name="behaviorID">行为id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProgramList(string behaviorID)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotBehaviorOperation robotBehaviorOperation = new RobotBehaviorOperation();
            RobotProgramOperation robotProgramOperation = new RobotProgramOperation();
            var behavior = await robotBehaviorOperation.GetModelAsync(t => t._id == behaviorID
                && t.MerchantID == merchantID);
            if (behavior == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到行为信息"));
            var list = await robotProgramOperation.GetModelListAsync(t => t.BehaviorID == behaviorID
                && t.MerchantID == merchantID);
            var data = (from dt in list
                        select new WebGetProgramList
                        {
                            ProgramID = dt._id,
                            ProgramName = dt.ProgramName
                        }).ToList();
            return Ok(new RecoverListModel<WebGetProgramList>()
            {
                Data = data,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = data.Count
            });
        }

        private class WebGetProgramList
        {
            public string ProgramID { get; set; }
            public string ProgramName { get; set; }
        }

        /// <summary>
        /// 获取方案信息
        /// </summary>
        /// <param name="programID">方案id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetProgramInfo(string programID)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotProgramOperation robotProgramOperation = new RobotProgramOperation();
            var program = await robotProgramOperation.GetModelAsync(t => t._id == programID
            && t.MerchantID == merchantID);
            if (program == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到方案信息"));
            var result = new WebRobotProgram()
            {
                ProgramID = program._id,
                BehaviorID = program.BehaviorID,
                ProgramName = program.ProgramName,
                Amountset = program.Amountset,
                DoubleType = program.DoubleType,
                BetTypeList = program.BetTypeList,
                IsEnable = program.IsEnable
            };
            return Ok(new RecoverClassModel<WebRobotProgram>()
            {
                Message = "获取成功",
                Model = result,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 添加方案信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddProgramInfo([FromBody] WebRobotProgram model)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotProgramOperation robotProgramOperation = new RobotProgramOperation();
            RobotBehaviorOperation robotBehaviorOperation = new RobotBehaviorOperation();
            var behavior = await robotBehaviorOperation.GetModelAsync(t => t._id == model.BehaviorID
                && t.MerchantID == merchantID);
            if (behavior == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到行为信息"));
            // 查询方案数量
            var count = await robotProgramOperation.GetCountAsync(t => t.BehaviorID == model.BehaviorID
            && t.MerchantID == merchantID);
            if (count >= 5) return Ok(new RecoverModel(RecoverEnum.失败, "方案数量限制5个！"));
            var program = new RobotProgram()
            {
                MerchantID = merchantID,
                Amountset = model.Amountset,
                BehaviorID = model.BehaviorID,
                BetTypeList = model.BetTypeList,
                DoubleType = model.DoubleType,
                ProgramName = model.ProgramName,
                IsEnable = model.IsEnable
            };
            await robotProgramOperation.InsertModelAsync(program);
            return Ok(new RecoverModel(RecoverEnum.成功, "添加成功"));
        }

        /// <summary>
        /// 修改方案信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> PutProgramInfo([FromBody] WebRobotProgram model)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotProgramOperation robotProgramOperation = new RobotProgramOperation();
            var program = await robotProgramOperation.GetModelAsync(t => t._id == model.ProgramID
            && t.MerchantID == merchantID);
            if (program == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到方案信息"));
            program.ProgramName = model.ProgramName;
            program.Amountset = model.Amountset;
            program.DoubleType = model.DoubleType;
            program.BetTypeList = model.BetTypeList;
            program.IsEnable = model.IsEnable;
            await robotProgramOperation.UpdateModelAsync(program);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 删除方案信息
        /// </summary>
        /// <param name="programID">方案id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteProgramInfo(string programID)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            RobotProgramOperation robotProgramOperation = new RobotProgramOperation();
            var program = await robotProgramOperation.GetModelAsync(t => t._id == programID
            && t.MerchantID == merchantID);
            if (program == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到方案信息"));
            await robotProgramOperation.DeleteModelOneAsync(t => t._id == programID && t.MerchantID == merchantID);
            return Ok(new RecoverModel(RecoverEnum.成功, "删除成功"));
        }

        /// <summary>
        /// 添加虚假用户
        /// </summary>
        /// <param name="userName">名称</param>
        /// <param name="derail">是否开启</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddShamUser(string userName, bool derail)
        {
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            //查询假人数量
            var count = await userOperation.GetCountAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.假人 && t.IsSupport == true);
            if (count >= 20) return Ok(new RecoverModel(RecoverEnum.失败, "最多添加20个机器人！"));
            var flag = await userOperation.GetModelAsync(t => t.LoginName == userName && t.MerchantID == merchantID);
            if (flag != null)
            {
                if (flag.Status == UserStatusEnum.删除)
                {
                    flag.Status = UserStatusEnum.假人;
                    await userOperation.UpdateModelAsync(flag);

                    var shamInfo = await shamRobotOperation.GetModelAsync(t => t.UserID == flag._id && t.MerchantID == merchantID);
                    if (shamInfo == null)
                    {
                        shamInfo = new ShamRobotmanage()
                        {
                            UserID = flag._id,
                            MerchantID = merchantID,
                            Check = derail
                        };
                        shamInfo.GameCheckInfo.ForEach(t =>
                        {
                            t.Check = derail;
                        });
                        await shamRobotOperation.InsertModelAsync(shamInfo);
                    }
                    return Ok(new RecoverModel(RecoverEnum.成功, "添加成功！"));
                }
                else
                    return Ok(new RecoverModel(RecoverEnum.失败, "已存在相同名称用户！"));
            }
            var user = new User()
            {
                Level = UserLevelEnum.黄铜,
                NickName = userName,
                LoginName = userName,
                Status = UserStatusEnum.假人,
                MerchantID = merchantID,
                OnlyCode = userOperation.GetNewUserOnlyCode(),
                Avatar = "UserImages/default.png",
                IsSupport = true
            };
            //添加默认方案
            var backwaterSetupOperation = new BackwaterSetupOperation();
            var backwaterSetup = backwaterSetupOperation.GetModel(t => t.MerchantID == merchantID, t => t.CreatedTime, true);
            if (backwaterSetup != null)
                user.ProgrammeID = backwaterSetup._id;
            VideoBackwaterSetupOperation videoBackwaterSetupOperation = new VideoBackwaterSetupOperation();
            var videoSetup = videoBackwaterSetupOperation.GetModel(t => t.MerchantID == merchantID, t => t.CreatedTime, true);
            if (videoSetup != null)
                user.VideoProgrammeID = videoSetup._id;
            await userOperation.InsertModelAsync(user);

            var shamUser = await shamRobotOperation.GetModelAsync(t => t.UserID == user._id && t.MerchantID == merchantID);
            if (shamUser == null)
            {
                shamUser = new ShamRobotmanage()
                {
                    UserID = user._id,
                    MerchantID = merchantID,
                    Check = derail
                };
                shamUser.GameCheckInfo.ForEach(t =>
                {
                    t.Check = derail;
                });
                await shamRobotOperation.InsertModelAsync(shamUser);
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "添加成功！"));
        }

        /// <summary>
        /// 虚假用户全部开启或关闭
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ShamUserAllOpenOrClost(bool derail)
        {
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userList = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.假人);
            foreach (var user in userList)
            {
                var shamUser = await shamRobotOperation.GetModelAsync(t => t.UserID == user._id && t.MerchantID == merchantID);
                if (shamUser == null) continue;
                shamUser.Check = derail;
                //shamUser.GameCheckInfo.ForEach(t =>
                //{
                //    if (string.IsNullOrEmpty(t.BehaviorID))
                //    {
                //        t.Check = false;
                //    }
                //    else
                //        t.Check = derail;
                //});
                await shamRobotOperation.UpdateModelAsync(shamUser);
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 控制假人开启关闭
        /// </summary>
        /// <param name="id">对应id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ShamUserOpenOrClose(string id)
        {
            ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var shamUser = await shamRobotOperation.GetModelAsync(t => t._id == id && t.MerchantID == merchantID);
            if (shamUser == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到假人信息!"));
            shamUser.Check = shamUser.Check ? false : true;
            //shamUser.GameCheckInfo.ForEach(t =>
            //{
            //    if (string.IsNullOrEmpty(t.BehaviorID))
            //    {
            //        t.Check = false;
            //    }
            //    else
            //        t.Check = shamUser.Check;
            //});
            await shamRobotOperation.UpdateModelAsync(shamUser);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取敏感操作日志
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Data
        ///         {
        ///             ID：编号
        ///             Type：类型
        ///             Location：操作位置
        ///             OpBcontent：操作前内容
        ///             OpAcontent：操作后内容
        ///             CreateTime：时间
        ///         }
        ///         Total：数据数量
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSensitiveInfos(int start = 1, int pageSize = 10)
        {
            SensitiveOperation sensitiveOperation = new SensitiveOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var total = await sensitiveOperation.GetCountAsync(t => t.MerchantID == merchantID);
            var list = sensitiveOperation.GetModelListByPaging(t => t.MerchantID == merchantID, t => t.CreatedTime, false, start, pageSize);
            var result = (from data in list
                          select new
                          {
                              ID = data._id,
                              Type = Enum.GetName(typeof(OpTypeEnum), data.OpType),
                              Location = Enum.GetName(typeof(OpLocationEnum), data.OpLocation),
                              data.OpBcontent,
                              data.OpAcontent,
                              CreateTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss")
                          }).ToList();
            return Ok(new { Data = result, Message = "查询成功", Status = RecoverEnum.成功, Total = total });
        }

        /// <summary>
        /// 手动上分
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="branch">上分金额</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ManualUpper(string userID, decimal branch, string remark)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户相关信息！"));
            await userOperation.UpperScore(user._id, merchantID, branch, ChangeTargetEnum.手动, "手动上分成功（手动上分）", remark);
            #region 通知
            user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            string message = reply.Remainder.Replace("{昵称}",
                user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName)
                .Replace("{变动分数}", branch.ToString("#0.00"))
                .Replace("{剩余分数}", user.UserMoney.ToString("#0.00"));
            var gameType = await Utils.GetRoomGameToUserGameType(merchantID, user._id);
            if (gameType != null)
                await RabbitMQHelper.SendAdminMessage(message, merchantID, gameType.Value);
            await RabbitMQHelper.SendUserPointChange(user._id, merchantID);
            #endregion
            return Ok(new RecoverModel(RecoverEnum.成功, "上分成功！"));
        }

        /// <summary>
        /// 手动下分
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="branch">下分金额</param>
        /// <param name="remark">备注</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> ManualLower(string userID, decimal branch, string remark)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到该用户相关信息！"));
            if (user.UserMoney < branch) return Ok(new RecoverModel(RecoverEnum.失败, "用户余额不足！"));
            var result = await userOperation.LowerScore(user._id, merchantID, branch, ChangeTargetEnum.手动, "手动下分成功（手动下分）", remark, orderStatus: OrderStatusEnum.下分成功);
            if(!result) return Ok(new RecoverModel(RecoverEnum.失败, "用户余额不足！"));
            #region 通知
            user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            string message = reply.Remainder.Replace("{昵称}",
user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName)
                .Replace("{变动分数}", (-branch).ToString("#0.00"))
                .Replace("{剩余分数}", user.UserMoney.ToString("#0.00"));
            var gameType = await Utils.GetRoomGameToUserGameType(merchantID, user._id);
            if (gameType != null)
                await RabbitMQHelper.SendAdminMessage(message, merchantID, gameType.Value);
            await RabbitMQHelper.SendUserPointChange(user._id, merchantID);
            #endregion
            return Ok(new RecoverModel(RecoverEnum.成功, "下分成功！"));
        }

        /// <summary>
        /// 获取用户下拉列表（正常用户）
        /// </summary>
        /// <param name="status">true：包含假人和正常用户 false：只包含正常用户</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUpDownUserList(bool status)
        {
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            Expression<Func<User, bool>> filter = null;
            if (status)
                filter = t => t.MerchantID == merchantID
            && (t.Status == UserStatusEnum.正常 || (t.Status == UserStatusEnum.假人 && t.IsSupport == false));
            else
                filter = t => t.MerchantID == merchantID
            && t.Status == UserStatusEnum.正常;
            var userList = await userOperation.GetModelListAsync(filter);
            var result = new List<dynamic>();
            userList.ForEach(user =>
            {
                var data = new
                {
                    UserID = user._id,
                    user.LoginName,
                    NickName = string.IsNullOrEmpty(user.MemoName) && user.ShowType ? user.NickName : user.MemoName,
                    user.OnlyCode
                };
                result.Add(data);
            });
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 获取历史上下分记录
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <param name="status">条件 1：申请上分 2：申请下分 3：手动上分 4：手动下分  全部传空</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Data
        ///         {
        ///             ID：编号
        ///             LoginName：登录名称
        ///             NickName：昵称
        ///             Amount：金额
        ///             ApplyTime：申请时间
        ///             OrderStatus：状态
        ///             Remark：备注
        ///             Management：管理操作
        ///             Status：用户状态
        ///         }
        ///         Total：数据数量
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetIntegralRecord(string keyword, int? status, DateTime startTime, DateTime endTime, int start = 1, int pageSize = 10)
        {
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            FilterDefinition<User> userFilter = userOperation.Builder.Where(t => t.MerchantID == merchantID);
            if (!string.IsNullOrEmpty(keyword))
            {
                userFilter &= userOperation.Builder.Where(t => t.OnlyCode == keyword || t.NickName.Contains(keyword) || t.MemoName.Contains(keyword));
            }
            var userList = await userOperation.GetModelListAsync(userFilter);
            var userIDList = userList.Select(t => t._id).ToList();
            FilterDefinition<UserIntegration> filter = userIntegrationOperation.Builder.Where(t =>
            t.MerchantID == merchantID
            && (t.Management == ManagementEnum.已同意 || t.Management == ManagementEnum.已拒绝)
            //&& (t.ChangeTarget == ChangeTargetEnum.手动 || t.ChangeTarget == ChangeTargetEnum.申请)
            && t.CreatedTime >= startTime
            && t.CreatedTime <= endTime);
            filter &= userIntegrationOperation.Builder.In(t => t.UserID, userIDList);
            if (status == null)
                filter &= userIntegrationOperation.Builder.Where(t => t.ChangeTarget == ChangeTargetEnum.手动 || t.ChangeTarget == ChangeTargetEnum.申请);
            //申请上分
            else if (status == 1)
                filter &= userIntegrationOperation.Builder.Where(t => t.ChangeTarget == ChangeTargetEnum.申请 && t.ChangeType == ChangeTypeEnum.上分);
            //申请下分
            else if (status == 2)
                filter &= userIntegrationOperation.Builder.Where(t => t.ChangeTarget == ChangeTargetEnum.申请 && t.ChangeType == ChangeTypeEnum.下分);
            //手动上分
            else if (status == 3)
                filter &= userIntegrationOperation.Builder.Where(t => t.ChangeTarget == ChangeTargetEnum.手动 && t.ChangeType == ChangeTypeEnum.上分);
            //手动下分
            else if (status == 4)
                filter &= userIntegrationOperation.Builder.Where(t => t.ChangeTarget == ChangeTargetEnum.手动 && t.ChangeType == ChangeTypeEnum.下分);
            var list = userIntegrationOperation.GetModelListByPaging(filter,
            t => t.CreatedTime, false, start, pageSize);
            var total = await userIntegrationOperation.GetCountAsync(filter);
            var result = new List<dynamic>();
            list.ForEach(data =>
            {
                var user = userList.Find(t => t._id == data.UserID);
                if (user == null) return;
                var code = new
                {
                    ID = data._id,
                    user.LoginName,
                    NickName = string.IsNullOrEmpty(user.MemoName) && user.ShowType ? user.NickName : user.MemoName,
                    user.OnlyCode,
                    Amount = data.ChangeType == ChangeTypeEnum.下分 ? data.Amount > 0 ? -data.Amount : data.Amount : data.Amount,
                    ApplyTime = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    OrderStatus = Enum.GetName(typeof(OrderStatusEnum), data.OrderStatus),
                    data.Remark,
                    Management = Enum.GetName(typeof(ManagementEnum), data.Management),
                    Status = Enum.GetName(typeof(NotesEnum), (int)data.Notes)
                };
                result.Add(code);
            });
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "查询成功！",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        /// <summary>
        /// 获取上下分数量总数及总分数
        /// </summary>
        /// <param name="status">条件 1：申请上分 2：申请下分 3：手动上分 4：手动下分  全部传空</param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTotalIntegral(int? status, DateTime startTime, DateTime endTime)
        {
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            UserOperation userOperation = new UserOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            FilterDefinition<UserIntegration> filter = userIntegrationOperation.Builder.Where(t =>
             t.MerchantID == merchantID
             && (t.Management == ManagementEnum.已同意 || t.Management == ManagementEnum.已拒绝) && t.Notes == NotesEnum.正常
             //&& (t.ChangeTarget == ChangeTargetEnum.手动 || t.ChangeTarget == ChangeTargetEnum.申请)
             && t.CreatedTime >= startTime
             && t.CreatedTime <= endTime);
            if (status == null)
                filter &= userIntegrationOperation.Builder.Where(t => t.ChangeTarget == ChangeTargetEnum.手动 || t.ChangeTarget == ChangeTargetEnum.申请);
            //申请上分
            else if (status == 1)
                filter &= userIntegrationOperation.Builder.Where(t => t.ChangeTarget == ChangeTargetEnum.申请 && t.ChangeType == ChangeTypeEnum.上分);
            //申请下分
            else if (status == 2)
                filter &= userIntegrationOperation.Builder.Where(t => t.ChangeTarget == ChangeTargetEnum.申请 && t.ChangeType == ChangeTypeEnum.下分);
            //手动上分
            else if (status == 3)
                filter &= userIntegrationOperation.Builder.Where(t => t.ChangeTarget == ChangeTargetEnum.手动 && t.ChangeType == ChangeTypeEnum.上分);
            //手动下分
            else if (status == 4)
                filter &= userIntegrationOperation.Builder.Where(t => t.ChangeTarget == ChangeTargetEnum.手动 && t.ChangeType == ChangeTypeEnum.下分);
            var data = userIntegrationOperation.GetModelList(filter);
            var upsum = data.FindAll(t => t.Management == ManagementEnum.已同意 && t.ChangeType == ChangeTypeEnum.上分).Sum(t => t.Amount);
            var downsum = data.FindAll(t => t.Management == ManagementEnum.已同意 && t.ChangeType == ChangeTypeEnum.下分).Sum(t => t.Amount);
            var total = await userIntegrationOperation.GetCountAsync(filter);
            return Ok(new
            {
                Message = "查询成功！",
                Status = RecoverEnum.成功,
                Data = new
                {
                    UpScore = upsum,
                    DownScore = downsum,
                    Total = total
                }
            });
        }

        /// <summary>
        /// 添加公告
        /// </summary>
        /// <param name="context">公告内容</param>
        /// <remarks>
        ///     ArticleType：类型  1：公告  2：消息
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddArticle([FromBody]WebArticle context)
        {
            ArticleOperation articleOperation = new ArticleOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var article = new Article()
            {
                Content = context.Content,
                MerchantID = merchantID,
                ArticleType = context.ArticleType,
                Open = true
            };
            await articleOperation.InsertModelAsync(article);
            if (context.ArticleType == ArticleTypeEnum.公告)
            {
                await RabbitMQHelper.SendOverallMessage(article.Content, merchantID, "SendListArticle");
                //关闭其它公告
                var openItem = await articleOperation.GetModelAsync(t => t.MerchantID == merchantID && t.ArticleType == ArticleTypeEnum.公告 && t.Open == true && t._id != article._id);
                if (openItem != null)
                {
                    openItem.Open = false;
                    await articleOperation.UpdateModelAsync(openItem);
                }
            }
            else
            {
                RabbitMQHelper.SendAllinMessage(article.Content, merchantID, "SendRoomArticle");
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "添加成功！"));
        }

        /// <summary>
        /// 获取公告列表
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Data
        ///         {
        ///             ID：编号
        ///             Type：类型
        ///             Content：内容
        ///         }
        ///         Total：数据数量
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetArticleList()
        {
            ArticleOperation articleOperation = new ArticleOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var list = articleOperation.GetModelList(t => t.MerchantID == merchantID && t.ArticleType == ArticleTypeEnum.公告, t => t.CreatedTime, false);
            var result = (from data in list
                          select new WebArticle
                          {
                              ID = data._id,
                              Type = Enum.GetName(typeof(ArticleTypeEnum), data.ArticleType),
                              Content = data.Content,
                              Open = data.Open,
                              Time = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss")
                          }).ToList();
            return Ok(new RecoverListModel<WebArticle>() { Data = result, Message = "查询成功！", Status = RecoverEnum.成功, Total = result.Count });
        }

        /// <summary>
        /// 获取公告信息
        /// </summary>
        /// <param name="id">公告id</param>
        /// <response>
        /// ## 返回结果
        ///     ID：编号
        ///     Type：类型
        ///     Content：内容
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetArticleByID(string id)
        {
            ArticleOperation articleOperation = new ArticleOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var article = await articleOperation.GetModelAsync(t => t._id == id && t.MerchantID == merchantID);
            if (article == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关公告！"));
            var result = new WebArticle
            {
                ID = article._id,
                Type = Enum.GetName(typeof(ArticleTypeEnum), article.ArticleType),
                Content = article.Content,
                ArticleType = article.ArticleType
            };
            return Ok(new RecoverClassModel<WebArticle>() { Message = "查询成功！", Model = result, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 删除公告
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteArticleByID(string id)
        {
            ArticleOperation articleOperation = new ArticleOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var article = await articleOperation.GetModelAsync(t => t._id == id && t.MerchantID == merchantID);
            if (article == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关公告！"));
            await articleOperation.DeleteModelOneAsync(t => t._id == id && t.MerchantID == merchantID);
            return Ok(new RecoverModel(RecoverEnum.成功, "删除成功！"));
        }

        /// <summary>
        /// 修改公告
        /// </summary>
        /// <param name="id">公告id</param>
        /// <param name="content">公告内容(只传Content)</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateArticle(string id, [FromBody]WebArticle content)
        {
            ArticleOperation articleOperation = new ArticleOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var article = await articleOperation.GetModelAsync(t => t._id == id && t.MerchantID == merchantID);
            if (article == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关公告！"));
            article.Content = content.Content;
            article.ArticleType = content.ArticleType;
            await articleOperation.UpdateModelAsync(article);
            await RabbitMQHelper.SendOverallMessage(article.Content, merchantID, "SendListArticle");
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 开启公告
        /// </summary>
        /// <param name="articleID">公告id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OpenArticle(string articleID)
        {
            ArticleOperation articleOperation = new ArticleOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var article = await articleOperation.GetModelAsync(t => t._id == articleID && t.MerchantID == merchantID);
            if (article == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关公告！"));
            article.Open = true;
            await articleOperation.UpdateModelAsync(article);
            if (article.ArticleType == ArticleTypeEnum.公告)
            {
                await RabbitMQHelper.SendOverallMessage(article.Content, merchantID, "SendListArticle");
                //关闭其它公告
                var openItem = await articleOperation.GetModelAsync(t => t.MerchantID == merchantID && t.ArticleType == ArticleTypeEnum.公告 && t.Open == true && t._id != article._id);
                if (openItem != null)
                {
                    openItem.Open = false;
                    await articleOperation.UpdateModelAsync(openItem);
                }
            }
            else
            {
                RabbitMQHelper.SendAllinMessage(article.Content, merchantID, "SendRoomArticle");
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 关闭公告
        /// </summary>
        /// <param name="articleID">公告id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> CloseArticle(string articleID)
        {
            ArticleOperation articleOperation = new ArticleOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var article = await articleOperation.GetModelAsync(t => t._id == articleID && t.MerchantID == merchantID);
            if (article == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关公告！"));
            article.Open = false;
            await articleOperation.UpdateModelAsync(article);
            await RabbitMQHelper.SendOverallMessage(" ", merchantID, "SendListArticle");
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }
        #region 手动开奖
        /// <summary>
        /// 获取游戏历史
        /// </summary>
        /// <param name="nper">期号</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页面数量</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SearchGameLottery(string nper, GameOfType gameType, int start = 1, int pageSize = 10)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            //var nper = CancelAnnouncement.GetGameNper(gameType);
            var gameLotteryList = new List<GameLottery>();
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            SemaphoreSlim taskLock = new SemaphoreSlim(5);
            long total = 0;
            try
            {
                if (Utils.GameTypeItemize(gameType))
                {
                    Lottery10Operation lottery10Operation = new Lottery10Operation();
                    var filter = lottery10Operation.Builder.Where(t => t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID));
                    if (!string.IsNullOrEmpty(nper))
                        filter &= lottery10Operation.Builder.Eq(t => t.IssueNum, nper);
                    var list = lottery10Operation.GetModelListByPaging(filter, t => t.IssueNum, false, start, pageSize);
                    total = await lottery10Operation.GetCountAsync(filter);
                    var tasks = new List<Task>();
                    foreach (var data in list)
                    {
                        var task = Task.Run(async () =>
                        {
                            try
                            {
                                await taskLock.WaitAsync();
                                var lottery = data;
                                var betInfos = await collection.FindListAsync(t => t.MerchantID == merchantID
                                && t.Nper == lottery.IssueNum
                                && t.GameType == gameType);
                                var handle = new GameLottery
                                {
                                    IssueNum = lottery.IssueNum,
                                    LotteryTime = lottery.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Nums = GameDiscrimination.SetupSeparation10(lottery, ','),
                                    BetAmount = betInfos.Sum(t => t.AllUseMoney),
                                    WinAmount = betInfos.Sum(t => t.AllMediumBonus),
                                    Status = betInfos.Exists(t => t.BetStatus == BetStatus.未开奖) ? "未开奖" : "已开奖"
                                };
                                gameLotteryList.Add(handle);
                            }
                            catch { }
                            finally
                            {
                                taskLock.Release();
                            }
                        });
                        tasks.Add(task);
                    }
                    Task.WaitAll(tasks.ToArray());
                }
                else
                {
                    Lottery5Operation lottery5Operation = new Lottery5Operation();
                    var filter = lottery5Operation.Builder.Where(t => t.GameType == gameType && (t.MerchantID == null || t.MerchantID == merchantID));
                    if (!string.IsNullOrEmpty(nper))
                        filter &= lottery5Operation.Builder.Eq(t => t.IssueNum, nper);
                    var list = lottery5Operation.GetModelListByPaging(filter, t => t.IssueNum, false, start, pageSize);
                    total = await lottery5Operation.GetCountAsync(filter);
                    var tasks = new List<Task>();
                    foreach (var data in list)
                    {
                        var task = Task.Run(async () =>
                        {
                            try
                            {
                                await taskLock.WaitAsync();
                                var lottery = data;
                                var betInfos = await collection.FindListAsync(t => t.MerchantID == merchantID
                                && t.Nper == lottery.IssueNum
                                && t.GameType == gameType);
                                var handle = new GameLottery
                                {
                                    IssueNum = lottery.IssueNum,
                                    LotteryTime = lottery.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                                    Nums = GameDiscrimination.SetupSeparation5(lottery, ','),
                                    BetAmount = betInfos.Sum(t => t.AllUseMoney),
                                    WinAmount = betInfos.Sum(t => t.AllMediumBonus),
                                    Status = betInfos.Exists(t => t.BetStatus == BetStatus.未开奖) ? "未开奖" : "已开奖"
                                };
                                gameLotteryList.Add(handle);
                            }
                            catch { }
                            finally
                            {
                                taskLock.Release();
                            }
                        });
                        tasks.Add(task);
                    }
                    Task.WaitAll(tasks.ToArray());
                }
            }
            catch (Exception e)
            {
                Utils.Logger.Error(JsonConvert.SerializeObject(e));
            }
            finally
            {
                taskLock.Dispose();
            }
            gameLotteryList = gameLotteryList.OrderByDescending(t => t.IssueNum).ToList();
            return Ok(new RecoverListModel<GameLottery>
            {
                Data = gameLotteryList,
                Message = "获取成功！",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        /// <summary>
        /// 获取游戏当期用户下注明细
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="nper"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGameLotteryDetailed(GameOfType gameType, string nper)
        {
            if (gameType <= 0) return Ok(new RecoverModel(RecoverEnum.失败, "未选择游戏类型"));
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var result = new List<GameLotteryDetailed>();
            if (Utils.GameTypeItemize(gameType))
            {
                Lottery10Operation lottery10Operation = new Lottery10Operation();
                var model = await lottery10Operation.GetModelAsync(t => t.GameType == gameType && t.IssueNum == nper && (t.MerchantID == null || t.MerchantID == merchantID));
                if (model == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到开奖号码信息！"));
            }
            else
            {
                Lottery5Operation lottery5Operation = new Lottery5Operation();
                var model = await lottery5Operation.GetModelAsync(t => t.GameType == gameType && t.IssueNum == nper && (t.MerchantID == null || t.MerchantID == merchantID));
                if (model == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到开奖号码信息！"));
            }

            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var userBet = await collection.FindListAsync(t => t.Nper == nper && t.MerchantID == merchantID && t.GameType == gameType);
            UserOperation userOperation = new UserOperation();
            foreach (var bet in userBet)
            {
                var user = await userOperation.GetModelAsync(t => t._id == bet.UserID && t.MerchantID == merchantID);
                foreach (var remark in bet.BetRemarks)
                {
                    var winBet = remark.OddBets.FindAll(t => t.BetStatus == BetStatusEnum.已中奖);
                    var winNum = string.Join(",", winBet.Select(t => string.Format("{0}{1}/{2}={3}", Enum.GetName(typeof(BetTypeEnum), (int)t.BetRule),
                       t.BetNo, t.BetMoney.ToString(), t.MediumBonus.ToString())).ToList());
                    var data = new GameLotteryDetailed()
                    {
                        IssueNum = nper,
                        UserName = string.IsNullOrEmpty(user.MemoName) && user.ShowType ? user.NickName : user.MemoName,
                        OnlyCode = user.OnlyCode,
                        BetAmount = remark.OddBets.Sum(t => t.BetMoney),
                        BetNum = remark.Remark,
                        BetTime = remark.BetTime.ToString("yyyy-MM-dd HH:mm:ss"),
                        WinAmount = remark.OddBets.Sum(t => t.MediumBonus),
                        WinNum = winNum,
                        Status = remark.OddBets.Exists(t => t.BetStatus == BetStatusEnum.已投注) ? "未开奖" : "已开奖"
                    };
                    result.Add(data);
                }
            }
            return Ok(new RecoverListModel<GameLotteryDetailed>
            {
                Data = result,
                Message = "获取成功！",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 添加10球游戏
        /// </summary>
        /// <param name="grasp10"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddGameLottery10([FromBody]Grasp10 grasp10, GameOfType gameType)
        {
            //判断是否有重复数字
            var list = new List<string>()
            {
                grasp10.One,
                grasp10.Two,
                grasp10.Three,
                grasp10.Four,
                grasp10.Five,
                grasp10.Six,
                grasp10.Seven,
                grasp10.Eight,
                grasp10.Nine,
                grasp10.Ten
            };
            list = list.Distinct().ToList();
            if (list.Count != 10) return Ok(new RecoverModel(RecoverEnum.失败, "存在重复数字，请确认后再填写！"));

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var status = await Utils.GetGameStatus(merchantID, gameType);
            if (Convert.ToDouble(grasp10.IssueNum) >= Convert.ToDouble(status.NextIssueNum))
                return Ok(new RecoverModel(RecoverEnum.失败, "期号不能大于最新一期！"));
            Lottery10Operation lottery10Operation1 = new Lottery10Operation();
            var minModel = lottery10Operation1.GetModel(t => t.MerchantID == null && t.CreatedTime >= DateTime.Now.Date, t => t.IssueNum, true);
            if (minModel != null)
            {
                if (Convert.ToDouble(grasp10.IssueNum) <= Convert.ToDouble(minModel.IssueNum))
                    return Ok(new RecoverModel(RecoverEnum.失败, "期号不能小于今天第一期！"));
            }
            var bson = await lottery10Operation1.GetModelAsync(t => t.GameType == gameType && t.IssueNum == grasp10.IssueNum && (t.MerchantID == null || t.MerchantID == merchantID));
            if (bson == null)
            {
                Lottery10Operation lottery10Operation = new Lottery10Operation();
                var model = GameAlgorithms.Algorithms10<Lottery10>(grasp10, gameType);
                model.MerchantID = merchantID;
                await lottery10Operation.InsertModelAsync(model);
                //开奖
                var result = await WinPrize.Win10Async(model, gameType, merchantID);
                if (!result.IsNull())
                {
                    //发送中奖用户积分
                    foreach (var data in result)
                    {
                        await RabbitMQHelper.SendUserPointChange(data.UserID, data.MerchantID);
                    }
                }

                return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
            }
            else
                return Ok(new RecoverModel(RecoverEnum.失败, "已存在此条游戏开奖记录！"));
        }

        /// <summary>
        /// 添加5球游戏
        /// </summary>
        /// <param name="grasp5"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddGameLottery5([FromBody]Grasp5 grasp5, GameOfType gameType)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var status = await Utils.GetGameStatus(merchantID, gameType);
            if (Convert.ToDouble(grasp5.IssueNum) >= Convert.ToDouble(status.NextIssueNum))
                return Ok(new RecoverModel(RecoverEnum.失败, "期号不能大于最新一期"));
            Lottery5Operation lottery5Operation1 = new Lottery5Operation();
            var minModel = lottery5Operation1.GetModel(t => t.MerchantID == null && t.CreatedTime >= DateTime.Now.Date, t => t.IssueNum, true);
            if (minModel != null)
            {
                if (Convert.ToDouble(grasp5.IssueNum) <= Convert.ToDouble(minModel.IssueNum))
                    return Ok(new RecoverModel(RecoverEnum.失败, "期号不能小于今天第一期！"));
            }
            var bson = await lottery5Operation1.GetModelAsync(t => t.GameType == gameType && t.IssueNum == grasp5.IssueNum && (t.MerchantID == null || t.MerchantID == merchantID));
            if (bson == null)
            {
                Lottery5Operation lottery5Operation = new Lottery5Operation();
                var model = GameAlgorithms.Algorithms5<Lottery5>(grasp5, gameType);
                model.MerchantID = merchantID;
                await lottery5Operation.InsertModelAsync(model);
                //开奖
                var result = await WinPrize.Win5Async(model, gameType, merchantID);
                if (!result.IsNull())
                {
                    //发送中奖用户积分
                    foreach (var data in result)
                    {
                        await RabbitMQHelper.SendUserPointChange(data.UserID, data.MerchantID);
                    }
                }

                return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
            }
            else
                return Ok(new RecoverModel(RecoverEnum.失败, "已存在此条游戏开奖记录！"));
        }

        /// <summary>
        /// 手动开奖
        /// </summary>
        /// <param name="nper"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ManualAward(string nper, GameOfType gameType)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (Utils.GameTypeItemize(gameType))
            {
                Lottery10Operation lottery10Operation1 = new Lottery10Operation();
                var bson = await lottery10Operation1.GetModelAsync(t => t.GameType == gameType && t.IssueNum == nper && (t.MerchantID == null || t.MerchantID == merchantID));
                if (bson == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到开奖号码信息！"));
                await WinPrize.Win10Async(bson, gameType, merchantID);
            }
            else
            {
                Lottery5Operation lottery5Operation1 = new Lottery5Operation();
                var bson = await lottery5Operation1.GetModelAsync(t => t.GameType == gameType && t.IssueNum == nper && (t.MerchantID == null || t.MerchantID == merchantID));
                if (bson == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到开奖号码信息！"));
                await WinPrize.Win5Async(bson, gameType, merchantID);
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }

        /// <summary>
        /// 获取特殊设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetReplySetUp()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            var result = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (result == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到数据！"));
            var data = Utils.ReplyDataToWeb(result);
            return Ok(new RecoverClassModel<WebReplySetUp>()
            {
                Message = "获取成功",
                Model = data,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 修改特殊设置
        /// </summary>
        /// <param name="webReply"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateReplySetUp([FromBody] WebReplySetUp webReply)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (webReply == null) return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误"));
            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            var result = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (result == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到相关数据"));
            Utils.WebReplyToData(webReply, ref result);
            await replySetUpOperation.UpdateModelAsync(result);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        public class GameLottery
        {
            public string IssueNum { get; set; }

            public string LotteryTime { get; set; }

            public string Nums { get; set; }
            public decimal BetAmount { get; set; }

            public decimal WinAmount { get; set; }

            public string Status { get; set; }

        }

        public class GameLotteryDetailed
        {
            public string IssueNum { get; set; }

            public string UserName { get; set; }

            public string OnlyCode { get; set; }

            public string BetTime { get; set; }

            public decimal BetAmount { get; set; }

            public decimal WinAmount { get; set; }

            public string BetNum { get; set; }

            public string WinNum { get; set; }

            public string Status { get; set; }
        }
        #endregion

        #region 飞单
        /// <summary>
        /// 获取飞单信息
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             AdvanceTime：封盘提前时间
        ///             SeurityNo：安全码
        ///             AutoSyn：自动同步状态
        ///             Password：密码
        ///             Remind：声音提示开关
        ///             OpenSheet：飞单开启状态
        ///             LowFraction：分数限制
        ///             MerchantName：商户名称
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFlySheetInfo()
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
            var info = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (info == null)
            {
                info = new MerchantSheet()
                {
                    MerchantID = merchantID
                };
                await merchantSheetOperation.InsertModelAsync(info);
            }
            var result = new WebMerchantSheet()
            {
                AdvanceTime = info.AdvanceTime,
                SeurityNo = info.SeurityNo,
                AutoSyn = info.AutoSyn,
                Password = info.Password,
                Remind = info.Remind,
                OpenSheet = info.OpenSheet,
                LowFraction = info.LowFraction,
                MerchantName = info.MerchantName,
                RoomNum = info.RoomNum
            };
            return Ok(new RecoverClassModel<WebMerchantSheet>()
            {
                Message = "获取成功！",
                Model = result,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 飞单账号添加信息
        /// </summary>
        /// <param name="seurityNo">安全码</param>
        /// <param name="merchantName">账号名称</param>
        /// <param name="merchantPwd">账号密码</param>
        /// <param name="roomNum">房间号</param>
        /// <response>
        ///     Model
        ///     {
        ///         UserID：目标用户Id
        ///         TargetID：目标商户id
        ///     }
        /// </response> 
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> AddSheetMerchant(string seurityNo, string merchantName, string merchantPwd, string roomNum)
        {
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t.SeurityNo == seurityNo);
            if (merchant == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查找到相关安全码信息！"));
            if (!merchant.SuperStatus) return Ok(new RecoverModel(RecoverEnum.失败, "目标商户不是超级商户，不能接收飞单，请联系平台开通！"));
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t.LoginName == merchantName && t.Password == Utils.MD5(merchantPwd) && t.MerchantID == merchant._id && t.Status == UserStatusEnum.正常);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.失败, "目标飞单账号或密码错误或不存在！"));
            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchant._id);
            if (room.RoomNum != roomNum)
            {
                //查询代理
                AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
                var agentInfo = await agentBackwaterOperation.GetModelAsync(t => t.ExtensionCode == roomNum && t.MerchantID == merchant._id);
                if (agentInfo == null)
                    return Ok(new RecoverModel(RecoverEnum.失败, "房间号错误！"));
            }
            MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var setup = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);
            setup.SeurityNo = seurityNo;
            setup.MerchantName = merchantName;
            setup.Password = merchantPwd;
            setup.RoomNum = roomNum;
            await merchantSheetOperation.UpdateModelAsync(setup);

            return Ok(new RecoverClassModel<dynamic>()
            {
                Model = new
                {
                    UserID = user._id,
                    TargetID = merchant._id
                },
                Message = "验证成功！",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 飞单同步
        /// </summary>
        /// <param name="time">提前时间</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SheetSynchronization(int time)
        {
            if (time < 0) return Ok(new RecoverModel(RecoverEnum.失败, "设置时间错误！"));
            MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var setup = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);
            setup.AdvanceTime = time;
            await merchantSheetOperation.UpdateModelAsync(setup);
            //判断是否登录
            var dicInfo = await Utils.GetMerchantFlySheetInfo(merchantID);
            if (dicInfo == null) return Ok(new RecoverModel(RecoverEnum.失败, "未登录飞单帐号，不能同步！"));
            FoundationSetupOperation foundationSetupOperation = new FoundationSetupOperation();
            var targetSetup = await foundationSetupOperation.GetModelAsync(t => t.MerchantID == dicInfo["TargetID"]);
            var foundation = await foundationSetupOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (targetSetup == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到目标商户设置！"));
            if (foundation == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到商户设置！"));
            foundation.LotteryFrontTime = targetSetup.LotteryFrontTime;
            foundation.LotteryFrontTime.ForEach(t =>
            {
                t.LotteryTime = (t.LotteryTime - time) <= 0 ? 0 : t.LotteryTime - time;
            });
            await foundationSetupOperation.UpdateModelAsync(foundation);
            RedisOperation.SetFoundationSetup(merchantID, foundation);

            return Ok(new RecoverModel(RecoverEnum.成功, "同步成功！"));
        }

        /// <summary>
        /// 自动修正封盘时间
        /// </summary>
        /// <param name="islock"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SheetAutoCorrection(bool islock)
        {
            MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var setup = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);
            setup.AutoSyn = islock;
            await merchantSheetOperation.UpdateModelAsync(setup);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }

        /// <summary>
        /// 飞单开启
        /// </summary>
        /// <param name="open"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FlySingleOpen(bool open)
        {
            MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var setup = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);
            setup.OpenSheet = open;
            await merchantSheetOperation.UpdateModelAsync(setup);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }

        /// <summary>
        /// 设置声音提示分数界限
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> TipsLimit(decimal num)
        {
            MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var setup = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);
            setup.LowFraction = num;
            await merchantSheetOperation.UpdateModelAsync(setup);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }

        /// <summary>
        /// 提示开关
        /// </summary>
        /// <param name="open"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> TipsOpen(bool open)
        {
            MerchantSheetOperation merchantSheetOperation = new MerchantSheetOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var setup = await merchantSheetOperation.GetModelAsync(t => t.MerchantID == merchantID);
            setup.Remind = open;
            await merchantSheetOperation.UpdateModelAsync(setup);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }

        /// <summary>
        /// 获取飞单列表
        /// </summary>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="start"></param>
        /// <param name="pageSize"></param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Data
        ///         {
        ///             UserName：玩家名称
        ///             GameType：游戏名称
        ///             Nper：期号
        ///             Remark：下注内容
        ///             UseMoney：注单金额
        ///             Time：下注时间
        ///             Status：状态
        ///         }
        ///         Total：数量
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMerchantSheetInfos(DateTime startTime, DateTime endTime, int start = 1, int pageSize = 10)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            //查看是否已经登录
            var dicInfo = await Utils.GetMerchantFlySheetInfo(merchantID);
            if (dicInfo == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未登录到高级商户！"));
            MerchantInternalOperation merchantInternalOperation = new MerchantInternalOperation();
            var filter = merchantInternalOperation.Builder.Where(t => t.MerchantID == merchantID && t.TargetMerchantID == dicInfo["TargetID"].ToString() && t.TargetUserID == dicInfo["UserID"].ToString() && t.CreatedTime >= startTime && t.CreatedTime <= endTime);
            var list = merchantInternalOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var total = await merchantInternalOperation.GetCountAsync(filter);
            var result = new List<WebMerchantInternal>();
            foreach (var data in list)
            {
                result.Add(FlyingSheet.MerchantInternalTransformation(data));
            }
            return Ok(new RecoverListModel<WebMerchantInternal>()
            {
                Data = result,
                Message = "获取成功！",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        /// <summary>
        /// 获取外部飞单失败记录
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
        ///         Data
        ///         {
        ///             GameName：游戏名称
        ///             Nper：期号
        ///             UUID：uuid标识   要显示
        ///             Status：状态
        ///             Time：下注时间
        ///             Orders：
        ///             {
        ///                 content：下注内容
        ///                 money：使用金额
        ///             }
        ///         }
        ///         Total：数量
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetFlySheetFail(DateTime startTime, DateTime endTime, int start = 1, int pageSize = 10)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
            var filter = sendFlyingOperation.Builder.Where(t => t.MerchantID == merchantID && t.MerchantID == merchantID && t.CreatedTime >= startTime && t.CreatedTime <= endTime);
            var list = sendFlyingOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var total = await sendFlyingOperation.GetCountAsync(filter);
            var datas = (from data in list
                         select new
                         {
                             GameName = data.game,
                             Time = data.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss"),
                             Nper = data.IssueCode,
                             UUID = data.uuid,
                             Orders = data.orders,
                             Status = Enum.GetName(typeof(SendFlyEnum), data.Status)
                         }).ToList();
            var result = new List<dynamic>();
            result.AddRange(datas);
            return Ok(new RecoverListModel<dynamic>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = total
            });
        }
        #endregion

        #region 模拟发送消息 
        /// <summary>
        /// 获取所有虚拟用户
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllShamUser()
        {
            UserOperation userOperation = new UserOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var userList = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.假人 && t.IsSupport == true);
            var result = new List<dynamic>();
            foreach (var user in userList)
            {
                var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                var data = new
                {
                    NickName = nickName,
                    UserID = user._id
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
        /// 机器人发送消息
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="gameType"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ShamUserSendMessage(string userID, GameOfType gameType, string message)
        {
            UserOperation userOperation = new UserOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID && t.Status == UserStatusEnum.假人 && t.IsSupport == true);
            if (user == null)
                return Ok(new RecoverModel(RecoverEnum.失败, "未查询到该用户信息！"));
            #region 发送消息
            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            var gameStatus = RedisOperation.GetValue<Utils.GameNextLottery>("GameStatus", Enum.GetName(typeof(GameOfType), gameType));
            var nper = gameStatus.NextNper;
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
                        if (user.Status == UserStatusEnum.正常)
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
                        if (user.Status == UserStatusEnum.正常)
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
                            await RabbitMQHelper.SendAdminMessage(string.Format("@{0}{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, result.Message), merchantID, gameType);
                        }
                        return Ok(new RecoverModel(RecoverEnum.成功, "发送成功！"));
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
            #endregion
        }
        #endregion

        #region 域名
        /// <summary>
        /// 获取域名信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetDomainInfo()
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchant.AgentID);
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var data = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agent.HighestAgentID);
            return Ok(new RecoverClassModel<dynamic>()
            {
                Model = new
                {
                    data.H5DomainDescription,
                    merchant.H5DomainUrl,
                    merchant.SetupSeurityNo,
                    merchant.SetupRoomNum
                },
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 设置商户h5url地址
        /// </summary>
        /// <param name="url"></param>
        /// <param name="seurityNo"></param>
        /// <param name="roomNum"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateDomain(string url, string seurityNo, string roomNum)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            merchant.H5DomainUrl = url;
            merchant.SetupSeurityNo = seurityNo;
            merchant.SetupRoomNum = roomNum;
            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }
        #endregion
    }
}