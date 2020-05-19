using Entity;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Operation.Abutment
{
    /// <summary>
    /// 10球游戏
    /// </summary>
    public partial class OddsOrdinaryOperation : MongoMiddleware<OddsOrdinary>
    {
        public async Task<OddsOrdinary> GetModelAsync(string merchantID, GameOfType gameType)
        {
            var model = await base.GetModelAsync(t=>t.MerchantID == merchantID && t.GameType == gameType);
            if (model == null)
            {
                model = new OddsOrdinary()
                { 
                    MerchantID = merchantID,
                    GameType = gameType
                };
                await base.InsertModelAsync(model);
            }
            return model;
        }
    }

    /// <summary>
    /// 5球游戏
    /// </summary>
    public partial class OddsSpecialOperation : MongoMiddleware<OddsSpecial>
    {
        public async Task<OddsSpecial> GetModelAsync(string merchantID, GameOfType gameType)
        {
            var model = await base.GetModelAsync(t => t.MerchantID == merchantID && t.GameType == gameType);
            if (model == null)
            {
                model = new OddsSpecial()
                {
                    MerchantID = merchantID,
                    GameType = gameType
                };
                await base.InsertModelAsync(model);
            }
            return model;
        }
    }
}
