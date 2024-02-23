using Xunit.Abstractions;

namespace Tests;

public class RawStringLiteralsTests
{
    private readonly ITestOutputHelper _logger;

    public RawStringLiteralsTests(ITestOutputHelper logger)
    {
        _logger = logger;
    }

    [Fact]
    public void SimpleStringLiteralsTest()
    {
        string value = """
            It is a long established fact that a reader will be distracted 
            by the readable content of a page when looking at its layout. 
            The point of using Lorem Ipsum 
            is that it has a more-or-less normal distribution of letters,
            """;
        _logger.WriteLine(value);
    }

    [Theory]
    [InlineData("marry", 88)]
    [InlineData("john", 53)]
    [InlineData("mike", 78)]
    public void FormatStringLiteralsTest(string name, int score)
    {
        //string value = $"""
        //    {{
        //        "score": {score},
        //        "name": {name}
        //    }}
        //    """;
        string value = $$"""
            {
                "score": {{score}},
                "name": "{{name}}"
            }
            """;
        _logger.WriteLine(value);
    }
}