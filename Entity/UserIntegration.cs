using Entity.BaccaratModel;

namespace Entity
{
    /// <summary>
    /// 用户积分变动表
    /// </summary>
    public sealed class UserIntegration : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 变动类型
        /// </summary>
        public ChangeTypeEnum ChangeType { get; set; }

        /// <summary>
        /// 变动级别
        /// </summary>
        public ChangeTargetEnum ChangeTarget { get; set; }

        /// <summary>
        /// 积分数量
        /// </summary>
        public decimal Amount { get; set; } = 0;

        /// <summary>
        /// 余额
        /// </summary>
        public decimal Balance { get; set; } = 0;

        /// <summary>
        /// 信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 操作对象
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 定单状态
        /// </summary>
        public OrderStatusEnum OrderStatus { get; set; }

        /// <summary>
        /// 管理操作
        /// </summary>
        public ManagementEnum Management { get; set; } = ManagementEnum.已同意;

        /// <summary>
        /// 单号
        /// </summary>
        public string OddNum { get; set; }

        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType? GameType { get; set; }

        /// <summary>
        /// 视讯游戏类型
        /// </summary>
        public BaccaratGameType? BaccaratGameType { get; set; }

        /// <summary>
        /// 视讯房间号
        /// </summary>
        public int? Znum { get; set; }

        /// <summary>
        /// 用户申请类型
        /// </summary>
        public NotesEnum Notes { get; set; } = NotesEnum.正常;
    }

    public enum ChangeTypeEnum
    {
        上分 = 1,
        下分 = 2
    }

    public enum ChangeTargetEnum
    {
        投注 = 1,
        中奖 = 2,
        系统 = 7,
        手动 = 8,
        申请 = 9,
        回水 = 10,
        退注 = 3,
        退一半 = 4
    }

    /// <summary>
    /// 定单状态
    /// </summary>
    public enum OrderStatusEnum
    {
        充值成功 = 1,
        充值失败 = 2,
        下分成功 = 3,
        下分失败 = 4,
        投注成功 = 5,
        投注失败 = 6,
        投注撤回 = 7,
        上分成功 = 8,
        上分失败 = 9,
        中奖上分 = 10,
        申请上分 = 11,
        申请下分 = 12,
        未结退注 = 13,
        开和退注 = 14
    }

    /// <summary>
    /// 管理操作
    /// </summary>
    public enum ManagementEnum
    {
        已同意 = 1,
        已拒绝 = 2,
        未审批 = 3
    }
}
