using Microsoft.AspNetCore.Mvc;

namespace TradingProject.Persistence.Api.Controllers;

[ApiController]
[Route("health")]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Get() => Ok("Healthy");
}
