using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using API.Auth;
using API.Interfaces;
using API.Models.Request;

namespace API.Controllers
{
    /// <summary>
    /// 验证相关
    /// </summary>
    [Produces("application/json")]
    [Route("[controller]")]
    [TunnelClientAuth]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private IHttpContextAccessor _accessor;

        public AuthController(IConfiguration configuration, IAuthService accountService, IHttpContextAccessor accessor)
        {
            _authService = accountService;
            _accessor = accessor;
        }

        /// <summary>
        /// 获取验证码
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetValidationCode")]
        [AllowAnonymous]
        public async Task<IActionResult> GetValidationCode()
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var result = await Task.FromResult(_authService.GetValidationCode(ip));
            return Ok(result);
        }

        /// <summary>
        /// login action
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> RequestToken([FromBody] TokenRequest request)
        {
            var ip = _accessor.HttpContext.Connection.RemoteIpAddress.ToString();
            var result =
                await Task.FromResult(_authService.Login(ip, request.Account, request.Password,
                    request.ValidationCode));
            return Ok(result);
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            var result = await Task.FromResult(_authService.ChangePassword(request.OldPassword, request.NewPassword));
            return Ok(result);
        }

        /// <summary>
        /// refresh token when token going to be expired
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("Refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> refresh([FromBody] RefreshTokenRequest request)
        {
            var result = await Task.FromResult(_authService.RefreshToken(request.Token, request.RefreshToken));
            return Ok(result);
        }

    }
}