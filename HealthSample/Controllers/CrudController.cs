using Microsoft.AspNetCore.Mvc;

namespace HealthSample.Controllers;
[ApiController]
[Route("[controller]")]
public class CrudController : ControllerBase
{
    private readonly ILogger<CrudController> _logger;

    public CrudController(ILogger<CrudController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType<string>(200)]
    public async Task<IActionResult> GetAsync()
    {
        await Task.Delay(100);
        return Ok("Hi");
    }
}
