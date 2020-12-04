using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace API.Auth
{
    public class AuthHandler : AuthorizationHandler<AuthRequirement>
    {
        readonly IHttpContextAccessor _httpContext;
        public AuthHandler(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, AuthRequirement requirement)
        {
            //If is super account then can do anything and should return true
            if (_httpContext.HttpContext.User.Identity.IsAuthenticated)
            {
                context.Succeed(requirement);
                return Task.CompletedTask;
            }
            context.Fail();
            return Task.CompletedTask;

        }
    }
}
