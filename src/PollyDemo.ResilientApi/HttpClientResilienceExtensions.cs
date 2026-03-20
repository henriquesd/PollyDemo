using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.Retry;
using System.Net;

namespace PollyDemo.ResilientApi;

public static class HttpClientResilienceExtensions
{
    private static Func<OnRetryArguments<HttpResponseMessage>, ValueTask> OnRetry => args =>
    {
        Console.WriteLine($"Retry attempt {args.AttemptNumber + 1} after {args.RetryDelay.TotalSeconds:F1}s delay. Status: {args.Outcome.Result?.StatusCode}");
        return default;
    };

    public static IServiceCollection AddConstantRetryClient(this IServiceCollection services, Uri baseAddress)
    {
        services.AddHttpClient("TargetApi-Constant", client =>
        {
            client.BaseAddress = baseAddress;
        })
        .AddResilienceHandler("constant-retry", pipelineBuilder =>
        {
            pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 5,
                Delay = TimeSpan.FromSeconds(2),
                BackoffType = DelayBackoffType.Constant,
                OnRetry = OnRetry
            });
        });

        return services;
    }

    public static IServiceCollection AddLinearRetryClient(this IServiceCollection services, Uri baseAddress)
    {
        services.AddHttpClient("TargetApi-Linear", client =>
        {
            client.BaseAddress = baseAddress;
        })
        .AddResilienceHandler("linear-retry", pipelineBuilder =>
        {
            pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 5,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Linear,
                OnRetry = OnRetry
            });
        });

        return services;
    }

    public static IServiceCollection AddExponentialRetryClient(this IServiceCollection services, Uri baseAddress)
    {
        services.AddHttpClient("TargetApi-Exponential", client =>
        {
            client.BaseAddress = baseAddress;
        })
        .AddResilienceHandler("exponential-retry", pipelineBuilder =>
        {
            pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 5,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                OnRetry = OnRetry
            });
        });

        return services;
    }

    public static IServiceCollection AddSelectiveRetryClient(this IServiceCollection services, Uri baseAddress)
    {
        services.AddHttpClient("TargetApi-Selective", client =>
        {
            client.BaseAddress = baseAddress;
        })
        .AddResilienceHandler("selective-retry", pipelineBuilder =>
        {
            pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 5,
                Delay = TimeSpan.FromSeconds(1),
                BackoffType = DelayBackoffType.Exponential,
                ShouldHandle = static args => ValueTask.FromResult(
                    args.Outcome.Result?.StatusCode is HttpStatusCode.TooManyRequests
                        or HttpStatusCode.ServiceUnavailable),
                OnRetry = OnRetry
            });
        });

        return services;
    }

    public static IServiceCollection AddRetryAfterClient(this IServiceCollection services, Uri baseAddress)
    {
        services.AddHttpClient("TargetApi-RetryAfter", client =>
        {
            client.BaseAddress = baseAddress;
        })
        .AddResilienceHandler("retry-after", pipelineBuilder =>
        {
            pipelineBuilder.AddRetry(new HttpRetryStrategyOptions
            {
                MaxRetryAttempts = 5,
                BackoffType = DelayBackoffType.Constant,
                Delay = TimeSpan.FromSeconds(1),
                DelayGenerator = static args =>
                {
                    if (args.Outcome.Result is HttpResponseMessage response
                        && response.Headers.RetryAfter?.Delta is TimeSpan retryAfter)
                    {
                        return ValueTask.FromResult<TimeSpan?>(retryAfter);
                    }

                    return ValueTask.FromResult<TimeSpan?>(null);
                },
                OnRetry = OnRetry
            });
        });

        return services;
    }
}
