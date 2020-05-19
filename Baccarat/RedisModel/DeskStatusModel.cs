using Entity.BaccaratModel;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Baccarat.RedisModel
{
    /// <summary>
    /// 游戏信息
    /// </summary>
    public class DeskStatusModel
    {
        /// <summary>
        /// 厅号
        /// </summary>
        public int Cid { get; set; }
        /// <summary>
        /// 游戏
        /// </summary>
        public int Gid { get; set; }
        /// <summary>
        /// 桌号
        /// </summary>
        public int Tid { get; set; }
        /// <summary>
        /// 靴
        /// </summary>
        public int Ch { get; set; }
        /// <summary>
        /// 铺
        /// </summary>
        public int Ci { get; set; }
        /// <summary>
        /// 桌名
        /// </summary>
        public string Tname { get; set; }
        /// <summary>
        /// 网络最高限红
        /// </summary>
        public decimal Xh { get; set; }
        /// <summary>
        /// 网络最低限红
        /// </summary>
        public decimal Dx { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 现场最高限红
        /// </summary>
        public decimal Txh { get; set; }
        /// <summary>
        /// 现场最低限红
        /// </summary>
        public decimal Tdx { get; set; }
        /// <summary>
        /// 包台1为包台中
        /// </summary>
        public int Monopolize { get; set; }
        /// <summary>
        /// 倒计时秒数
        /// </summary>
        public int Ttime { get; set; }
        /// <summary>
        /// 桌子状态 free init stop close openning ,init 状态可以下注
        /// </summary>
        public string Cstate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int Bt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public byte Vt { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Va { get; set; }
        /// <summary>
        /// 计时开始时间
        /// </summary>
        public DateTime Inittime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<JObject> Vmoney { get; set; } = new List<JObject>();
        /// <summary>
        /// 
        /// </summary>
        public int vcount { get; set; }
    }

    /// <summary>
    /// 游戏开奖下发任务信息
    /// </summary>
    public class TaskDistributionModel
    {
        /// <summary>
        /// 期号 
        /// </summary>
        public string Nper { get; set; }
        /// <summary>
        /// 游戏类型
        /// </summary>
        public BaccaratGameType GameType { get; set; }

        /// <summary>
        /// 开奖信息
        /// </summary>
        public string Lottery { get; set; }

        /// <summary>
        /// 桌号
        /// </summary>
        public int ZNum { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public List<string> MerchantIDList { get; set; }

        /// <summary>
        /// 唯一码
        /// </summary>
        public string UUID { get; set; } = Guid.NewGuid().ToString();
    }
}
