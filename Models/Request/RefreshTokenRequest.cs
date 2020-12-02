using System.ComponentModel.DataAnnotations;

namespace API.Models.Request
{
    public class RefreshTokenRequest
    {
        /// <summary>
        ///从登录API中获取的token
        /// </summary>
        [Required]
        public string Token { get; set; }
        /// <summary>
        /// 从登录API中获取的refresh token 
        /// </summary>
        [Required]
        public string RefreshToken { get; set; }
    }
}