using Entity;
using Entity.BaccaratModel;
using Entity.GraspModel;
using MongoDB.Driver;
using Operation.Abutment;
using Operation.Baccarat;
using Operation.RedisAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static Operation.Common.Utils;

namespace Operation.Common
{
    /// <summary>
    /// 游戏算法
    /// </summary>
    public static class GameAlgorithms
    {
        /// <summary>
        /// 将抓取数据转换并添加到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grasp">抓取数据</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static T Algorithms10<T>(Grasp10 grasp, GameOfType gameType) where T : Lottery10, new()
        {
            if (grasp == null) return null;
            var data = new Lottery10()
            {
                IssueNum = grasp.IssueNum,
                Num1 = grasp.One,
                Num2 = grasp.Two,
                Num3 = grasp.Three,
                Num4 = grasp.Four,
                Num5 = grasp.Five,
                Num6 = grasp.Six,
                Num7 = grasp.Seven,
                Num8 = grasp.Eight,
                Num9 = grasp.Nine,
                Num10 = grasp.Ten,
                LotteryTime = grasp.AddTime,
                GameType = gameType
            };
            #region 算法
            data.Count = Convert.ToInt32(grasp.One) + Convert.ToInt32(grasp.Two);
            data.CountSize = data.Count > 11 ? SizeEnum.大 : SizeEnum.小;
            data.Sindou = data.Count % 2 == 0 ? SindouEnum.双 : SindouEnum.单;
            data.DraTig1 = Convert.ToInt32(data.Num1) > Convert.ToInt32(data.Num10) ? DraTig.龙 : DraTig.虎;
            data.DraTig2 = Convert.ToInt32(data.Num2) > Convert.ToInt32(data.Num9) ? DraTig.龙 : DraTig.虎;
            data.DraTig3 = Convert.ToInt32(data.Num3) > Convert.ToInt32(data.Num8) ? DraTig.龙 : DraTig.虎;
            data.DraTig4 = Convert.ToInt32(data.Num4) > Convert.ToInt32(data.Num7) ? DraTig.龙 : DraTig.虎;
            data.DraTig5 = Convert.ToInt32(data.Num5) > Convert.ToInt32(data.Num6) ? DraTig.龙 : DraTig.虎;
            #endregion
            T result = new T();
            var type = result.GetType();
            var dataType = data.GetType();
            foreach (var propertie in dataType.GetProperties())
            {
                type.GetProperty(propertie.Name).SetValue(result, propertie.GetValue(data));
            }
            return result;
        }

        /// <summary>
        /// 将抓取数据转换并添加到数据库
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="grasp">抓取数据</param>
        /// <param name="gameType">游戏类型</param>
        /// <returns></returns>
        public static T Algorithms5<T>(Grasp5 grasp, GameOfType gameType) where T : Lottery5, new()
        {
            if (grasp == null) return null;
            var data = new Lottery5()
            {
                IssueNum = grasp.IssueNum,
                Num1 = grasp.Wan,
                Num2 = grasp.Qian,
                Num3 = grasp.Bai,
                Num4 = grasp.Shi,
                Num5 = grasp.Ge,
                LotteryTime = grasp.AddTime,
                GameType = gameType
            };
            #region 算法
            data.Count = Convert.ToInt32(data.Num1) + Convert.ToInt32(data.Num2) + Convert.ToInt32(data.Num3) + Convert.ToInt32(data.Num4) + Convert.ToInt32(data.Num5);
            data.CountSize = data.Count < 23 ? SizeEnum.小 : SizeEnum.大;
            data.CountSinDou = data.Count % 2 == 0 ? SindouEnum.双 : SindouEnum.单;
            data.DraTig = Convert.ToInt32(data.Num1) == Convert.ToInt32(data.Num5) ? DraTig5.和 : Convert.ToInt32(data.Num1) < Convert.ToInt32(data.Num5) ? DraTig5.虎 : DraTig5.龙;
            data.Front3 = GetRule(data.Num1, data.Num2, data.Num3);
            data.Middle3 = GetRule(data.Num2, data.Num3, data.Num4);
            data.Back3 = GetRule(data.Num3, data.Num4, data.Num5);
            #endregion
            T result = new T();
            var type = result.GetType();
            var dataType = data.GetType();
            foreach (var propertie in dataType.GetProperties())
            {
                type.GetProperty(propertie.Name).SetValue(result, propertie.GetValue(data));
            }
            return result;
        }

        /// <summary>
        /// 获取规律
        /// </summary>
        /// <param name="num1">数字1</param>
        /// <param name="num2">数字2</param>
        /// <param name="num3">数字3</param>
        /// <returns></returns>
        private static RuleEnum GetRule(string num1, string num2, string num3)
        {
            int[] array = new int[] { Convert.ToInt32(num1), Convert.ToInt32(num2), Convert.ToInt32(num3) };
            Array.Sort(array);
            if (array[0] == array[1])
                return array[1] == array[2] ? RuleEnum.豹子 : RuleEnum.对子;
            else if (array[1] == array[2])
                return RuleEnum.对子;
            //判断第一球是否为0
            else if (array[0] == 0)
            {
                //第二球为1
                if (array[1] == 1)
                    return array[2] == 9 ? RuleEnum.顺子 : array[1] + 1 == array[2] ? RuleEnum.顺子 : RuleEnum.半顺;
                //第三球为9
                else if (array[2] == 9)
                    return array[2] - 1 == array[1] ? RuleEnum.顺子 : RuleEnum.半顺;
                else
                {
                    if (array[0] + 1 == array[1])
                        return array[1] + 1 == array[2] ? RuleEnum.顺子 : RuleEnum.半顺;
                    else if (array[1] + 1 == array[2])
                        return RuleEnum.半顺;
                    else
                        return RuleEnum.杂六;
                }
            }
            //第一球和第二球
            else if ((array[0] + 1 == 10 ? 0 : array[0] + 1) == array[1])
                return (array[1] + 1 == 10 ? 0 : array[1] + 1) == array[2] ? RuleEnum.顺子 : RuleEnum.半顺;
            else if (array[0] + 1 == array[1])
                return array[1] + 1 == array[2] ? RuleEnum.顺子 : RuleEnum.半顺;
            else if (array[1] + 1 == array[2])
                return RuleEnum.半顺;
            else
                return RuleEnum.杂六;
        }
    }

    /// <summary>
    /// 用户下注
    /// </summary>
    public static class GameBetsMessage
    {
        /// <summary>
        /// 用户下注状态
        /// </summary>
        public class GameBetStatus
        {
            /// <summary>
            /// 游戏状态
            /// </summary>
            public BetStatuEnum Status { get; set; }

            /// <summary>
            /// 输出
            /// </summary>
            public string OutPut { get; set; }

            /// <summary>
            /// 使用金额
            /// </summary>
            public decimal UseMoney { get; set; }

            /// <summary>
            /// 单号
            /// </summary>
            public string OddNum { get; set; }

