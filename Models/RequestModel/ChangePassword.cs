using System.ComponentModel.DataAnnotations;

namespace API.Models.RequestModel
{

    /// <summary>
    /// 
    /// </summary>
    public class ChangePassword
    {
        /// <summary>
        /// 老密码
        /// </summary>
        [Required]
        public string OldPassword { get; set; }
        /// <summary>
        /// 新密码
        /// </summary>
        [Required]
        public string NewPassword { get; set; }

    }
}
