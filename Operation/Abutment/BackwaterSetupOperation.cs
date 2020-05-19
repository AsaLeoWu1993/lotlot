using Entity;
using System.Threading.Tasks;

namespace Operation.Abutment
{
    public partial class BackwaterSetupOperation : MongoMiddleware<BackwaterSetup>
    {
        /// <summary>
        /// 根据回水Id和安全码获取回水信息
        /// </summary>
        /// <param name="id">回水id</param>
        /// <param name="merchantID">商户id</param>
        /// <returns></returns>
        public async Task<BackwaterSetup> GetBackwaterByIDAndNo(string id, string merchantID)
        {
            return await base.GetModelAsync(t => t._id == id && t.MerchantID == merchantID);
        }
    }

    /// <summary>
    /// 视讯游戏回水
    /// </summary>
    public partial class VideoBackwaterSetupOperation : MongoMiddleware<VideoBackwaterSetup>
    {

    }
}
