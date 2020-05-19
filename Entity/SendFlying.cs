using System.Collections.Generic;

namespace Entity
{
    public class SendFlying : BaseModel
    {
        public string MerchantID { get; set; }
        public string uuid { get; set; }
        public string IssueCode { get; set; }
        public string game { get; set; }

        public List<FlyingBet> orders = new List<FlyingBet>();

        public SendFlyEnum Status { get; set; } = SendFlyEnum.等待发送;
    }

    public enum SendFlyEnum
    { 
        已接收 = 1,
        未接收 = 2,
        等待发送 = 3,
        预发送 = 4
    }


    public class FlyingBet
    {
        public string content { get; set; }
        public decimal money { get; set; }
        public string OddNum { get; set; }
    }
}
