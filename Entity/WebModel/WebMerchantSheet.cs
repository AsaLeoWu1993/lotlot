using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.WebModel
{
    public class WebMerchantSheet
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string MerchantName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 安全码
        /// </summary>
        public string SeurityNo { get; set; }

        /// <summary>
        /// 房间号
        /// </summary>
        public string RoomNum { get; set; }

        /// <summary>
        /// 自动同步
        /// </summary>
        public bool AutoSyn { get; set; } = false;

        /// <summary>
        /// 封盘提前时间
        /// </summary>
        public int AdvanceTime { get; set; } = 0;

        /// <summary>
        /// 飞单状态
        /// </summary>
        public bool OpenSheet { get; set; } = false;

        /// <summary>
        /// 声音提示开关
        /// </summary>
        public bool Remind { get; set; } = false;

        /// <summary>
        /// 分数限制
        /// </summary>
        public decimal LowFraction { get; set; } = 0;
    }
}
