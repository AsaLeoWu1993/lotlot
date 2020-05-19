using System.Collections.Generic;

namespace Entity
{
    /// <summary>
    /// 机器人行为管理
    /// </summary>
    public class ShamRobotmanage : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 用户id
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool Check { get; set; }

        /// <summary>
        /// 绑定行为
        /// </summary>
        public List<GameBetType> GameCheckInfo { get; set; } = new List<GameBetType>()
        {
            new GameBetType(){ GameType = GameOfType.北京赛车 },
            new GameBetType(){ GameType = GameOfType.幸运飞艇 },
            new GameBetType(){ GameType = GameOfType.重庆时时彩 },
            new GameBetType(){ GameType = GameOfType.极速赛车 },
            new GameBetType(){ GameType = GameOfType.澳州10 },
            new GameBetType(){ GameType = GameOfType.澳州5 },
            new GameBetType(){ GameType = GameOfType.爱尔兰赛马 },
            new GameBetType(){ GameType = GameOfType.爱尔兰快5 },
            new GameBetType(){ GameType = GameOfType.幸运飞艇168 },
            new GameBetType(){ GameType = GameOfType.极速时时彩 },
        };
    }

    public class GameBetType
    {
        /// <summary>
        /// 游戏类型
        /// </summary>
        public GameOfType GameType { get; set; }

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Check { get; set; } = false;

        /// <summary>
        /// 行为id
        /// </summary>
        public string BehaviorID { get; set; }
    }

    #region 行为
    /// <summary>
    /// 机器人行为
    /// </summary>
    public class RobotBehavior : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 行为名称
        /// </summary>
        public string BehaviorName { get; set; }

        /// <summary>
        /// 攻击
        /// </summary>
        public RomIntervalTime Attack { get; set; } = new RomIntervalTime()
        {
            Check = true,
            StartTime = 15,
            EndTime = 250
        };

        /// <summary>
        /// 攻击查询
        /// </summary>
        public ProbabilityText AttackQuery { get; set; } = new ProbabilityText()
        {
            StartTime = 20,
            EndTime = 120
        };

        /// <summary>
        /// 停战查询
        /// </summary>
        public ProbabilityText ArmisticeQuery { get; set; } = new ProbabilityText()
        {
            StartTime = 15,
            EndTime = 60
        };

        /// <summary>
        /// 上分命令
        /// </summary>
        public FractionalCommand UpCmd { get; set; } =
            new FractionalCommand
            {
                Keyword = "上分",
                Limit = 500,
                Variety = 1000
            };

        /// <summary>
        /// 下分命令
        /// </summary>
        public FractionalCommand DownCmd { get; set; } =
            new FractionalCommand
            {
                Keyword = "下分",
                Limit = 2000,
                Variety = 1000
            };

        /// <summary>
        /// 低于分数停止
        /// </summary>
        public BelowStop StopCmd { get; set; } = new BelowStop()
        {
            Limit = 0
        };

        /// <summary>
        /// 结束下分
        /// </summary>
        public bool EndPoint { get; set; } = true;
    }

    /// <summary>
    /// 随机开始结束时间
    /// </summary>
    public class RomIntervalTime
    {
        /// <summary>
        /// 开始时间
        /// </summary>
        public int StartTime { get; set; } = 5;

        /// <summary>
        /// 结束时间
        /// </summary>
        public int EndTime { get; set; } = 40;

        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Check { get; set; } = true;
    }

    /// <summary>
    /// 机率
    /// </summary>
    public class ProbabilityText : RomIntervalTime
    {
        /// <summary>
        /// 概率
        /// </summary>
        public double Probability { get; set; } = 0.1;

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; set; } = "查";
    }

    /// <summary>
    /// 上下分分数命令
    /// </summary>
    public class FractionalCommand
    {
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Check { get; set; } = true;

        /// <summary>
        /// 分数界限
        /// </summary>
        public decimal Limit { get; set; }

        /// <summary>
        /// 变化分数
        /// </summary>
        public decimal Variety { get; set; } = 500;

        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; set; }
    }

    /// <summary>
    /// 低于分数停止
    /// </summary>
    public class BelowStop
    {
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool Check { get; set; } = true;

        /// <summary>
        /// 分数界限
        /// </summary>
        public decimal Limit { get; set; } = 200;
    }
    #endregion

    /// <summary>
    /// 机器人行为方案
    /// </summary>
    public class RobotProgram : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

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
        public DoubleEnum DoubleType { get; set; } = DoubleEnum.不翻倍;

        /// <summary>
        /// 金额设置 
        /// </summary>
        public string Amountset { get; set; } = "5,10,20,40,50,100,150,200";

        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; } = true;

        /// <summary>
        /// 下注类型
        /// </summary>
        public List<string> BetTypeList { get; set; } = new List<string>();
    }

    public enum DoubleEnum
    {
        不翻倍 = 1,
        中翻倍 = 2,
        不中翻倍 = 3
    }
}
