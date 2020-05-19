using Entity;
using System.Threading.Tasks;

namespace Operation.Abutment
{
    public partial class RoomOperation : MongoMiddleware<Room>
    {

        /// <summary>
        /// 根据商户id获取房间信息
        /// </summary>
        /// <param name="merchantID">商户Id</param>
        /// <returns></returns>
        public async Task<Room> GetRoomByMerchantID(string merchantID)
        {
            return await GetModelAsync(t => t.MerchantID == merchantID);
        }

        /// <summary>
        /// 根据房间id获取房间信息
        /// </summary>
        /// <param name="roomID">房间id</param>
        /// <returns></returns>
        public async Task<Room> GetRoomByID(string roomID)
        {
            return await base.GetModelAsync(t => t._id == roomID);
        }
    }
}
