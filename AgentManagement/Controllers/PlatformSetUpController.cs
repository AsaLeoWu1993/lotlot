using AgentManagement.Manipulate;
using Entity;
using Entity.AgentModel;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    /// 平台设置
    /// </summary>
    [Route("api/[controller]/[action]")]
    [ApiController]
    [EnableCors("AllowAllOrigin")]
    public class PlatformSetUpController : ControllerBase
    {
        readonly PlatformSetUpOperation platformSetUpOperation = null;
        readonly AgentMainLogOperation agentMainLogOperation = null;

        private readonly IHostingEnvironment _hostingEnvironment = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostingEnvironment"></param>
        public PlatformSetUpController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
            platformSetUpOperation = new PlatformSetUpOperation();
            agentMainLogOperation = new AgentMainLogOperation();
        }

        /// <summary>
        /// 获取商户地址
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetMerchantUrl()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        MerchantUrl = "",
                        MerchantMUrl = "",
                        MerchantAppUrl = ""
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        model.MerchantUrl,
                        model.MerchantMUrl,
                        model.MerchantAppUrl
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
        }

        /// <summary>
        /// 修改商户地址
        /// </summary>
        /// <param name="merchantUrl">商户后台地址</param>
        /// <param name="merchantMUrl">商户后台手机端地址</param>
        /// <param name="merchantAppUrl">商户游戏app地址</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateMerchantUrl(string merchantUrl, string merchantMUrl, string merchantAppUrl)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (data == null)
            {
                data = new PlatformSetUp()
                {
                    MerchantUrl = merchantUrl,
                    MerchantMUrl = merchantMUrl,
                    MerchantAppUrl = merchantAppUrl
                };
                await platformSetUpOperation.InsertModelAsync(data);
            }
            else
            {
                data.MerchantUrl = merchantUrl;
                data.MerchantMUrl = merchantMUrl;
                data.MerchantAppUrl = merchantAppUrl;
                await platformSetUpOperation.UpdateModelAsync(data);
            }
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改商户地址",
                Status = OperationStatusEnum.成功,
                Remark = "修改商户地址"
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取app地址
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAppUrl()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        AppUrl1 = "",
                        AppUrl2 = ""
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        model.AppUrl1,
                        model.AppUrl2
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
        }

        /// <summary>
        /// 修改app地址
        /// </summary>
        /// <param name="url1">地址1</param>
        /// <param name="url2">地址2</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAppUrl(string url1, string url2)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (data == null)
            {
                data = new PlatformSetUp()
                {
                    AppUrl1 = url1,
                    AppUrl2 = url2
                };
                await platformSetUpOperation.InsertModelAsync(data);
            }
            else
            {
                data.AppUrl1 = url1;
                data.AppUrl2 = url2;
                await platformSetUpOperation.UpdateModelAsync(data);
            }
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改app地址",
                Status = OperationStatusEnum.成功,
                Remark = "修改app地址"
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取公众号图片地址
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSubscription()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new RecoverKeywordModel()
                {
                    Keyword = "",
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
                return Ok(new RecoverKeywordModel()
                {
                    Keyword = model.Subscription,
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
        }

        /// <summary>
        /// 修改公众号图片地址
        /// </summary>
        /// <param name="fileinput">图片</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSubscription(IFormFile fileinput)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            if (fileinput == null) return Ok(new RecoverModel(RecoverEnum.失败, "未选择图片！"));
            var url = await BlobHelper.UploadImageToBlob(fileinput, "Subscription");
            if (string.IsNullOrEmpty(url)) return Ok(new RecoverModel(RecoverEnum.失败, "图片格式错误！"));
            if (url == "1") return Ok(new RecoverModel(RecoverEnum.失败, "图片大小最大为20M！"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (data == null)
            {
                data = new PlatformSetUp()
                {
                    Subscription = url
                };
                await platformSetUpOperation.InsertModelAsync(data);
            }
            else
            {
                data.Subscription = url;
                await platformSetUpOperation.UpdateModelAsync(data);
            }
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改公众号图片",
                Status = OperationStatusEnum.成功,
                Remark = "修改公众号图片"
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取app公共号设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAppSetUp()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        App_AppID = "",
                        App_AppSecret = ""
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        model.App_AppID,
                        model.App_AppSecret
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
        }

        /// <summary>
        /// 修改app公共号设置
        /// </summary>
        /// <param name="appID">appID</param>
        /// <param name="appSecret">appSecret</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAppSetUp(string appID, string appSecret)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (data == null)
            {
                data = new PlatformSetUp()
                {
                    App_AppID = appID,
                    App_AppSecret = appSecret
                };
                await platformSetUpOperation.InsertModelAsync(data);
            }
            else
            {
                data.App_AppID = appID;
                data.App_AppSecret = appSecret;
                await platformSetUpOperation.UpdateModelAsync(data);
            }
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改app公共号设置",
                Status = OperationStatusEnum.成功,
                Remark = "修改app公共号设置"
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取web公共号设置
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetWebSetUp()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        Web_AppID = "",
                        Web_AppSecret = ""
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
                return Ok(new RecoverClassModel<dynamic>()
                {
                    Model = new
                    {
                        model.Web_AppID,
                        model.Web_AppSecret
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
        }

        /// <summary>
        /// 修改web公共号设置
        /// </summary>
        /// <param name="appID">appID</param>
        /// <param name="appSecret">appSecret</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateWebSetUp(string appID, string appSecret)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (data == null)
            {
                data = new PlatformSetUp()
                {
                    Web_AppID = appID,
                    Web_AppSecret = appSecret
                };
                await platformSetUpOperation.InsertModelAsync(data);
            }
            else
            {
                data.Web_AppID = appID;
                data.Web_AppSecret = appSecret;
                await platformSetUpOperation.UpdateModelAsync(data);
            }
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改web公共号设置",
                Status = OperationStatusEnum.成功,
                Remark = "修改web公共号设置"
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取微信开关
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetSwitch()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new
                {
                    Keyword = true,
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
                return Ok(new
                {
                    Keyword = model.WeChatSwitch,
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
        }

        /// <summary>
        /// 修改微信开关
        /// </summary>
        /// <param name="status">状态</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateSwitch(bool status)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (data == null)
            {
                data = new PlatformSetUp()
                {
                    WeChatSwitch = status
                };
                await platformSetUpOperation.InsertModelAsync(data);
            }
            else
            {
                data.WeChatSwitch = status;
                await platformSetUpOperation.UpdateModelAsync(data);
            }
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改微信开关状态",
                Status = OperationStatusEnum.成功,
                Remark = "修改微信开关状态"
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取游戏视频地址
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGameVideoUrl(GameOfType gameType)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new
                {
                    Keyword = "",
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
            {
                var gameVideo = model.GameVideos.Find(t => t.GameType == gameType);
                var url = gameVideo == null ? "" : gameVideo.Url;
                return Ok(new
                {
                    Keyword = url,
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            }
        }

        /// <summary>
        /// 修改游戏视频地址
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <param name="url">地址</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateGameVideoUrl(GameOfType gameType, string url)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            var data = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (data == null)
            {
                data = new PlatformSetUp()
                {
                    GameVideos = new List<GameVideo>()
                    {
                        new GameVideo()
                        {
                            GameType = gameType,
                            Url = url
                        }
                    }
                };
                await platformSetUpOperation.InsertModelAsync(data);
            }
            else
            {
                if (data.GameVideos.IsNull())
                {
                    data.GameVideos = new List<GameVideo>()
                    {
                        new GameVideo()
                        {
                            GameType = gameType,
                            Url = url
                        }
                    };
                }
                else
                {
                    var gameVideo = data.GameVideos.Find(t => t.GameType == gameType);
                    if (gameVideo == null)
                    {
                        data.GameVideos.Add(new GameVideo()
                        {
                            GameType = gameType,
                            Url = url
                        });
                    }
                    else
                    {
                        data.GameVideos.ForEach(t =>
                        {
                            if (t.GameType == gameType)
                            {
                                t.Url = url;
                            }
                        });
                    }
                }
                await platformSetUpOperation.UpdateModelAsync(data);
            }
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改游戏视频地址",
                Status = OperationStatusEnum.成功,
                Remark = "修改游戏视频地址" + string.Format("\r\n{0}:{1}", Enum.GetName(typeof(GameOfType), (int)gameType), url)
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取游戏视频地址
        /// </summary>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGameSetUp(GameOfType gameType)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new
                {
                    Keyword = "",
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            else
            {
                var infos = model.GameBasicsSetups.Find(t => t.GameType == gameType);
                return Ok(new RecoverClassModel<GameBasicsSetup>
                {
                    Model = new GameBasicsSetup
                    {
                        GameType = gameType,
                        DayNum = infos == null ? 0 : infos.DayNum,
                        FirstNper = infos == null ? "" : infos.FirstNper,
                        StartTime = infos == null ? new DateTime() : infos.StartTime,
                        Interval = infos == null ? 0 : infos.Interval
                    },
                    Message = "获取成功",
                    Status = RecoverEnum.成功
                });
            }
        }

        /// <summary>
        /// 修改游戏设置
        /// </summary>
        /// <param name="setup"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateGameSetUp([FromBody]GameBasicsSetup setup)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var agentID = HttpContext.User.FindFirstValue("AgentID");
            if (setup.Interval <= 0) return Ok(new RecoverModel(RecoverEnum.失败, "间隔时间不能小于0"));
            if (setup.StartTime > DateTime.Now)
                return Ok(new RecoverModel(RecoverEnum.失败, "设置起点时间不能大于当前时间！"));
            //北京赛车
            if (setup.GameType == GameOfType.北京赛车)
            {
                if (setup.StartTime.Hour < 9 && setup.StartTime.Minute < 13)
                    return Ok(new RecoverModel(RecoverEnum.失败, "北京赛车起点时间不能在00:00-09:13区间！"));
                if (setup.Interval <= 60)
                    return Ok(new RecoverModel(RecoverEnum.失败, "北京赛车间隔时间不能少于60！"));
            }
            var data = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (data == null)
            {
                data = new PlatformSetUp()
                {
                    GameBasicsSetups = new List<GameBasicsSetup>
                    {
                        setup
                    }
                };
                await platformSetUpOperation.InsertModelAsync(data);
            }
            else
            {
                if (data.GameBasicsSetups.IsNull())
                {
                    data.GameBasicsSetups.Add(setup);
                }
                else
                {
                    var gameSetup = data.GameBasicsSetups.Find(t => t.GameType == setup.GameType);
                    if (gameSetup == null)
                    {
                        data.GameBasicsSetups.Add(setup);
                    }
                    else
                    {
                        data.GameBasicsSetups.ForEach(t =>
                        {
                            if (t.GameType == setup.GameType)
                            {
                                t.DayNum = setup.DayNum;
                                t.FirstNper = setup.FirstNper;
                                t.Interval = setup.Interval;
                                t.StartTime = setup.StartTime;
                            }
                        });
                    }
                }
                await platformSetUpOperation.UpdateModelAsync(data);
            }
            var log = new AgentMainLog()
            {
                AgentID = agentID,
                LoginIP = HttpContext.GetIP(),
                OperationMsg = "修改游戏设置",
                Status = OperationStatusEnum.成功,
                Remark = "修改游戏设置" + string.Format("\r\n{0}", Enum.GetName(typeof(GameOfType), (int)setup.GameType))
            };
            await agentMainLogOperation.InsertModelAsync(log);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功"));
        }

        /// <summary>
        /// 获取app域名列表
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAppUrls()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
                return Ok(new RecoverListModel<AppCanUserUrl>()
                {
                    Data = null,
                    Message = "获取成功",
                    Status = RecoverEnum.成功,
                    Total = 0
                });

            return Ok(new RecoverListModel<AppCanUserUrl>()
            {
                Data = model.AppUrls,
                Message = "获取成功",
                Status = RecoverEnum.成功,
                Total = model.AppUrls.Count
            });
        }

        /// <summary>
        /// 修改app域名列表
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateAppUrls([FromBody]List<AppCanUserUrl> models)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            var model = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            if (model == null)
            {
                model = new PlatformSetUp()
                {
                    AppUrls = models
                };
                await platformSetUpOperation.InsertModelAsync(model);
            }
            else
            {
                model.AppUrls = models;
                await platformSetUpOperation.UpdateModelAsync(model);
            }
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }

        /// <summary>
        /// 获取版本号
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVersionNum()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var setup = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            return Ok(new RecoverKeywordModel(RecoverEnum.成功, "获取成功！", setup.VersionNum));
        }

        /// <summary>
        /// 修改版本号
        /// </summary>
        /// <param name="versionNum">版本号</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateVersionNum(string versionNum)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var setup = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            setup.VersionNum = versionNum;
            await platformSetUpOperation.UpdateModelAsync(setup);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }

        /// <summary>
        /// 修改版本文件
        /// </summary>
        /// <param name="fileinput">文件</param>
        /// <returns></returns>
        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<IActionResult> UpdateVersionFile(IFormFile fileinput)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            if (fileinput == null) return Ok(new RecoverModel(RecoverEnum.失败, "未选择图片！"));
            var url = await BlobHelper.UploadImageToBlob(fileinput, "VersionFile");
            if (string.IsNullOrEmpty(url)) return Ok(new RecoverModel(RecoverEnum.失败, "图片格式错误！"));
            if (url == "1") return Ok(new RecoverModel(RecoverEnum.失败, "图片大小最大为20M！"));

            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var setup = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            setup.VersionFileUrl = url;
            await platformSetUpOperation.UpdateModelAsync(setup);
            return Ok(new RecoverModel(RecoverEnum.成功, "操作成功！"));
        }

        /// <summary>
        /// 获取h5微信授权域名
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetGrantUrl()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var setup = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = setup.GrantUrl,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 设置h5微信授权域名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateGrantUrln(string url)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var setup = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            setup.GrantUrl = url;
            await platformSetUpOperation.UpdateModelAsync(setup);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功"));
        }

        /// <summary>
        /// 获取视讯播放
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetVideoUrl()
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var setup = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            return Ok(new RecoverKeywordModel()
            {
                Keyword = setup.VideoUrl,
                Message = "获取成功",
                Status = RecoverEnum.成功
            });
        }

        /// <summary>
        /// 设置视讯播放
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateVideoUrn(string url)
        {
            if (!HttpContext.IsAdmin()) return Ok(new RecoverModel(RecoverEnum.未授权, "该用户无权利"));
            PlatformSetUpOperation platformSetUpOperation = new PlatformSetUpOperation();
            var setup = await platformSetUpOperation.GetModelAsync(t => t._id != null);
            setup.VideoUrl = url;
            await platformSetUpOperation.UpdateModelAsync(setup);
            RedisOperation.UpdateString("VideoUrl", url, 600);
            return Ok(new RecoverModel(RecoverEnum.成功, "设置成功"));
        }
    }
}