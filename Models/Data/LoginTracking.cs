namespace API.Models.Data
{
    /// <summary>
    /// 登录追踪
    /// </summary>
    public class LoginTracking : BaseEntity
    {
        /// <summary>
        /// IP地址
        /// </summary>
        public string Ip { get; set; }
        /// <summary>
        /// 失败次数
        /// </summary>
        public int FailedCount { get; set; }
        /// <summary>
        /// 验证码
        /// </summary>
        public string ValidationCode { get; set; }

    }
}
