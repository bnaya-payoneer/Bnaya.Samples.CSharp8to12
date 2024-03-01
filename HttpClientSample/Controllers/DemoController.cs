using Microsoft.AspNetCore.Mvc;

namespace HttpClientSample.Controllers;
[ApiController]
[Route("[controller]")]
public class DemoController : ControllerBase
{


    private readonly ILogger<DemoController> _logger;

    public DemoController(ILogger<DemoController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public Task<string> GetAsync()
    {
        return Task.FromResult("Hello World");
    }
}
