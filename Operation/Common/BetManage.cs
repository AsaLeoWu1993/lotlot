using Operation.Abutment;
using Operation.Baccarat;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Operation.Common
{
    public static class BetManage
    {
        /// <summary>
        /// 游戏下注表
        /// </summary>
        private static readonly Dictionary<string, UserBetInfoOperation> UserBetDic = new Dictionary<string, UserBetInfoOperation>();

        /// <summary>
        /// 游戏信息表
        /// </summary>
        private static readonly Dictionary<string, UserSendMessageOperation> UserMessageDic = new Dictionary<string, UserSendMessageOperation>();
        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <returns></returns>
        public static async Task BetInit()
        {
            DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
            var databaseList = await databaseAddressOperation.GetModelListAsync(t => t._id != null);
            foreach (var database in databaseList)
            {
                UserBetInfoOperation userBetInfoOperation = new UserBetInfoOperation(database.Address, database.DBName);
                if (UserBetDic.ContainsKey(database._id))
                    UserBetDic[database._id] = userBetInfoOperation;
                else
                    UserBetDic.Add(database._id, userBetInfoOperation);

                UserSendMessageOperation userSendMessageOperation = new UserSendMessageOperation(database.Address, database.DBName);
                if (UserMessageDic.ContainsKey(database._id))
                    UserMessageDic[database._id] = userSendMessageOperation;
                else
                    UserMessageDic.Add(database._id, userSendMessageOperation);
            }
        }

        /// <summary>
        /// 获取下注连接池
        /// </summary>
        /// <param name="databaseID">地址id</param>
        /// <returns></returns>
        public static async Task<UserBetInfoOperation> GetBetInfoOperation(string databaseID)
        {
            if (!UserBetDic.ContainsKey(databaseID))
            {
                await BetInit();
                return UserBetDic[databaseID];
            }
            var info = UserBetDic[databaseID];
            return info;
        }

        /// <summary>
        /// 获取房间信息连接池
        /// </summary>
        /// <param name="databaseID"></param>
        /// <returns></returns>
        public static async Task<UserSendMessageOperation> GetMessageOperation(string databaseID)
        {
            if (!UserMessageDic.ContainsKey(databaseID))
            {
                await BetInit();
                return UserMessageDic[databaseID];
            }
            var info = UserMessageDic[databaseID];
            return info;
        }

        #region 视讯游戏
        /// <summary>
        /// 游戏下注表
        /// </summary>
        private static readonly Dictionary<string, BaccaratBetOperation> UserBaccaratBetDic = new Dictionary<string, BaccaratBetOperation>();

        /// <summary>
        /// 初始化连接
        /// </summary>
        /// <returns></returns>
        public static async Task BaccaratBetInit()
        {
            DatabaseAddressOperation databaseAddressOperation = new DatabaseAddressOperation();
            var databaseList = await databaseAddressOperation.GetModelListAsync(t => t._id != null);
            foreach (var database in databaseList)
            {
                BaccaratBetOperation baccaratBetOperation = new BaccaratBetOperation(database.Address, database.DBName);
                if (UserBaccaratBetDic.ContainsKey(database._id))
                    UserBaccaratBetDic[database._id] = baccaratBetOperation;
                else
                    UserBaccaratBetDic.Add(database._id, baccaratBetOperation);
            }
        }

        /// <summary>
        /// 获取下注连接池
        /// </summary>
        /// <param name="databaseID">地址id</param>
        /// <returns></returns>
        public static async Task<BaccaratBetOperation> GetBaccaratBetOperation(string databaseID)
        {
            if (!UserBaccaratBetDic.ContainsKey(databaseID))
            {
                await BaccaratBetInit();
                return UserBaccaratBetDic[databaseID];
            }
            var info = UserBaccaratBetDic[databaseID];
            return info;
        }
        #endregion
    }
}
