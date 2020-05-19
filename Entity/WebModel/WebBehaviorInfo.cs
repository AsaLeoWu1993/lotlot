using System.Collections.Generic;

namespace Entity.WebModel
{
    public class WebShamRobot
    {
        /// <summary>
        /// 绑定行为
        /// </summary>
        public List<GameBetType> GameCheckInfo { get; set; }
    }

    public class WebBehaviorInfo
    {
        public string BehaviorID { get; set; }

        public string BehaviorName { get; set; }
        /// <summary>
        /// 攻击
        /// </summary>
        public RomIntervalTime Attack { get; set; }

        /// <summary>
        /// 攻击查询
        /// </summary>
        public ProbabilityText AttackQuery { get; set; }

        /// <summary>
        /// 停战查询
        /// </summary>
        public ProbabilityText ArmisticeQuery { get; set; }

        /// <summary>
        /// 上分命令
        /// </summary>
        public FractionalCommand UpCmd { get; set; }

        /// <summary>
        /// 下分命令
        /// </summary>
        public FractionalCommand DownCmd { get; set; }

        /// <summary>
        /// 低于分数停止
        /// </summary>
        public BelowStop StopCmd { get; set; }

        /// <summary>
        /// 结束下分
        /// </summary>
        public bool EndPoint { get; set; }
    }

    public class WebRobotProgram
    {
        /// <summary>
        /// 方案id
        /// </summary>
        public string ProgramID { get; set; }

        /// <summary>
        /// 行为id
        /// </summary>
        public string BehaviorID { get; set; }

        /// <summary>
        /// 方案名称
        /// </summary>
        public string ProgramName { get; set; }

        /// <summary>
        /// 是否翻倍
        /// </summary>
        public DoubleEnum DoubleType { get; set; }

        /// <summary>
        /// 金额设置 
        /// </summary>
        public string Amountset { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }

        /// <summary>
        /// 下注类型
        /// </summary>
        public List<string> BetTypeList { get; set; }
    }
}
