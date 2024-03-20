using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.Registry;
using Polly.Retry;
using System.Text.Json;

namespace CachingDemo.Controllers;
[ApiController]
[Route("[controller]")]
public class DistributedCachingController : ControllerBase
{
    private readonly ILogger<DistributedCachingController> _logger;
    private readonly IDistributedCache _distributedCache;
    private readonly TimeProvider _timeProvider;
    private static readonly DistributedCacheEntryOptions SLIDING = new DistributedCacheEntryOptions
    {
        SlidingExpiration = TimeSpan.FromSeconds(6)
    };

    public DistributedCachingController(
        ILogger<DistributedCachingController> logger,
        IDistributedCache distributedCache,
        TimeProvider? timeProvider = null)
    {
        _logger = logger;
        _distributedCache = distributedCache;
        _timeProvider = timeProvider ?? TimeProvider.System;
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
}