            /// <summary>
            /// 注单信息
            /// </summary>
            public BetRemarkInfo BetInfos { get; set; }
        }
        /// <summary>
        /// 通常游戏
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="message">消息</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        /// <param name="notes">注单</param>
        /// <returns></returns>
        public static async Task<GameBetStatus> General(string userID, GameOfType gameType, string message, string merchantID, string nper, NotesEnum notes = NotesEnum.正常)
        {
            var result = new GameBetStatus()
            {
                Status = BetStatuEnum.格式错误
            };
            try
            {
                message = message.Trim('\n').Trim('\r');
                var list = new List<UserBetInfos>();
                if (message.Contains("和大") || message.Contains("和小") || message.Contains("和单") || message.Contains("和双"))
                {
                    #region 冠亚和大小单双
                    var replaceChar = message.Substring(0, 2);
                    if (string.IsNullOrEmpty(replaceChar))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    list.Add(new UserBetInfos()
                    {
                        Num = replaceChar.Replace("和", ""),
                        Integral = Convert.ToDecimal(message.Replace(replaceChar, "")),
                        Type = BetTypeEnum.冠亚,
                    });
                    #endregion
                }
                else if (message.Contains("和") && message.Contains("/"))
                {
                    #region 冠亚和值
                    var items = message.Replace("和", "").Split('/');
                    if (items.Length < 2)
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var nums = items[0];
                    if (string.IsNullOrEmpty(items[1]))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var integral = Convert.ToDecimal(items[1]);
                    var chars = nums.ToCharArray();
                    for (int i = 0; i < chars.Length; i++)
                    {
                        var num = chars[i];
                        var value = string.Empty;
                        if (num.ToString() == "2" || num.ToString() == "0")
                        {
                            result.OutPut = "书写格式错误，投注无效";
                            return result;
                        }
                        else if (num.ToString() == "1")
                        {
                            value = chars[i].ToString() + chars[i + 1].ToString();
                            ++i;
                        }
                        else
                            value = chars[i].ToString();
                        var userBetInfo = new UserBetInfos()
                        {
                            Num = value,
                            Type = BetTypeEnum.冠亚,
                            Integral = integral
                        };
                        list.Add(userBetInfo);
                    }
                    #endregion
                }
                else if (message.Contains("龙") || message.Contains("虎"))
                {
                    #region 龙虎处理  1-5 不重复
                    var splitChar = message.Contains("龙") ? '龙' : '虎';
                    var items = message.Split(splitChar);
                    var nums = items[0];
                    if (string.IsNullOrEmpty(items[1]))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var integral = Convert.ToDecimal(items[1]);
                    var chars = nums.ToCharArray();
                    if (chars.Length == 0) chars = "1".ToCharArray();
                    List<string> array = new List<string>();
                    for (int i = 0; i < chars.Length; i++)
                    {
                        var num = chars[i].ToString();
                        if (Convert.ToInt32(num) > 5 || Convert.ToInt32(num) < 0)
                        {
                            result.OutPut = "书写格式错误，投注无效";
                            return result;
                        }
                        array.Add(num);
                    }
                    if (IsSameWithArrayContains(array.ToArray()))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    foreach (var item in array)
                    {
                        BetTypeEnum @betEnum = BetTypeEnum.第一名;
                        if (item == "1") @betEnum = BetTypeEnum.第一名;
                        else if (item == "2") @betEnum = BetTypeEnum.第二名;
                        else if (item == "3") @betEnum = BetTypeEnum.第三名;
                        else if (item == "4") @betEnum = BetTypeEnum.第四名;
                        else if (item == "5") @betEnum = BetTypeEnum.第五名;
                        else
                        {
                            result.OutPut = "书写格式错误，投注无效";
                            return result;
                        }
                        var userBetInfo = new UserBetInfos()
                        {
                            Integral = integral,
                            Num = splitChar.ToString(),
                            Type = @betEnum
                        };
                        list.Add(userBetInfo);
                    }
                    #endregion
                }
                else if (message.Contains("大") || message.Contains("小") || message.Contains("单") || message.Contains("双"))
                {
                    #region 定位大小单双
                    var splitChar = message.Contains("大") ? '大' : message.Contains("小") ? '小' : message.Contains("单") ? '单' : '双';
                    var items = message.Split(splitChar);
                    var nums = items[0];
                    if (string.IsNullOrEmpty(items[1]))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var integral = Convert.ToDecimal(items[1]);
                    var chars = nums.ToCharArray();
                    if (chars.Length == 0) chars = "1".ToCharArray();
                    for (int i = 0; i < chars.Length; i++)
                    {
                        var num = chars[i].ToString();
                        var userBetInfo = new UserBetInfos()
                        {
                            Integral = integral,
                            Num = splitChar.ToString(),
                            Type = Get10BetEnumByNum(num)
                        };
                        list.Add(userBetInfo);
                    }
                    #endregion
                }
                else
                {
                    #region 定位数字
                    if (message.Split('/').Length == 2) message = "1/" + message;
                    var items = message.Split('/');
                    var ranks = items[0].ToCharArray();
                    var bolls = items[1].ToCharArray();
                    if (!items[0].IsNumber() || !items[1].IsNumber() || !items[2].IsNumber())
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    if (string.IsNullOrEmpty(items[2]))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var integral = Convert.ToDecimal(items[2]);
                    foreach (var rank in ranks)
                    {
                        var row = rank.ToString();
                        var betType = Get10BetEnumByNum(row);
                        foreach (var boll in bolls)
                        {
                            var col = boll.ToString();
                            if (col == "0")
                                col = "10";
                            var userBetInfo = new UserBetInfos()
                            {
                                Integral = integral,
                                Num = col,
                                Type = betType
                            };
                            list.Add(userBetInfo);
                        }
                    }
                    #endregion
                }
                string oddNum = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                result = await HandleBetNum(list, userID, gameType, merchantID, nper, oddNum, notes, message);
                //添加日志
                if (result.Status == BetStatuEnum.正常)
                {
                    var userOpertion = new UserOperation();
                    await userOpertion.LowerScore(userID, merchantID, list.Sum(t => t.Integral), ChangeTargetEnum.投注,
                         string.Format("投注{0}\r\n ({1})", Enum.GetName(typeof(GameOfType), (int)gameType), nper), message,
                         oddNum, OrderStatusEnum.投注成功, gameType);
                    result.UseMoney = list.Sum(t => t.Integral);
                }
                return result;
            }
            catch (Exception)
            {
                //Utils.Logger.Error(string.Format("游戏：{0}  下注信息：{1}  错误：{2}", gameType, message, e));
                result.Status = BetStatuEnum.格式错误;
                result.OutPut = "书写格式错误，投注无效";
                return result;
            }
        }

