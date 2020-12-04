using API.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Shinetech.Common.Helper;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;

namespace API.Auth
{
    public class UserAccount : ICurrentUser
    {
        private readonly IHttpContextAccessor _accessor;
        readonly TokenUser _tokenUser = new TokenUser();
        public UserAccount(IHttpContextAccessor accessor, IConfiguration configuration)
        {
            _accessor = accessor;
            var userStr = GetClaimValueByType(Const.AuthHeaderUser).FirstOrDefault().ObjToString();
            if (!string.IsNullOrEmpty(userStr))
            {
                _tokenUser = JsonSerializer.Deserialize<TokenUser>(userStr);
            }
        }

        public string Name => _tokenUser.FirstName;

        public int Id => _tokenUser.UserId;

        public bool IsAuthenticated()
        {
            return _accessor.HttpContext?.User.Identity != null && _accessor.HttpContext != null && _accessor.HttpContext.User.Identity.IsAuthenticated;
        }

        public bool IsSuperAccount()
        {
            if (!IsAuthenticated()) return false;
            return true;
        }

        public string GetToken()
        {
            return _accessor.HttpContext.Request.Headers["Authorization"].ObjToString().Replace("Bearer ", "");
        }

        public List<string> GetUserInfoFromToken(string claimType)
        {

            var jwtHandler = new JwtSecurityTokenHandler();
            if (!string.IsNullOrEmpty(GetToken()))
            {
                JwtSecurityToken jwtToken = jwtHandler.ReadJwtToken(GetToken());

                return (from item in jwtToken.Claims
                        where item.Type == claimType
                        select item.Value).ToList();
            }
            else
            {
                return new List<string>() { };
            }
        }

        public IEnumerable<Claim> GetClaimsIdentity()
        {
            if (_accessor.HttpContext==null)
            {
                return new List<Claim>();
            }
            return _accessor.HttpContext.User.Claims;
        }

        public List<string> GetClaimValueByType(string claimType)
        {

            return (from item in GetClaimsIdentity()
                    where item.Type == claimType
                    select item.Value).ToList();

        }
    }
}

