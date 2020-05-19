using Entity;

namespace Operation.Abutment
{
    public partial class MerchantOperation : MongoMiddleware<Merchant>
    {
        /// <summary>
        /// 根据商户名和安全码查询商户信息
        /// </summary>
        /// <param name="merchantName">商户名</param>
        /// <returns></returns>
        public Merchant GetMerchantInfoByName(string merchantName)
        {
            var result = base.GetModel(t => t.MeName == merchantName);
            return result;
        }
        /// <summary>
        /// 根据商户名和安全码查询商户信息
        /// </summary>
        /// <param name="merchantName">商户名</param>
        /// <param name="seurityNo">安全码</param>
        /// <returns></returns>
        public Merchant GetMerchantInfoByNameAndNo(string merchantName, string seurityNo)
        {
            var result = base.GetModel(t => t.MeName == merchantName || t.SeurityNo == seurityNo);
            return result;
        }
    }
}
