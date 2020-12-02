using Microsoft.AspNetCore.Authorization;

namespace API.Auth
{
    public class TunnelClientAuth : AuthorizeAttribute
    {
        public string Capabilities { get; set; }
        public bool LookupCapabilities { get; set; } = false;
    }
}
