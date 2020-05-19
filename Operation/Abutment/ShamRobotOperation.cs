using Entity;

namespace Operation.Abutment
{
    /// <summary>
    /// 机器人
    /// </summary>
    public partial class ShamRobotOperation : MongoMiddleware<ShamRobotmanage>
    {

    }

    /// <summary>
    /// 行为
    /// </summary>
    public partial class RobotBehaviorOperation : MongoMiddleware<RobotBehavior>
    {

    }

    /// <summary>
    /// 方案
    /// </summary>
    public partial class RobotProgramOperation : MongoMiddleware<RobotProgram>
    {

    }
}
