using Microsoft.AspNetCore.Mvc;

namespace PollyDemo.TargetApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private static int _slowRequestCount;

    [HttpGet]
    public IActionResult Get()
    {
        if (Random.Shared.Next(1, 4) == 1)
        {
            return Ok(new { Temperature = 25, Summary = "Sunny" });
        }

        return StatusCode(500);
    }

    [HttpGet("slow")]
    public IActionResult GetSlow()
    {
        var count = Interlocked.Increment(ref _slowRequestCount);

        if (count <= 4)
        {
            return StatusCode(503, new { Message = "Service temporarily unavailable" });
        }

        Interlocked.Exchange(ref _slowRequestCount, 0);
        return Ok(new { Temperature = 20, Summary = "Cloudy" });
    }

    [HttpGet("rate-limited")]
    public IActionResult GetRateLimited()
    {
        if (Random.Shared.Next(1, 4) == 1)
        {
            return Ok(new { Temperature = 22, Summary = "Partly Cloudy" });
        }

        Response.Headers["Retry-After"] = "2";
        return StatusCode(429, new { Message = "Too many requests" });
    }
}
