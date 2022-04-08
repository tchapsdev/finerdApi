using Microsoft.AspNetCore.Mvc;

namespace Finerd.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HeathController : ControllerBase
    {
      
        private readonly ILogger<HeathController> _logger;

        public HeathController(ILogger<HeathController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new
            {
                Application = "Finerd",
                School = "UQAM",
                Year = 2022,
                Version = "1.0"
            });
        }
    }
}