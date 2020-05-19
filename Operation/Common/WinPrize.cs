using Entity;
using Entity.BaccaratModel;
using Entity.WebModel;
using MongoDB.Driver;
using Newtonsoft.Json;
using Operation.Abutment;
using Operation.Baccarat;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Operation.Common
{
    /// <summary>
    /// 中奖处理
    /// </summary>
    public static class WinPrize
    {
        private class BusinessAddress
        {
            public string Path { get; set; }

            public string MerchantID { get; set; }
        }

        /// <summary>
        /// /10球游戏开奖
        /// </summary>
        /// <param name="model"></param>
        /// <param name="gameType"></param>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task<List<WinningPrizeClass>> Win10Async(Lottery10 model, GameOfType gameType, string merchantID)
        {
            try
            {
                if (model == null)
                    throw new Exception(string.Format("{0}未采集到数据", Enum.GetName(typeof(GameOfType), (int)gameType)));
                var type = model.GetType();
                UserOperation userOperation = new UserOperation();
                UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
                MerchantOperation merchantOperation = new MerchantOperation();
                var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
                var result = new List<WinningPrizeClass>();
                #region 处理下注信息
                var address = await Utils.GetAddress(merchantID);
                UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
                #region 判断中奖
                var collection = userBetInfoOperation.GetCollection(merchantID);
                var userBetInfosFilter = userBetInfoOperation.Builder.Where(t => t.MerchantID == merchantID &&
                t.BetStatus == BetStatus.未开奖 && t.Nper == model.IssueNum && t.GameType == gameType);
                var userBetInfos = await collection.FindListAsync(userBetInfosFilter);
                var msg = string.Empty;
                var bill = string.Empty;
                if (userBetInfos.IsNull())
                {
                    #region 处理中奖数据和其它数据
                    if (string.IsNullOrEmpty(model.MerchantID))
                    {
                        msg = await Win10MessageAsync(model, gameType, merchantID, userBetInfos);
                        RedisOperation.SetHash("GameWinMessage", merchantID + Enum.GetName(typeof(GameOfType), gameType), msg);

                        bill = await GameOnlineInfo(merchantID, gameType, userBetInfos);
                        RedisOperation.SetHash("GameBill", merchantID + Enum.GetName(typeof(GameOfType), gameType), bill);
                    }
                    #endregion
                    return null;
                };
                OddsOrdinaryOperation oddsOrdinaryOperation = new OddsOrdinaryOperation();
                var oddsOrdinary = await oddsOrdinaryOperation.GetModelAsync(merchantID, gameType);
                var oddType = oddsOrdinary.GetType();
                #region 商户开奖
                foreach (var bet in userBetInfos)
                {
                    foreach (var remark in bet.BetRemarks)
                    {
                        var isWin = false;
                        foreach (var info in remark.OddBets)
                        {
                            if (info.BetStatus != BetStatusEnum.已投注) continue;
                            if (info.BetRule == BetTypeEnum.冠亚)
                            {
                                //冠亚和数字
                                if (info.BetNo.IsNumber())
                                {
                                    if (info.BetNo == model.Count.ToString())
                                    {
                                        isWin = true;
                                        var odd = (decimal)oddType.GetProperty("SNum" + info.BetNo).GetValue(oddsOrdinary);
                                        info.MediumBonus = info.BetMoney * odd;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                #region 冠亚和大小单双
                                else if (info.BetNo == "大")
                                {
                                    if (model.CountSize == SizeEnum.大)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsOrdinary.SDa;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "小")
                                {
                                    if (model.CountSize == SizeEnum.小)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsOrdinary.SXiao;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "单")
                                {
                                    if (model.Sindou == SindouEnum.单)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsOrdinary.SDan;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "双")
                                {
                                    if (model.Sindou == SindouEnum.双)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsOrdinary.SShuang;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                #endregion
                            }
                            else if ((int)info.BetRule >= (int)BetTypeEnum.第一名 && (int)info.BetRule <= (int)BetTypeEnum.第十名)
                            {
                                #region 定位数字
                                if (info.BetNo.IsNumber())
                                {
                                    isWin = true;
                                    var num = (int)info.BetRule - 1;
                                    var bollNum = Convert.ToInt32(type.GetProperty("Num" + num.ToString()).GetValue(model));
                                    if (Convert.ToInt32(info.BetNo) == bollNum)
                                    {
                                        var odd = (decimal)oddType.GetProperty("Num" + info.BetNo).GetValue(oddsOrdinary);
                                        info.MediumBonus = info.BetMoney * odd;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                #endregion
                                #region 定位大小单双
                                else if (info.BetNo == "大" || info.BetNo == "小" || info.BetNo == "单" || info.BetNo == "双")
                                {
                                    var num = (int)info.BetRule - 1;
                                    var bollNum = Convert.ToInt32(type.GetProperty("Num" + num.ToString()).GetValue(model));
                                    if (info.BetNo == "大")
                                    {
                                        if (bollNum > 5)
                                        {
                                            isWin = true;
                                            info.MediumBonus = info.BetMoney * oddsOrdinary.Da;
                                            info.BetStatus = BetStatusEnum.已中奖;
                                        }
                                        else info.BetStatus = BetStatusEnum.未中奖;
                                    }
                                    else if (info.BetNo == "小")
                                    {
                                        if (bollNum < 6)
                                        {
                                            isWin = true;
                                            info.MediumBonus = info.BetMoney * oddsOrdinary.Xiao;
                                            info.BetStatus = BetStatusEnum.已中奖;
                                        }
                                        else info.BetStatus = BetStatusEnum.未中奖;
                                    }
                                    else if (info.BetNo == "单")
                                    {
                                        if (bollNum % 2 != 0)
                                        {
                                            isWin = true;
                                            info.MediumBonus = info.BetMoney * oddsOrdinary.Dan;
                                            info.BetStatus = BetStatusEnum.已中奖;
                                        }
                                        else info.BetStatus = BetStatusEnum.未中奖;
                                    }
                                    else
                                    {
                                        if (bollNum % 2 == 0)
                                        {
                                            isWin = true;
                                            info.MediumBonus = info.BetMoney * oddsOrdinary.Shuang;
                                            info.BetStatus = BetStatusEnum.已中奖;
                                        }
                                        else info.BetStatus = BetStatusEnum.未中奖;
                                    }
                                }
                                #endregion
                                #region 龙虎
                                else if (info.BetNo == "龙")
                                {
                                    var num = (int)info.BetRule - 1;
                                    var bollNum = (int)type.GetProperty("DraTig" + num.ToString()).GetValue(model);
                                    if (bollNum == (int)DraTig.龙)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsOrdinary.Long;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "虎")
                                {
                                    var num = (int)info.BetRule - 1;
                                    var bollNum = (int)type.GetProperty("DraTig" + num.ToString()).GetValue(model);
                                    if (bollNum == (int)DraTig.虎)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsOrdinary.Hu;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                #endregion
                            }
                        }
                        if (isWin)
                        {
                            //添加日志
                            await userOperation.UpperScore(bet.UserID, merchantID,
                                remark.OddBets.Sum(t => t.MediumBonus), ChangeTargetEnum.中奖,
                                string.Format("【{0}】【{1}】中奖", Enum.GetName(typeof(GameOfType), gameType), model.IssueNum),
                                remark.Remark, remark.OddNum, OrderStatusEnum.中奖上分, gameType);

                            var winUser = new WinningPrizeClass()
                            {
                                MerchantID = merchantID,
                                UserID = bet.UserID
                            };
                            if (!result.Exists(t => t.MerchantID == winUser.MerchantID && t.UserID == bet.UserID))
                                result.Add(winUser);
                        }
                    }
                    bet.BetStatus = BetStatus.已开奖;
                    bet.AllMediumBonus = bet.BetRemarks.Sum(t => t.OddBets.Sum(t => t.MediumBonus));
                }
                #endregion
                //处理数据
                await collection.DeleteManyAsync(userBetInfoOperation.Builder.In(t => t._id, userBetInfos.Select(t => t._id).ToList()));
                await collection.InsertManyAsync(userBetInfos);

                #region 处理中奖数据和其它数据
                if (string.IsNullOrEmpty(model.MerchantID))
                {
                    msg = await Win10MessageAsync(model, gameType, merchantID, userBetInfos);
                    RedisOperation.SetHash("GameWinMessage", merchantID + Enum.GetName(typeof(GameOfType), gameType), msg);

                    bill = await GameOnlineInfo(merchantID, gameType, userBetInfos);
                    RedisOperation.SetHash("GameBill", merchantID + Enum.GetName(typeof(GameOfType), gameType), bill);
                }
                #endregion
                #endregion
                #endregion
                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        public class WinningPrizeClass
        {
            public string MerchantID { get; set; }
            public string UserID { get; set; }
        }

        /// <summary>
        /// 5球游戏开奖
        /// </summary>
        /// <param name="model">开奖记录</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户id</param>
        /// <returns></returns>
        public static async Task<List<WinningPrizeClass>> Win5Async(Lottery5 model, GameOfType gameType, string merchantID)
        {
            try
            {
                if (model == null)
                    throw new Exception(string.Format("{0}未采集到数据", Enum.GetName(typeof(GameOfType), (int)gameType)));
                var type = model.GetType();
                UserOperation userOperation = new UserOperation();
                UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
                MerchantOperation merchantOperation = new MerchantOperation();
                var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);
                var result = new List<WinningPrizeClass>();
                #region 中奖处理
                var address = await Utils.GetAddress(merchantID);
                UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
                #region 判断中奖
                var collection = userBetInfoOperation.GetCollection(merchantID);
                //var userBetInfos = userBetInfoOperation.GetModelList(t => t.MerchantID == merchantID &&
                //t.BetStatus == BetStatusEnum.已投注 && t.Nper == type.GetProperty("IssueNum").GetValue(model).ToString() && t.GameType == gameType);
                var userBetInfosFilter = userBetInfoOperation.Builder.Where(t => t.MerchantID == merchantID &&
                t.BetStatus == BetStatus.未开奖 && t.Nper == model.IssueNum && t.GameType == gameType);
                var userBetInfos = await collection.FindListAsync(userBetInfosFilter);
                var msg = string.Empty;
                var bill = string.Empty;
                if (userBetInfos.IsNull())
                {
                    #region 处理中奖数据和其它数据
                    if (string.IsNullOrEmpty(model.MerchantID))
                    {
                        msg = await Win5MessageAsync(model, gameType, merchantID, userBetInfos);
                        RedisOperation.SetHash("GameWinMessage", merchantID + Enum.GetName(typeof(GameOfType), gameType), msg);

                        bill = await GameOnlineInfo(merchantID, gameType, userBetInfos);
                        RedisOperation.SetHash("GameBill", merchantID + Enum.GetName(typeof(GameOfType), gameType), bill);
                    }
                    #endregion
                    return null;
                };
                //判断用户中奖
                List<UserBetInfo> winList = new List<UserBetInfo>();
                List<UserBetInfo> lostList = new List<UserBetInfo>();
                var bsonRoom = await Utils.GetRoomInfosAsync(merchantID, gameType);
                if (bsonRoom.KaiEquality == null)
                    bsonRoom.KaiEquality = KaiHeEnum.返还本金;
                //是否返回本金
                var flag = bsonRoom.KaiEquality == KaiHeEnum.返还本金;
                OddsSpecialOperation oddsSpecialOperation = new OddsSpecialOperation();
                var oddsSpecial = await oddsSpecialOperation.GetModelAsync(merchantID, gameType);
                var oddType = oddsSpecial.GetType();
                foreach (var bet in userBetInfos)
                {
                    foreach (var remark in bet.BetRemarks)
                    {
                        var isWin = false;
                        foreach (var info in remark.OddBets)
                        {
                            if (info.BetStatus != BetStatusEnum.已投注) continue;
                            #region 总和大小单双
                            if (info.BetRule == BetTypeEnum.总和)
                            {
                                if (info.BetNo == "大")
                                {
                                    if (model.CountSize == SizeEnum.大)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsSpecial.CDa;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "小")
                                {
                                    if (model.CountSize == SizeEnum.小)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsSpecial.CXiao;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "单")
                                {
                                    if (model.CountSinDou == SindouEnum.单)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsSpecial.CDan;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "双")
                                {
                                    if (model.CountSinDou == SindouEnum.双)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsSpecial.CShuang;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                            }
                            #endregion
                            #region 万个龙虎和
                            else if (info.BetRule == BetTypeEnum.龙)
                            {
                                var num = Convert.ToInt32(type.GetProperty("DraTig").GetValue(model));
                                if (num == (int)DraTig5.龙)
                                {
                                    isWin = true;
                                    info.MediumBonus = info.BetMoney * oddsSpecial.Long;
                                    info.BetStatus = BetStatusEnum.已中奖;
                                }
                                else
                                {
                                    info.BetStatus = BetStatusEnum.未中奖;
                                    if (flag && model.DraTig == DraTig5.和)
                                    {
                                        info.BetStatus = BetStatusEnum.开和退注;
                                        await userOperation.UpperScore(bet.UserID, merchantID,
                                            info.BetMoney, ChangeTargetEnum.系统,
                                            string.Format("【{0}】开和返回本金", Enum.GetName(typeof(GameOfType), gameType)),
                                            string.Format("【{0}】开和返回本金", Enum.GetName(typeof(GameOfType), gameType)),
                                            orderStatus: OrderStatusEnum.上分成功, gameType: gameType);
                                    }
                                }
                            }
                            else if (info.BetRule == BetTypeEnum.虎)
                            {
                                var num = Convert.ToInt32(type.GetProperty("DraTig").GetValue(model));
                                if (num == (int)DraTig5.虎)
                                {
                                    isWin = true;
                                    info.MediumBonus = info.BetMoney * oddsSpecial.Hu;
                                    info.BetStatus = BetStatusEnum.已中奖;
                                }
                                {
                                    info.BetStatus = BetStatusEnum.未中奖;
                                    if (flag && model.DraTig == DraTig5.和)
                                    {
                                        info.BetStatus = BetStatusEnum.开和退注;
                                        await userOperation.UpperScore(bet.UserID, merchantID,
                                            info.BetMoney, ChangeTargetEnum.系统,
                                            string.Format("【{0}】开和返回本金", Enum.GetName(typeof(GameOfType), gameType)),
                                            string.Format("【{0}】开和返回本金", Enum.GetName(typeof(GameOfType), gameType)),
                                            orderStatus: OrderStatusEnum.上分成功, gameType: gameType);
                                    }
                                }
                            }
                            else if (info.BetRule == BetTypeEnum.和)
                            {
                                var num = Convert.ToInt32(type.GetProperty("DraTig").GetValue(model));
                                if (num == (int)DraTig5.和)
                                {
                                    isWin = true;
                                    info.MediumBonus = info.BetMoney * oddsSpecial.He;
                                    info.BetStatus = BetStatusEnum.已中奖;
                                }
                                else
                                    info.BetStatus = BetStatusEnum.未中奖;
                            }
                            #endregion
                            #region 定位球
                            else if (info.BetRule == BetTypeEnum.第一球 || info.BetRule == BetTypeEnum.第二球 ||
                   info.BetRule == BetTypeEnum.第三球 || info.BetRule == BetTypeEnum.第四球 || info.BetRule == BetTypeEnum.第五球)
                            {
                                var num = (int)info.BetRule - 11;
                                var bollNum = Convert.ToInt32(type.GetProperty("Num" + num.ToString()).GetValue(model));
                                if (info.BetNo.IsNumber())
                                {
                                    if (Convert.ToInt32(info.BetNo) == bollNum)
                                    {
                                        isWin = true;
                                        var odd = (decimal)oddType.GetProperty("Num" + info.BetNo).GetValue(oddsSpecial);
                                        info.MediumBonus = info.BetMoney * odd;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "大")
                                {
                                    if (bollNum > 4)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsSpecial.Da;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "小")
                                {
                                    if (bollNum < 5)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsSpecial.Xiao;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "单")
                                {
                                    if (bollNum % 2 != 0)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsSpecial.Dan;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                                else if (info.BetNo == "双")
                                {
                                    if (bollNum % 2 == 0)
                                    {
                                        isWin = true;
                                        info.MediumBonus = info.BetMoney * oddsSpecial.Shuang;
                                        info.BetStatus = BetStatusEnum.已中奖;
                                    }
                                    else info.BetStatus = BetStatusEnum.未中奖;
                                }
                            }
                            #endregion
                            #region 前三 中三 后三
                            else if (info.BetRule == BetTypeEnum.前三 || info.BetRule == BetTypeEnum.中三 || info.BetRule == BetTypeEnum.后三)
                            {
                                decimal odd = 0;
                                if (info.BetNo == "豹子")
                                    odd = oddsSpecial.Baozi;
                                else if (info.BetNo == "顺子")
                                    odd = oddsSpecial.Shunzi;
                                else if (info.BetNo == "半顺")
                                    odd = oddsSpecial.Banshun;
                                else if (info.BetNo == "对子")
                                    odd = oddsSpecial.Duizi;
                                else if (info.BetNo == "杂六")
                                    odd = oddsSpecial.Zaliu;
                                var dic = GameBetsMessage.EnumToDictionary(typeof(RuleEnum));
                                var key = GameBetsMessage.GetEnumByStatus<RuleEnum>(dic[info.BetNo]);
                                switch (info.BetRule)
                                {
                                    case BetTypeEnum.前三:
                                        if ((int)type.GetProperty("Front3").GetValue(model) == (int)key)
                                        {
                                            isWin = true;
                                            info.MediumBonus = info.BetMoney * odd;
                                            info.BetStatus = BetStatusEnum.已中奖;
                                        }
                                        else info.BetStatus = BetStatusEnum.未中奖;
                                        break;
                                    case BetTypeEnum.中三:
                                        if ((int)type.GetProperty("Middle3").GetValue(model) == (int)key)
                                        {
                                            isWin = true;
                                            info.MediumBonus = info.BetMoney * odd;
                                            info.BetStatus = BetStatusEnum.已中奖;
                                        }
                                        else info.BetStatus = BetStatusEnum.未中奖;
                                        break;
                                    case BetTypeEnum.后三:
                                        if ((int)type.GetProperty("Back3").GetValue(model) == (int)key)
                                        {
                                            isWin = true;
                                            info.MediumBonus = info.BetMoney * odd;
                                            info.BetStatus = BetStatusEnum.已中奖;
                                        }
                                        else info.BetStatus = BetStatusEnum.未中奖;
                                        break;
                                }
                            }
                            #endregion
                        }
                        if (isWin)
                        {
                            //添加日志
                            await userOperation.UpperScore(bet.UserID, merchantID,
                            remark.OddBets.Sum(t => t.MediumBonus), ChangeTargetEnum.中奖,
                            string.Format("【{0}】【{1}】中奖", Enum.GetName(typeof(GameOfType), gameType), model.IssueNum),
                            remark.Remark, remark.OddNum, OrderStatusEnum.中奖上分, gameType);

                            var winUser = new WinningPrizeClass()
                            {
                                MerchantID = merchantID,
                                UserID = bet.UserID
                            };
                            if (!result.Exists(t => t.MerchantID == winUser.MerchantID && t.UserID == bet.UserID))
                                result.Add(winUser);
                        }
                    }
                    bet.BetStatus = BetStatus.已开奖;
                    bet.AllMediumBonus = bet.BetRemarks.Sum(t => t.OddBets.Sum(t => t.MediumBonus));
                }
                #endregion
                #endregion
                //处理数据
                await collection.DeleteManyAsync(userBetInfoOperation.Builder.In(t => t._id, userBetInfos.Select(t => t._id).ToList()));
                await collection.InsertManyAsync(userBetInfos);
                #region 处理中奖数据和其它数据
                if (string.IsNullOrEmpty(model.MerchantID))
                {
                    msg = await Win5MessageAsync(model, gameType, merchantID, userBetInfos);
                    RedisOperation.SetHash("GameWinMessage", merchantID + Enum.GetName(typeof(GameOfType), gameType), msg);

                    bill = await GameOnlineInfo(merchantID, gameType, userBetInfos);
                    RedisOperation.SetHash("GameBill", merchantID + Enum.GetName(typeof(GameOfType), gameType), bill);
                }
                #endregion
                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }

        /// <summary>
        /// 10球中奖核算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="gameType"></param>
        /// <param name="merchantID"></param>
        /// <param name="userBetInfos"></param>
        /// <returns></returns>
        public static async Task<string> Win10MessageAsync<T>(T model, GameOfType gameType, string merchantID, List<UserBetInfo> userBetInfos) where T : Lottery10
        {
            var foundation = RedisOperation.GetFoundationSetup(merchantID);
            if (foundation == null) return null;
            var type = model.GetType();
            //中奖号码
            List<string> winNums = new List<string>();
            for (int i = 1; i < 11; i++)
            {
                winNums.Add(type.GetProperty("Num" + i.ToString()).GetValue(model).ToString());
            }
            var strWinNums = string.Join(" ", winNums);
            var nper = model.IssueNum;
            var onlineList = await SealupMessage.GetPersonInfos(merchantID);
            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            //var userCount = await SealupMessage.GetAllPersonInfos(merchantID, gameType);
            var message = foundation.WinningDetails
                .Replace("{期号}", nper).Replace("{游戏}", Enum.GetName(typeof(GameOfType), gameType))
                .Replace("{当期期号}", GameHandle.GetGameNper(nper, gameType))
                .Replace("{玩家数量}", (onlineList.Count + room.Online).ToString())
                .Replace("{在线人数}", (onlineList.Count + room.Online).ToString())
                .Replace("{玩家总分}", onlineList.Sum(t => t.UserMoney).ToString())
                .Replace("{开奖信息}", strWinNums);

            //中奖名单  下注名单
            var winBetList = userBetInfos.FindAll(t => t.BetStatus != BetStatus.未开奖);
            UserOperation userOperation = new UserOperation();
            var userIDList = winBetList.Select(t => t.UserID).Distinct().ToList();
            var userList = await userOperation.GetModelListAsync(userOperation.Builder.In(t => t._id, userIDList));
            //Utils.Logger.Error(string.Format("结算 商户：{0} 游戏：{1}  期号：{2} 下注人数：{3}", merchantID, gameType, nper, userList.Count));
            if (message.Contains("{中奖记录}"))
            {
                List<string> result = new List<string>();
                foreach (var userID in userIDList)
                {
                    var user = userList.Find(t => t._id == userID);
                    StringBuilder sb = new StringBuilder();
                    var bet = winBetList.Find(t => t.UserID == userID);
                    if (bet.AllMediumBonus == 0) continue;
                    foreach (var remark in bet.BetRemarks)
                    {
                        foreach (var info in remark.OddBets)
                        {
                            if (info.BetStatus == BetStatusEnum.已中奖)
                            {
                                sb.Append(string.Format("{0}{1}/{2}={3}", Enum.GetName(typeof(BetTypeEnum), (int)info.BetRule),
                            info.BetNo, info.BetMoney.ToString(), info.MediumBonus.ToString()));
                            }
                        }
                    }
                    result.Add(string.Format("[{0}]{1}[{2}]", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, sb.ToString(), (bet.AllMediumBonus - bet.AllUseMoney).ToString()));
                }
                message = message.Replace("{中奖记录}", result.Count == 0 ? "" : string.Join("\r\n", result));
            }
            if (message.Contains("{结算记录}"))
            {
                List<string> result = new List<string>();
                foreach (var userID in userIDList)
                {
                    var user = userList.Find(t => t._id == userID);
                    var userBetList = userBetInfos.FindAll(t => t.UserID == userID);
                    StringBuilder sb = new StringBuilder();
                    var bet = winBetList.Find(t => t.UserID == userID);
                    foreach (var remark in bet.BetRemarks)
                    {
                        foreach (var info in remark.OddBets)
                        {
                            sb.Append(string.Format("{0}{1}/{2}={3}", Enum.GetName(typeof(BetTypeEnum), info.BetRule),
                            info.BetNo, info.BetMoney.ToString(), (info.MediumBonus - info.BetMoney).ToString()));
                        }
                    }
                    result.Add(string.Format("[{0}]{1}[{2}]", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, sb.ToString(), (bet.AllMediumBonus - bet.AllUseMoney).ToString()));
                }
                message = message.Replace("{结算记录}", result.Count == 0 ? "" : string.Join("\r\n", result));
            }
            if (message.Contains("{中奖}"))
            {
                List<string> result = new List<string>();
                foreach (var userID in userIDList)
                {
                    var user = userList.Find(t => t._id == userID);
                    var bet = winBetList.Find(t => t.UserID == userID);
                    foreach (var remark in bet.BetRemarks)
                    {
                        if (remark.OddBets.Exists(t => t.BetStatus == BetStatusEnum.已中奖))
                        {
                            result.Add(string.Format("[{0}][{1}]", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                                 bet.AllMediumBonus - bet.AllUseMoney));
                            break;
                        }

                    }
                }
                message = message.Replace("{中奖}", result.Count == 0 ? "" : string.Join("\r\n", result));
            }
            if (message.Contains("{结算}"))
            {
                //List<string> result = new List<string>();
                //foreach (var userID in userIDList)
                //{
                //    var user = userList.Find(t => t._id == userID);
                //    var bet = winBetList.Find(t => t.UserID == userID);
                //    result.Add(string.Format("[{0}][{1}]", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                //            bet.AllMediumBonus - bet.AllUseMoney));
                //}
                //message = message.Replace("{结算}", result.Count == 0 ? "" : string.Join("\r\n", result));
                //结算内容格式
                var format = foundation.Settlement;
                List<string> result = new List<string>();
                foreach (var bet in winBetList)
                {
                    var user = userList.Find(t => t._id == bet.UserID);
                    var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                    if (bet.AllMediumBonus > 0)
                    {
                        //中奖详细
                        var winBets = new List<string>();
                        foreach (var remark in bet.BetRemarks)
                        {
                            foreach (var info in remark.OddBets)
                            {
                                if (info.BetStatus == BetStatusEnum.已中奖)
                                {
                                    winBets.Add(string.Format("{0}-{1}-{2}={3}", Enum.GetName(typeof(BetTypeEnum), info.BetRule),
                                info.BetNo, info.BetMoney.ToString(), info.MediumBonus.ToString()));
                                }
                            }
                        }
                        var trans = foundation.Settlement.Replace("{玩家}", nickName)
                            .Replace("{当期盈亏}", (bet.AllMediumBonus - bet.AllUseMoney).ToString("#0.00"))
                            .Replace("{中奖详细}", string.Join(",", winBets));
                        result.Add(trans);
                    }
                    else
                        result.Add(foundation.NotSettlement.Replace("{玩家}", nickName)
                            .Replace("{当期盈亏}", (bet.AllMediumBonus - bet.AllUseMoney).ToString("#0.00")));
                }
                message = message.Replace("{结算}", string.Join("\r\n", result));
            }
            return message;
        }

        /// <summary>
        /// 5球开奖核算
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="gameType"></param>
        /// <param name="merchantID"></param>
        /// <param name="userBetInfos"></param>
        /// <returns></returns>
        public static async Task<string> Win5MessageAsync<T>(T model, GameOfType gameType, string merchantID, List<UserBetInfo> userBetInfos) where T : Lottery5
        {
            var foundation = RedisOperation.GetFoundationSetup(merchantID);
            if (foundation == null) return null;
            var type = model.GetType();
            //中奖号码
            List<string> winNums = new List<string>();
            for (int i = 1; i < 6; i++)
            {
                winNums.Add(type.GetProperty("Num" + i.ToString()).GetValue(model).ToString());
            }
            var strWinNums = string.Join(" ", winNums);

            var nper = model.IssueNum;
            var onlineList = await SealupMessage.GetPersonInfos(merchantID);
            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            var message = foundation.WinningDetails
                .Replace("{期号}", nper).Replace("{游戏}", Enum.GetName(typeof(GameOfType), (int)gameType))
                .Replace("{当期期号}", GameHandle.GetGameNper(nper, gameType))
                .Replace("{玩家数量}", (onlineList.Count + room.Online).ToString())
                .Replace("{在线人数}", (onlineList.Count + room.Online).ToString())
                .Replace("{玩家总分}", onlineList.Sum(t => t.UserMoney).ToString())
                .Replace("{开奖信息}", strWinNums);

            //中奖名单  下注名单
            var winBetList = userBetInfos.FindAll(t => t.BetStatus == BetStatus.已开奖);
            UserOperation userOperation = new UserOperation();
            var userIDList = winBetList.Select(t => t.UserID).Distinct().ToList();
            var userList = await userOperation.GetModelListAsync(userOperation.Builder.In(t => t._id, userIDList));
            //Utils.Logger.Error(string.Format("结算 商户：{0} 游戏：{1}  期号：{2} 下注人数：{3}", merchantID, gameType, nper, userList.Count));
            if (message.Contains("{中奖记录}"))
            {
                List<string> result = new List<string>();
                foreach (var userID in userIDList)
                {
                    var user = userList.Find(t => t._id == userID);
                    var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                    StringBuilder sb = new StringBuilder();
                    var bet = winBetList.Find(t => t.UserID == userID);
                    if (bet.AllMediumBonus == 0) continue;
                    foreach (var remark in bet.BetRemarks)
                    {
                        foreach (var info in remark.OddBets)
                        {
                            if (info.BetStatus == BetStatusEnum.已中奖)
                            {
                                sb.Append(string.Format("{0}{1}/{2}={3}", Enum.GetName(typeof(BetTypeEnum), (int)info.BetRule),
                            info.BetNo, info.BetMoney.ToString(), info.MediumBonus.ToString()));
                            }
                        }
                    }
                    result.Add(string.Format("[{0}]{1}[{2}]", nickName, sb.ToString(), (bet.AllMediumBonus - bet.BetRemarks.Sum(t => t.OddBets.FindAll(x => x.BetStatus != BetStatusEnum.开和退注).Sum(x => x.BetMoney))).ToString()));
                }
                message = message.Replace("{中奖记录}", result.Count == 0 ? "" : string.Join("\r\n", result));
            }
            if (message.Contains("{结算记录}"))
            {
                List<string> result = new List<string>();
                foreach (var userID in userIDList)
                {
                    var user = userList.Find(t => t._id == userID);
                    var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                    var userBetList = userBetInfos.FindAll(t => t.UserID == userID);
                    StringBuilder sb = new StringBuilder();
                    var bet = winBetList.Find(t => t.UserID == userID);
                    foreach (var remark in bet.BetRemarks)
                    {
                        foreach (var info in remark.OddBets)
                        {
                            sb.Append(string.Format("{0}{1}/{2}={3}", Enum.GetName(typeof(BetTypeEnum), info.BetRule),
                            info.BetNo, info.BetMoney.ToString(), (info.MediumBonus - info.BetMoney).ToString()));
                        }
                    }
                    result.Add(string.Format("[{0}]{1}[{2}]", nickName, sb.ToString(), (bet.AllMediumBonus - bet.BetRemarks.Sum(t => t.OddBets.FindAll(x => x.BetStatus != BetStatusEnum.开和退注).Sum(x => x.BetMoney))).ToString()));
                }
                message = message.Replace("{结算记录}", result.Count == 0 ? "" : string.Join("\r\n", result));
            }
            if (message.Contains("{中奖}"))
            {
                List<string> result = new List<string>();
                foreach (var userID in userIDList)
                {
                    var user = userList.Find(t => t._id == userID);
                    var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                    var bet = winBetList.Find(t => t.UserID == userID);
                    foreach (var remark in bet.BetRemarks)
                    {
                        if (remark.OddBets.Exists(t => t.BetStatus == BetStatusEnum.已中奖))
                        {
                            result.Add(string.Format("[{0}][{1}]", nickName,
                                 (bet.AllMediumBonus - bet.BetRemarks.Sum(t => t.OddBets.FindAll(x => x.BetStatus != BetStatusEnum.开和退注).Sum(x => x.BetMoney))).ToString()));
                            break;
                        }

                    }
                }
                message = message.Replace("{中奖}", result.Count == 0 ? "" : string.Join("\r\n", result));
            }
            if (message.Contains("{结算}"))
            {
                //List<string> result = new List<string>();
                //foreach (var userID in userIDList)
                //{
                //    var user = userList.Find(t => t._id == userID);
                //    var bet = winBetList.Find(t => t.UserID == userID);
                //    result.Add(string.Format("[{0}][{1}]", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                //            bet.AllMediumBonus - bet.AllUseMoney));
                //}
                //message = message.Replace("{结算}", result.Count == 0 ? "" : string.Join("\r\n", result));
                //结算内容格式
                List<string> result = new List<string>();
                foreach (var bet in winBetList)
                {
                    var user = userList.Find(t => t._id == bet.UserID);
                    var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                    if (bet.AllMediumBonus > 0)
                    {
                        //中奖详细
                        var winBets = new List<string>();
                        foreach (var remark in bet.BetRemarks)
                        {
                            foreach (var info in remark.OddBets)
                            {
                                if (info.BetStatus == BetStatusEnum.已中奖)
                                {
                                    winBets.Add(string.Format("{0}-{1}-{2}={3}", Enum.GetName(typeof(BetTypeEnum), info.BetRule),
                                info.BetNo, info.BetMoney.ToString(), info.MediumBonus.ToString()));
                                }
                            }
                        }
                        var trans = foundation.Settlement.Replace("{玩家}", nickName)
                            .Replace("{当期盈亏}", (bet.AllMediumBonus - bet.BetRemarks.Sum(t => t.OddBets.FindAll(x => x.BetStatus != BetStatusEnum.开和退注).Sum(x => x.BetMoney))).ToString())
                            .Replace("{中奖详细}", string.Join(",", winBets));
                        result.Add(trans);
                    }
                    else
                        result.Add(foundation.NotSettlement.Replace("{玩家}", nickName)
                            .Replace("{当期盈亏}", (bet.AllMediumBonus - bet.BetRemarks.Sum(t => t.OddBets.FindAll(x => x.BetStatus != BetStatusEnum.开和退注).Sum(x => x.BetMoney))).ToString()));
                }
                message = message.Replace("{结算}", string.Join("\r\n", result));
            }
            return message;
        }

        /// <summary>
        /// 百家乐中奖核算
        /// </summary>
        /// <param name="model"></param>
        /// <param name="merchantID"></param>
        /// <returns></returns>
        public static async Task<string> WinBaccaratMessageAsync(BaccaratLottery model, string merchantID)
        {
            try
            {
                var foundation = RedisOperation.GetVideoFoundationSetup(merchantID);
                if (foundation == null) return null;
                var onlineList = await SealupMessage.GetPersonInfos(merchantID);
                RoomOperation roomOperation = new RoomOperation();
                var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
                //var userCount = await SealupMessage.GetAllPersonInfos(merchantID, gameType);
                var message = foundation.WinningDetails
                    .Replace("{期号}", model.IssueNum).Replace("{游戏}", Enum.GetName(typeof(BaccaratGameType), model.GameType))
                    .Replace("{当期期号}", model.IssueNum)
                    .Replace("{玩家数量}", (onlineList.Count + room.Online).ToString())
                    .Replace("{在线人数}", (onlineList.Count + room.Online).ToString())
                    .Replace("{玩家总分}", onlineList.Sum(t => t.UserMoney).ToString())
                    .Replace("{开奖信息}", Utils.GetDescriptionName(typeof(BaccaratWinEnum), model.Result));

                var address = await Utils.GetAddress(merchantID);
                BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
                var collection = baccaratBetOperation.GetCollection(merchantID);
                var winBetList = await collection.FindListAsync(t => t.Nper == model.IssueNum && t.ZNum == model.ZNum && t.GameType == model.GameType && t.BetStatus == BetStatus.已开奖);
                UserOperation userOperation = new UserOperation();
                var userIDList = winBetList.Select(t => t.UserID).Distinct().ToList();
                var userList = await userOperation.GetModelListAsync(userOperation.Builder.In(t => t._id, userIDList));
                //Utils.Logger.Error(string.Format("结算 商户：{0} 游戏：{1}  期号：{2} 下注人数：{3}", merchantID, gameType, nper, userList.Count));
                if (message.Contains("{中奖记录}"))
                {
                    List<string> result = new List<string>();
                    foreach (var userID in userIDList)
                    {
                        var user = userList.Find(t => t._id == userID);
                        StringBuilder sb = new StringBuilder();
                        var bet = winBetList.Find(t => t.UserID == userID);
                        if (bet.AllMediumBonus == 0) continue;
                        foreach (var remark in bet.BetRemarks)
                        {
                            if (remark.BetStatus == BaccaratBetEnum.已中奖)
                            {
                                sb.Append(string.Format("{0}/{1}={2}", Enum.GetName(typeof(BaccaratBetType), remark.BetRule),
                            remark.BetMoney.ToString(), remark.MediumBonus.ToString()));
                            }
                        }
                        result.Add(string.Format("[{0}]{1}[{2}]", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, sb.ToString(), (bet.AllMediumBonus - bet.AllUseMoney).ToString()));
                    }
                    message = message.Replace("{中奖记录}", result.Count == 0 ? "" : string.Join("\r\n", result));
                }
                if (message.Contains("{结算记录}"))
                {
                    List<string> result = new List<string>();
                    foreach (var userID in userIDList)
                    {
                        var user = userList.Find(t => t._id == userID);
                        var userBetList = winBetList.FindAll(t => t.UserID == userID);
                        StringBuilder sb = new StringBuilder();
                        var bet = winBetList.Find(t => t.UserID == userID);
                        foreach (var info in bet.BetRemarks)
                        {
                            sb.Append(string.Format("{0}/{1}={2}", Enum.GetName(typeof(BaccaratBetType), info.BetRule),
                                 info.BetMoney.ToString(), (info.MediumBonus - info.BetMoney).ToString()));
                        }
                        result.Add(string.Format("[{0}]{1}[{2}]", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, sb.ToString(), (bet.AllMediumBonus - bet.AllUseMoney).ToString()));
                    }
                    message = message.Replace("{结算记录}", result.Count == 0 ? "" : string.Join("\r\n", result));
                }
                if (message.Contains("{中奖}"))
                {
                    List<string> result = new List<string>();
                    foreach (var userID in userIDList)
                    {
                        var user = userList.Find(t => t._id == userID);
                        var bet = winBetList.Find(t => t.UserID == userID);
                        foreach (var remark in bet.BetRemarks)
                        {
                            if (remark.BetStatus == BaccaratBetEnum.已中奖)
                            {
                                result.Add(string.Format("[{0}][{1}]", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                                         bet.AllMediumBonus - bet.AllUseMoney));
                                break;
                            }
                        }
                    }
                    message = message.Replace("{中奖}", result.Count == 0 ? "" : string.Join("\r\n", result));
                }
                if (message.Contains("{结算}"))
                {
                    //List<string> result = new List<string>();
                    //foreach (var userID in userIDList)
                    //{
                    //    var user = userList.Find(t => t._id == userID);
                    //    var bet = winBetList.Find(t => t.UserID == userID);
                    //    result.Add(string.Format("[{0}][{1}]", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName,
                    //            bet.AllMediumBonus - bet.AllUseMoney));
                    //}
                    //message = message.Replace("{结算}", result.Count == 0 ? "" : string.Join("\r\n", result));
                    //结算内容格式
                    var format = foundation.Settlement;
                    List<string> result = new List<string>();
                    foreach (var bet in winBetList)
                    {
                        var user = userList.Find(t => t._id == bet.UserID);
                        var nickName = user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName;
                        if (bet.AllMediumBonus > 0)
                        {
                            //中奖详细
                            var winBets = new List<string>();
                            foreach (var info in bet.BetRemarks)
                            {
                                if (info.BetStatus == BaccaratBetEnum.已中奖)
                                {
                                    winBets.Add(string.Format("{0}-{1}={2}", Enum.GetName(typeof(BaccaratBetType), info.BetRule),
                                info.BetMoney.ToString(), info.MediumBonus.ToString()));
                                }
                            }
                            var trans = foundation.Settlement.Replace("{玩家}", nickName)
                                .Replace("{当期盈亏}", (bet.AllMediumBonus - bet.AllUseMoney).ToString("#0.00"))
                                .Replace("{中奖详细}", string.Join(",", winBets));
                            result.Add(trans);
                        }
                        else
                            result.Add(foundation.NotSettlement.Replace("{玩家}", nickName)
                                .Replace("{当期盈亏}", (bet.AllMediumBonus - bet.AllUseMoney).ToString("#0.00")));
                    }
                    message = message.Replace("{结算}", string.Join("\r\n", result));
                }
                return message;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                return null;
            }
        }

        /// <summary>
        /// 各个房间用户账单信息
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <param name="nper">游戏期号</param>
        /// <returns></returns>
        public static async Task<string> GameOnlineInfo(string merchantID, GameOfType gameType, List<UserBetInfo> userBetInfos)
        {
            UserOperation userOperation = new UserOperation();
            var foundation = RedisOperation.GetFoundationSetup(merchantID);
            if (foundation == null) return null;
            var bsonData = await Utils.GetRoomInfosAsync(merchantID, gameType);
            if (bsonData == null) return null;
            var roomName = bsonData.GameRoomName;
            var msg = foundation.MembershipScore;
            var result = new List<string>();
            var allPerson = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID
            && (t.Status == UserStatusEnum.正常 || t.Status == UserStatusEnum.假人));

            allPerson = allPerson.OrderByDescending(t => t.UserMoney).ToList();
            //低于分数不显示
            decimal min = bsonData.MinimumAmount == null ? 1 : bsonData.MinimumAmount.Value < 1 ? 1 : bsonData.MinimumAmount.Value;
            var userIDList = userBetInfos.Select(t => t.UserID).Distinct().ToList();
            foreach (var user in allPerson)
            {
                if (userIDList.Contains(user._id))
                {
                    result.Add(string.Format("{0}:{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.UserMoney));
                }
                else if (user.UserMoney >= min)
                {
                    result.Add(string.Format("{0}:{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.UserMoney));
                }
            }
            if (foundation.ShowBillTable)
            {
                return JsonConvert.SerializeObject(new
                {
                    Bill = result
                });
            }
            #region 在线人数
            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            var count = allPerson.Count + room.Online;
            msg = msg.Replace("{在线人数}", count.ToString())
                .Replace("{玩家总分}", allPerson.Sum(t => t.UserMoney).ToString());
            #endregion

            msg = msg.Replace("{玩家数量}", count.ToString())
                .Replace("{微群名字}", roomName)
                .Replace("{账单内容}", string.Join("\r\n", result));
            return msg;
        }


        /// <summary>
        /// 视讯游戏账单
        /// </summary>
        /// <param name="merchantID"></param>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public static async Task<string> GameBaccaratOnlineInfo(string merchantID, BaccaratGameType gameType)
        {
            UserOperation userOperation = new UserOperation();
            var foundation = RedisOperation.GetVideoFoundationSetup(merchantID);
            if (foundation == null) return null;
            VideoRoomOperation videoRoomOperation = new VideoRoomOperation();
            var bsonData = await videoRoomOperation.GetModelAsync(t => t.MerchantID == merchantID && t.GameType == gameType);
            if (bsonData == null) return null;
            var roomName = bsonData.GameRoomName;
            var msg = foundation.MembershipScore;
            var result = new List<string>();
            var allPerson = await userOperation.GetModelListAsync(t => t.MerchantID == merchantID
            && (t.Status == UserStatusEnum.正常 || t.Status == UserStatusEnum.假人));
            allPerson = allPerson.OrderByDescending(t => t.UserMoney).ToList();
            //低于分数不显示
            decimal min = bsonData.MinimumAmount == null ? 1 : bsonData.MinimumAmount.Value < 1 ? 1 : bsonData.MinimumAmount.Value;
            ////查询注单
            //var address = await Utils.GetAddress(merchantID);
            //BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
            //var collection = baccaratBetOperation.GetCollection(merchantID);
            //var userBetInfos = await collection.FindListAsync(t => t.ZNum == znum && t.Nper == nper && t.BetStatus == BetStatus.已开奖);
            //var userIDList = userBetInfos.Select(t => t.UserID).Distinct().ToList();
            //foreach (var user in allPerson)
            //{
            //    if (userIDList.Contains(user._id))
            //    {
            //        result.Add(string.Format("{0}:{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.UserMoney));
            //    }
            //    else if (user.UserMoney >= min)
            //    {
            //        result.Add(string.Format("{0}:{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.UserMoney));
            //    }
            //}
            //if (foundation.ShowBillTable)
            //{
            //    return JsonConvert.SerializeObject(new
            //    {
            //        Bill = result
            //    });
            //}
            foreach (var user in allPerson)
            {
                if (user.UserMoney >= min)
                {
                    result.Add(string.Format("{0}:{1}", user.ShowType ? user.NickName : string.IsNullOrEmpty(user.MemoName) ? user.NickName : user.MemoName, user.UserMoney));
                }
            }
            if (foundation.ShowBillTable)
            {
                return JsonConvert.SerializeObject(new
                {
                    Bill = result
                });
            }
            #region 在线人数
            RoomOperation roomOperation = new RoomOperation();
            var room = await roomOperation.GetModelAsync(t => t.MerchantID == merchantID);
            var count = allPerson.Count + room.Online;
            msg = msg.Replace("{在线人数}", count.ToString())
                .Replace("{玩家总分}", allPerson.Sum(t => t.UserMoney).ToString());
            #endregion

            msg = msg.Replace("{玩家数量}", count.ToString())
                .Replace("{微群名字}", roomName)
                .Replace("{账单内容}", string.Join("\r\n", result));
            return msg;
        }

        public class OnlinePersonInfo
        {
            public string UserID { get; set; }
            public string NickName { get; set; }
            public decimal UserMoney { get; set; }
        }

        #region 百家乐开奖
        public static async Task<List<WinningPrizeClass>> WinBaccaratAsync(BaccaratLottery model, string merchantID)
        {
            try
            {
                UserOperation userOperation = new UserOperation();
                var merchantOperation = new MerchantOperation();
                var result = new List<WinningPrizeClass>();
                var merchant = await merchantOperation.GetModelAsync(t => t._id == merchantID);

                var address = await Utils.GetAddress(merchantID);
                BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
                #region 处理下注信息
                var userIntegrationOperation = new UserIntegrationOperation();
                VideoRoomOperation videoRoomOperation = new VideoRoomOperation();
                var room = await videoRoomOperation.GetModelAsync(t => t.MerchantID == merchantID && t.GameType == model.GameType);
                #region 判断中奖
                var collection = baccaratBetOperation.GetCollection(merchantID);
                var betFilter = baccaratBetOperation.Builder.Where(t => t.MerchantID == merchantID && t.ZNum == model.ZNum &&
                t.BetStatus == BetStatus.未开奖 && t.Nper == model.IssueNum && t.GameType == model.GameType);
                var userBetInfos = await collection.FindListAsync(betFilter);
                if (userBetInfos.IsNull())
                    return null;
                #region 未结
                if (model.Result == BaccaratWinEnum.未结 || model.Result == BaccaratWinEnum.退注)
                {
                    userBetInfos.ForEach(async t =>
                    {
                        t.BetStatus = BetStatus.已开奖;
                        t.BetRemarks.ForEach(async x =>
                        {
                            x.BetStatus = BaccaratBetEnum.已退注;
                            await userOperation.UpperScore(t.UserID, t.MerchantID,
                            x.BetMoney, ChangeTargetEnum.系统,
                            msg: string.Format("【{0}】【{1}】未结退注", Enum.GetName(typeof(BaccaratGameType), model.GameType), t.Nper),
                            remark: x.Remark, orderStatus: OrderStatusEnum.未结退注);
                        });
                        await collection.DeleteOneAsync(t => t._id == t._id);
                        await collection.InsertOneAsync(t);
                    });
                    return null;
                }
                #endregion
                OddsBaccaratOperation oddsBaccaratOperation = new OddsBaccaratOperation();
                var oddsBaccarat = await oddsBaccaratOperation.GetModelAsync(t => t.MerchantID == merchantID);
                #region 开奖
                foreach (var bet in userBetInfos)
                {
                    foreach (var info in bet.BetRemarks)
                    {
                        var isWin = false;
                        if (info.BetStatus != BaccaratBetEnum.已投注) continue;
                        #region 开奖
                        if (model.Result == BaccaratWinEnum.和)
                        {
                            if (info.BetRule == BaccaratBetType.庄 || info.BetRule == BaccaratBetType.闲)
                            {
                                if (room.KaiEquality == VideoKaiHeEnum.通杀)
                                {
                                    info.BetStatus = BaccaratBetEnum.未中奖;
                                }
                                else if (room.KaiEquality == VideoKaiHeEnum.退一半)
                                {
                                    info.BetStatus = BaccaratBetEnum.开和退一半;
                                    info.MediumBonus = info.BetMoney / 2;
                                }
                                else
                                {
                                    info.BetStatus = BaccaratBetEnum.开和退注;
                                    info.MediumBonus = info.BetMoney;
                                }
                                if (info.MediumBonus > 0)
                                {
                                    await userOperation.UpperScore(bet.UserID, bet.MerchantID,
                                    info.MediumBonus, ChangeTargetEnum.系统,
                                    msg: string.Format("【{0}】【{1}】开和退注", Enum.GetName(typeof(BaccaratGameType), model.GameType), bet.Nper),
                                    remark: info.Remark, orderStatus: OrderStatusEnum.开和退注);
                                }
                            }
                            continue;
                        }
                        if (info.BetRule == BaccaratBetType.和)
                        {
                            if (model.Result == BaccaratWinEnum.和 || model.Result == BaccaratWinEnum.和_庄对
                                || model.Result == BaccaratWinEnum.和_庄闲对 || model.Result == BaccaratWinEnum.和_闲对)
                            {
                                isWin = true;
                                var odd = oddsBaccarat.He;
                                info.MediumBonus = info.BetMoney * odd;
                                info.BetStatus = BaccaratBetEnum.已中奖;
                            }
                            else info.BetStatus = BaccaratBetEnum.未中奖;
                        }
                        else if (info.BetRule == BaccaratBetType.庄)
                        {
                            if (model.Result == BaccaratWinEnum.庄 || model.Result == BaccaratWinEnum.庄_庄对
                                || model.Result == BaccaratWinEnum.庄_庄闲对 || model.Result == BaccaratWinEnum.庄_闲对)
                            {
                                isWin = true;
                                var odd = oddsBaccarat.Banker;
                                info.MediumBonus = info.BetMoney * odd;
                                info.BetStatus = BaccaratBetEnum.已中奖;
                            }
                            else info.BetStatus = BaccaratBetEnum.未中奖;
                        }
                        else if (info.BetRule == BaccaratBetType.闲)
                        {
                            if (model.Result == BaccaratWinEnum.闲 || model.Result == BaccaratWinEnum.闲_庄对
                                || model.Result == BaccaratWinEnum.闲_庄闲对 || model.Result == BaccaratWinEnum.闲_闲对)
                            {
                                isWin = true;
                                var odd = oddsBaccarat.Player;
                                info.MediumBonus = info.BetMoney * odd;
                                info.BetStatus = BaccaratBetEnum.已中奖;
                            }
                            else info.BetStatus = BaccaratBetEnum.未中奖;
                        }
                        else if (info.BetRule == BaccaratBetType.庄对)
                        {
                            if (model.Result == BaccaratWinEnum.和_庄对
                                || model.Result == BaccaratWinEnum.和_庄闲对
                                || model.Result == BaccaratWinEnum.庄_庄对
                                || model.Result == BaccaratWinEnum.庄_庄闲对
                                || model.Result == BaccaratWinEnum.闲_庄对
                                || model.Result == BaccaratWinEnum.闲_庄闲对)
                            {
                                isWin = true;
                                var odd = oddsBaccarat.BankerPair;
                                info.MediumBonus = info.BetMoney * odd;
                                info.BetStatus = BaccaratBetEnum.已中奖;
                            }
                            else info.BetStatus = BaccaratBetEnum.未中奖;
                        }
                        else if (info.BetRule == BaccaratBetType.闲对)
                        {
                            if (model.Result == BaccaratWinEnum.和_闲对
                                || model.Result == BaccaratWinEnum.和_庄闲对
                                || model.Result == BaccaratWinEnum.庄_闲对
                                || model.Result == BaccaratWinEnum.庄_庄闲对
                                || model.Result == BaccaratWinEnum.闲_闲对
                                || model.Result == BaccaratWinEnum.闲_庄闲对)
                            {
                                isWin = true;
                                var odd = oddsBaccarat.PlayerPair;
                                info.MediumBonus = info.BetMoney * odd;
                                info.BetStatus = BaccaratBetEnum.已中奖;
                            }
                            else info.BetStatus = BaccaratBetEnum.未中奖;
                        }
                        else if (info.BetRule == BaccaratBetType.任意对子)
                        {
                            if (model.Result == BaccaratWinEnum.和_庄对
                                || model.Result == BaccaratWinEnum.和_闲对
                                || model.Result == BaccaratWinEnum.和_庄闲对
                                || model.Result == BaccaratWinEnum.庄_庄对
                                || model.Result == BaccaratWinEnum.庄_闲对
                                || model.Result == BaccaratWinEnum.庄_庄闲对
                                || model.Result == BaccaratWinEnum.闲_庄对
                                || model.Result == BaccaratWinEnum.闲_闲对
                                || model.Result == BaccaratWinEnum.闲_庄闲对)
                            {
                                isWin = true;
                                var odd = oddsBaccarat.AnyPair;
                                info.MediumBonus = info.BetMoney * odd;
                                info.BetStatus = BaccaratBetEnum.已中奖;
                            }
                            else info.BetStatus = BaccaratBetEnum.未中奖;
                        }
                        #endregion
                        if (isWin)
                        {
                            await userOperation.UpperScore(bet.UserID, bet.MerchantID,
                                info.MediumBonus, ChangeTargetEnum.系统,
                                msg: string.Format("【{0}】【{1}】中奖", Enum.GetName(typeof(BaccaratGameType), model.GameType), bet.Nper),
                                remark: info.Remark, orderStatus: OrderStatusEnum.中奖上分);
                            var winUser = new WinningPrizeClass()
                            {
                                MerchantID = merchantID,
                                UserID = bet.UserID
                            };
                            if (!result.Exists(t => t.MerchantID == winUser.MerchantID && t.UserID == bet.UserID))
                                result.Add(winUser);
                        }
                    }
                    bet.BetStatus = BetStatus.已开奖;
                    bet.AllMediumBonus = bet.BetRemarks.FindAll(t => t.BetStatus == BaccaratBetEnum.已中奖).Sum(t => t.MediumBonus);
                }
                //处理数据
                await collection.DeleteManyAsync(baccaratBetOperation.Builder.In(t => t._id, userBetInfos.Select(t => t._id).ToList()));
                await collection.InsertManyAsync(userBetInfos);
                #endregion
                #endregion
                #endregion
                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
            }
            return null;
        }
        #endregion
    }

    /// <summary>
    /// 游戏处理
    /// </summary>
    public static class GameHandle
    {
        /// <summary>
        /// 获取下一期期号
        /// </summary>
        /// <param name="lastNper">最新一期期号</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static string GetGameNper(string lastNper, GameOfType gameType)
        {
            switch (gameType)
            {
                case GameOfType.幸运飞艇:
                case GameOfType.幸运飞艇168:
                    if (lastNper.Substring(lastNper.Length - 3, 3) == "180")
                        return DateTime.Now.ToString("yyyyMMdd") + "001";
                    break;
                case GameOfType.重庆时时彩:
                    if (lastNper.Substring(lastNper.Length - 3, 3) == "059")
                        return DateTime.Now.ToString("yyyyMMdd") + "001";
                    break;
            }
            return (Convert.ToInt64(lastNper) + 1).ToString();
        }

        /// <summary>
        /// 获取游戏上一期
        /// </summary>
        /// <param name="lastNper">最后一期期号</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static string GetGamePreNper(string lastNper, GameOfType gameType)
        {
            lastNper = lastNper.Replace("-", "");
            switch (gameType)
            {
                case GameOfType.幸运飞艇:
                case GameOfType.幸运飞艇168:
                    if (lastNper.Substring(lastNper.Length - 3, 3) == "001")
                    {
                        var date = new DateTime(Convert.ToInt32(lastNper.Substring(0, 4)),
                            Convert.ToInt32(lastNper.Substring(4, 2)), Convert.ToInt32(lastNper.Substring(6, 2)));

                        return date.AddDays(-1).ToString("yyyyMMdd") + "180";
                    }
                    break;
                case GameOfType.重庆时时彩:
                    if (lastNper.Substring(lastNper.Length - 3, 3) == "001")
                    {
                        var date = new DateTime(Convert.ToInt32(lastNper.Substring(0, 4)),
                            Convert.ToInt32(lastNper.Substring(4, 2)), Convert.ToInt32(lastNper.Substring(6, 2)));

                        return date.AddDays(-1).ToString("yyyyMMdd") + "059";
                    }
                    break;
            }
            return (Convert.ToInt64(lastNper) - 1).ToString();
        }
    }
}
