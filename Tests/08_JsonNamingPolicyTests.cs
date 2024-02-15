using System.Text.Json;

using FakeItEasy;
using Xunit.Abstractions;

namespace Tests;

public class JsonNamingPolicyTests
{
    private readonly ITestOutputHelper _logger;

    public JsonNamingPolicyTests(ITestOutputHelper logger)
    {
        _logger = logger;
    }


    [Theory]
    [InlineData("BnayaEshet", "bnaya_eshet")]
    [InlineData("bnayaEshet", "bnaya_eshet")]
    public void FormatStringLiteralsTest(string name, string expected)
    {
        string snakeCase = JsonNamingPolicy.SnakeCaseLower.ConvertName(name);
        _logger.WriteLine(snakeCase);
        Assert.Equal(expected, snakeCase);
    }
}