using Entity;
using System.Threading.Tasks;

namespace Operation.Abutment
{
    public partial class BetLimitOrdinaryOperation : MongoMiddleware<BetLimitOrdinary>
    {
        public async Task<BetLimitOrdinary> GetModelAsync(string merchantID, GameOfType gameType)
        {
            var model = await base.GetModelAsync(t => t.MerchantID == merchantID && t.GameType == gameType);
            if (model == null)
            {
                model = new BetLimitOrdinary()
                {
                    MerchantID = merchantID,
                    GameType = gameType
                };
                await base.InsertModelAsync(model);
            }
            return model;
        }
    }

    public partial class BetLimitSpecialOperation : MongoMiddleware<BetLimitSpecial>
    {
        public async Task<BetLimitSpecial> GetModelAsync(string merchantID, GameOfType gameType)
        {
            var model = await base.GetModelAsync(t => t.MerchantID == merchantID && t.GameType == gameType);
            if (model == null)
            {
                model = new BetLimitSpecial()
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
