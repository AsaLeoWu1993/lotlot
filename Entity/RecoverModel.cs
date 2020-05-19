using System.Collections.Generic;

namespace Entity
{
    /// <summary>
    /// 返回消息
    /// </summary>
    public class RecoverModel
    {
        public RecoverModel()
        { }

        public RecoverModel(RecoverEnum status, string message)
        {
            Status = status;
            Message = message;
        }
        /// <summary>
        /// 返回状态
        /// </summary>
        public RecoverEnum Status { get; set; }

        /// <summary>
        /// 返回消息
        /// </summary>
        public string Message { get; set; }
    }

    /// <summary>
    /// 返回关键字
    /// </summary>
    public class RecoverKeywordModel : RecoverModel
    {
        public RecoverKeywordModel() { }

        public RecoverKeywordModel(RecoverEnum status, string message, string keyword)
        {
            Status = status;
            Message = message;
            Keyword = keyword;
        }
        /// <summary>
        /// 关键字
        /// </summary>
        public string Keyword { get; set; }
    }

    /// <summary>
    /// 返回实体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RecoverClassModel<T> : RecoverModel where T : class
    {
        /// <summary>
        /// 实体
        /// </summary>
        public T Model { get; set; }
    }

    /// <summary>
    /// 返回数据集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RecoverListModel<T> : RecoverModel where T : class
    {
        /// <summary>
        /// 数据信息
        /// </summary>
        public List<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// 数据数量
        /// </summary>
        public long Total { get; set; }
    }

    public enum RecoverEnum
    {
        成功 = 100,
        未查询到相关数据 = 101,
        失败 = 200,
        参数错误 = 201,
        未授权 = 300,
        身份过期 = 301,
        身份冻结 = 302,
        异地登录 = 303,
        系统错误 = 400
    }
}
