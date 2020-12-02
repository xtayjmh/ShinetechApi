using Shinetech.Common;

namespace API.Interfaces
{
    public interface IAuthService : IService
    {
        CommonResponse GetInfoBySubDomain(string subDomain);
        CommonResponse Login(string ip, string userName, string password, string validationCode);
        CommonResponse RefreshToken(string token, string refreshToken);
        CommonResponse ChangePassword(string oldPassword, string newPassword);
        string GetValidationCode(string ip);
        string GenerateTempToken();
    }
}
