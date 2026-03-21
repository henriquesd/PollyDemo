using PollyDemo.ResilientApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var targetApiBaseAddress = new Uri(builder.Configuration["TargetApi:BaseAddress"]!);

builder.Services
    .AddConstantRetryClient(targetApiBaseAddress)
    .AddLinearRetryClient(targetApiBaseAddress)
    .AddExponentialRetryClient(targetApiBaseAddress)
    .AddSelectiveRetryClient(targetApiBaseAddress)
    .AddRetryAfterClient(targetApiBaseAddress)
    .AddCircuitBreakerClient(targetApiBaseAddress)
    .AddTimeoutClient(targetApiBaseAddress)
    .AddFallbackClient(targetApiBaseAddress)
    .AddTimeoutFallbackClient(targetApiBaseAddress);

var app = builder.Build();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
