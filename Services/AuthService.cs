using API.Auth;
using API.Interfaces;
using API.Models.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Shinetech.Common;
using Shinetech.Common.Helper;
using Shinetech.Infrastructure.Contract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace API.Services
{
    public class TokenUser
    {
        //  [JsonPropertyName("user_id")]
        public int UserID { get; set; }
        //[JsonPropertyName("first_name")]
        public string FirstName { get; set; }
        //[JsonPropertyName("tenant_codes")]
        public string TenantCodes { get; set; }
        public bool IsAdmin { get; set; }

    }
    public class AuthService : IAuthService
    {


        private readonly IRepository<User> _accountRepository;
        private readonly IRepository<LoginTracking> _trackingRepository;

        private readonly IConfiguration _configuration;
        private readonly string _securityKey = "";
        private readonly int _tokenExpireMinutes;
        private readonly int _tokenCanRefreshWithinMinutes;
        private readonly string _aesEncryptKey = "";
        private readonly ICurrentUser currentUser;
        protected readonly ICrudRepository<ActionLog> logRepository;
        public AuthService(IConfiguration configuration, IRepository<User> accountRepository, IRepository<LoginTracking> trackingRepository, ICurrentUser user, ICrudRepository<ActionLog> _logRepository)
        {
            currentUser = user;
            string environmentKeyValue = configuration[Const.TokenSecurityKey];
            _aesEncryptKey = configuration[Const.AesEncryptKey];

            var tokenCanRefreshWithinMinutes = configuration[Const.TokenCanRefreshWithinMinutes];
            _tokenCanRefreshWithinMinutes = string.IsNullOrEmpty(tokenCanRefreshWithinMinutes) ? 120 : Convert.ToInt32(tokenCanRefreshWithinMinutes);
            _configuration = configuration;

            var stringMinutes = configuration[Const.TokenExpireMinutes];
            _tokenExpireMinutes = string.IsNullOrEmpty(stringMinutes) ? 10 : Convert.ToInt32(stringMinutes);

            _accountRepository = accountRepository;
            _securityKey = environmentKeyValue;
            _trackingRepository = trackingRepository;

            logRepository = _logRepository;
        }
        public CommonResponse Login(string ip, string userName, string password, string validationCode)
        {
            logRepository.DbSet.Add(new ActionLog()
            {
                Who = userName,
                Content = "尝试登录，IP地址" + ip
            });
            logRepository.Save();


            var existsTracking = _trackingRepository.DbSet.FirstOrDefault(r => r.Ip == ip);
            if (existsTracking != null)
            {
                if (existsTracking.FailedCount >= 3 && (validationCode != existsTracking.ValidationCode || string.IsNullOrEmpty(validationCode)))
                {
                    return new CommonResponse()
                    {
                        code = (int)ResponseCode.BadRequest,
                        message = "ValidateionCodeError"
                    };
                }
            }
            else
            {
                _trackingRepository.DbSet.Add(new LoginTracking() { Ip = ip, FailedCount = 0, ValidationCode = "" });
                _trackingRepository.Save();
                existsTracking = _trackingRepository.DbSet.FirstOrDefault(r => r.Ip == ip);
            }
            var loginAccount = _accountRepository.Get(r => r.LoginName == userName && !r.IsDelete && !r.IsEnable).FirstOrDefault();
            if (loginAccount != null)
            {
                var passwordMatch = CryptoHelper.Crypto.VerifyHashedPassword(loginAccount.Password, password);
                if (passwordMatch)
                {
                    existsTracking.FailedCount = 0;
                    _trackingRepository.Save();

                    logRepository.DbSet.Add(new ActionLog()
                    {
                        Who = loginAccount.Name,
                        Content = "用户登录成功"
                    });
                    logRepository.Save();

                    return GenericUserToken(loginAccount);
                }
                else
                {
                    existsTracking.FailedCount = existsTracking.FailedCount + 1;
                    _trackingRepository.Save();
                    return new CommonResponse()
                    {
                        code = (int)ResponseCode.Unauthorized,
                        message = "Invalid Credentials."
                    };
                }

            }
            existsTracking.FailedCount = existsTracking.FailedCount + 1;
            _trackingRepository.Save();
            return new CommonResponse()
            {
                code = (int)ResponseCode.Unauthorized,
                message = "Invalid Credentials."
            };
        }
        public string GetValidationCode(string ip)
        {
            var existsTracking = _trackingRepository.DbSet.FirstOrDefault(r => r.Ip == ip);
            if (existsTracking == null)
            {
                _trackingRepository.DbSet.Add(new LoginTracking() { Ip = ip, FailedCount = 0, ValidationCode = "" });
                _trackingRepository.Save();
                existsTracking = _trackingRepository.DbSet.FirstOrDefault(r => r.Ip == ip);
            }
            if (existsTracking.FailedCount < 3)
            {
                return "";
            }
            var code = ValidationCode.GetRandomString(4);
            existsTracking.ValidationCode = code;
            _trackingRepository.Save();
            return "data:image/jpg;base64," + Convert.ToBase64String(ValidationCode.GetVcodeImg(code));
        }

        private CommonResponse GenericUserToken(User loginAccount, bool isInter = false)
        {

            var user = new TokenUser
            {
                UserID = loginAccount.Id,
                FirstName = loginAccount.Name.ToString(),
                IsAdmin = loginAccount.IsAdmin
            };

            var tokenHelper = new TokenHelper(_securityKey);
            var pars = new Dictionary<string, object> { { Const.AuthHeaderUser, user } };

            var tokenStr = tokenHelper.GenerateAccessToken(null, pars, _tokenExpireMinutes);
            var refreshToken = tokenHelper.GenerateRefreshToken(tokenStr);

            if (isInter == true)
            {
                return new CommonResponse()
                {
                    data = tokenStr
                };
            }
            return new CommonResponse()
            {
                data = new
                {
                    token = tokenStr,
                    refresh_token = refreshToken
                }
            };
        }

        public string GenerateTempToken()
        {
            var loginAccount = _accountRepository.DbSet.AsNoTracking().FirstOrDefault(r => r.IsDelete == false);
            return (string)GenericUserToken(loginAccount, true).data;
        }


        public CommonResponse GetInfoBySubDomain(string domain)
        {
            var result = new CommonResponse();
            return result;
        }

        private object List<T>()
        {
            throw new NotImplementedException();
        }

        public CommonResponse ChangePassword(string OldPassword, string newPassword)
        {
            logRepository.DbSet.Add(new ActionLog()
            {
                Who = currentUser.Name,
                Content = "尝试更改密码"
            });
            logRepository.Save();


            int userID = currentUser.Id;
            var loginAccount = _accountRepository.Get(r => r.Id == userID).FirstOrDefault();
            if (loginAccount != null)
            {
                var passwordMatch = CryptoHelper.Crypto.VerifyHashedPassword(loginAccount.Password, OldPassword);
                if (passwordMatch)
                {
                    loginAccount.Password = CryptoHelper.Crypto.HashPassword(newPassword);
                    loginAccount.UpdatedBy = currentUser.Id;
                    loginAccount.UpdatedTime = DateTime.Now;
                    _accountRepository.Update(loginAccount);
                    _accountRepository.Save();

                    logRepository.DbSet.Add(new ActionLog()
                    {
                        Who = currentUser.Name,
                        Content = "更改密码成功"
                    });
                    logRepository.Save();


                    return new CommonResponse()
                    {
                        code = (int)ResponseCode.OK,
                        message = ""
                    };
                }
                else
                {
                    return new CommonResponse()
                    {
                        code = (int)ResponseCode.PasswordError,
                        message = "Invalid Old Password"
                    };
                }
            }
            return new CommonResponse()
            {
                code = (int)ResponseCode.Unauthorized,
                message = "Invalid Credentials."
            };
        }
        public CommonResponse RefreshToken(string token, string refreshToken)
        {
            var result = new CommonResponse();
            var tokenHelper = new TokenHelper(_securityKey);
            var claims = tokenHelper.GetPrincipalFromAccessToken(token);
            if (claims != null)
            {
                var expire = claims.Claims.FirstOrDefault(r => r.Type == "exp").Value;
                var expireTime = DateTimeHelper.ConvertToDateTime(Convert.ToInt64(expire));
                if (DateTime.Now < expireTime.AddMinutes(_tokenCanRefreshWithinMinutes))
                {
                    if (tokenHelper.ValidateRefreshToken(refreshToken, token))
                    {
                        var userModel = claims.Claims.FirstOrDefault(r => r.Type == "user").Value.ObjToString();
                        var user = JsonSerializer.Deserialize<TokenUser>(userModel);
                        if (user != null)
                        {
                            var loginAccount = _accountRepository.Get(r => r.Id == user.UserID).FirstOrDefault();
                            if (loginAccount != null)
                            {
                                return GenericUserToken(loginAccount);
                            }
                        }
                    }
                    else
                    {
                        result.code = (int)ResponseCode.Forbidden;
                    }

                }
                else
                {
                    result.code = (int)ResponseCode.TokenExpired;
                }
            }
            else
            {
                result.code = (int)ResponseCode.BadRequest;
            }
            return result;
        }
    }
}
