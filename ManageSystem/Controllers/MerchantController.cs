using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using ManageSystem.Hubs;
using ManageSystem.Manipulate;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Agent;
using Operation.Baccarat;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.DrawingCore.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ManageSystem.Controllers
{
    /// <summary>
    /// 商户处理
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    [MerchantAuthentication]
    public class MerchantController : ControllerBase
    {
        #region
        private readonly IHubContext<ChatHub> context;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hub"></param>
        public MerchantController(IHubContext<ChatHub> hub)
        {
            context = hub;
        }
        #endregion
        #region 商户操作
        /// <summary>
        /// 商户登录
        /// </summary>
        /// <param name="name">商户名称</param>
        /// <param name="password">商户密码</param>
        /// <param name="code">验证码</param>
        /// <param name="type">登录类型 !:电脑 2:手机</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             MeName：商户名称
        ///             MerchantID：安全码
        ///             RoomNum：房间号
        ///             SeurityNo：安全码
        ///             Tips：登录时间超过一天
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        [NotMerchantAuthentication]
        public async Task<IActionResult> MerchantLogin(string name, string password, string code, int type = 1)
        {
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = merchantOperation.GetMerchantInfoByName(name);
            var session = RedisOperation.GetString(HttpContext.GetIP());
            if (string.IsNullOrEmpty(session)) return Ok(new RecoverModel(RecoverEnum.参数错误, "请重新获取验证码！"));
            if (session.ToLower() != code.ToLower())
                return Ok(new RecoverModel(RecoverEnum.参数错误, "验证码错误！"));
            if (merchant == null)
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到对应商户信息！"));
            #region 验证
            if (merchant.Password != Utils.MD5(password))
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "商户帐号或密码错误！"));

            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchant.AgentID);
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agent.HighestAgentID);
            if (merchant.MaturityTime <= DateTime.Now && setup.Formal)
                return Ok(new RecoverModel(RecoverEnum.身份过期, "房间已到期请及时续费！"));
            #endregion

            #region redis缓存
            var key = string.Empty;
            var token = Guid.NewGuid().ToString();
            var dic = new Dictionary<string, string>
            {
                { "MerchantName", merchant.MeName },
                { "MerchantID", merchant._id },
                { "SeurityNo", merchant.SeurityNo },
                { "MaturityTime", merchant.MaturityTime.ToString("yyyy-MM-dd HH:mm:ss") },
                { "Type", type.ToString() },
                { "AgentID", agent.HighestAgentID },
                { "Token", token}
            };
            if (type == 1)
            {
                key = merchant.MeName + merchant._id;
            }
            else
            {
                key = "app" + merchant.MeName + merchant._id;
            }
            key = key.Replace("-", "");
            RedisOperation.UpdateCacheKey(key, dic, 20);
            #endregion

            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchant._id);

            var tips = merchant.LoginTime == null ? false : (DateTime.Now - merchant.LoginTime.Value).TotalDays >= 1 ? true : false;
            merchant.LoginTime = DateTime.Now;
            merchant.OnLineTime = DateTime.Now;
            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverClassModel<dynamic>()
            {
                Status = RecoverEnum.成功,
                Message = "登录成功！",
                Model = new
                {
                    MerchantID = merchant._id,
                    MerchantName = merchant.MeName,
                    room.RoomNum,
                    merchant.SeurityNo,
                    Tips = tips,
                    Key = key,
                    Token = token,
                    Status = merchant.SecurityStatus,
                    merchant.MarsCurrency
                }
            });
        }

        /// <summary>
        /// app登录商户
        /// </summary>
        /// <param name="name">商户名称</param>
        /// <param name="password">商户密码</param>
        /// <param name="type">登录类型 1:电脑 2:手机</param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             MeName：商户名称
        ///             MerchantID：安全码
        ///             RoomNum：房间号
        ///             SeurityNo：安全码
        ///             Tips：登录时间超过一天
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        [NotMerchantAuthentication]
        public async Task<IActionResult> MerchantLoginApp(string name, string password, int type = 1)
        {
            MerchantOperation merchantOperation = new MerchantOperation();
            var merchant = merchantOperation.GetMerchantInfoByName(name);
            if (merchant == null)
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到对应商户信息！"));
            #region 
            if (merchant.Password != Utils.MD5(password))
                return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "商户帐号或密码错误！"));
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchant.AgentID);
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var setup = await advancedSetupOperation.GetModelAsync(t => t.AgentID == agent.HighestAgentID);
            if (merchant.MaturityTime <= DateTime.Now && setup.Formal)
                return Ok(new RecoverModel(RecoverEnum.身份过期, "房间已到期请及时续费！"));
            #endregion

            #region redis缓存
            var key = string.Empty;
            var token = Guid.NewGuid().ToString();
            var dic = new Dictionary<string, string>
            {
                { "MerchantName", merchant.MeName },
                { "MerchantID", merchant._id },
                { "SeurityNo", merchant.SeurityNo },
                { "MaturityTime", merchant.MaturityTime.ToString("yyyy-MM-dd HH:mm:ss") },
                { "Type", type.ToString() },
                { "AgentID", agent.HighestAgentID },
                { "Token", token}
            };
            if (type == 1)
            {
                key = merchant.MeName + merchant._id;
            }
            else
            {
                key = "app" + merchant.MeName + merchant._id;
            }
            key = key.Replace("-", "");
            RedisOperation.UpdateCacheKey(key, dic, 20);
            #endregion

            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchant._id);

            var tips = merchant.LoginTime == null ? false : (DateTime.Now - merchant.LoginTime.Value).Days >= 1 ? true : false;
            merchant.LoginTime = DateTime.Now;
            merchant.OnLineTime = DateTime.Now;
            await merchantOperation.UpdateModelAsync(merchant);

            return Ok(new RecoverClassModel<dynamic>()
            {
                Status = RecoverEnum.成功,
                Message = "登录成功！",
                Model = new
                {
                    MerchantID = merchant._id,
                    MerchantName = merchant.MeName,
                    room.RoomNum,
                    merchant.SeurityNo,
                    Tips = tips,
                    Key = key,
                    Token = token,
                    Status = merchant.SecurityStatus,
                    merchant.MarsCurrency
                }
            });
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [NotMerchantAuthentication]
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
        /// 商户退出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult MerchantExit()
        {
            var key = HttpContext.Request.Headers["MerchantAuthorization"].ToString();
            RedisOperation.DeleteKey(key);
            HttpContext.Session.Clear();
            return Ok(new RecoverModel(RecoverEnum.成功, "退出成功！"));
        }
        #endregion

        #region 基础设置
        /// <summary>
        /// 获取商户基础设置
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             ID：id
        ///             CustomMsg：自定义时间 
        ///             CustomTime：自定义消息
        ///             EntertainedAfterMsg：封盘后消息
        ///             EntertainedAfterTime：封盘后时间
        ///             EntertainedFrontMsg：封盘前消息
        ///             EntertainedFrontTime：封盘前时间
        ///             LotteryFrontTime：开奖前时间
        ///             {
        ///                 Type：游戏类型 1：赛车 2：飞艇 3：时时彩 4：极速 5：澳10  6：澳5  6种都要封装
        ///                 LotteryTime：时间
        ///             }
        ///             LotteryFrontMsg：开奖前消息
        ///             MembershipScore：会员积分
        ///             WinningDetails：中奖明细
        ///             ProhibitChe：禁止撤单
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSetupInfo()
        {
            FoundationSetupOperation foundationSetupOperation = new FoundationSetupOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = await foundationSetupOperation.GetFoundationByNoAsync(merchantID);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关基础设置！"));
            var result = new WebFoundationSetup()
            {
                ID = data._id,
                CustomMsg = data.CustomMsg,
                CustomTime = data.CustomTime,
                EntertainedAfterMsg = data.EntertainedAfterMsg,
                EntertainedAfterTime = data.EntertainedAfterTime,
                EntertainedFrontMsg = data.EntertainedFrontMsg,
                EntertainedFrontTime = data.EntertainedFrontTime,
                LotteryFrontMsg = data.LotteryFrontMsg,
                LotteryFrontTime = data.LotteryFrontTime,
                MembershipScore = data.MembershipScore,
                WinningDetails = data.WinningDetails,
                ProhibitChe = data.ProhibitChe,
                ShowBillTable = data.ShowBillTable,
                Settlement = data.Settlement,
                NotSettlement = data.NotSettlement
            };
            if (!result.LotteryFrontTime.Exists(t => t.Type == GameOfType.极速时时彩))
                result.LotteryFrontTime.Add(new LotteryItem()
                { 
                    LotteryTime = 20,
                    Type = GameOfType.极速时时彩
                });
            return Ok(new RecoverClassModel<WebFoundationSetup>() { Message = "查询成功！", Model = result, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 获取视讯基础设置
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             ID：id
        ///             CustomMsg：自定义时间 
        ///             CustomTime：自定义消息
        ///             EntertainedAfterMsg：封盘后消息
        ///             EntertainedAfterTime：封盘后时间
        ///             EntertainedFrontMsg：封盘前消息
        ///             EntertainedFrontTime：封盘前时间
        ///             LotteryFrontMsg：开奖前消息
        ///             MembershipScore：会员积分
        ///             WinningDetails：中奖明细
        ///             ProhibitChe：禁止撤单
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoSetupInfo()
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            VideoFoundationSetupOperation videoFoundationSetupOperation = new VideoFoundationSetupOperation();
            var data = await videoFoundationSetupOperation.GetFoundationByNoAsync(merchantID);
            var result = new WebFoundationSetup()
            {
                ID = data._id,
                CustomMsg = data.CustomMsg,
                CustomTime = data.CustomTime,
                EntertainedAfterMsg = data.EntertainedAfterMsg,
                EntertainedAfterTime = data.EntertainedAfterTime,
                EntertainedFrontMsg = data.EntertainedFrontMsg,
                EntertainedFrontTime = data.EntertainedFrontTime,
                LotteryFrontMsg = data.LotteryFrontMsg,
                MembershipScore = data.MembershipScore,
                WinningDetails = data.WinningDetails,
                ProhibitChe = data.ProhibitChe,
                ShowBillTable = data.ShowBillTable,
                Settlement = data.Settlement,
                NotSettlement = data.NotSettlement
            };
            return Ok(new RecoverClassModel<WebFoundationSetup>() { Message = "查询成功！", Model = result, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 修改基础设置
        /// </summary>
        /// <param name="data"></param>
        /// <remarks>
        ///##  参数说明
        ///             ID：id
        ///             CustomMsg：自定义时间 
        ///             CustomTime：自定义消息
        ///             EntertainedAfterMsg：封盘后消息
        ///             EntertainedAfterTime：封盘后时间
        ///             EntertainedFrontMsg：封盘前消息
        ///             EntertainedFrontTime：封盘前时间
        ///             LotteryFrontTime：开奖前时间
        ///             {
        ///                 Type：游戏类型 1：赛车 2：飞艇 3：时时彩 4：极速 5：澳10  6：澳5  6种都要封装
        ///                 LotteryTime：时间
        ///             }
        ///             LotteryFrontMsg：开奖前消息
        ///             MembershipScore：会员积分
        ///             WinningDetails：中奖明细
        ///             ProhibitChe：禁止撤单
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSetupInfo([FromBody] WebFoundationSetup data)
        {

            FoundationSetupOperation foundationSetupOperation = new FoundationSetupOperation();
            if (data == null) return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var model = await foundationSetupOperation.GetFoundationByIDAndNo(data.ID, merchantID);
            if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关基础设置！"));
            model.CustomMsg = data.CustomMsg;
            model.CustomTime = data.CustomTime; 
            model.EntertainedAfterMsg = data.EntertainedAfterMsg;
            model.EntertainedAfterTime = data.EntertainedAfterTime;
            model.EntertainedFrontMsg = data.EntertainedFrontMsg;
            model.EntertainedFrontTime = data.EntertainedFrontTime;
            model.LotteryFrontMsg = data.LotteryFrontMsg;
            model.LotteryFrontTime = data.LotteryFrontTime;
            model.MembershipScore = data.MembershipScore;
            model.WinningDetails = data.WinningDetails;
            model.ProhibitChe = data.ProhibitChe;
            model.ShowBillTable = data.ShowBillTable;
            model.Settlement = data.Settlement;
            model.NotSettlement = data.NotSettlement;
            await foundationSetupOperation.UpdateModelAsync(model);
            RedisOperation.SetFoundationSetup(merchantID, model);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 修改视讯基础设置
        /// </summary>
        /// <param name="data"></param>
        /// <remarks>
        ///##  参数说明
        ///             ID：id
        ///             CustomMsg：自定义时间 
        ///             CustomTime：自定义消息
        ///             EntertainedAfterMsg：封盘后消息
        ///             EntertainedAfterTime：封盘后时间
        ///             EntertainedFrontMsg：封盘前消息
        ///             EntertainedFrontTime：封盘前时间
        ///             LotteryFrontMsg：开奖前消息
        ///             MembershipScore：会员积分
        ///             WinningDetails：中奖明细
        ///             ProhibitChe：禁止撤单
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateVideoSetupInfo([FromBody] WebFoundationSetup data)
        {
            VideoFoundationSetupOperation videoFoundationSetupOperation = new VideoFoundationSetupOperation();
            if (data == null) return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var model = await videoFoundationSetupOperation.GetFoundationByNoAsync(merchantID);
            if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关基础设置！"));
            model.CustomMsg = data.CustomMsg;
            model.CustomTime = data.CustomTime;
            model.EntertainedAfterMsg = data.EntertainedAfterMsg;
            model.EntertainedAfterTime = data.EntertainedAfterTime;
            model.EntertainedFrontMsg = data.EntertainedFrontMsg;
            model.EntertainedFrontTime = data.EntertainedFrontTime;
            model.LotteryFrontMsg = data.LotteryFrontMsg;
            model.MembershipScore = data.MembershipScore;
            model.WinningDetails = data.WinningDetails;
            model.ProhibitChe = data.ProhibitChe;
            model.ShowBillTable = data.ShowBillTable;
            model.Settlement = data.Settlement;
            model.NotSettlement = data.NotSettlement;
            await videoFoundationSetupOperation.UpdateModelAsync(model);
            RedisOperation.SetVideoFoundationSetup(merchantID, model);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }
        #endregion

        #region 获取商户游戏限额
        /// <summary>
        /// 获取商户限额信息
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <remarks>
        ///##  参数说明
        ///     gameType：游戏类型
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             ID：id 
        ///             GuessNum：数字
        ///             GuessDxds：大小单双
        ///             GuessLongHu：龙虎
        ///             GuessGYHDxds：冠亚和大小单双
        ///             GuessGYHNum：冠亚和数字
        ///             GuessBanshun：半顺
        ///             GuessBaozi：豹子
        ///             GuessHe：和
        ///             GuessShunzi：顺子
        ///             GuessZaliu：杂六
        ///             {
        ///                 MinBet:最小投注限额
        ///                 MaxBet:最大投注限额
        ///                 AllMaxBet：所有人最大投注
        ///                 SingleBet：网盘单手限额
        ///             }
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBetLimitInfo(GameOfType gameType)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (!Utils.GameTypeItemize(gameType))
            {
                BetLimitSpecialOperation betLimitSpecialOperation = new BetLimitSpecialOperation();
                var model = await betLimitSpecialOperation.GetModelAsync(merchantID, gameType);
                if (model == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到限额数据！"));
                else
                {
                    var result = BetLimitSpecialToWeb(model);
                    return Ok(new RecoverClassModel<WebBetLimitSpecial>()
                    {
                        Message = "查询成功！",
                        Model = result,
                        Status = RecoverEnum.成功
                    });
                }
            }
            else
            {
                BetLimitOrdinaryOperation betLimitOrdinaryOperation = new BetLimitOrdinaryOperation();
                var model = await betLimitOrdinaryOperation.GetModelAsync(merchantID, gameType);
                if (model == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到限额数据！"));
                else
                {
                    var result = BetLimitOrdinaryToWeb(model);
                    return Ok(new RecoverClassModel<WebBetLimitOrdinary>() { Message = "查询成功！", Model = result, Status = RecoverEnum.成功 });
                }
            }
        }
        private WebBetLimitOrdinary BetLimitOrdinaryToWeb(BetLimitOrdinary data)
        {
            var result = new WebBetLimitOrdinary();
            var resultType = result.GetType();
            var replyType = data.GetType();
            result.ID = data._id;
            foreach (var propertie in replyType.GetProperties())
            {
                var property = resultType.GetProperty(propertie.Name);
                if (property == null) continue;
                resultType.GetProperty(propertie.Name).SetValue(result, propertie.GetValue(data));
            }
            return result;
        }

        private WebBetLimitSpecial BetLimitSpecialToWeb(BetLimitSpecial data)
        {
            var result = new WebBetLimitSpecial();
            var resultType = result.GetType();
            var replyType = data.GetType();
            result.ID = data._id;
            foreach (var propertie in replyType.GetProperties())
            {
                var property = resultType.GetProperty(propertie.Name);
                if (property == null) continue;
                resultType.GetProperty(propertie.Name).SetValue(result, propertie.GetValue(data));
            }
            return result;
        }

        /// <summary>
        /// 获取视讯游戏限额
        /// </summary>
        /// <param name="gameType">游戏类型 1：百家乐 2：牛牛 3：龙虎</param>
        /// <remarks>
        ///##  参数说明
        ///     gameType：游戏类型 1：百家乐 2：牛牛 3：龙虎
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             ID：id
        ///             AllTotalQuotas：所有玩法上限 
        ///             GuessAPPair：任意/完美对子
        ///             GuessBPPair：庄/闲对子
        ///             GuessHe：和
        ///             GuessQueue：庄/闲/大/小
        ///             TotalSingleLimit：单个玩家限额
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoBetLimitInfo(BaccaratGameType gameType)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (gameType == BaccaratGameType.百家乐)
            {
                BetLimitBaccaratOperation betLimitBaccaratOperation = new BetLimitBaccaratOperation();
                var data = await betLimitBaccaratOperation.GetModelAsync(t => t.MerchantID == merchantID);
                if (data == null)
                {
                    data = new BetLimitBaccarat()
                    {
                        MerchantID = merchantID
                    };
                    await betLimitBaccaratOperation.InsertModelAsync(data);
                }
                var result = new WebVideoBetLimit()
                {
                    ID = data._id,
                    AllTotalQuotas = data.AllTotalQuotas,
                    GuessAPPair = data.GuessAPPair,
                    GuessBPPair = data.GuessBPPair,
                    GuessHe = data.GuessHe,
                    GuessQueue = data.GuessQueue,
                    TotalSingleLimit = data.TotalSingleLimit
                };
                return Ok(new RecoverClassModel<WebVideoBetLimit>()
                {
                    Message = "获取成功！",
                    Model = result,
                    Status = RecoverEnum.成功
                });
            }
            else
                return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
        }
        #endregion

        #region 修改限额
        /// <summary>
        /// 修改限额
        /// </summary>
        /// <param name="data">对应限额json数据</param>
        /// <param name="id">对应id</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateBetLimitInfo(dynamic data, string id, GameOfType gameType)
        {
            var str = data.ToString().Replace("\r\n", "").Replace("\\", "");

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (!Utils.GameTypeItemize(gameType))
            {
                var betLimitSpecialOperation = new BetLimitSpecialOperation();
                var model = await betLimitSpecialOperation.GetModelAsync(merchantID, gameType);
                if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, string.Format("未查询到{0}限额数据！", Enum.GetName(typeof(GameOfType), (int)gameType))));
                var record = JsonConvert.DeserializeObject<WebBetLimitSpecial>(data.ToString());
                var preMsg = GetBetLimitInfo(model);
                BetWebToLimitSpecial(record, ref model);
                await betLimitSpecialOperation.UpdateModelAsync(model);
                SensitiveOperation sensitiveOperation = new SensitiveOperation();
                var type = gameType == GameOfType.重庆时时彩 ? OpLocationEnum.时时彩限额操作 :
                    gameType == GameOfType.澳州5 ? OpLocationEnum.澳5限额操作 :
                    gameType == GameOfType.爱尔兰快5 ? OpLocationEnum.爱尔兰快5限额操作
                    : OpLocationEnum.极速时时彩限额操作;
                var sensitive = new Sensitive()
                {
                    MerchantID = merchantID,
                    OpLocation = type,
                    OpType = OpTypeEnum.修改,
                    OpAcontent = GetBetLimitInfo(model),
                    OpBcontent = preMsg
                };
                await sensitiveOperation.InsertModelAsync(sensitive);
                return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
            }
            else
            {
                BetLimitOrdinaryOperation betLimitOrdinaryOperation = new BetLimitOrdinaryOperation();
                var model = await betLimitOrdinaryOperation.GetModelAsync(merchantID, gameType);
                if (model == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, string.Format("未查询到{0}限额数据！", Enum.GetName(typeof(GameOfType), (int)gameType))));
                var record = JsonConvert.DeserializeObject<WebBetLimitOrdinary>(data.ToString());
                var preMsg = GetBetLimitInfo(model);
                BetWebToLimitOrdinary(record, ref model);
                await betLimitOrdinaryOperation.UpdateModelAsync(model);

                SensitiveOperation sensitiveOperation = new SensitiveOperation();
                var type = gameType == GameOfType.北京赛车 ? OpLocationEnum.赛车限额操作 :
                    gameType == GameOfType.幸运飞艇 ? OpLocationEnum.飞艇限额操作 :
                    gameType == GameOfType.极速赛车 ? OpLocationEnum.极速限额操作 :
                    gameType == GameOfType.澳州10 ? OpLocationEnum.澳10限额操作 :
                    gameType == GameOfType.爱尔兰赛马 ? OpLocationEnum.爱尔兰赛马限额操作
                    : OpLocationEnum.幸运飞艇168限额操作;
                var sensitive = new Sensitive()
                {
                    MerchantID = merchantID,
                    OpLocation = type,
                    OpType = OpTypeEnum.修改,
                    OpAcontent = GetBetLimitInfo(model),
                    OpBcontent = preMsg
                };
                await sensitiveOperation.InsertModelAsync(sensitive);

                return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
            }
        }

        private void BetWebToLimitOrdinary(WebBetLimitOrdinary data, ref BetLimitOrdinary result)
        {
            var resultType = result.GetType();
            var replyType = data.GetType();
            foreach (var propertie in replyType.GetProperties())
            {
                if (propertie.Name == "ID") continue;
                resultType.GetProperty(propertie.Name).SetValue(result, propertie.GetValue(data));
            }
        }

        private void BetWebToLimitSpecial(WebBetLimitSpecial data, ref BetLimitSpecial result)
        {
            var resultType = result.GetType();
            var replyType = data.GetType();
            foreach (var propertie in replyType.GetProperties())
            {
                if (propertie.Name == "ID") continue;
                resultType.GetProperty(propertie.Name).SetValue(result, propertie.GetValue(data));
            }
        }

        /// <summary>
        /// 修改视讯限额
        /// </summary>
        /// <param name="model"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateVideoBetLimitInfo([FromBody]WebVideoBetLimit model, BaccaratGameType gameType)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (gameType == BaccaratGameType.百家乐)
            {
                BetLimitBaccaratOperation betLimitBaccaratOperation = new BetLimitBaccaratOperation();
                var data = await betLimitBaccaratOperation.GetModelAsync(t => t.MerchantID == merchantID);
                if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, string.Format("未查询到{0}限额数据！", Enum.GetName(typeof(BaccaratGameType), (int)gameType))));
                #region 日志
                Sensitive sensitive = new Sensitive()
                {
                    MerchantID = merchantID,
                    OpType = OpTypeEnum.修改,
                    OpLocation = OpLocationEnum.百家乐限额操作,
                    OpAcontent = GetVideoBetLimitInfo(data),
                    OpBcontent = GetVideoBetLimitInfo(model)
                };
                #endregion
                SensitiveOperation sensitiveOperation = new SensitiveOperation();
                data.AllTotalQuotas = model.AllTotalQuotas;
                data.GuessAPPair = model.GuessAPPair;
                data.GuessBPPair = model.GuessBPPair;
                data.GuessHe = model.GuessHe;
                data.GuessQueue = model.GuessQueue;
                data.TotalSingleLimit = model.TotalSingleLimit;
                await betLimitBaccaratOperation.UpdateModelAsync(data);
                await sensitiveOperation.InsertModelAsync(sensitive);
                return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
            }
            else
                return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
        }

        private string GetBetLimitInfo<T>(T data) where T : BetLimit
        {
            StringBuilder sb = new StringBuilder();
            PropertyInfo[] peroperties = data.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

            sb.Append(string.Format("单个玩家所有玩法总限额：{0}\r\n", data.TotalSingleLimit));
            sb.Append(string.Format("所有玩家所有玩法总限额：{0}\r\n", data.AllTotalQuotas));

            foreach (PropertyInfo property in peroperties)
            {
                object[] objs = property.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (objs.Length > 0)
                {
                    var info = (QuotaAttrInfo)property.GetValue(data);
                    sb.AppendFormat("{0}:最小投注限额：{1}，个人最大投注：{2}，所有人最大投注：{3}\r\n", ((DescriptionAttribute)objs[0]).Description, info.MinBet, info.MaxBet, info.AllMaxBet);
                }
            }
            return sb.ToString();
        }

        private string GetVideoBetLimitInfo(WebVideoBetLimit model)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("单个玩家所有玩法总限额:{0}\r\n", model.TotalSingleLimit);
            builder.AppendFormat("所有玩家所有玩法总限额:{0}\r\n", model.AllTotalQuotas);

            //庄闲大小
            var zxds = model.GuessQueue;
            builder.AppendFormat(@"庄/闲/大/小:
最小投注限额：{0}，个人最大投注：{1}，所有人最大投注：{2}\r\n", zxds.MinBet, zxds.MaxBet, zxds.AllMaxBet);
            //庄闲对
            var bp = model.GuessBPPair;
            builder.AppendFormat(@"庄对/闲对:
最小投注限额：{0}，个人最大投注：{1}，所有人最大投注：{2}\r\n", bp.MinBet, bp.MaxBet, bp.AllMaxBet);
            //任/完对子
            var rw = model.GuessAPPair;
            builder.AppendFormat(@"任意对子/完美对子:
最小投注限额：{0}，个人最大投注：{1}，所有人最大投注：{2}\r\n", rw.MinBet, rw.MaxBet, rw.AllMaxBet);
            //和
            var he = model.GuessHe;
            builder.AppendFormat(@"和:
最小投注限额：{0}，个人最大投注：{1}，所有人最大投注：{2}\r\n", he.MinBet, he.MaxBet, he.AllMaxBet);
            return builder.ToString();
        }

        private string GetVideoBetLimitInfo(BetLimitBaccarat model)
        {
            StringBuilder builder = new StringBuilder();
            builder.AppendFormat("单个玩家所有玩法总限额:{0}\r\n", model.TotalSingleLimit);
            builder.AppendFormat("所有玩家所有玩法总限额:{0}\r\n", model.AllTotalQuotas);

            //庄闲大小
            var zxds = model.GuessQueue;
            builder.AppendFormat(@"庄/闲/大/小:
最小投注限额：{0}，个人最大投注：{1}，所有人最大投注：{2}\r\n", zxds.MinBet, zxds.MaxBet, zxds.AllMaxBet);
            //庄闲对
            var bp = model.GuessBPPair;
            builder.AppendFormat(@"庄对/闲对:
最小投注限额：{0}，个人最大投注：{1}，所有人最大投注：{2}\r\n", bp.MinBet, bp.MaxBet, bp.AllMaxBet);
            //任/完对子
            var rw = model.GuessAPPair;
            builder.AppendFormat(@"任意对子/完美对子:
最小投注限额：{0}，个人最大投注：{1}，所有人最大投注：{2}\r\n", rw.MinBet, rw.MaxBet, rw.AllMaxBet);
            //和
            var he = model.GuessHe;
            builder.AppendFormat(@"和:
最小投注限额：{0}，个人最大投注：{1}，所有人最大投注：{2}\r\n", he.MinBet, he.MaxBet, he.AllMaxBet);
            return builder.ToString();
        }
        #endregion

        #region 获取商户游戏赔率
        /// <summary>
        /// 获取商户游戏赔率
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetOddsInfo(GameOfType gameType)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (!Utils.GameTypeItemize(gameType))
            {
                var oddsSpecialOperation = new OddsSpecialOperation();
                var model = await oddsSpecialOperation.GetModelAsync(merchantID, gameType);
                if (model == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到赔率数据！"));
                else
                {
                    var result = Utils.GetWebOddsData<BaseModel, WebOddsSpecial>(model);
                    return Ok(new RecoverClassModel<WebOddsSpecial>() { Message = "查询成功！", Model = result, Status = RecoverEnum.成功 });
                }
            }
            else
            {
                var oddsOrdinaryOperation = new OddsOrdinaryOperation();
                var model = await oddsOrdinaryOperation.GetModelAsync(merchantID, gameType);
                if (model == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到赔率数据！"));
                else
                {
                    var result = Utils.GetWebOddsData<BaseModel, WebOddsOrdinary>(model);
                    return Ok(new RecoverClassModel<WebOddsOrdinary>() { Message = "查询成功！", Model = result, Status = RecoverEnum.成功 });
                }
            }
        }

        /// <summary>
        /// 获取视讯游戏赔率
        /// </summary>
        /// <param name="gameType">游戏类型 1：百家乐 2：牛牛 3：龙虎</param>
        /// <remarks>
        ///##  参数说明
        ///     gameType：游戏类型 1：百家乐 2：牛牛 3：龙虎
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             ID：id
        ///             AnyPair：任意对子 
        ///             Banker：庄家
        ///             BankerPair：庄对
        ///             Da：大
        ///             He：和
        ///             PerfectPair：完美对子 
        ///             Player：闲家
        ///             PlayerPair：闲对
        ///             Xiao：小
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoOddsInfo(BaccaratGameType gameType)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (gameType == BaccaratGameType.百家乐)
            {
                OddsBaccaratOperation oddsBaccaratOperation = new OddsBaccaratOperation();
                var data = await oddsBaccaratOperation.GetModelAsync(t => t.MerchantID == merchantID);
                if (data == null)
                {
                    data = new OddsBaccarat()
                    {
                        MerchantID = merchantID
                    };
                    await oddsBaccaratOperation.InsertModelAsync(data);
                }
                var result = new WebOddsBaccarat()
                {
                    ID = data._id,
                    AnyPair = data.AnyPair,
                    Banker = data.Banker,
                    BankerPair = data.BankerPair,
                    Da = data.Da,
                    He = data.He,
                    PerfectPair = data.PerfectPair,
                    Player = data.Player,
                    PlayerPair = data.PlayerPair,
                    Xiao = data.Xiao
                };
                return Ok(new RecoverClassModel<WebOddsBaccarat>()
                {
                    Message = "获取成功！",
                    Model = result,
                    Status = RecoverEnum.成功
                });
            }
            return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
        }
        #endregion

        #region 修改赔率
        /// <summary>
        /// 修改赔率
        /// </summary>
        /// <param name="data">对应限额json数据</param>
        /// <param name="id">对应id</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateOddsInfo(dynamic data, string id, GameOfType gameType)
        {
            var str = data.ToString();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (!Utils.GameTypeItemize(gameType))
            {
                var oddsSpecialOperation = new OddsSpecialOperation();
                var model = await oddsSpecialOperation.GetModelAsync(merchantID, gameType);
                var type = gameType == GameOfType.重庆时时彩 ? OpLocationEnum.时时彩赔率操作 :
                    gameType == GameOfType.澳州5 ? OpLocationEnum.澳5赔率操作 :
                    gameType == GameOfType.爱尔兰快5? OpLocationEnum.爱尔兰快5赔率操作
                    : OpLocationEnum.极速时时彩赔率操作;
                Sensitive sensitive = new Sensitive()
                {
                    OpLocation = type
                };
                var result = UnifiedPutOdds<OddsSpecial, WebOddsSpecial>(ref model, str, merchantID, sensitive);
                await oddsSpecialOperation.UpdateModelAsync(model);
                return result;
            }
            else
            {
                var oddsOrdinaryOperation = new OddsOrdinaryOperation();
                var model = await oddsOrdinaryOperation.GetModelAsync(merchantID, gameType);
                var type = gameType == GameOfType.北京赛车 ? OpLocationEnum.赛车赔率操作 :
                    gameType == GameOfType.幸运飞艇 ? OpLocationEnum.飞艇赔率操作 :
                    gameType == GameOfType.极速赛车 ? OpLocationEnum.极速赔率操作 :
                    gameType == GameOfType.澳州10 ? OpLocationEnum.澳10赔率操作 :
                    gameType == GameOfType.爱尔兰赛马 ? OpLocationEnum.爱尔兰赛马赔率操作
                    : OpLocationEnum.幸运飞艇168赔率操作;
                Sensitive sensitive = new Sensitive()
                {
                    OpLocation = type
                };
                var result = UnifiedPutOdds<OddsOrdinary, WebOddsOrdinary>(ref model, str, merchantID, sensitive);
                await oddsOrdinaryOperation.UpdateModelAsync(model);
                return result;
            }
        }

        /// <summary>
        /// 修改视讯赔率
        /// </summary>
        /// <param name="model"></param>
        /// <param name="id">id</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateVideoOddsInfo([FromBody] WebOddsBaccarat model, string id, BaccaratGameType gameType)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            if (gameType == BaccaratGameType.百家乐)
            {
                OddsBaccaratOperation oddsBaccaratOperation = new OddsBaccaratOperation();
                var data = await oddsBaccaratOperation.GetModelAsync(t => t.MerchantID == merchantID && t._id == id);
                if (data == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到赔率信息！"));
                Sensitive sensitive = new Sensitive()
                {
                    MerchantID = merchantID,
                    OpType = OpTypeEnum.修改,
                    OpLocation = OpLocationEnum.百家乐赔率操作,
                    OpAcontent = string.Format("庄：{0}\r\n闲：{1}\r\n和：{2}\r\n大：{3}\r\n小：{4}\r\n庄对：{5}\r\n闲对：{6}\r\n任意对子：{7}\r\n完美对子：{8}",
                    model.Banker, model.Player, model.He, model.Da, model.Xiao, model.BankerPair, model.PlayerPair, model.AnyPair, model.PerfectPair),
                    OpBcontent = string.Format("庄：{0}\r\n闲：{1}\r\n和：{2}\r\n大：{3}\r\n小：{4}\r\n庄对：{5}\r\n闲对：{6}\r\n任意对子：{7}\r\n完美对子：{8}",
                    data.Banker, data.Player, data.He, data.Da, data.Xiao, data.BankerPair, data.PlayerPair, data.AnyPair, data.PerfectPair)
                };
                SensitiveOperation sensitiveOperation = new SensitiveOperation();
                data.Banker = model.Banker;
                data.Player = model.Player;
                data.He = model.He;
                data.BankerPair = model.BankerPair;
                data.PlayerPair = model.PlayerPair;
                data.Da = model.Da;
                data.Xiao = model.Xiao;
                data.AnyPair = model.AnyPair;
                data.PerfectPair = model.PerfectPair;
                await oddsBaccaratOperation.UpdateModelAsync(data);
                await sensitiveOperation.InsertModelAsync(sensitive);
                return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
            }
            else
                return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
        }

        private IActionResult UnifiedPutOdds<T1, T2>(ref T1 model, string data, string merchantID, Sensitive sensitive) where T1 : Odds where T2 : WebOdds
        {
            if (model == null) throw new Exception("未查询到相关赔率数据！");
            var record = JsonConvert.DeserializeObject<T2>(data.ToString());
            sensitive.MerchantID = merchantID;
            sensitive.OpType = OpTypeEnum.修改;
            sensitive.OpBcontent = Utils.GetEntityContent(model);
            Utils.GetOddsData(ref model, record);
            sensitive.OpAcontent = Utils.GetEntityContent(model);
            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            sensitiveOperation.InsertModel(sensitive);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }
        #endregion

        #region 首页上下分
        /// <summary>
        /// 获取用户申请上下分
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Data
        ///         {
        ///             ID：id 
        ///             Amount：金额
        ///             ApplyTime：申请时间
        ///             Avatar：头像
        ///             ChangeType：上下分 1：上分 2：下分
        ///             LoginName：登录名称
        ///             Management：状态  1：已同意 2：已拒绝 3：未审批
        ///             Message：上分/下分
        ///             Status：用户状态
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUserApply()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            var list = userIntegrationOperation.GetModelList(t => t.MerchantID == merchantID
            && (t.OrderStatus == OrderStatusEnum.申请上分 || t.OrderStatus == OrderStatusEnum.申请下分)
            && t.ChangeTarget == ChangeTargetEnum.申请 && t.Management == ManagementEnum.未审批 && t.CreatedTime > DateTime.Today && t.CreatedTime < DateTime.Today.AddDays(1),
            t => t.CreatedTime, false);
            UserOperation userOperation = new UserOperation();
            var result = new List<WebUserIntegrationHome>();
            foreach (var data in list)
            {
                var user = await userOperation.GetModelAsync(t => t._id == data.UserID && t.MerchantID == merchantID);
                var name = string.IsNullOrEmpty(user.MemoName) ? user.LoginName : user.MemoName;
                WebUserIntegrationHome home = new WebUserIntegrationHome()
                {
                    ID = data._id,
                    Amount = (data.ChangeType == ChangeTypeEnum.上分 ? "" : "-") + data.Amount.ToString(),
                    ApplyTime = data.CreatedTime.ToString("MM-dd HH:mm"),
                    Avatar = user.Avatar,
                    ChangeType = data.ChangeType,
                    LoginName = user.NickName + "(" + name + ")",
                    Management = data.Management,
                    Message = data.Message,
                    OnlyCode = user.OnlyCode,
                    UserMoney = user.UserMoney,
                    Status = Enum.GetName(typeof(NotesEnum), (int)data.Notes)
                };
                result.Add(home);
            }
            return Ok(new RecoverListModel<WebUserIntegrationHome>()
            {
                Data = result,
                Message = "获取成功！",
                Status = RecoverEnum.成功,
                Total = result.Count
            });
        }

        /// <summary>
        /// 获取今日总上下分
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetTotalScore()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            var upList = await userIntegrationOperation.GetModelListAsync(t => t.MerchantID == merchantID &&
            t.ChangeTarget == ChangeTargetEnum.申请 && t.OrderStatus == OrderStatusEnum.申请上分
            && t.CreatedTime > DateTime.Today && t.CreatedTime < DateTime.Today.AddDays(1)
            && t.ChangeType == ChangeTypeEnum.上分 && t.Notes == NotesEnum.正常 && t.Management == ManagementEnum.已同意);

            var lowList = await userIntegrationOperation.GetModelListAsync(t => t.MerchantID == merchantID &&
            t.ChangeTarget == ChangeTargetEnum.申请 && t.OrderStatus == OrderStatusEnum.申请下分
            && t.CreatedTime > DateTime.Today && t.CreatedTime < DateTime.Today.AddDays(1)
            && t.ChangeType == ChangeTypeEnum.下分 && t.Notes == NotesEnum.正常 && t.Management == ManagementEnum.已同意);

            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功！",
                Model = new { Up = upList.Sum(t => t.Amount), Low = lowList.Sum(t => t.Amount) },
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 同意操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AgreeOperate(string id)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            var data = await userIntegrationOperation.GetModelAsync(t => t._id == id && t.Management == ManagementEnum.未审批);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到申请信息！"));
            if (data.Management != ManagementEnum.未审批)
                return Ok(new RecoverModel(RecoverEnum.失败, "数据错误！"));
            data.Management = ManagementEnum.已同意;
            data.Message = Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType) + "成功";
            data.Remark = Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType) + "成功";
            //修改用户积分
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == data.UserID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到用户信息！"));
            if (data.ChangeType == ChangeTypeEnum.上分)
                user.UserMoney += data.Amount;
            //else
            //{
            //    if (user.UserMoney < data.Amount)
            //    {
            //        if (data.GameType != null)
            //            await RabbitMQHelper.SendAdminMessage(string.Format("用户{0}积分不足，下分失败", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName), merchantID, data.GameType.Value);
            //        //await SignalRSendMessage.SendAdminMessage(string.Format("用户{0}积分不足，下分失败",user.NickName), merchantID, data.GameType.Value, context);
            //        data.Management = ManagementEnum.已拒绝;
            //        data.Message = Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType) + "失败（用户积分不足）";
            //        data.Remark = Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType) + "失败（用户积分不足）";
            //        await userIntegrationOperation.UpdateModelAsync(data);
            //        return Ok(new RecoverModel(RecoverEnum.失败, "用户积分不足！"));
            //    }
            //    user.UserMoney -= data.Amount;
            //};
            data.Balance = user.UserMoney;
            await userIntegrationOperation.UpdateModelAsync(data);
            await userOperation.UpdateModelAsync(user);
            ReplySetUpOperation replySetUpOperation = new ReplySetUpOperation();
            var reply = await replySetUpOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (data.GameType != null || data.BaccaratGameType != null)
            {
                var amount = data.ChangeType == ChangeTypeEnum.上分 ? data.Amount : -data.Amount;
                string message = reply.Remainder.Replace("{昵称}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName)
                    .Replace("{变动分数}", amount.ToString("#0.00"))
                    .Replace("{剩余分数}", user.UserMoney.ToString("#0.00"));
                if (data.GameType != null) await RabbitMQHelper.SendAdminMessage(message, merchantID, data.GameType.Value);
                else if (data.BaccaratGameType != null) await RabbitMQHelper.SendBaccaratAdminMessage(message, merchantID, data.Znum.Value, data.BaccaratGameType.Value);
            }
            await RabbitMQHelper.SendUserPointChange(data.UserID, merchantID);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }

        /// <summary>
        /// 拒绝操作
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RefuseOperate(string id)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            var data = await userIntegrationOperation.GetModelAsync(t => t._id == id && t.MerchantID == merchantID);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到申请信息！"));
            if (data.Management != ManagementEnum.未审批)
                return Ok(new RecoverModel(RecoverEnum.失败, "数据错误！"));
            data.Management = ManagementEnum.已拒绝;
            data.Message = Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType) + "失败（手动拒绝）";
            data.Remark = Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType) + "失败（手动拒绝）";
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == data.UserID && t.MerchantID == merchantID);
            if (user == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到用户信息！"));
            if (data.ChangeType == ChangeTypeEnum.下分)
            {
                await userOperation.UpperScore(user._id, merchantID, data.Amount, ChangeTargetEnum.系统, "下分被拒退回", "下分被拒退回", orderStatus: OrderStatusEnum.上分成功);
            }
            data.Balance = user.UserMoney;
            await userIntegrationOperation.UpdateModelAsync(data);
            string message = string.Format("用户{0}{1}{2}被拒绝！", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, Enum.GetName(typeof(ChangeTypeEnum), (int)data.ChangeType),
               data.Amount);
            await RabbitMQHelper.SendAdminMessage(message, merchantID, data.GameType.Value);
            await RabbitMQHelper.SendUserPointChange(data.UserID, merchantID);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }
        #endregion

        #region 其它信息
        /// <summary>
        /// 获取页头商户信息等
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMerchantInfos()
        {
            var merchantOperation = new MerchantOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            var userList = await new UserOperation().GetModelListAsync(t => t.MerchantID == merchantID
            && t.Status != UserStatusEnum.删除 && t.Status != UserStatusEnum.假人);
            var allAmount = userList.Sum(t => t.UserMoney);
            var address = await Utils.GetAddress(merchantID);
            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);
            var startTime = DateTime.Now > DateTime.Today.AddHours(6) ? DateTime.Today.AddHours(6) : DateTime.Today.AddHours(6).AddDays(-1);
            var endTime = startTime.AddDays(1);
            var allBetList = await collection.FindListAsync(t => t.MerchantID == merchantID && t.Notes == NotesEnum.正常
            && t.CreatedTime >= startTime && t.CreatedTime <= endTime && t.BetStatus == BetStatus.已开奖);
            var proLoss = allBetList.Sum(t => t.BetRemarks.Sum(x => x.OddBets.Sum(y => y.BetMoney))) - allBetList.Sum(t => t.BetRemarks.Sum(x => x.OddBets.Sum(y => y.MediumBonus)));
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功！",
                Model = new
                {
                    AllAmount = allAmount,
                    ProLoss = proLoss,
                    MaturityTime = merchant.MaturityTime.ToString("yyyy-MM-dd HH:mm:ss")
                },
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取总人数和在线人数
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAndOnline()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            BsonOperation bsonOperation = new BsonOperation("SignalR");
            FilterDefinition<BsonDocument> filter = bsonOperation.Builder.Eq("MerchantID", merchantID);
            var list = (await bsonOperation.Collection.FindAsync(filter)).ToList();
            var idList = new List<string>();
            foreach (var data in list)
            {
                idList.Add(data["UserID"].ToString());
            }
            idList = idList.Distinct().ToList();

            UserOperation userOperation = new UserOperation();
            //总人数
            var allCount = await userOperation.GetCountAsync(t => t.MerchantID == merchantID && t.Status == UserStatusEnum.正常);

            //var shamList = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID
            //&& t.Status == UserStatusEnum.假人);

            ////查看启用假人数量
            //ShamRobotOperation shamRobotOperation = new ShamRobotOperation();
            //var shamCount = await shamRobotOperation.GetCountAsync(shamRobotOperation.Builder.Where(t => t.MerchantID == merchantID && t.Check == true)
            //    & shamRobotOperation.Builder.In(t => t.UserID, shamList.Select(t => t._id).ToList()));

            var result = new
            {
                AllCount = (int)allCount,
                OnlineCount = idList.Count
            };
            return Ok(new RecoverClassModel<dynamic>()
            {
                Message = "获取成功",
                Model = result,
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取今日未结注单
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetUnfinished()
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
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
            var betList = await collection.FindListAsync(t => t.MerchantID == merchantID && t.Notes == NotesEnum.正常 && t.BetStatus == BetStatus.未开奖
            && t.CreatedTime >= startTime && t.CreatedTime <= endTime);
            var count = betList.Sum(t => t.BetRemarks.Sum(x => x.OddBets.Sum(y => y.BetMoney))).ToString("#0.00");
            return Ok(new RecoverKeywordModel()
            {
                Keyword = count,
                Status = RecoverEnum.成功,
                Message = "获取成功"
            });
        }

        /// <summary>
        /// 获取系统配置
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetConfiguration()
        {
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        GameApp = "",
                        BackApp = "",
                        H5Url = ""
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        GameApp = model.MerchantAppUrl,
                        BackApp = model.MerchantMUrl,
                        H5Url = model.Subscription
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
        }

        /// <summary>
        /// 获取销售后台滚动公告
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAgentNotice()
        {
            var merchantOperation = new MerchantOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            AgentUserOperation agentUserOperation = new AgentUserOperation();
            var agent = await agentUserOperation.GetModelAsync(t => t._id == merchant.AgentID);
            if (agent == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息！"));
            var upagent = await agentUserOperation.GetModelAsync(t => t._id == agent.HighestAgentID);
            if (upagent == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理信息！"));
            //查询高级代理设置
            AdvancedSetupOperation advancedSetupOperation = new AdvancedSetupOperation();
            var result = await advancedSetupOperation.GetModelAsync(t => t.AgentID == upagent._id);
            if (result == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到代理设置！"));
            return Ok(new RecoverKeywordModel()
            {
                Keyword = result.MerchantNotice,
                Status = RecoverEnum.成功,
                Message = "获取成功"
            });
        }

        /// <summary>
        /// 获取游戏信息
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult GetGameList()
        {
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
        /// 实时刷新商户状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> OnlineSupport()
        {
            var merchantOperation = new MerchantOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            merchant.OnLineTime = DateTime.Now;
            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverModel(RecoverEnum.成功, "刷新成功！"));
        }

        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="oldPwd">旧密码</param>
        /// <param name="newPwd">新密码</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdatePassword(string oldPwd, string newPwd)
        {
            if (oldPwd == newPwd)
                return Ok(new RecoverModel(RecoverEnum.失败, "新密码不能与原密码相同！"));
            var merchantOperation = new MerchantOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            if (merchant.Password != Utils.MD5(oldPwd))
                return Ok(new RecoverModel(RecoverEnum.失败, "原密码错误！"));

            merchant.Password = Utils.MD5(newPwd);

            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }

        /// <summary>
        /// 设置安全密码开关
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSecurityStatus(bool status)
        {
            var merchantOperation = new MerchantOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            if (string.IsNullOrEmpty(merchant.SecurityPwd))
                return Ok(new RecoverModel(RecoverEnum.失败, "未设置安全密码，不能修改！"));
            merchant.SecurityStatus = status;
            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }

        /// <summary>
        /// 设置安全密码
        /// </summary>
        /// <param name="newPwd">新密码</param>
        /// <param name="oldPwd">旧密码   第一次密码不需要填写</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSecurityPwd(string newPwd, string oldPwd)
        {
            if (string.IsNullOrEmpty(newPwd))
                return Ok(new RecoverModel(RecoverEnum.参数错误, "请输入预修改安全码！"));
            var merchantOperation = new MerchantOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            if (!string.IsNullOrEmpty(merchant.SecurityPwd))
            {
                if (string.IsNullOrEmpty(oldPwd))
                    return Ok(new RecoverModel(RecoverEnum.参数错误, "请输入原安全码！"));
                if (merchant.SecurityPwd != Utils.MD5(oldPwd))
                    return Ok(new RecoverModel(RecoverEnum.失败, "原密码错误！"));
            }
            merchant.SecurityPwd = Utils.MD5(newPwd);
            await merchantOperation.UpdateModelAsync(merchant);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功！"));
        }

        /// <summary>
        /// 验证安全密码
        /// </summary>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> VerificationSecurityPwd(string pwd)
        {
            var merchantOperation = new MerchantOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            if (merchant.SecurityPwd != Utils.MD5(pwd))
                return Ok(new RecoverModel(RecoverEnum.失败, "验证失败！"));
            return Ok(new RecoverModel(RecoverEnum.成功, "验证成功！"));
        }

        /// <summary>
        /// 获取商户火星币
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMerchantCurrency()
        {
            var merchantOperation = new MerchantOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = merchant.MarsCurrency.ToString("#0.00"),
                Message = "获取成功！",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 获取游戏信息状态
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGameInfos()
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var dic = GameBetsMessage.EnumToDictionary(typeof(GameOfType));
            var result = new List<WebAppGameInfos>();
            foreach (var item in dic)
            {
                var gameType = GameBetsMessage.GetEnumByStatus<GameOfType>(item.Value);
                var code = await Utils.GetGameStatus(merchantID, gameType);
                result.Add(code);
            }
            return Ok(new RecoverListModel<WebAppGameInfos>()
            { 
                Data = result,
                Total = result.Count,
                Status = RecoverEnum.成功,
                Message = "获取成功！"
            });
        }
        #endregion
    }
}