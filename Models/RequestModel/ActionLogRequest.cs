namespace API.Models.RequestModel
{
    /// <summary>
    /// 操作日志
    /// </summary>
    public class ActionLogRequest : BaseRequestModel
    {
        /// <summary>
        /// 操作人
        /// </summary>
        public string Who { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>

        public string Content { get; set; }
    }
}
