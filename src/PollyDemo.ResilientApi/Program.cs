using Microsoft.Extensions.Http.Resilience;
using Polly;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddHttpClient("TargetApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["TargetApi:BaseAddress"]!);
})
.AddResilienceHandler("retry-pipeline", pipelineBuilder =>
{
    pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
    {
        MaxRetryAttempts = 5,
        Delay = TimeSpan.FromSeconds(1),
        BackoffType = DelayBackoffType.Exponential,
        UseJitter = true,
        OnRetry = args =>
        {
            Console.WriteLine($"Retry attempt {args.AttemptNumber} after {args.RetryDelay.TotalSeconds:F1}s delay. Status: {args.Outcome.Result?.StatusCode}");
            return default;
        }
    });
});

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();