using Microsoft.AspNetCore.Mvc;

namespace PollyDemo.TargetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        if (Random.Shared.Next(1, 11) <= 7)
        {
            return StatusCode(500);
        }

        return Ok(new { Temperature = 25, Summary = "Sunny" });
    }
}
