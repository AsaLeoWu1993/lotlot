using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.AgentModel
{
    /// <summary>
    /// 虚拟货币
    /// </summary>
    public class VirtualCurrency : BaseModel
    {
        /// <summary>
        /// 代理id
        /// </summary>
        public string AgentID { get; set; }

        public List<CurrencyInfo> CurrencyInfo { get; set; } = new List<CurrencyInfo>();
    }

    public class CurrencyInfo
    { 
        /// <summary>
        /// 货币类型
        /// </summary>
        public CurrencyEnum CurrencyType { get; set; }

        /// <summary>
        /// 货币地址
        /// </summary>
        public string CurrencyPath { get; set; }
    }

    public enum CurrencyEnum
    {
        泰达币 = 1
    }
}
