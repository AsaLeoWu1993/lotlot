using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    /// <summary>
    /// 商户飞单记录
    /// </summary>
    public class MerchantInternal : BaseModel
    {
        /// <summary>
        /// 商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 目标商户id
        /// </summary>
        public string TargetMerchantID { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 目标用户id
        /// </summary>
        public string TargetUserID { get; set; }

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
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 游戏名称
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public FlyEnum Status { get; set; } = FlyEnum.等待飞单;

        /// <summary>
        /// 期号
        /// </summary>
        public string Nper { get; set; }
    }

    public enum FlyEnum
    {
        等待飞单 = 1,
        飞单成功 = 2,
        飞单失败 = 3
    }
}
