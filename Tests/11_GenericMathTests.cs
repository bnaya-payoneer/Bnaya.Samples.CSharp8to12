using FakeItEasy;
using Xunit.Abstractions;

namespace Tests;

public class GenericMathTests
{
    private readonly ITestOutputHelper _logger;

    public GenericMathTests(ITestOutputHelper logger)
    {
        _logger = logger;
    }

    [Fact]
    public void SimpleStringLiteralsTest()
    {
    }

    [Theory]
    [InlineData("marry", 88)]
    [InlineData("john", 53)]
    [InlineData("mike", 78)]
    public void FormatStringLiteralsTest(string name, int score)
    {
    }
}