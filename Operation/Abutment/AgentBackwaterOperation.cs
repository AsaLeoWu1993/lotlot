using Entity;

namespace Operation.Abutment
{
    public partial class AgentBackwaterOperation : MongoMiddleware<AgentBackwater>
    {
        /// <summary>
        /// 根据用户id和安全码获取代理回水
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="merchantID">安全码</param>
        /// <returns></returns>
        public AgentBackwater GetAgentByUserIDAndNo(string userID, string merchantID)
        {
            return base.GetModel(t => t.UserID == userID && t.MerchantID == merchantID);
        }

        /// <summary>
        /// 根据代理回水id和安全码获取代理回水
        /// </summary>
        /// <param name="id">代理回水id</param>
        /// <param name="merchantID">安全码</param>
        /// <returns></returns>
        public AgentBackwater GetAgentByIDAndNo(string id, string merchantID)
        {
            return base.GetModel(t => t._id == id && t.MerchantID == merchantID);
        }
    }
}
