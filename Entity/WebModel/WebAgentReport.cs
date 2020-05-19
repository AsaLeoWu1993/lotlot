namespace Entity.WebModel
{
    public class WebAgentReport
    {
        public string AgentID { get; set; }

        public string NickName { get; set; }

        public string OnlyCode { get; set; }

        public decimal Pk10 { get; set; } = 0;

        public decimal Xyft { get; set; } = 0;

        public decimal Jssc { get; set; } = 0;

        public decimal Cqssc { get; set; } = 0;

        public decimal Azxy10 { get; set; } = 0;

        public decimal Azxy5 { get; set; } = 0;

        public decimal Ireland10 { get; set; } = 0;

        public decimal Ireland5 { get; set; } = 0;

        public decimal Xyft168 { get; set; } = 0;

        public decimal Jsssc { get; set; } = 0;

        public decimal Ascent { get; set; } = 0;

        public decimal Baccarat { get; set; } = 0;

        public BackStatusEnum BackStatus { get; set; }

        public string Time { get; set; }
    }
}
