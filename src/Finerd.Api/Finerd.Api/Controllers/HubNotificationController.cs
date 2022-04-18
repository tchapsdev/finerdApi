using Finerd.Api.Hubs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Finerd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class HubNotificationController : ControllerBase
    {
        //IHubContext<NotificationHub> _hubContext;
        //public NotificationController(IHubContext<NotificationHub> hubContext)
        //{
        //    _hubContext = hubContext;
        //}

        public IHubContext<NotificationHub, INotificationClient> _notificationHubContext { get; }
        public HubNotificationController(IHubContext<NotificationHub, INotificationClient> notificationHubContext)
        {
            _notificationHubContext = notificationHubContext;
        }
        
        [HttpPost("{message}")]
        public async void Post(string message)
        {
            await _notificationHubContext.Clients.All.ReceiveMessage("Finerd Broadcast", message);
        }

        [HttpPost("{connectionId}/{message}")]
        public async void Post(string connectionId, string message)
        {
            await _notificationHubContext.Clients.Clients(connectionId).ReceiveMessage(connectionId, message);
        }

        [HttpPost]
        public async Task SendMessage(string user, string message)
        {
            await _notificationHubContext.Clients.All.ReceiveMessage(user, message);
        }

        //public async Task<IActionResult> Index()
        //{
        //    await _hubContext.Clients.All.SendAsync("Notify", $"Home page loaded at: {DateTime.Now}");
        //    return Ok();
        //}

    }
}