        /// <summary>
        /// 特殊游戏
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="message">消息</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        /// <param name="notes">注单</param>
        /// <returns></returns>
        public static async Task<GameBetStatus> Special(string userID, GameOfType gameType, string message, string merchantID, string nper, NotesEnum notes = NotesEnum.正常)
        {
            var result = new GameBetStatus()
            {
                Status = BetStatuEnum.格式错误
            };
            try
            {
                var list = new List<UserBetInfos>();
                #region 通买
                if (message.Contains("通买"))
                {
                    if (message.Contains("/"))
                    {
                        #region 通买数字
                        var items = message.Replace("通买", "").Split('/');
                        var nums = items[0];
                        if (string.IsNullOrEmpty(items[1]))
                        {
                            result.OutPut = "书写格式错误，投注无效";
                            return result;
                        }
                        var integral = Convert.ToDecimal(items[1]);
                        var chars = nums.ToCharArray();
                        for (int i = 0; i < chars.Length; i++)
                        {
                            var num = chars[i].ToString();
                            for (int j = 1; j < 6; j++)
                            {
                                var betType = Get5BetEnumByNum(j.ToString());
                                list.Add(new UserBetInfos()
                                {
                                    Integral = integral,
                                    Num = num,
                                    Type = betType
                                });
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region 通买前中后三
                        var key = message.Substring(2, 2);
                        if (string.IsNullOrEmpty(message.Remove(0, 4)))
                        {
                            result.OutPut = "书写格式错误，投注无效";
                            return result;
                        }
                        var integral = Convert.ToDecimal(message.Remove(0, 4));
                        list.Add(new UserBetInfos()
                        {
                            Integral = integral,
                            Num = key,
                            Type = BetTypeEnum.前三
                        });
                        list.Add(new UserBetInfos()
                        {
                            Integral = integral,
                            Num = key,
                            Type = BetTypeEnum.中三
                        });
                        list.Add(new UserBetInfos()
                        {
                            Integral = integral,
                            Num = key,
                            Type = BetTypeEnum.后三
                        });
                        #endregion
                    }
                }
                #endregion
                #region 总和
                else if (message.Contains("总和"))
                {
                    var betType = BetTypeEnum.总和;
                    var key = message.Substring(2, 1);
                    if(key != "大" && key != "小"&& key != "单" && key != "双")
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    if (string.IsNullOrEmpty(message.Remove(0, 3)))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var integral = Convert.ToDecimal(message.Remove(0, 3));
                    list.Add(new UserBetInfos()
                    {
                        Integral = integral,
                        Type = betType,
                        Num = key
                    });
                }
                #endregion
                #region 龙虎和
                else if (message.Contains("龙") || message.Contains("虎"))
                {
                    var key = message.Substring(0, 1);
                    var integral = Convert.ToDecimal(message.Remove(0, 1));
                    list.Add(new UserBetInfos()
                    {
                        Integral = integral,
                        Type = key == "龙" ? BetTypeEnum.龙
                        : BetTypeEnum.虎,
                        Num = ""
                    });
                }
                else if (message.Contains("和"))
                {
                    var key = message.Substring(0, 1);
                    var integral = Convert.ToDecimal(message.Remove(0, 1));
                    list.Add(new UserBetInfos()
                    {
                        Integral = integral,
                        Type = BetTypeEnum.和,
                        Num = ""
                    });
                }
                #endregion
                #region 前三中三后三
                else if (message.Contains("前三") || message.Contains("中三") || message.Contains("后三"))
                {
                    var dic = EnumToDictionary(typeof(BetTypeEnum));
                    var key = message.Substring(2, 2);
                    var betType = GetEnumByStatus<BetTypeEnum>(dic[message.Substring(0, 2)]);
                    if (string.IsNullOrEmpty(message.Remove(0, 4)))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var integral = Convert.ToDecimal(message.Remove(0, 4));
                    list.Add(new UserBetInfos()
                    {
                        Num = key,
                        Integral = integral,
                        Type = betType
                    });
                }
                #endregion
                #region 豹子对子顺子半顺杂六
                else if (message.Contains("豹子") || message.Contains("对子") || message.Contains("顺子")
                    || message.Contains("半顺") || message.Contains("杂六"))
                {
                    var key = message.Substring(0, 2);
                    if (string.IsNullOrEmpty(message.Remove(0, 2)))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var integral = Convert.ToDecimal(message.Remove(0, 2));
                    BetTypeEnum[] betTypes = { BetTypeEnum.前三, BetTypeEnum.中三, BetTypeEnum.后三 };
                    foreach (var betType in betTypes)
                    {
                        list.Add(new UserBetInfos()
                        {
                            Num = key,
                            Integral = integral,
                            Type = betType
                        });
                    }
                }
                #endregion
                #region 大小单双
                else if (message.Contains("大") || message.Contains("小") || message.Contains("单") || message.Contains("双"))
                {
                    var splitChar = message.Contains("大") ? '大' : message.Contains("小") ? '小' : message.Contains("单") ? '单' : '双';
                    var items = message.Split(splitChar);
                    var nums = items[0];
                    if (string.IsNullOrEmpty(items[1]))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var integral = Convert.ToDecimal(items[1]);
                    var chars = nums.ToCharArray();
                    if (chars.Length == 0) chars = "1".ToCharArray();
                    if (chars.Length > 5)
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    for (int i = 0; i < chars.Length; i++)
                    {
                        var num = chars[i].ToString();
                        if (Convert.ToInt32(num) > 5 && Convert.ToInt32(num) == 0)
                        {
                            result.OutPut = "书写格式错误，投注无效";
                            return result;
                        }
                        var userBetInfo = new UserBetInfos()
                        {
                            Integral = integral,
                            Num = splitChar.ToString(),
                            Type = Get5BetEnumByNum(num)
                        };
                        list.Add(userBetInfo);
                    }
                }
                #endregion
                #region 定位
                else
                {
                    if (message.Split('/').Length == 2) message = "1/" + message;
                    var items = message.Split('/');
                    var ranks = items[0].ToCharArray();
                    var bolls = items[1].ToCharArray();
                    if (!items[0].IsNumber() || !items[1].IsNumber() || !items[2].IsNumber())
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    if (string.IsNullOrEmpty(items[2]))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var integral = Convert.ToDecimal(items[2]);
                    foreach (var rank in ranks)
                    {
                        //球位置
                        var row = rank.ToString();
                        if (Convert.ToInt32(row) > 6 || row == "0")
                        {
                            result.OutPut = "书写格式错误，投注无效";
                            return result;
                        }
                        var betType = Get5BetEnumByNum(row);
                        foreach (var boll in bolls)
                        {
                            //球号
                            var col = boll.ToString();
                            var userBetInfo = new UserBetInfos()
                            {
                                Integral = integral,
                                Num = col,
                                Type = betType
                            };
                            list.Add(userBetInfo);
                        }
                    }
                }
                #endregion
                string oddNum = DateTime.Now.ToString("yyyyMMddHHmmssffffff");
                result = await HandleBetNum(list, userID, gameType, merchantID, nper, oddNum, notes, message);
                //添加日志
                if (result.Status == BetStatuEnum.正常)
                {
                    var userOpertion = new UserOperation();
                    await userOpertion.LowerScore(userID, merchantID, list.Sum(t => t.Integral), ChangeTargetEnum.投注,
                        string.Format("投注{0}\r\n ({1})", Enum.GetName(typeof(GameOfType),
                        (int)gameType), nper), message, oddNum, OrderStatusEnum.投注成功, gameType);
                    result.UseMoney = list.Sum(t => t.Integral);
                }
                return result;
            }
            catch (Exception)
            {
                //Utils.Logger.Error(string.Format("游戏：{0}  下注信息：{1}  错误：{2}", gameType, message, e));
                result.Status = BetStatuEnum.格式错误;
                result.OutPut = "书写格式错误，投注无效";
                return result;
            }
        }

        /// <summary>
        /// 用户锁
        /// </summary>
        private static Dictionary<string, object> lockdic = new Dictionary<string, object>();

        /// <summary>
        /// 处理用户投注信息 判断用户积分问题
        /// </summary>
        /// <param name="userBetInfos">下注信息</param>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        /// <param name="oddNum">单号</param>
        /// <returns></returns>
        private static async Task<GameBetStatus> HandleBetNum(List<UserBetInfos> userBetInfos, string userID, GameOfType gameType, string merchantID, string nper, string oddNum, NotesEnum notes = NotesEnum.正常, string message = null)
        {
            if (!lockdic.ContainsKey(userID))
                lockdic.Add(userID, new object());

            var address = await Utils.GetAddress(merchantID);
            var judLimits = await JudLimits(userBetInfos, gameType, merchantID, nper, userID);
            if (judLimits.Status != BetStatuEnum.正常)
                return judLimits;
            var userOpertion = new UserOperation();
            var user = userOpertion.GetModel(t => t._id == userID && t.MerchantID == merchantID);
            if (user == null)
            {
                judLimits.OutPut = "未查询到用户信息";
                judLimits.Status = BetStatuEnum.用户错误;
                return judLimits;
            }
            var count = userBetInfos.Sum(t => t.Integral);
            if (user.UserMoney < count)
            {
                judLimits.OutPut = "积分不足";
                judLimits.Status = BetStatuEnum.积分不足;
                return judLimits;
            }

            UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
            var collection = userBetInfoOperation.GetCollection(merchantID);

            //添加用户投注信息
            var list = new List<OddBetInfo>();
            foreach (var info in userBetInfos)
            {
                var data = new OddBetInfo()
                {
                    BetMoney = info.Integral,
                    BetNo = info.Num.Trim(),
                    BetRule = info.Type
                };
                list.Add(data);
            }
            var insertData = new BetRemarkInfo()
            {
                OddBets = list,
                OddNum = oddNum,
                Remark = message
            };
            lock (lockdic[userID])
            {
                var result = collection.FindOne(t => t.UserID == userID && t.Nper == nper && t.GameType == gameType && t.BetStatus == BetStatus.未开奖);
                if (result == null)
                {
                    result = new UserBetInfo()
                    {
                        MerchantID = merchantID,
                        UserID = userID,
                        GameType = gameType,
                        Notes = notes,
                        SendFly = user.Record,
                        Nper = nper,
                        BetRemarks = new List<BetRemarkInfo>()
                    {
                        insertData
                    },
                        AllUseMoney = insertData.OddBets.Sum(t => t.BetMoney)
                    };
                    collection.InsertOne(result);
                }
                else
                {
                    var betInfo = result.BetRemarks;
                    betInfo.Add(insertData);
                    result.BetRemarks = betInfo;
                    result.AllUseMoney = betInfo.Sum(x => x.OddBets.Sum(t => t.BetMoney));
                    collection.FindOneAndReplace(userBetInfoOperation.Builder.Eq(t => t._id, result._id), result);
                }
            }
            judLimits.Status = BetStatuEnum.正常;
            judLimits.OddNum = oddNum;
            judLimits.BetInfos = insertData;
            return judLimits;
        }

        /// <summary>
        /// 判断游戏投注上下限
        /// </summary>
        /// <param name="userBetInfos">下注列表</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        /// <param name="userID">用户id</param>
        /// <param name="output">输出值</param>
        /// <returns></returns>
        private static async Task<GameBetStatus> JudLimits(List<UserBetInfos> userBetInfos, GameOfType gameType, string merchantID, string nper, string userID)
        {
            var result = new GameBetStatus()
            {
                Status = BetStatuEnum.格式错误
            };
            try
            {
                if (userBetInfos.IsNull())
                {
                    result.OutPut = "书写格式错误，投注无效";
                    return result;
                }
                //整合数据
                userBetInfos = userBetInfos.GroupBy(t => new { t.Num, t.Type })
                    .Select(t => new UserBetInfos()
                    {
                        Num = t.Key.Num,
                        Type = t.Key.Type,
                        Integral = t.Sum(t => t.Integral)
                    }).ToList();
                foreach (var data in userBetInfos)
                {
                    if (data.Integral <= 0)
                    {
                        result.OutPut = "金额错误，下注失败";
                        return result;
                    }
                }
                //游戏玩法分类
                decimal playAllBetInfo = 0;
                decimal totalSingleLimit = 0;
                decimal allTotalQuotas = 0;
                if (!Utils.GameTypeItemize(gameType))
                {
                    BetLimitSpecialOperation betLimitSpecialOperation = new BetLimitSpecialOperation();
                    var betModel = await betLimitSpecialOperation.GetModelAsync(merchantID, gameType);
                    if (betModel == null)
                    {
                        result.OutPut = "金额错误，下注失败";
                        return result;
                    }
                    totalSingleLimit = betModel.TotalSingleLimit;
                    allTotalQuotas = betModel.AllTotalQuotas;

                    #region 所有玩家总限额
                    var userAllBetAmount = userBetInfos.Sum(t => t.Integral);
                    var address = await Utils.GetAddress(merchantID);
                    UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
                    var collection = userBetInfoOperation.GetCollection(merchantID);
                    var allBetInfo = await collection.FindListAsync(t => t.MerchantID == merchantID && t.GameType == gameType
                    && t.Nper == nper && t.BetStatus == BetStatus.未开奖);
                    if ((allBetInfo.Sum(t => t.AllUseMoney) + userAllBetAmount) > allTotalQuotas)
                    {
                        result.OutPut = string.Format("所有玩家所有玩法总限额{0}", allTotalQuotas);
                        result.Status = BetStatuEnum.限额;
                        return result;
                    }
                    //判断个人
                    var userBetInfo = allBetInfo.FindAll(t => t.UserID == userID);
                    if ((userBetInfo.Sum(t => t.AllUseMoney) + userAllBetAmount) > totalSingleLimit)
                    {
                        result.OutPut = string.Format("单个玩家所有玩法总限额{0}", totalSingleLimit);
                        result.Status = BetStatuEnum.限额;
                        return result;
                    }
                    #endregion

                    #region 判断
                    foreach (var bet in userBetInfos)
                    {
                        var betType = bet.Type;
                        var strType = bet.Num;
                        var integral = bet.Integral;
                        playAllBetInfo = allBetInfo.Sum(t => t.BetRemarks.Sum(t => t.OddBets.FindAll(t => t.BetRule == betType && t.BetNo == strType).Sum(t => t.BetMoney)));
                        var attrInfo = new QuotaAttrInfo();
                        #region 总和
                        if (betType == BetTypeEnum.总和)
                        {
                            attrInfo = betModel.GuessCountDxds;
                        }
                        else if (betType == BetTypeEnum.龙 || betType == BetTypeEnum.虎)
                            attrInfo = betModel.GuessLongHu;
                        else if (betType == BetTypeEnum.和)
                            attrInfo = betModel.GuessHe;
                        #endregion
                        #region 定位球
                        else if (betType == BetTypeEnum.第一球 || betType == BetTypeEnum.第二球 ||
                        betType == BetTypeEnum.第三球 || betType == BetTypeEnum.第四球 || betType == BetTypeEnum.第五球)
                        {
                            var num = ((int)BetTypeEnum.第一球 - 11).ToString();
                            if (strType.IsNumber())
                                attrInfo = betModel.GuessNum;
                            else
                                attrInfo = betModel.GuessDxds;
                        }
                        #endregion
                        #region 特殊
                        else if (betType == BetTypeEnum.前三 || betType == BetTypeEnum.中三 || betType == BetTypeEnum.后三)
                        {
                            if (strType == "豹子")
                                attrInfo = betModel.GuessBaozi;
                            else if (strType == "顺子")
                                attrInfo = betModel.GuessShunzi;
                            else if (strType == "半顺")
                                attrInfo = betModel.GuessBanshun;
                            else if (strType == "对子")
                                attrInfo = betModel.GuessDuizi;
                            else if (strType == "杂六")
                                attrInfo = betModel.GuessZaliu;
                            else
                            {
                                result.OutPut = "金额错误，下注失败";
                                result.Status = BetStatuEnum.格式错误;
                                return result;
                            }
                        }
                        else
                        {
                            result.OutPut = "金额错误，下注失败";
                            result.Status = BetStatuEnum.格式错误;
                            return result;
                        }
                        #endregion

                        //判断单人最大投注
                        var playUserBetInfo = userBetInfo.Sum(t => t.BetRemarks.Sum(t => t.OddBets.FindAll(t => t.BetRule == betType && t.BetNo == strType).Sum(t => t.BetMoney))); ;
                        if (playUserBetInfo + integral > attrInfo.MaxBet)
                        {
                            result.OutPut = string.Format("个人最大投注限额{0}", attrInfo.MaxBet);
                            result.Status = BetStatuEnum.限额;
                            return result;
                        }
                        //判断所有人最大投注
                        if (playAllBetInfo + integral > attrInfo.AllMaxBet)
                        {
                            result.OutPut = string.Format("所有人最大投注限额{0}", attrInfo.AllMaxBet);
                            result.Status = BetStatuEnum.限额;
                            return result;
                        }

                        //单注最小下注
                        if (attrInfo.MinBet > integral)
                        {
                            result.OutPut = string.Format("个人投注金额不可以小于{0}", attrInfo.MinBet);
                            result.Status = BetStatuEnum.限额;
                            return result;
                        }
                    }
                    #endregion
                }
                else
                {
                    BetLimitOrdinaryOperation betLimitOrdinaryOperation = new BetLimitOrdinaryOperation();
                    var betModel = await betLimitOrdinaryOperation.GetModelAsync(merchantID, gameType);
                    if (betModel == null)
                    {
                        result.OutPut = "金额错误，下注失败";
                        result.Status = BetStatuEnum.格式错误;
                        return result;
                    }
                    totalSingleLimit = betModel.TotalSingleLimit;
                    allTotalQuotas = betModel.AllTotalQuotas;

                    #region 所有玩家总限额
                    QuotaAttrInfo attrInfo = new QuotaAttrInfo();
                    var userAllBetAmount = userBetInfos.Sum(t => t.Integral);
                    var address = await Utils.GetAddress(merchantID);
                    UserBetInfoOperation userBetInfoOperation = await BetManage.GetBetInfoOperation(address);
                    var collection = userBetInfoOperation.GetCollection(merchantID);
                    var allBetInfo = await collection.FindListAsync(t => t.MerchantID == merchantID && t.GameType == gameType
                    && t.Nper == nper && t.BetStatus == BetStatus.未开奖);
                    //判断个人
                    var userBetInfo = allBetInfo.FindAll(t => t.UserID == userID);
                    if ((userBetInfo.Sum(t => t.BetRemarks.Sum(t => t.OddBets.Sum(t => t.BetMoney))) + userAllBetAmount) > totalSingleLimit)
                    {
                        result.OutPut = string.Format("单个玩家所有玩法总限额{0}", totalSingleLimit);
                        result.Status = BetStatuEnum.限额;
                        return result;
                    }
                    if ((allBetInfo.Sum(t => t.BetRemarks.Sum(t => t.OddBets.Sum(t => t.BetMoney))) + userAllBetAmount) > allTotalQuotas)
                    {
                        result.OutPut = string.Format("所有玩家所有玩法总限额{0}", allTotalQuotas);
                        result.Status = BetStatuEnum.限额;
                        return result;
                    }
                    #endregion

                    foreach (var bet in userBetInfos)
                    {
                        var betType = bet.Type;
                        var strType = bet.Num;
                        var integral = bet.Integral;
                        playAllBetInfo = allBetInfo.Sum(t => t.BetRemarks.Sum(t => t.OddBets.FindAll(t => t.BetRule == betType && t.BetNo == strType).Sum(t => t.BetMoney)));
                        attrInfo = new QuotaAttrInfo();
                        #region 定位数字
                        if ((int)betType >= (int)BetTypeEnum.第一名 && (int)betType <= (int)BetTypeEnum.第十名)
                        {
                            var num = ((int)BetTypeEnum.第一名 - 1).ToString();
                            //数字  则表示为下注的球号 定位猜数字
                            if (strType.IsNumber())
                                attrInfo = betModel.GuessNum;
                            else if (strType == "大" || strType == "小" || strType == "单" || strType == "双")
                                attrInfo = betModel.GuessDxds;
                            else if (strType == "龙" || strType == "虎")
                                attrInfo = betModel.GuessLongHu;
                            else
                            {
                                result.OutPut = "格式错误，下注失败";
                                result.Status = BetStatuEnum.格式错误;
                                return result;
                            }
                        }
                        #endregion
                        #region 冠亚
                        else if (betType == BetTypeEnum.冠亚)
                        {
                            if (strType.IsNumber())
                                attrInfo = betModel.GuessGYHNum;
                            else if (strType == "大" || strType == "小" || strType == "单" || strType == "双")
                                attrInfo = betModel.GuessGYHDxds;
                            else
                            {
                                result.OutPut = "格式错误，下注失败";
                                result.Status = BetStatuEnum.格式错误;
                                return result;
                            }
                        }
                        #endregion

                        //判断所有人最大投注
                        if (playAllBetInfo + integral > attrInfo.AllMaxBet)
                        {
                            result.OutPut = string.Format("所有人最大投注限额{0}", attrInfo.AllMaxBet);
                            result.Status = BetStatuEnum.限额;
                            return result;
                        }
                        //判断单人最大投注
                        var playUserBetInfo = userBetInfo.Sum(t => t.BetRemarks.Sum(t => t.OddBets.FindAll(t => t.BetRule == betType && t.BetNo == strType).Sum(t => t.BetMoney))); ;
                        if (playUserBetInfo + integral > attrInfo.MaxBet)
                        {
                            result.OutPut = string.Format("个人最大投注限额{0}", attrInfo.MaxBet);
                            result.Status = BetStatuEnum.限额;
                            return result;
                        }
                        //单注最小下注
                        if (attrInfo.MinBet > integral)
                        {
                            result.OutPut = string.Format("个人投注金额不可以小于{0}", attrInfo.MinBet);
                            result.Status = BetStatuEnum.限额;
                            return result;
                        }
                    }
                }
                result.Status = BetStatuEnum.正常;
                return result;
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                result.Status = BetStatuEnum.格式错误;
                return result;
            }
        }

        public enum BetStatuEnum
        {
            正常 = 1,
            积分不足 = 2,
            用户错误 = 3,
            格式错误 = 4,
            限额 = 5
        }

        /// <summary>
        /// 判断字符串是否是数字
        /// </summary>
        public static bool IsNumber(this string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            string pattern = @"^[0-9]*[0-9][0-9]*$";
            return Regex.IsMatch(s, pattern);
        }

        private class UserBetInfos
        {
            public string Num { get; set; }

            public decimal Integral { get; set; }

            public BetTypeEnum Type { get; set; }
        }

        /// <summary>
        /// 枚举转字典(无需获取描述时使用)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Dictionary<string, int> EnumToDictionary(Type type)
        {
            string[] Names = System.Enum.GetNames(type);

            Array Values = System.Enum.GetValues(type);

            var dic = new Dictionary<string, int>();

            for (int i = 0; i < Values.Length; i++)
            {
                dic.Add(Names[i].ToString(), (int)Values.GetValue(i));
            }

            return dic;
        }

        /// <summary>
        /// 获取枚举
        /// </summary>
        /// <typeparam name="T">枚举</typeparam>
        /// <param name="status">状态值</param>
        /// <returns></returns>
        public static T GetEnumByStatus<T>(int status)
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            return (T)Enum.ToObject(enumType, status);
        }

        public static bool IsSameWithArrayContains(string[] arr)
        {
            var newArr = new string[arr.Length];
            var idx = 0;
            foreach (var i in arr)
            {
                if (false == newArr.Contains(i))
                {
                    newArr[idx] = i;
                    idx++;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取10球游戏对应名次
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static BetTypeEnum Get10BetEnumByNum(string num)
        {
            if (num == "0") return BetTypeEnum.第十名;
            else if (num == "1") return BetTypeEnum.第一名;
            else if (num == "2") return BetTypeEnum.第二名;
            else if (num == "3") return BetTypeEnum.第三名;
            else if (num == "4") return BetTypeEnum.第四名;
            else if (num == "5") return BetTypeEnum.第五名;
            else if (num == "6") return BetTypeEnum.第六名;
            else if (num == "7") return BetTypeEnum.第七名;
            else if (num == "8") return BetTypeEnum.第八名;
            else if (num == "9") return BetTypeEnum.第九名;
            else throw new Exception("未知数字");
        }

        /// <summary>
        /// 获取5球游戏对应名次
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private static BetTypeEnum Get5BetEnumByNum(string num)
        {
            if (num == "1") return BetTypeEnum.第一球;
            else if (num == "2") return BetTypeEnum.第二球;
            else if (num == "3") return BetTypeEnum.第三球;
            else if (num == "4") return BetTypeEnum.第四球;
            else if (num == "5") return BetTypeEnum.第五球;
            else throw new Exception("未知数字");
        }

        #region 百家乐

        /// <summary>
        /// 百家乐下注
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="znum">桌号</param>
        /// <param name="message">消息</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        /// <param name="notes">注单状态</param>
        /// <returns></returns>
        public static async Task<GameBetStatus> Baccarat(string userID, int znum, string message, string merchantID, string nper, NotesEnum notes = NotesEnum.正常)
        {
            var result = new GameBetStatus()
            {
                Status = BetStatuEnum.格式错误
            };
            try
            {
                message = message.Trim('\n').Trim('\r');
                var keyword = string.Empty;
                if (message.Contains("庄对"))
                {
                    keyword = "庄对";
                }
                else if (message.Contains("闲对"))
                    keyword = "闲对";
                else if (message.Contains("任意对子"))
                    keyword = "任意对子";
                else if (message.Contains("和"))
                    keyword = "和";
                else if (message.Contains("庄"))
                    keyword = "庄";
                else if (message.Contains("闲"))
                    keyword = "闲";
                var enums = EnumToDictionary(typeof(BaccaratBetType));
                if (enums.ContainsKey(keyword))
                {
                    var type = enums[keyword];
                    var betType = GetEnumByStatus<BaccaratBetType>(type);
                    var money = message.Replace(keyword, "");
                    //金额
                    if (string.IsNullOrEmpty(money))
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var integral = Convert.ToDecimal(money);
                    if (integral <= 0)
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }

                    #region 判断金额
                    BetLimitBaccaratOperation betLimitBaccaratOperation = new BetLimitBaccaratOperation();
                    var betModel = await betLimitBaccaratOperation.GetModelAsync(t => t.MerchantID == merchantID);
                    if (betModel == null)
                    {
                        result.OutPut = "书写格式错误，投注无效";
                        return result;
                    }
                    var totalSingleLimit = betModel.TotalSingleLimit;
                    var allTotalQuotas = betModel.AllTotalQuotas;
                    var address = await GetAddress(merchantID);
                    BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
                    var collection = baccaratBetOperation.GetCollection(merchantID);
                    var allBetInfo = await collection.FindListAsync(t => t.MerchantID == merchantID && t.ZNum == znum
                    && t.Nper == nper && t.BetStatus == BetStatus.未开奖);
                    if ((allBetInfo.Sum(t => t.AllUseMoney) + integral) > allTotalQuotas)
                    {
                        result.OutPut = string.Format("所有玩家所有玩法总限额{0}", allTotalQuotas);
                        result.Status = BetStatuEnum.限额;
                        return result;
                    }
                    //判断个人
                    var userBetInfo = allBetInfo.FindAll(t => t.UserID == userID);
                    if ((userBetInfo.Sum(t => t.AllUseMoney) + integral) > totalSingleLimit)
                    {
                        result.OutPut = string.Format("单个玩家所有玩法总限额{0}", totalSingleLimit);
                        result.Status = BetStatuEnum.限额;
                        return result;
                    }
                    #endregion

                    #region 限额
                    QuotaAttrInfo attrInfo = new QuotaAttrInfo();
                    var playAllBetInfo = allBetInfo.FindAll(t => t.BetRemarks.Exists(x => x.BetRule == betType));
                    switch (betType)
                    {
                        case BaccaratBetType.和:
                            attrInfo = betModel.GuessHe;
                            break;
                        case BaccaratBetType.庄:
                        case BaccaratBetType.闲:
                            attrInfo = betModel.GuessQueue;
                            break;
                        case BaccaratBetType.庄对:
                        case BaccaratBetType.闲对:
                            attrInfo = betModel.GuessBPPair;
                            break;
                        case BaccaratBetType.任意对子:
                            attrInfo = betModel.GuessAPPair;
                            break;
                        default:
                            break;
                    };

                    //判断单人最大投注
                    var playUserBetInfo = playAllBetInfo.FindAll(t => t.UserID == userID);
                    if (playUserBetInfo.Sum(t => t.BetRemarks.Sum(t => t.BetMoney)) + integral > attrInfo.MaxBet)
                    {
                        result.OutPut = string.Format("个人最大投注限额{0}", attrInfo.MaxBet);
                        result.Status = BetStatuEnum.限额;
                        return result;
                    }

                    //判断所有人最大投注
                    if (playAllBetInfo.Sum(t => t.BetRemarks.Sum(t => t.BetMoney)) + integral > attrInfo.AllMaxBet)
                    {
                        result.OutPut = string.Format("所有人最大投注限额{0}", attrInfo.AllMaxBet);
                        result.Status = BetStatuEnum.限额;
                        return result;
                    }

                    //单注最小下注
                    if (attrInfo.MinBet > integral)
                    {
                        result.OutPut = string.Format("个人投注金额不可以小于{0}", attrInfo.MinBet);
                        result.Status = BetStatuEnum.限额;
                        return result;
                    }
                    #endregion

                    #region 确认下注
                    UserOperation userOperation = new UserOperation();
                    var user = userOperation.GetModel(t => t.MerchantID == merchantID && t._id == userID);
                    if (user.UserMoney < integral)
                    {
                        result.OutPut = "积分不足";
                        result.Status = BetStatuEnum.积分不足;
                        return result;
                    }
                    var baccaratBet = await collection.FindOneAsync(t => t.UserID == userID && t.Nper == nper && t.GameType == BaccaratGameType.百家乐 && t.ZNum == znum && t.BetStatus == BetStatus.未开奖);
                    var data = new BaccaratBetRemarkInfo()
                    {
                        BetMoney = integral,
                        BetRule = betType,
                        BetStatus = BaccaratBetEnum.已投注,
                        Remark = message
                    };
                    if (baccaratBet == null)
                    {
                        baccaratBet = new BaccaratBet()
                        {
                            MerchantID = merchantID,
                            BetStatus = BetStatus.未开奖,
                            UserID = userID,
                            Notes = notes,
                            Nper = nper,
                            ZNum = znum,
                            GameType = BaccaratGameType.百家乐,
                            BetRemarks = new List<BaccaratBetRemarkInfo>() { data },
                            AllUseMoney = data.BetMoney
                        };
                        await collection.InsertOneAsync(baccaratBet);
                    }
                    else
                    {
                        var betInfo = baccaratBet.BetRemarks;
                        betInfo.Add(data);
                        baccaratBet.BetRemarks = betInfo;
                        baccaratBet.AllUseMoney = betInfo.Sum(x => x.BetMoney);
                        await collection.FindOneAndReplaceAsync(baccaratBetOperation.Builder.Eq(t => t._id, baccaratBet._id), baccaratBet);
                    }
                    //添加记录
                    await userOperation.LowerScore(userID, merchantID, integral, ChangeTargetEnum.投注,
                        string.Format("投注{0}\r\n ({1})", Enum.GetName(typeof(BaccaratGameType), BaccaratGameType.百家乐), nper), message,
                        data.OddNum, OrderStatusEnum.投注成功, videoGameType: BaccaratGameType.百家乐);
                    result.UseMoney = integral;
                    result.OddNum = data.OddNum;
                    result.Status = BetStatuEnum.正常;
                    return result;
                    #endregion
                }
                else
                {
                    result.OutPut = "书写格式错误，投注无效";
                    return result;
                }
            }
            catch (Exception e)
            {
                Utils.Logger.Error(e);
                result.OutPut = "书写格式错误，投注无效";
                result.Status = BetStatuEnum.格式错误;
                return result;
            }
        }
        #endregion
    }

    /// <summary>
    /// 撤销 查询
    /// </summary>
    public static class CancelAnnouncement
    {
        /// <summary>
        /// 撤销单一下注
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        public static async Task<Tuple<string, decimal>> CancelOne(string userID, GameOfType gameType, string merchantID, string nper, ReplySetUp reply)
        {
            Tuple<string, decimal> returnResult;
            try
            {
                string output = string.Empty;
                var address = await Utils.GetAddress(merchantID);
                UserBetInfoOperation userBetInfoOperation = BetManage.GetBetInfoOperation(address).GetAwaiter().GetResult();
                var collection = userBetInfoOperation.GetCollection(merchantID);
                var data = await collection.FindOneAsync(t => t.UserID == userID && t.MerchantID == merchantID && t.Nper == nper && t.BetStatus == BetStatus.未开奖);
                if (data == null)
                {
                    output = await InstructionConversion(reply.NotOrders, userID, merchantID, gameType, nper);
                    returnResult = Tuple.Create<string, decimal>(output, 0);
                    return returnResult;
                }
                var result = data.BetRemarks.Last();
                //取消飞单
                //SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
                //sendFlyingOperation.UpdateSendFlyModel(merchantID, gameType, nper, oddList, false).GetAwaiter().GetResult();
                var key = merchantID + Enum.GetName(typeof(GameOfType), gameType);
                RedisOperation.DeleteCacheKey(key, result.OddNum);
                //添加日志
                var userOperation = new UserOperation();
                var userAmount = result.OddBets.Sum(t => t.BetMoney);
                await userOperation.UpperScore(userID, merchantID, userAmount, ChangeTargetEnum.投注, "投注撤回", "取消投注",
                    result.OddNum, OrderStatusEnum.投注撤回, gameType);
                //output = string.Format(@"【{0}回合】 取消
                //                          本期投注，返回积分
                //                         【{1}】， 剩余积分
                //                         【{2}】", data.Nper, oddList.Sum(t => t.BetMoney), surplus.Result);
                var updateData = data.BetRemarks;
                updateData.RemoveAll(t => t.OddNum == result.OddNum);
                if (updateData.IsNull())
                {
                    await collection.DeleteOneAsync(t => t._id == data._id);
                }
                else
                {
                    data.BetRemarks = updateData;
                    data.AllUseMoney = updateData.Sum(t => t.OddBets.Sum(t => t.BetMoney));
                    await collection.UpdateOneAsync(data);
                }

                output = await Utils.InstructionConversion(reply.CancelOrder, userID, merchantID, gameType, nper);
                returnResult = Tuple.Create(output, userAmount);
            }
            catch
            {
                var output = await Utils.InstructionConversion(reply.NotOrders, userID, merchantID, gameType, nper);
                returnResult = Tuple.Create<string, decimal>(output, 0);
            }
            return returnResult;
        }

        /// <summary>
        /// 撤销全部
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        public static async Task<Tuple<string, decimal>> CancelAll(string userID, GameOfType gameType, string merchantID, string nper, ReplySetUp reply)
        {
            Tuple<string, decimal> tuple;
            string output = string.Empty;
            try
            {
                var address = await Utils.GetAddress(merchantID);
                UserBetInfoOperation userBetInfoOperation = BetManage.GetBetInfoOperation(address).GetAwaiter().GetResult();
                var collection = userBetInfoOperation.GetCollection(merchantID);
                var data = await collection.FindOneAsync(t => t.UserID == userID && t.MerchantID == merchantID && t.Nper == nper && t.BetStatus == BetStatus.未开奖);
                if (data == null)
                {
                    output = InstructionConversion(reply.NotOrders, userID, merchantID, gameType, nper).Result;
                    tuple = Tuple.Create<string, decimal>(output, 0);
                    return tuple;
                }
                decimal amount = 0;
                foreach (var dt in data.BetRemarks)
                {
                    amount += dt.OddBets.Sum(t => t.BetMoney);
                }
                //添加日志
                var userOperation = new UserOperation();
                var surplus = await userOperation.UpperScore(userID, merchantID, amount, ChangeTargetEnum.投注, "投注撤回", "取消投注",
                    string.Join(",", data.BetRemarks.Select(t => t.OddNum).Distinct().ToList()), OrderStatusEnum.投注撤回, gameType);
                //删除定单
                await collection.DeleteOneAsync(t => t._id == data._id);
                //output = string.Format(@"【{0}回合】 取消
                //                          本期投注，返回积分
                //                         【{1}】， 剩余积分
                //                         【{2}】", list[0].Nper, list.Sum(t => t.BetMoney), surplus.Result);
                //SendFlyingOperation sendFlyingOperation = new SendFlyingOperation();
                //sendFlyingOperation.UpdateSendFlyModel(merchantID, gameType, nper, list, false).GetAwaiter().GetResult();
                var key = merchantID + Enum.GetName(typeof(GameOfType), gameType);
                var oddNums = data.BetRemarks.Select(t => t.OddNum).Distinct().ToList();
                foreach (var oddNum in oddNums)
                {
                    RedisOperation.DeleteCacheKey(key, oddNum);
                }
                output = await InstructionConversion(reply.CancelOrder, userID, merchantID, gameType, nper);
                tuple = Tuple.Create(output, amount);
            }
            catch
            {
                output = Utils.InstructionConversion(reply.NotOrders, userID, merchantID, gameType, nper).Result;
                tuple = Tuple.Create<string, decimal>(output, 0);
            }
            return tuple;
        }

        /// <summary>
        /// 撤销视讯单一下注
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        /// <param name="reply">设置</param>
        /// <param name="znum">桌号</param>
        /// <returns></returns>
        public static async Task<Tuple<string, decimal>> CancelVideoOne(string userID, BaccaratGameType gameType, string merchantID, string nper, ReplySetUp reply, int znum)
        {
            var output = string.Empty;
            decimal userAmount = 0;
            Tuple<string, decimal> returnResult;
            try
            {
                var address = await Utils.GetAddress(merchantID);
                BaccaratBetOperation baccaratBetOperation = await BetManage.GetBaccaratBetOperation(address);
                var collection = baccaratBetOperation.GetCollection(merchantID);
                var data = await collection.FindOneAsync(t => t.UserID == userID && t.MerchantID == merchantID && t.Nper == nper && t.BetStatus == BetStatus.未开奖);
                if (data == null)
                {
                    output = await Utils.InstructionConversion(reply.NotOrders, userID, merchantID, null, nper, null, gameType);
                    returnResult = Tuple.Create<string, decimal>(output, 0);
                    return returnResult;
                }

                var result = data.BetRemarks.Last();
                var updateData = data.BetRemarks;
                updateData.RemoveAll(t => t.OddNum == result.OddNum);
                if (updateData.IsNull())
                {
                    await collection.DeleteOneAsync(t => t._id == data._id);
                }
                else
                {
                    data.BetRemarks = updateData;
                    data.AllUseMoney = updateData.Sum(t => t.BetMoney);
                    await collection.UpdateOneAsync(data);
                }
                //添加日志
                var userOperation = new UserOperation();
                userAmount = result.BetMoney;
                await userOperation.UpperScore(userID, merchantID, userAmount, ChangeTargetEnum.投注, "投注撤回", "取消投注",
                    result.OddNum, OrderStatusEnum.投注撤回, null, gameType);
                output = await Utils.InstructionConversion(reply.CancelOrder, userID, merchantID, null, nper, null, gameType);
                returnResult = Tuple.Create(output, userAmount);
            }
            catch
            {
                output = await Utils.InstructionConversion(reply.NotOrders, userID, merchantID, null, nper, null, gameType);
                returnResult = Tuple.Create<string, decimal>(output, 0);
            }
            return returnResult;
        }

        /// <summary>
        /// 撤销视讯全部
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        /// <param name="reply">设置</param>
        /// <param name="znum">桌号</param>
        /// <returns></returns>
        public static async Task<Tuple<string, decimal>> CancelVideoAll(string userID, BaccaratGameType gameType, string merchantID, string nper, ReplySetUp reply, int znum)
        {
            var output = string.Empty;
            decimal userAmount = 0;
            Tuple<string, decimal> returnResult;
            try
            {
                var address = await Utils.GetAddress(merchantID);
                BaccaratBetOperation baccaratBetOperation = await  BetManage.GetBaccaratBetOperation(address);
                var collection = baccaratBetOperation.GetCollection(merchantID);
                var data = await collection.FindOneAsync(t => t.UserID == userID && t.MerchantID == merchantID && t.Nper == nper && t.BetStatus == BetStatus.未开奖);
                if (data == null)
                {
                    output = await Utils.InstructionConversion(reply.NotOrders, userID, merchantID, null, nper, null, gameType);
                    returnResult = Tuple.Create<string, decimal>(output, 0);
                    return returnResult;
                }
                
                decimal amount = data.BetRemarks.Sum(t=>t.BetMoney);
                //添加日志
                var userOperation = new UserOperation();
                userAmount = amount;
                //删除定单
                await collection.DeleteOneAsync(t => t._id == data._id);
                var surplus = userOperation.UpperScore(userID, merchantID, userAmount, ChangeTargetEnum.投注, "投注撤回", "取消投注",
                    string.Join(",", data.BetRemarks.Select(t => t.OddNum).Distinct().ToList()), OrderStatusEnum.投注撤回, null, gameType);
                output = await Utils.InstructionConversion(reply.CancelOrder, userID, merchantID, null, nper, null, gameType);
                returnResult = Tuple.Create(output, userAmount);
            }
            catch
            {
                output = await Utils.InstructionConversion(reply.NotOrders, userID, merchantID, null, nper, null, gameType);
                returnResult = Tuple.Create<string, decimal>(output, 0);
            }
            return returnResult;
        }

        /// <summary>
        /// 查询流水
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">用户类型</param>
        /// <param name="merchantID">商户id</param>
        /// <param name="nper">期号</param>
        public static async Task<string> CheckStream(string userID, GameOfType gameType, string merchantID, string nper, ReplySetUp reply)
        {
            #region
            //Dictionary<RInfoEnum, string> dic = new Dictionary<RInfoEnum, string>()
            //{
            //    {RInfoEnum.投注, "本期投注：{0}" },
            //    {RInfoEnum.使用, "当前使用：{0}" },
            //    {RInfoEnum.积分, "当前积分：{0}" },
            //    {RInfoEnum.流水, "今日流水：{0}" },
            //    {RInfoEnum.输赢, "今日胜负：{0}" }
            //};
            //UserBetInfoOperation userBetInfoOperation = new UserBetInfoOperation();
            //var list = userBetInfoOperation.GetModelList(t => t.UserID == userID && t.MerchantID == merchantID && t.GameType == gameType
            //&& t.CreatedTime >= DateTime.Today && t.CreatedTime <= DateTime.Today.AddDays(1).AddSeconds(-1) && t.BetStatus != BetStatusEnum.已取消 && t.BetStatus != BetStatusEnum.已投注);
            ////总投入
            //var useInvestment = list == null ? 0 : list.Sum(t => t.BetMoney);
            ////中奖金额
            //var userWinList = userBetInfoOperation.GetModelList(t => t.UserID == userID && t.MerchantID == merchantID && t.GameType == gameType
            //&& t.CreatedTime >= DateTime.Today && t.CreatedTime <= DateTime.Today.AddDays(1).AddSeconds(-1) && t.BetStatus == BetStatusEnum.已中奖);
            //var userWinMoney = userWinList == null ? 0 : userWinList.Sum(t => t.MediumBonus);
            ////本期投注
            //var userBetList = userBetInfoOperation.GetModelList(t => t.UserID == userID && t.MerchantID == merchantID && t.GameType == gameType
            //&& t.Nper == nper && t.BetStatus == BetStatusEnum.已投注);
            //var betOddNums = string.Empty;
            //if (userBetList == null || userBetList.Count == 0)
            //    betOddNums = string.Empty;
            //else
            //{
            //    var noList = userBetList.Select(t => t.OddNum).Distinct().ToList();
            //    UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            //    var infos = userIntegrationOperation.GetModelList(userIntegrationOperation.Builder.In(t => t.OddNum, noList));
            //    betOddNums = "\r\n" + string.Join("\r\n", infos.Select(t => t.Remark).ToList());
            //}
            //UserOperation userOperation = new UserOperation();
            //var user = userOperation.GetModel(t => t._id == userID && t.MerchantID == merchantID);
            //foreach (var item in items)
            //{
            //    if (item.Open)
            //    {
            //        switch (item.Index)
            //        {
            //            case RInfoEnum.使用:
            //                dic[item.Index] = string.Format(dic[item.Index], userBetList.Sum(t => t.BetMoney));
            //                break;
            //            case RInfoEnum.投注:
            //                dic[item.Index] = string.Format(dic[item.Index], betOddNums);
            //                break;
            //            case RInfoEnum.流水:
            //                dic[item.Index] = string.Format(dic[item.Index], useInvestment);
            //                break;
            //            case RInfoEnum.积分:
            //                dic[item.Index] = string.Format(dic[item.Index], user.UserMoney);
            //                break;
            //            case RInfoEnum.输赢:
            //                dic[item.Index] = string.Format(dic[item.Index], userWinMoney - useInvestment);
            //                break;
            //        }
            //    }
            //}
            //var result = items.FindAll(t => t.Open).OrderBy(t => t.Index).ToList();
            //foreach (var item in result)
            //{
            //    output += dic[item.Index] + "\r\n";
            //}
            #endregion
            var message = reply.CheckScore + "\r\n" + reply.CheckStream;
            return await Utils.InstructionConversion(message, userID, merchantID, gameType, nper);
        }

        /// <summary>
        /// 申请上分
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户号</param>
        /// <param name="amount">积分</param>
        /// <param name="status">1：正常  2：虚拟</param>
        /// <param name="baccaratGameType">视讯游戏类型</param>
        /// <param name="znum">房间号</param>
        public static async Task<string> UpperScore(string userID, GameOfType? gameType, string merchantID, decimal amount, NotesEnum status = NotesEnum.正常, BaccaratGameType? baccaratGameType = null, int? znum = 0)
        {
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            UserOperation userOperation = new UserOperation();
            var user = await userOperation.GetModelAsync(t => t._id == userID && t.MerchantID == merchantID);
            UserIntegration userIntegration = new UserIntegration()
            {
                UserID = userID,
                MerchantID = merchantID,
                OrderStatus = OrderStatusEnum.申请上分,
                Amount = amount,
                Balance = user.UserMoney,
                ChangeTarget = ChangeTargetEnum.申请,
                ChangeType = ChangeTypeEnum.上分,
                GameType = gameType,
                Management = ManagementEnum.未审批,
                Message = "申请上分",
                Remark = "申请上分",
                Notes = status,
                BaccaratGameType = baccaratGameType,
                Znum = znum
            };
            await userIntegrationOperation.InsertModelAsync(userIntegration);
            return userIntegration._id;
        }

        /// <summary>
        /// 申请下分
        /// </summary>
        /// <param name="userID">用户id</param>
        /// <param name="gameType">游戏类型</param>
        /// <param name="merchantID">商户号</param>
        /// <param name="amount">积分</param>
        /// <param name="status">1：正常  2：虚拟</param>
        /// <param name="baccaratGameType">视讯游戏类型</param>
        /// <param name="znum">房间号</param>
        public static async Task<string> LowerScore(string userID, GameOfType? gameType, string merchantID, decimal amount, NotesEnum status = NotesEnum.正常, BaccaratGameType? baccaratGameType = null, int? znum = 0)
        {
            UserIntegrationOperation userIntegrationOperation = new UserIntegrationOperation();
            UserOperation userOperation = new UserOperation();
            var user = userOperation.GetModel(t => t._id == userID && t.MerchantID == merchantID);
            await userOperation.LowerScore(userID, merchantID, amount, ChangeTargetEnum.系统, "申请下分", "申请下分", orderStatus: OrderStatusEnum.下分成功);
            UserIntegration userIntegration = new UserIntegration()
            {
                UserID = userID,
                MerchantID = merchantID,
                OrderStatus = OrderStatusEnum.申请下分,
                Amount = amount,
                Balance = user.UserMoney,
                ChangeTarget = ChangeTargetEnum.申请,
                ChangeType = ChangeTypeEnum.下分,
                GameType = gameType,
                Management = ManagementEnum.未审批,
                Message = "申请下分",
                Remark = "申请下分",
                Notes = status,
                BaccaratGameType = baccaratGameType,
                Znum = znum
            };
            await userIntegrationOperation.InsertModelAsync(userIntegration);
            return userIntegration._id;
        }

        /// <summary>
        /// 获取游戏开奖号码
        /// </summary>
        /// <param name="gameType"></param>
        /// <param name="nper"></param>
        /// <returns></returns>
        public static string GetGameNums(GameOfType gameType, string nper, ref string lotTime, char sep = '|', string merchantID = null)
        {
            string nums = string.Empty;
            if (Utils.GameTypeItemize(gameType))
            {
                Lottery10Operation lottery10Operation = new Lottery10Operation();
                var model = lottery10Operation.GetModel(t => t.GameType == gameType && t.IssueNum == nper && (t.MerchantID == null || t.MerchantID == merchantID));
                if (model != null)
                {
                    nums = GameDiscrimination.SetupSeparation10(model, sep);
                    lotTime = model.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            else
            {
                Lottery5Operation lottery5Operation = new Lottery5Operation();
                var model = lottery5Operation.GetModel(t => t.GameType == gameType && t.IssueNum == nper && (t.MerchantID == null || t.MerchantID == merchantID));
                if (model != null)
                {
                    nums = GameDiscrimination.SetupSeparation5(model, sep);
                    lotTime = model.CreatedTime.ToString("yyyy-MM-dd HH:mm:ss");
                }
            }
            return nums;
        }

        /// <summary>
        /// 获取游戏最新一期号码
        /// </summary>
        /// <param name="gameType"></param>
        /// <returns></returns>
        public static string GetGameNper(GameOfType gameType)
        {
            var model = RedisOperation.GetValue<GameNextLottery>("GameStatus", Enum.GetName(typeof(GameOfType), gameType));
            string nums = model.NextNper;
            return nums;
        }
    }
}
