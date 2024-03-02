using Microsoft.AspNetCore.Mvc;

namespace DemoOfHttpClient.Controllers;
[ApiController]
[Route("[controller]")]
public class FactoryController : ControllerBase
{
    private readonly ILogger<FactoryController> _logger;
    private readonly HttpClient _client;

    public FactoryController(
        ILogger<FactoryController> logger,
        IHttpClientFactory factory)
    {
        _logger = logger;
        _client = factory.CreateClient("cat-facts");
    }

    [HttpGet]
    [ProducesResponseType<CatFact>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAsync()
    {
        CatFact fact = await _client.GetFromJsonAsync<CatFact>("");
        return Ok(fact);
    }
}
