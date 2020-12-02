using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;

namespace API.Auth
{
    public class AuthCheck
    {
        readonly IHttpContextAccessor _httpContext;
        public AuthCheck(IHttpContextAccessor context)
        {
            _httpContext = context;
        }

        public bool checkCapabilities(AuthorizationHandlerContext context)
        {
            //If is super account then can do anything and should return true
            if (context.User.HasClaim("IsSuperAccount", "true"))
            {
                return true;
            }

            Endpoint endpoint = (Endpoint)context.Resource;

            TunnelClientAuth authAttribute = endpoint.Metadata.GetMetadata<TunnelClientAuth>();

            if (authAttribute is object) //checks that it actually found the attribute
            {
                //Check whether the attribute specified that certain capabilities had to exist
                if (!String.IsNullOrEmpty(authAttribute.Capabilities))
                {
                    String[] capabilities = authAttribute.Capabilities.Split(",");

                    foreach (var capability in capabilities)
                    {
                        if (context.User.HasClaim("capability", capability))
                        {
                            return true;
                        }
                    }

                    //user account did not have required capability
                    return false;
                }
            }

            return true;
        }
    }
}
