using Microsoft.AspNetCore.Mvc;

namespace PollyDemo.ResilientApi.Controllers;


[ApiController]
[Route("api/[controller]")]
public class RetryController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public RetryController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("constant")]
    public async Task<IActionResult> GetWithConstantRetry()
    {
        var client = _httpClientFactory.CreateClient("TargetApi-Constant");
        var response = await client.GetAsync("/api/weather");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        return StatusCode((int)response.StatusCode);
    }

    [HttpGet("linear")]
    public async Task<IActionResult> GetWithLinearRetry()
    {
        var client = _httpClientFactory.CreateClient("TargetApi-Linear");
        var response = await client.GetAsync("/api/weather");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        return StatusCode((int)response.StatusCode);
    }

    [HttpGet("exponential")]
    public async Task<IActionResult> GetWithExponentialRetry()
    {
        var client = _httpClientFactory.CreateClient("TargetApi-Exponential");
        var response = await client.GetAsync("/api/weather");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        return StatusCode((int)response.StatusCode);
    }

    [HttpGet("selective")]
    public async Task<IActionResult> GetWithSelectiveRetry()
    {
        var client = _httpClientFactory.CreateClient("TargetApi-Selective");
        var response = await client.GetAsync("/api/weather/slow");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        return StatusCode((int)response.StatusCode);
    }

    [HttpGet("retry-after")]
    public async Task<IActionResult> GetWithRetryAfter()
    {
        var client = _httpClientFactory.CreateClient("TargetApi-RetryAfter");
        var response = await client.GetAsync("/api/weather/rate-limited");

        if (response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            return Ok(content);
        }

        return StatusCode((int)response.StatusCode);
    }
}
