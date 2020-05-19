using Entity;
using Entity.BaccaratModel;
using MongoDB.Driver;
using Operation.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Operation.Abutment
{
    public partial class UserOperation : MongoMiddleware<User>
    {
        /// <summary>
        /// 用户上分
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="score">分数</param>
        /// <param name="changeTarget">上分类型</param>
        /// <param name="msg">信息</param>
        /// <param name="remark">备注</param>
        /// <param name="oddNum">单号</param>
        /// <param name="orderStatus">定单状态</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="videoGameType">视讯游戏类型</param>
        /// <returns>剩余积分</returns>
        public async Task<decimal> UpperScore(string userID, string merchantID, decimal score, ChangeTargetEnum changeTarget, string msg = "", string remark = "", string oddNum = "", OrderStatusEnum orderStatus = OrderStatusEnum.充值成功, GameOfType? gameType = null, BaccaratGameType? videoGameType = null)
        {
            var user = await GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) throw new Exception("未查询到相关用户！");
            //添加日志
            UserIntegration userIntegration = new UserIntegration()
            {
                Amount = score,
                MerchantID = merchantID,
                Balance = user.UserMoney + score,
                ChangeType = ChangeTypeEnum.上分,
                Message = msg,
                Remark = remark,
                ChangeTarget = changeTarget,
                UserID = user._id,
                OrderStatus = orderStatus,
                GameType = gameType,
                BaccaratGameType = videoGameType
            };
            if (!string.IsNullOrEmpty(oddNum))
                userIntegration.OddNum = oddNum;
            user.UserMoney += score;
            await UpdateModelAsync(user);
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            await userIntegrationOperation.InsertModelAsync(userIntegration);
            return user.UserMoney;
        }

        /// <summary>
        /// 用户下分
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="score">分数</param>
        /// <param name="changeTarget">上分类型</param>
        /// <param name="msg">信息</param>
        /// <param name="remark">备注</param>
        /// <param name="oddNum">单号</param>
        /// <param name="orderStatus">定单状态</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="videoGameType">视讯游戏类型</param>
        /// <returns>剩余积分</returns>
        public async Task<bool> LowerScore(string userID, string merchantID, decimal score, ChangeTargetEnum changeTarget, string msg = "", string remark = "", string oddNum = "", OrderStatusEnum orderStatus = OrderStatusEnum.充值成功, GameOfType? gameType = null, BaccaratGameType? videoGameType = null)
        {
            var user = await GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null) throw new Exception("未查询到相关用户！");
            if (user.UserMoney < score) return false;
            //添加日志
            UserIntegration userIntegration = new UserIntegration()
            {
                Amount = -score,
                MerchantID = merchantID,
                Balance = user.UserMoney - score,
                ChangeType = ChangeTypeEnum.下分,
                Message = msg,
                Remark = remark,
                ChangeTarget = changeTarget,
                UserID = user._id,
                OrderStatus = orderStatus,
                GameType = gameType,
                BaccaratGameType = videoGameType
            };
            if (!string.IsNullOrEmpty(oddNum))
                userIntegration.OddNum = oddNum;
            user.UserMoney -= score;
            await UpdateModelAsync(user);
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            await userIntegrationOperation.InsertModelAsync(userIntegration);
            return true;
        }

        /// <summary>
        /// 根据商户id获取代理用户列表
        /// </summary>
        /// <param name="merchantID">商户id</param>
        /// <returns></returns>
        public List<User> GetAgentUsersByNo(string merchantID)
        {
            return GetModelList(t => t.MerchantID == merchantID && t.IsAgent);
        }

        /// <summary>
        /// 获取用户余额
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="userID"></param>
        /// <returns></returns>
        public async Task<decimal> GetUserMoney(string merchantID, string userID)
        {
            var user = await GetModelAsync(t => t.MerchantID == merchantID
            && t._id == userID);
            return user == null ? 0 : user.UserMoney;
        }

        /// <summary>
        /// 添加新用户获取唯一码
        /// </summary>
        /// <returns></returns>
        public string GetNewUserOnlyCode()
        {
            var list = GetModelList(t => true, t => t.OnlyCode, false);
            if (list.IsNull()) return "10000";
            var codeList = list.Select(t => t.OnlyCode).OrderBy(t => int.Parse(t)).ToList();
            var code = Convert.ToInt32(codeList.Last());
            if (code < 10000) return "10000";
            return (++code).ToString();
        }
    }
}
