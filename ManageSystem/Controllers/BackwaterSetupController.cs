using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using ManageSystem.Manipulate;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Operation.Abutment;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ManageSystem.Controllers
{
    /// <summary>
    /// 回水信息
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    [MerchantAuthentication]
    public class BackwaterSetupController : ControllerBase
    {
        #region 彩票回水
        /// <summary>
        /// 获取彩票回水设置数据
        /// </summary>
        /// <param name="name">方案名称</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBackwaterList(string name, int start = 1, int pageSize = 10)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var backwaterSetupOperation = new BackwaterSetupOperation();
            FilterDefinition<BackwaterSetup> filter = backwaterSetupOperation.Builder.Where(t => t.MerchantID == merchantID);
            if (!string.IsNullOrEmpty(name))
                filter &= backwaterSetupOperation.Builder.Regex(t => t.Name, name);
            var total = await backwaterSetupOperation.GetCountAsync(filter);
            var list = backwaterSetupOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var result = new List<WebBackwaterSetup>();
            foreach (var data in list)
            {
                result.Add(GetWebData(data));
            }
            return Ok(new RecoverListModel<WebBackwaterSetup>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        /// <summary>
        /// 添加彩票返水
        /// </summary>
        /// <param name="webBackwater"></param>
        /// <remarks>
        ///##  参数说明
        ///     GameType：游戏类型 1：赛车 2：飞艇 3：时时彩 4：极速 5：澳10  6：澳5
        ///     Maxrecord：最大统计
        ///     Minrecord：最小统计
        ///     Odds：返水比例
        ///     Pattern：模式  1：流水模式 2：输赢模式
        ///     Name：方案名称
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddBackwater([FromBody] WebBackwaterSetup webBackwater)
        {
            if (webBackwater == null)
                return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
            var backwaterSetupOperation = new BackwaterSetupOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = new BackwaterSetup()
            {
                MerchantID = merchantID,
                GameType = webBackwater.GameType,
                Maxrecord = webBackwater.Maxrecord,
                Minrecord = webBackwater.Minrecord,
                Odds = webBackwater.Odds,
                Pattern = webBackwater.Pattern,
                Name = webBackwater.Name
            };
            await backwaterSetupOperation.InsertModelAsync(data);
            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID,
                OpLocation = OpLocationEnum.回水方案操作,
                OpType = OpTypeEnum.添加,
                OpAcontent = string.Format("添加回水方案{0},模式:{1},游戏:{2},最小统计:{3},最大统计:{4},比例:{5}", data.Name
                , Enum.GetName(typeof(PatternEnum), (int)data.Pattern), data.GameType == null ? "所有" : Enum.GetName(typeof(GameOfType), (int)data.GameType.Value),
                data.Minrecord, data.Maxrecord, data.Odds)
            };
            await sensitiveOperation.InsertModelAsync(sensitive);
            return Ok(new RecoverModel(RecoverEnum.成功, "添加成功！"));
        }

        /// <summary>
        /// 获取返水数据
        /// </summary>
        /// <param name="backID"></param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             ID:id
        ///             GameType：游戏类型 1：赛车 2：飞艇 3：时时彩 4：极速 5：澳10  6：澳5
        ///             Maxrecord：最大统计
        ///             Minrecord：最小统计
        ///             Odds：返水比例
        ///             Pattern：模式  1：流水模式 2：输赢模式
        ///             UserType：用户类型 1：黄铜  2：白银  3：黄金  4：铂金  5：钻石  6：银皇冠  7：金皇冠
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetBackwaterByID(string backID)
        {
            var backwaterSetupOperation = new BackwaterSetupOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = await backwaterSetupOperation.GetBackwaterByIDAndNo(backID, merchantID);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到返水设置！"));
            var result = GetWebData(data);
            return Ok(new RecoverClassModel<WebBackwaterSetup>() { Message = "获取成功！", Model = result, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 修改返水
        /// </summary>
        /// <param name="webBackwater"></param>
        /// <remarks>
        ///##  参数说明
        ///     ID：id
        ///     GameType：游戏类型 1：赛车 2：飞艇 3：时时彩 4：极速 5：澳10  6：澳5
        ///     Maxrecord：最大统计
        ///     Minrecord：最小统计
        ///     Odds：返水比例
        ///     Pattern：模式  1：流水模式 2：输赢模式
        ///     UserType：用户类型 1：黄铜  2：白银  3：黄金  4：铂金  5：钻石  6：银皇冠  7：金皇冠
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateBackwater([FromBody] WebBackwaterSetup webBackwater)
        {
            if (webBackwater == null) return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
            var backwaterSetupOperation = new BackwaterSetupOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = await backwaterSetupOperation.GetBackwaterByIDAndNo(webBackwater.ID, merchantID);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到返水设置！"));

            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID,
                OpLocation = OpLocationEnum.回水方案操作,
                OpType = OpTypeEnum.添加,
                OpBcontent = string.Format("回水方案{0},模式:{1},游戏:{2},最小统计:{3},最大统计:{4},比例:{5}", data.Name
                , Enum.GetName(typeof(PatternEnum), (int)data.Pattern), data.GameType == null ? "所有" : Enum.GetName(typeof(GameOfType), (int)data.GameType.Value),
                data.Minrecord, data.Maxrecord, data.Odds),
                OpAcontent = string.Format("回水方案{0},模式:{1},游戏:{2},最小统计:{3},最大统计:{4},比例:{5}", webBackwater.Name
                , Enum.GetName(typeof(PatternEnum), (int)webBackwater.Pattern), webBackwater.GameType == null ? "所有" : Enum.GetName(typeof(GameOfType), (int)webBackwater.GameType.Value),
                webBackwater.Minrecord, webBackwater.Maxrecord, webBackwater.Odds)
            };
            await sensitiveOperation.InsertModelAsync(sensitive);

            data.GameType = webBackwater.GameType;
            data.Maxrecord = webBackwater.Maxrecord;
            data.Minrecord = webBackwater.Minrecord;
            data.Odds = webBackwater.Odds;
            data.Pattern = webBackwater.Pattern;
            data.Name = webBackwater.Name;
            await backwaterSetupOperation.UpdateModelAsync(data);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 删除返水
        /// </summary>
        /// <param name="backID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteBackwaterByID(string backID)
        {
            var backwaterSetupOperation = new BackwaterSetupOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = await backwaterSetupOperation.GetBackwaterByIDAndNo(backID, merchantID);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到返水设置！"));
            await backwaterSetupOperation.DeleteModelOneAsync(t => t._id == backID);
            return Ok(new RecoverModel(RecoverEnum.成功, "删除成功"));
        }

        private WebBackwaterSetup GetWebData(BackwaterSetup backwater)
        {
            if (backwater == null) return null;
            var result = new WebBackwaterSetup()
            {
                ID = backwater._id,
                GameType = backwater.GameType,
                Maxrecord = backwater.Maxrecord,
                Minrecord = backwater.Minrecord,
                Odds = backwater.Odds,
                Pattern = backwater.Pattern,
                Name = backwater.Name
            };
            return result;
        }
        #endregion
        #region 视讯回水
        /// <summary>
        /// 获取视讯游戏回水
        /// </summary>
        /// <param name="name">方案名称</param>
        /// <param name="start">页码</param>
        /// <param name="pageSize">页数</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoBackwaterList(string name, int start = 1, int pageSize = 10)
        {
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            VideoBackwaterSetupOperation videoBackwaterSetupOperation = new VideoBackwaterSetupOperation();
            FilterDefinition<VideoBackwaterSetup> filter = videoBackwaterSetupOperation.Builder.Where(t => t.MerchantID == merchantID);
            if (!string.IsNullOrEmpty(name))
                filter &= videoBackwaterSetupOperation.Builder.Regex(t => t.Name, name);
            var total = await videoBackwaterSetupOperation.GetCountAsync(filter);
            var list = videoBackwaterSetupOperation.GetModelListByPaging(filter, t => t.CreatedTime, false, start, pageSize);
            var result = new List<WebVideoBackwaterSetup>();
            foreach (var data in list)
            {
                result.Add(GetWebVideoData(data));
            }
            return Ok(new RecoverListModel<WebVideoBackwaterSetup>()
            {
                Data = result,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = total
            });
        }

        private WebVideoBackwaterSetup GetWebVideoData(VideoBackwaterSetup backwater)
        {
            if (backwater == null) return null;
            var result = new WebVideoBackwaterSetup()
            {
                ID = backwater._id,
                GameType = backwater.GameType,
                Maxrecord = backwater.Maxrecord,
                Minrecord = backwater.Minrecord,
                Odds = backwater.Odds,
                Pattern = backwater.Pattern,
                Name = backwater.Name
            };
            return result;
        }

        /// <summary>
        /// 添加视讯返水
        /// </summary>
        /// <param name="model"></param>
        /// <remarks>
        ///##  参数说明
        ///     GameType：游戏类型 1：百家乐
        ///     Maxrecord：最大统计
        ///     Minrecord：最小统计
        ///     Odds：返水比例
        ///     Pattern：模式  1：流水模式 2：输赢模式
        ///     Name：方案名称
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> AddVideoBackwater([FromBody] WebVideoBackwaterSetup model)
        {
            if (model == null)
                return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
            var videoBackwaterSetupOperation = new VideoBackwaterSetupOperation();

            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = new VideoBackwaterSetup()
            {
                MerchantID = merchantID,
                GameType = model.GameType,
                Maxrecord = model.Maxrecord,
                Minrecord = model.Minrecord,
                Odds = model.Odds,
                Pattern = model.Pattern,
                Name = model.Name
            };
            await videoBackwaterSetupOperation.InsertModelAsync(data);
            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID,
                OpLocation = OpLocationEnum.回水方案操作,
                OpType = OpTypeEnum.添加,
                OpAcontent = string.Format("添加视讯回水方案{0},模式:{1},游戏:{2},最小统计:{3},最大统计:{4},比例:{5}", data.Name
                , Enum.GetName(typeof(PatternEnum), (int)data.Pattern), data.GameType == null ? "所有" : Enum.GetName(typeof(BaccaratGameType), (int)data.GameType.Value),
                data.Minrecord, data.Maxrecord, data.Odds)
            };
            await sensitiveOperation.InsertModelAsync(sensitive);
            return Ok(new RecoverModel(RecoverEnum.成功, "添加成功！"));
        }

        /// <summary>
        /// 获取返水数据
        /// </summary>
        /// <param name="backID"></param>
        /// <response>
        /// ## 返回结果
        ///     {
        ///         Status：返回状态
        ///         Message：返回消息
        ///         Model
        ///         {
        ///             ID:id
        ///             GameType：游戏类型 1：百家乐
        ///             Maxrecord：最大统计
        ///             Minrecord：最小统计
        ///             Odds：返水比例
        ///             Pattern：模式  1：流水模式 2：输赢模式
        ///         }
        ///     }
        /// </response>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoBackwaterByID(string backID)
        {
            var videoBackwaterSetupOperation = new VideoBackwaterSetupOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = await videoBackwaterSetupOperation.GetModelAsync(t => t._id == backID && t.MerchantID == merchantID);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到返水设置！"));
            var result = GetWebVideoData(data);
            return Ok(new RecoverClassModel<WebVideoBackwaterSetup>() { Message = "获取成功！", Model = result, Status = RecoverEnum.成功 });
        }

        /// <summary>
        /// 修改视讯返水
        /// </summary>
        /// <param name="model"></param>
        /// <remarks>
        ///##  参数说明
        ///     ID：id
        ///     GameType：游戏类型 1：百家乐
        ///     Maxrecord：最大统计
        ///     Minrecord：最小统计
        ///     Odds：返水比例
        ///     Pattern：模式  1：流水模式 2：输赢模式
        /// </remarks>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateVideoBackwater([FromBody] WebVideoBackwaterSetup model)
        {
            if (model == null) return Ok(new RecoverModel(RecoverEnum.参数错误, "参数错误！"));
            var videoBackwaterSetupOperation = new VideoBackwaterSetupOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = await videoBackwaterSetupOperation.GetModelAsync(t => t._id == model.ID && t.MerchantID == merchantID);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到返水设置！"));

            SensitiveOperation sensitiveOperation = new SensitiveOperation();
            var sensitive = new Sensitive()
            {
                MerchantID = merchantID,
                OpLocation = OpLocationEnum.回水方案操作,
                OpType = OpTypeEnum.添加,
                OpBcontent = string.Format("回水方案{0},模式:{1},游戏:{2},最小统计:{3},最大统计:{4},比例:{5}", data.Name
                , Enum.GetName(typeof(PatternEnum), (int)data.Pattern), data.GameType == null ? "所有" : Enum.GetName(typeof(BaccaratGameType), (int)data.GameType.Value),
                data.Minrecord, data.Maxrecord, data.Odds),
                OpAcontent = string.Format("回水方案{0},模式:{1},游戏:{2},最小统计:{3},最大统计:{4},比例:{5}", model.Name
                , Enum.GetName(typeof(PatternEnum), (int)model.Pattern), model.GameType == null ? "所有" : Enum.GetName(typeof(BaccaratGameType), (int)model.GameType.Value),
                model.Minrecord, model.Maxrecord, model.Odds)
            };
            await sensitiveOperation.InsertModelAsync(sensitive);

            data.GameType = model.GameType;
            data.Maxrecord = model.Maxrecord;
            data.Minrecord = model.Minrecord;
            data.Odds = model.Odds;
            data.Pattern = model.Pattern;
            data.Name = model.Name;
            await videoBackwaterSetupOperation.UpdateModelAsync(data);
            return Ok(new RecoverModel(RecoverEnum.成功, "修改成功"));
        }

        /// <summary>
        /// 删除视讯返水
        /// </summary>
        /// <param name="backID"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DeleteVideoBackwaterByID(string backID)
        {
            var videoBackwaterSetupOperation = new VideoBackwaterSetupOperation();
            var merchantID = HttpContext.Items["MerchantID"].ToString();
            var data = await videoBackwaterSetupOperation.GetModelAsync(t => t._id == backID && t.MerchantID == merchantID);
            if (data == null) return Ok(new RecoverModel(RecoverEnum.未查询到相关数据, "未查询到返水设置！"));
            await videoBackwaterSetupOperation.DeleteModelOneAsync(t => t._id == backID);
            return Ok(new RecoverModel(RecoverEnum.成功, "删除成功"));
        }
        #endregion
    }
}