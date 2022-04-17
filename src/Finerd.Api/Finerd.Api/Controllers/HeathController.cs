using Finerd.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Finerd.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeathController : ControllerBase
    {
        private readonly ILogger<HeathController> _logger;

        private readonly IPushNotificationService _pushNotificationService;

        public HeathController(ILogger<HeathController> logger, IPushNotificationService pushNotificationService)
        {
            _logger = logger;
            _pushNotificationService = pushNotificationService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            var x = _pushNotificationService.SendNotification(new PushNotification.PushSubscription { }, "paylod");
            return Ok(new
            {
                Application = "Finerd",
                School = "UQAM",
                Year = 2022,
                Version = "1.0"
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("/.well-known/pki-validation/starfield.html")]
        public IActionResult Getstarfield()
        {
            return Ok("ainhn0koois7l03cpkgarva5ek");
        }
    }
}