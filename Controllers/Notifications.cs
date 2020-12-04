using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    public class Notifications : Hub
    {
        private readonly ILogger<Notifications> _logger;
        public Notifications(
            ILogger<Notifications> logger)
        {
            _logger = logger;
        }
    }
}
