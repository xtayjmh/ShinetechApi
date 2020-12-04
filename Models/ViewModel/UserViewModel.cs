using Shinetech.Common;

namespace API.Models.ViewModel
{
    [GeneratedController(description: "用户管理")]
    public class UserViewModel : BaseViewModel
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        public string LoginName { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 是否是管理员
        /// </summary>
        public bool IsAdmin { get; set; }
        /// <summary>
        /// 启用禁用
        /// </summary>
        public bool IsEnable { get; set; }
    }
}
