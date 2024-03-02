using Microsoft.AspNetCore.Mvc;

namespace DemoOfHttpClient.Controllers;
[ApiController]
[Route("[controller]")]
public class TypedController : ControllerBase
{
    private readonly ILogger<TypedController> _logger;
    private readonly HttpCatFacts _client;

    public TypedController(
        ILogger<TypedController> logger,
        HttpCatFacts client)
    {
        _logger = logger;
        _client = client;
    }

    [HttpGet]
    [ProducesResponseType<CatFact>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync()
    {
        CatFact fact = await _client.GetCatFactAsync();
        return Ok(fact);
    }
}
