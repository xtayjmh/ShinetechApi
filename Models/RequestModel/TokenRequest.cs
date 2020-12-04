using System.ComponentModel.DataAnnotations;

namespace API.Models.RequestModel
{
    public class TokenRequest
    {

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Required]
        public string Password { get; set; }
        public string ValidationCode { get; set; }
    }
}
