using System.ComponentModel.DataAnnotations;

namespace API.Models.RequestModel
{
    public class ChangePasswordRequest
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
