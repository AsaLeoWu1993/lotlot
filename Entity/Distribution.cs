using Entity.BaccaratModel;
using System.Collections.Generic;

namespace Entity
{
    /// <summary>
    /// 任务分发
    /// </summary>
    public class Distribution : BaseModel
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public List<string> MerchantIDList { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string Nper { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType? GameType { get; set; }

        /// <summary>
        /// 视讯游戏类型
        /// </summary>
        public BaccaratGameType? VGameType { get; set; }

        /// <summary>
        /// 桌号
        /// </summary>
        public int ZNum { get; set; }
    }
}
