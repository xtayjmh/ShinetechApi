using System.Collections.Generic;
using System.Security.Claims;

namespace API.Interfaces
{
    public interface ICurrentUser
    {
        string Name { get; }
        int Id { get; }
        bool IsAuthenticated();
        bool IsSuperAccount();
        IEnumerable<Claim> GetClaimsIdentity();
        List<string> GetClaimValueByType(string claimType);

        string GetToken();
        List<string> GetUserInfoFromToken(string claimType);
    }
}