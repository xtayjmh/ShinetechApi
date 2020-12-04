using System.ComponentModel.DataAnnotations;

namespace API.Models.RequestModel
{
    /// <summary>
    /// 
    /// </summary>
    public class UserRequest : BaseRequestModel
    {
        /// <summary>
        /// 登录账号
        /// </summary>
        [Required]
        public string LoginName { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 姓名
        /// </summary>
        [MaxLength(50)]
        [Required]
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
