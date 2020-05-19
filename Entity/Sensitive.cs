namespace Entity
{
    /// <summary>
    /// 敏感操作表
    /// </summary>
    public sealed class Sensitive : BaseModel
    {
        /// <summary>
        /// 对应商户id
        /// </summary>
        public string MerchantID { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public OpTypeEnum OpType { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public OpLocationEnum OpLocation { get; set; }

        /// <summary>
        /// 操作前内容
        /// </summary>
        public string OpBcontent { get; set; }

        /// <summary>
        /// 操作后内容
        /// </summary>
        public string OpAcontent { get; set; }
    }

    public enum OpTypeEnum
    {
        修改 = 1,
        删除 = 2,
        添加 = 3
    }

    public enum OpLocationEnum
    {
        赛车赔率操作 = 1,
        飞艇赔率操作 = 2,
        时时彩赔率操作 = 3,
        极速赔率操作 = 4,
        澳10赔率操作 = 5,
        澳5赔率操作 = 6,
        用户状态切换 = 7,
        删除用户 = 8,
        切换代理 = 9,
        修改用户信息 = 10,
        撤销注单 = 11,
        回水方案操作 = 12,
        房间开和设置 = 13,
        代理回水设置 = 14,
        赛车限额操作 = 15,
        飞艇限额操作 = 16,
        时时彩限额操作 = 17,
        极速限额操作 = 18,
        澳10限额操作 = 19,
        澳5限额操作 = 20,
        爱尔兰赛马赔率操作 = 21,
        爱尔兰快5赔率操作 = 22,
        爱尔兰赛马限额操作 = 23,
        爱尔兰快5限额操作 = 24,
        百家乐限额操作 = 25,
        百家乐赔率操作 = 26,
        幸运飞艇168赔率操作 = 27,
        幸运飞艇168限额操作 = 28, 
        极速时时彩赔率操作 = 29,
        极速时时彩限额操作 = 30
    }
}
