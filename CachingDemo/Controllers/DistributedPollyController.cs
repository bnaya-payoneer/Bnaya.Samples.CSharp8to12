using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.Registry;
using Polly.Retry;
using System.Text.Json;

namespace CachingDemo.Controllers;
[ApiController]
[Route("[controller]")]
public class DistributedPollyControllerController : ControllerBase
{
    private readonly ILogger<DistributedPollyControllerController> _logger;
    private readonly IDistributedCache _distributedCache;
    private readonly TimeProvider _timeProvider;
    private readonly ResiliencePipeline _pipeline;
    //private readonly IAsyncPolicy<DateTimeOffset> _distributedPolicy;

    public DistributedPollyControllerController(
        ILogger<DistributedPollyControllerController> logger,
        ResiliencePipelineBuilder resiliencePipelineBuilder,
        //IReadOnlyPolicyRegistry<string> policyRegistry,
        TimeProvider? timeProvider = null)
    {
        _logger = logger;
        _distributedCache = distributedCache;
        _timeProvider = timeProvider ?? TimeProvider.System;
        _pipeline = resiliencePipelineBuilder
           .AddRetry(new RetryStrategyOptions()) // Add retry using the default options
           .AddTimeout(TimeSpan.FromSeconds(10)) // Add 10 seconds timeout
           .Build(); // Builds the resilience pipeline

        //_distributedPolicy = policyRegistry.Get<IAsyncPolicy<DateTimeOffset>>("py-cache");
    }

    [HttpGet("distributed/{id}")]
    public async Task<DateTimeOffset> GetDistrebutedAsync(int id)
    {
        string key = $"date/{id}";
        var cached = await _distributedCache.GetStringAsync(key);
        if (cached != null)
        {
            var cachedResult = JsonSerializer.Deserialize<DateTimeOffset>(cached);
            return cachedResult;
        }

        DateTimeOffset now = _timeProvider.GetUtcNow();
        var json = JsonSerializer.Serialize(now);
        await _distributedCache.SetStringAsync(key, json, SLIDING);
        return now;
    }

    [HttpDelete("distributed/{id}")]
    public async Task<IActionResult> DeleteDistributedAsync(int id)
    {
        string key = $"date/{id}";
        await _distributedCache.RemoveAsync(key);
        return Ok();
    }

    [HttpPut("distributed/{id}")]
    public async Task<DateTimeOffset> UpdateDistributedAsync(int id)
    {
        string key = $"date/{id}";
        var cached = await _distributedCache.GetStringAsync(key);
        if (cached != null)
        {
            var cachedResult = JsonSerializer.Deserialize<DateTimeOffset>(cached);
            return cachedResult;
        }

        DateTimeOffset now = _timeProvider.GetUtcNow();
        var json = JsonSerializer.Serialize(now);
        await _distributedCache.SetStringAsync(key, json, SLIDING);
        return now;
    }

}
