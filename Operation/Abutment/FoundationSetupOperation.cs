using Entity;
using Operation.Common;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Operation.Abutment
{
    public partial class FoundationSetupOperation : MongoMiddleware<FoundationSetup>
    {
        /// <summary>
        /// 根据商户id获取基础设置
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <returns></returns>
        public FoundationSetup GetFoundationByNo(string merchantID)
        {
            var model = base.GetModel(t => t.MerchantID == merchantID);
            if (model == null)
            {
                model = new FoundationSetup()
                { 
                    MerchantID = merchantID,
                    LotteryFrontTime = new List<LotteryItem>()
        {
            new LotteryItem()
            {
                Type = GameOfType.北京赛车,
                LotteryTime = 60
            },
            new LotteryItem()
            {
                Type = GameOfType.幸运飞艇,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.重庆时时彩,
                LotteryTime = 60
            },
            new LotteryItem()
            {
                Type = GameOfType.极速赛车,
                LotteryTime = 20
            },
            new LotteryItem()
            {
                Type = GameOfType.澳州10,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.澳州5,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.爱尔兰赛马,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.爱尔兰快5,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.幸运飞艇168,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.极速时时彩,
                LotteryTime = 20
            }
        }
                };
                base.InsertModel(model);
            }
            if (model.LotteryFrontTime.IsNull())
            {
                model.LotteryFrontTime = new List<LotteryItem>()
        {
            new LotteryItem()
            {
                Type = GameOfType.北京赛车,
                LotteryTime = 60
            },
            new LotteryItem()
            {
                Type = GameOfType.幸运飞艇,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.重庆时时彩,
                LotteryTime = 60
            },
            new LotteryItem()
            {
                Type = GameOfType.极速赛车,
                LotteryTime = 20
            },
            new LotteryItem()
            {
                Type = GameOfType.澳州10,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.澳州5,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.爱尔兰赛马,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.爱尔兰快5,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.幸运飞艇168,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.极速时时彩,
                LotteryTime = 20
            }
        };
                base.UpdateModel(model);
            }
            return model;
        }

        /// <summary>
        /// 根据商户id获取基础设置
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <returns></returns>
        public async Task<FoundationSetup> GetFoundationByNoAsync(string merchantID)
        {
            var model = await base.GetModelAsync(t => t.MerchantID == merchantID); ;
            if (model == null)
            {
                model = new FoundationSetup()
                {
                    MerchantID = merchantID,
                    LotteryFrontTime = new List<LotteryItem>()
        {
            new LotteryItem()
            {
                Type = GameOfType.北京赛车,
                LotteryTime = 60
            },
            new LotteryItem()
            {
                Type = GameOfType.幸运飞艇,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.重庆时时彩,
                LotteryTime = 60
            },
            new LotteryItem()
            {
                Type = GameOfType.极速赛车,
                LotteryTime = 20
            },
            new LotteryItem()
            {
                Type = GameOfType.澳州10,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.澳州5,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.爱尔兰赛马,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.爱尔兰快5,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.幸运飞艇168,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.极速时时彩,
                LotteryTime = 20
            }
        }
                };
                await base.InsertModelAsync(model);
            }
            if (model.LotteryFrontTime.IsNull())
            {
                model.LotteryFrontTime = new List<LotteryItem>()
        {
            new LotteryItem()
            {
                Type = GameOfType.北京赛车,
                LotteryTime = 60
            },
            new LotteryItem()
            {
                Type = GameOfType.幸运飞艇,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.重庆时时彩,
                LotteryTime = 60
            },
            new LotteryItem()
            {
                Type = GameOfType.极速赛车,
                LotteryTime = 20
            },
            new LotteryItem()
            {
                Type = GameOfType.澳州10,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.澳州5,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.爱尔兰赛马,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.爱尔兰快5,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.幸运飞艇168,
                LotteryTime = 30
            },
            new LotteryItem()
            {
                Type = GameOfType.极速时时彩,
                LotteryTime = 20
            }
        };
                await base.UpdateModelAsync(model);
            }
            return model;
        }

        /// <summary>
        /// 根据基础设置id和商户id获取基础设置
        /// </summary>
        /// <param name="id">基础设置id</param>
        /// <param name="merchantID">商户id</param>
        /// <returns></returns>
        public async Task<FoundationSetup> GetFoundationByIDAndNo(string id, string merchantID)
        {
            return await base.GetModelAsync(t => t.MerchantID == merchantID && t._id == id);
        }
    }

    public partial class VideoFoundationSetupOperation : MongoMiddleware<VideoFoundationSetup>
    {
        /// <summary>
        /// 查询视讯设置
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public VideoFoundationSetup GetFoundationByNo(string merchantID)
        {
            var result = base.GetModel(t => t.MerchantID == merchantID);
            if (result == null)
            {
                result = new VideoFoundationSetup()
                {
                    MerchantID = merchantID
                };
                base.InsertModel(result);
            }
            return result;
        }

        /// <summary>
        /// 查询视讯设置
        /// </summary>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public async Task<VideoFoundationSetup> GetFoundationByNoAsync(string merchantID)
        {
            var result = await base.GetModelAsync(t => t.MerchantID == merchantID);
            if (result == null)
            {
                result = new VideoFoundationSetup()
                {
                    MerchantID = merchantID
                };
                await base.InsertModelAsync(result);
            }
            return result;
        }
    }
}
