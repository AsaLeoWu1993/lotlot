using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using ManageSystem.Manipulate;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Baccarat;
using Operation.Common;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using static Operation.Common.Utils;

namespace ManageSystem.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    [MerchantAuthentication]
    public class RoomController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment = null;
        private delegate IActionResult GamesDelegate(string gameID, string roomID);

        public RoomController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        #region 普通方法 
        /// <summary>
        /// 获取房间信息
        /// </summary>
        /// <remarks>
        ///##  参数说明
        ///     
        /// </remarks>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             ID：房间id 
        ///             QQ：QQ
        ///             WeChat：微信号
        ///             SubAccount：上分帐户
        ///             {
        ///                 WeChatNum：微信帐号
        ///                 WeChatQRcode：微信二维码
        ///                 AlipayNum：支付宝帐号
        ///                 AlipayQRcode：支付宝二维码
        ///                 ShamOnfirm：虚拟玩家自动确认上下分请求
        ///                 ShamOnfirmTime：虚拟玩家自动确认上下分请求时间
        ///                 AdminPortrait：管理员头像
        ///             }
        ///             Online：在线人数
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoomInfo()
        {
            var roomOperation = new RoomOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var room = await roomOperation.GetRoomByMerchantID(merchantID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到商户房间信息！"));
            var data = new WebRoom
            {
                ID = room._id,
                QQ = room.QQ,
                WeChat = room.WeChat,
                SubAccount = room.SubAccount,
                Online = room.Online,
                ShamOnfirm = room.ShamOnfirm,
                ShamOnfirmTime = room.ShamOnfirmTime,
                CustomerUrl = room.CustomerUrl,
                CustomerOpen = room.CustomerOpen,
                AdminPortrait = room.AdminPortrait
            };
            return Ok(new RecoverClassModel<WebRoom>() { Message = "获取成功！", Model = data, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 修改客服
        /// </summary>
        /// <param name="roomID">房间id</param>
        /// <param name="qq">qq</param>
        /// <param name="weChat">微信号</param>
        /// <param name="shamOnfirm">虚拟玩家自动确认上下分请求</param>
        /// <param name="shamOnfirmTime">虚拟玩家自动确认上下分请求时间</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateRoomCus(string roomID, string qq, string weChat, bool shamOnfirm, int shamOnfirmTime)
        {
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetRoomByID(roomID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            room.QQ = qq;
            room.WeChat = weChat;
            room.ShamOnfirm = shamOnfirm;
            room.ShamOnfirmTime = shamOnfirmTime;
            await roomOperation.UpdateModelAsync(room);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 修改在线人数
        /// </summary>
        /// <param name="roomID">房间id</param>
        /// <param name="num">修改人数</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateRoomOnline(string roomID, int num)
        {
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetRoomByID(roomID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            room.Online = num;
            await roomOperation.UpdateModelAsync(room);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 修改微信上分帐户
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="fileinput">图片文件</param>
        /// <param name="weChat">微信号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateRoomWeChatPayment(string roomID, IFormFile fileinput, string weChat)
        {
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetRoomByID(roomID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            room.SubAccount.WeChatNum = weChat;
            if (fileinput == null)
            {
                await roomOperation.UpdateModelAsync(room);
                return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
            }
            var url = await BlobHelper.UploadImageToBlob(fileinput, "RoomImages");
            if (string.IsNullOrEmpty(url)) return Ok(new RecoverModel(RecoverEnum.失败, "图片格式错误！"));
            if (url == "1") return Ok(new RecoverModel(RecoverEnum.失败, "图片大小最大为20M！"));
            room.SubAccount.WeChatQRcode = url;
            await roomOperation.UpdateModelAsync(room);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 修改支付宝上分帐户
        /// </summary>
        /// <param name="roomID">房间号</param>
        /// <param name="fileinput">图片文件</param>
        /// <param name="alipay">支付宝</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateRoomAlipayPayment(string roomID, IFormFile fileinput, string alipay)
        {
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetRoomByID(roomID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            room.SubAccount.AlipayNum = alipay;
            if (fileinput == null)
            {
                await roomOperation.UpdateModelAsync(room);
                return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
            }
            var url = await BlobHelper.UploadImageToBlob(fileinput, "RoomImages");
            if (string.IsNullOrEmpty(url)) return Ok(new RecoverModel(RecoverEnum.失败, "图片格式错误！"));
            if (url == "1") return Ok(new RecoverModel(RecoverEnum.失败, "图片大小最大为20M！"));
            room.SubAccount.AlipayQRcode = url;
            await roomOperation.UpdateModelAsync(room);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 修改管理员头像
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="fileinput"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAdminPortrait(string roomID, IFormFile fileinput)
        {
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetRoomByID(roomID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            var url = await BlobHelper.UploadImageToBlob(fileinput, "AdminPortrait");
            if (string.IsNullOrEmpty(url)) return Ok(new RecoverModel(RecoverEnum.失败, "图片格式错误！"));
            if (url == "1") return Ok(new RecoverModel(RecoverEnum.失败, "图片大小最大为20M！"));
            room.AdminPortrait = url;
            await roomOperation.UpdateModelAsync(room);
            RedisOperation.SetAdminPortrait(room.MerchantID, url);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 修改客服800地址
        /// </summary>
        /// <param name="roomID"></param>
        /// <param name="url"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateCustomerUrl(string roomID, string url, bool status)
        {
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetRoomByID(roomID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            room.CustomerUrl = url;
            room.CustomerOpen = status;
            await roomOperation.UpdateModelAsync(room);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }
        #endregion

        #region 游戏房间 
        /// <summary>
        /// 获取对应游戏房间设置
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        ///  /// <remarks>
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
        ///             GameRoomName：游戏房间名称 
        ///             GameRoomNum：游戏房号码
        ///             Status：状态  1：开启  2：关闭
        ///             Minin：上进入房间最低金额
        ///             Che：撤销
        ///             Back：回水通知
        ///             RInfoItems：
        ///             {
        ///                 Index：信息 1：积分  2：输赢  3：流水  4：投注  5：使用
        ///                 Open：是否开启
        ///             }
        ///             MinimumAmount：账单显示最低金额
        ///             Bill：账单  1：单排  2：双排
        ///             SwitchBackwater：回水设置 
        ///             KaiEquality：开和  10球游戏为空
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGameOfRoomInfo(GameOfType gameType)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = await Utils.GetRoomInfosAsync(merchantID, gameType);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            var result = new WebRoomGameKai
            {
                ID = data._id,
                GameRoomName = data.GameRoomName,
                GameRoomNum = room.RoomNum,
                Status = data.Status,
                Minin = data.Minin,
                //Che = data.Che,
                Back = data.Back,
                RInfoItems = data.RInfoItems,
                MinimumAmount = data.MinimumAmount,
                Bill = data.Bill,
                KaiEquality = data.KaiEquality,
                SwitchBackwater = data.SwitchBackwater,
                LotteryRecord = data.LotteryRecord,
                VideoRecord = data.VideoRecord,
                Revoke = data.Revoke,
                Instructions = data.Instructions,
                HaltSales = data.HaltSales,
                HaltTime = data.HaltTime,
                OnSaleTime = data.OnSaleTime
            };
            return Ok(new RecoverClassModel<WebRoomGameKai>() { Message = "获取成功！", Model = result, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 获取视讯游戏房间设置
        /// </summary>
        /// <param name="gameType">游戏类型 1：百家乐 2：牛牛 3：龙虎</param>
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
        ///             GameRoomName：游戏房间名称 
        ///             GameRoomNum：游戏房号码
        ///             Status：状态  1：开启  2：关闭
        ///             Minin：上进入房间最低金额
        ///             Che：撤销
        ///             Back：回水通知
        ///             MinimumAmount：账单显示最低金额
        ///             Bill：账单  1：单排  2：双排
        ///             SwitchBackwater：回水设置 
        ///             KaiEquality：开和  1：全退 2：退一半 3：通杀
        ///             LotteryRecord：彩票飞单  1：不飞单 2：高级  3：外部
        ///             VideoRecord：视讯飞单
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoGameOfRoomInfo(BaccaratGameType gameType)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            VideoRoomOperation videoRoomOperation = new VideoRoomOperation();
            var data = await videoRoomOperation.GetModelAsync(t => t.MerchantID == merchantID && t.GameType == gameType);
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (data == null)
            {
                data = new VideoRoom()
                {
                    MerchantID = merchantID,
                    RoomID = room._id,
                    GameRoomNum = room.RoomNum,
                    GameType = gameType
                };
                await videoRoomOperation.InsertModelAsync(data);
            }
            var result = new WebVideoRoomGame()
            {
                ID = data._id,
                Back = data.Back,
                Bill = data.Bill,
                //Che = data.Che,
                GameRoomName = data.GameRoomName,
                GameRoomNum = data.GameRoomNum,
                KaiEquality = data.KaiEquality,
                MinimumAmount = data.MinimumAmount,
                Minin = data.Minin,
                Status = data.Status,
                SwitchBackwater = data.SwitchBackwater,
                LotteryRecord = data.LotteryRecord,
                VideoRecord = data.VideoRecord,
                Revoke = data.Revoke,
                Instructions = data.Instructions,
                HaltSales = data.HaltSales,
                HaltTime = data.HaltTime,
                OnSaleTime = data.OnSaleTime
            };
            return Ok(new RecoverClassModel<WebVideoRoomGame>()
            {
                Message = "获取成功！",
                Model = result,
                Status = RecoverEnum.成功
            });
        }
        #endregion

        #region 修改游戏房间
        /// <summary>
        /// 修改时时彩游戏房间信息
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="webRoom"></param>
        /// <remarks>
        ///##  参数说明
        ///     ID：id
        ///     GameRoomName：游戏房间名称 
        ///     GameRoomNum：游戏房号码
        ///     Status：状态  1：开启  2：关闭
        ///     Minin：上进入房间最低金额
        ///     Che：撤销
        ///     Back：回水通知
        ///     RInfoItems：
        ///     {
        ///          Index：信息 1：积分  2：输赢  3：流水  4：投注  5：使用
        ///          Open：是否开启
        ///      }
        ///      MinimumAmount：账单显示最低金额
        ///      Bill：账单  1：单排  2：双排
        ///      KaiEquality：开和  1：返还本金  2：通杀龙虎
        ///      Revoke：禁止撤单
        ///      Instructions：app赔率
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateGameofRoomInfo(GameOfType gameType, [FromBody] WebRoomGameKai webRoom)
        {
            var roomOperation = new RoomOperation();
            RoomGameDetailedOperation roomGameDetailedOperation = new RoomGameDetailedOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到房间信息！"));
            var data = await Utils.GetRoomInfosAsync(merchantID, gameType);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            if (data.Status != webRoom.Status)
                await SwitchGames(webRoom.ID, gameType);
            if (!Utils.GameTypeItemize(gameType))
            {
                if (data.KaiEquality == null) data.KaiEquality = KaiHeEnum.返还本金;
                if (data.KaiEquality != webRoom.KaiEquality)
                {
                    SensitiveOperation sensitiveOperation = new SensitiveOperation();
                    var sensitive = new Sensitive()
                    {
                        MerchantID = merchantID,
                        OpLocation = OpLocationEnum.房间开和设置,
                        OpType = OpTypeEnum.修改,
                        OpBcontent = Enum.GetName(typeof(GameOfType), (int)gameType) + Enum.GetName(typeof(KaiHeEnum), (int)data.KaiEquality),
                        OpAcontent = Enum.GetName(typeof(GameOfType), (int)gameType) + Enum.GetName(typeof(KaiHeEnum), (int)webRoom.KaiEquality)
                    };
                    await sensitiveOperation.InsertModelAsync(sensitive);
                }
                data.KaiEquality = webRoom.KaiEquality;
            }
            data.GameRoomName = webRoom.GameRoomName;
            data.Status = webRoom.Status;
            data.MinimumAmount = webRoom.MinimumAmount;
            //data.Che = webRoom.Che;
            data.Back = webRoom.Back;
            data.RInfoItems = webRoom.RInfoItems;
            data.Minin = webRoom.Minin;
            data.Bill = webRoom.Bill;
            data.VideoRecord = webRoom.VideoRecord;
            data.LotteryRecord = webRoom.LotteryRecord;
            data.Revoke = webRoom.Revoke;
            data.Instructions = webRoom.Instructions;
            data.HaltSales = webRoom.HaltSales;
            data.HaltTime = webRoom.HaltTime;
            data.OnSaleTime = webRoom.OnSaleTime;

            //判断房间号是否与代理重复
            AgentBackwaterOperation agentBackwaterOperation = new AgentBackwaterOperation();
            var agent = await agentBackwaterOperation.GetModelAsync(t => t.MerchantID == merchantID && t.ExtensionCode == webRoom.GameRoomNum);
            if (agent != null)
                return Ok(new RecoverModel(RecoverEnum.成功, "已存在相同房间号！"));

            room.RoomNum = webRoom.GameRoomNum;
            await roomOperation.UpdateModelAsync(room);
            data.SwitchBackwater = webRoom.SwitchBackwater;
            await roomGameDetailedOperation.UpdateModelAsync(data);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }

        /// <summary>
        /// 修改视讯游戏房间信息
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="model">数据集合</param>
        /// <remarks>
        ///##  参数说明
        ///     ID：id
        ///     GameRoomName：游戏房间名称 
        ///     GameRoomNum：游戏房号码
        ///     Status：状态  1：开启  2：关闭
        ///     Minin：上进入房间最低金额
        ///     Che：撤销
        ///     Back：回水通知
        ///     MinimumAmount：账单显示最低金额
        ///     Bill：账单  1：单排  2：双排
        ///     SwitchBackwater：回水设置 
        ///     KaiEquality：开和  1：全退 2：退一半 3：通杀
        ///      Revoke：禁止撤单
        ///      Instructions：app赔率
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateVideoGameofRoomInfo(BaccaratGameType gameType, [FromBody]WebVideoRoomGame model)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            VideoRoomOperation videoRoomOperation = new VideoRoomOperation();
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到房间信息！"));
            var data = await videoRoomOperation.GetModelAsync(t => t._id == model.ID && t.GameType == gameType && t.MerchantID == merchantID);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            room.RoomNum = model.GameRoomNum;
            //游戏开启关闭通知
            if (data.Status != model.Status)
                await SwitchVideoGames(data._id, gameType);

            data.GameRoomName = model.GameRoomName;
            data.Status = model.Status;
            data.MinimumAmount = model.MinimumAmount;
            //data.Che = model.Che;
            data.Back = model.Back;
            data.Minin = model.Minin;
            data.Bill = model.Bill;
            data.KaiEquality = model.KaiEquality;
            data.SwitchBackwater = model.SwitchBackwater;
            data.VideoRecord = model.VideoRecord;
            data.LotteryRecord = model.LotteryRecord;
            data.Revoke = model.Revoke;
            data.Instructions = model.Instructions;
            data.HaltSales = model.HaltSales;
            data.HaltTime = model.HaltTime;
            data.OnSaleTime = model.OnSaleTime;

            await videoRoomOperation.UpdateModelAsync(data);
            await roomOperation.UpdateModelAsync(room);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功！"));
        }
        #endregion

        #region 控制游戏开启关闭
        /// <summary>
        /// 获取对应商户下游戏id
        /// </summary>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Data
        ///         {
        ///             ID：id
        ///             Type：1：赛车 2：飞艇 3：时时彩 4：极速 5：澳10  6：澳5
        ///             Status：状态  1：开启  2：关闭
        ///         }
        ///         Total：数据数量
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGameIDs()
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var roomOperation = new RoomOperation();
            RoomGameDetailedOperation roomGameDetailedOperation = new RoomGameDetailedOperation();
            var dic = GameBetsMessage.EnumToDictionary(typeof(GameOfType));
            var room = await roomOperation.GetRoomByMerchantID(merchantID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            List<dynamic> result = new List<dynamic>();
            foreach (var item in dic)
            {
                var pargameType = GameBetsMessage.GetEnumByStatus<GameOfType>(item.Value);
                var infos = await Utils.GetRoomInfosAsync(merchantID, pargameType);
                if (infos == null)
                    return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, string.Format("未查询到相关{0}房间信息！", Enum.GetName(typeof(GameOfType), (int)pargameType))));
                result.Add(new { Type = pargameType, ID = infos._id, infos.Status });
            }
            return Ok(new RecoverListModel<dynamic>() { Data = result, Message = "查询成功！", Status = RecoverEnum.成功, Total = result.Count });
        }

        /// <summary>
        /// 切换游戏开关状态
        /// </summary>
        /// <param name="gameID"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SwitchGames(string gameID, GameOfType type)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            var roomGameDetailedOperation = new RoomGameDetailedOperation();
            var data = await Utils.GetRoomInfosAsync(merchantID, type);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关游戏房间信息！"));
            data.Status = data.Status == RoomStatus.开启 ? RoomStatus.关闭 : RoomStatus.开启;
            await roomGameDetailedOperation.UpdateModelAsync(data);

            //获取游戏状态
            var gameStatus = await GameDiscrimination.EachpartAsync(type, merchantID);
            RedisOperation.SetHash("MerchantGameStatus", merchantID + Enum.GetName(typeof(GameOfType), type), JsonConvert.SerializeObject(gameStatus));
            await RabbitMQHelper.SendGameMessage(JsonConvert.SerializeObject(gameStatus), merchantID, "SendRoomMessage", type);
            await RabbitMQHelper.SendOverallMessage(JsonConvert.SerializeObject(gameStatus), merchantID, "SendListMessage");
            return Ok(new RecoverModel(RecoverEnum.成功, "切换成功！"));
        }

        /// <summary>
        /// 切换视讯游戏开关状态
        /// </summary>
        /// <param name="gameID">游戏id</param>
        /// <param name="type">游戏类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> SwitchVideoGames(string gameID, BaccaratGameType type)
        {

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关房间信息！"));
            var videoRoomOperation = new VideoRoomOperation();
            var data = await videoRoomOperation.GetModelAsync(t => t.MerchantID == merchantID && t.RoomID == room._id && t._id == gameID && t.GameType == type);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到相关游戏房间信息！"));
            data.Status = data.Status == RoomStatus.开启 ? RoomStatus.关闭 : RoomStatus.开启;
            await videoRoomOperation.UpdateModelAsync(data);

            //获取游戏状态
            //var gameStatus = await GameDiscrimination.EachpartAsync(type, merchantID, baseMongo);
            //RabbitMQHelper.SendGameMessage(JsonConvert.SerializeObject(gameStatus), merchantID, "SendRoomMessage", type);
            //RabbitMQHelper.SendOverallMessage(JsonConvert.SerializeObject(gameStatus), merchantID, "SendListMessage");
            return Ok(new RecoverModel(RecoverEnum.成功, "切换成功！"));
        }
        #endregion

        /// <summary>
        /// 获取房间号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRoomNum()
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            if (room == null) return Ok(new RecoverModel(RecoverEnum.失败, "未查询到房间信息！"));
            return Ok(new RecoverClassModel<string>
            {
                Message = "获取成功！",
                Model = room.RoomNum,
                Status = RecoverEnum.成功
            });
        }
    }
}