using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.WebModel
{
    public class WebMerchantInternal
    {
        /// <summary>
        /// 下注信息
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 使用金额
        /// </summary>
        public decimal UseMoney { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public string GameType { get; set; }

        /// <summary>
        /// 游戏名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// 期号
        /// </summary>
        public string Nper { get; set; }
    }
}
