
using Shinetech.Common;

namespace API.Models.ViewModel
{
    [GeneratedController(description: "操作日志")]
    /// <summary>
    /// 操作日志
    /// </summary>
    public class ActionLogViewModel : BaseViewModel
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
