using Microsoft.AspNetCore.Mvc;
using Bnaya.Samples.Entities;

namespace Bnaya.Samples.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SampleController : ControllerBase
    {
        private readonly ILogger<SampleController> _logger;

        public SampleController(
            ILogger<SampleController> logger//,
            )
        {
            _logger = logger;
        }

        [HttpGet("v1/{id}")]
        [ProducesResponseType<Person>(200)]
        public async Task<IActionResult> GetV1Async(int id)
        {
            await Task.Delay((id % 10) * 100);
            Person result = GetPerson(id);
            _logger.LogSample(result);
            return Ok(result);
        }

        [HttpGet("v2/{id}")]
        [ProducesResponseType<Person>(200)]
        public async Task<IActionResult> GetV2Async(int id)
        {
            await Task.Delay((id % 10) * 100);
            Person result = GetPerson(id);
            _logger.LogSample(result);
            return Ok(result);
        }

        private static Person GetPerson(int id)
        {
            return new Person(
                id,
                Faker.Name.First(),
                Faker.Name.Last(),
                Faker.Phone.Number(),
                "1234");
        }
    }
}
