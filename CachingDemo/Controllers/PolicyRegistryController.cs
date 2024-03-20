using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Polly;
using Polly.Registry;
using Polly.Retry;
using System.Text.Json;

namespace CachingDemo.Controllers;
[ApiController]
[Route("[controller]")]
public class PolicyRegistryControllerController : ControllerBase
{
    private readonly ILogger<PolicyRegistryControllerController> _logger;
    private readonly TimeProvider _timeProvider;
    private readonly IAsyncPolicy _distributedPolicy;

    public PolicyRegistryControllerController(
        ILogger<PolicyRegistryControllerController> logger,
        IReadOnlyPolicyRegistry<string> policyRegistry,
        TimeProvider? timeProvider = null)
    {
        _logger = logger;
        _timeProvider = timeProvider ?? TimeProvider.System;

        _distributedPolicy = policyRegistry.Get<IAsyncPolicy>("py-cache");
    }

    [HttpGet("{failTimes}")]
    public async Task<IActionResult> GetDistrebutedAsync(int failTimes)
    {
        var now = _timeProvider.GetUtcNow();
        var list = new List<double>();
       await _distributedPolicy.ExecuteAndCaptureAsync(async () =>
       {
           double duration = (_timeProvider.GetUtcNow() - now).TotalSeconds;
           list.Add(duration);
           if (failTimes > 0)
           {
               failTimes--;
               throw new Exception("Failed");
           }
       });
        return Ok(list);
    }

}
