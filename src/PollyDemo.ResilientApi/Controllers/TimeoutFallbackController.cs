using Microsoft.AspNetCore.Mvc;

namespace PollyDemo.ResilientApi.Controllers;

[ApiController]
[Route("api/timeout-fallback")]
public class TimeoutFallbackController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TimeoutFallbackController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("weather")]
    public async Task<IActionResult> GetWithTimeoutFallback()
    {
        var client = _httpClientFactory.CreateClient("TargetApi-TimeoutFallback");
        var response = await client.GetAsync("/api/weather/delayed");

        var content = await response.Content.ReadAsStringAsync();
        return Content(content, "application/json");
    }
}
