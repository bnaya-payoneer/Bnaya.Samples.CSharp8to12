using Xunit.Abstractions;

namespace Tests;

public class PatternMatchingExpressionTests
{
    private readonly ITestOutputHelper _logger;

    public PatternMatchingExpressionTests(ITestOutputHelper logger)
    {
        _logger = logger;
    }

    [Theory]
    [InlineData(-1, "Solid")]
    [InlineData(0, "solid / liquid transition")]
    [InlineData(100, "liquid / gas transition")]
    [InlineData(60, "liquid")]
    [InlineData(101, "gas")]
    public void WaterTest(int temperature, string expected)
    {
        string state = temperature switch
        {
            < 0 => "Solid",
            0 => "solid / liquid transition",
            100 => "liquid / gas transition",
            < 100 => "liquid",
            _ => "gas"
        };

        _logger.WriteLine($"{temperature}: {state}");
        Assert.Equal(expected, state);
    }

    [Theory]
    [InlineData("1,DEPOSIT,,30", 30)]
    [InlineData("2,WITHDRAWAL,bla bla,2017,09,10", 10)]
    [InlineData("3,FEE,1,discount,vip", 1)]
    public void CsvTest(string line, decimal expected)
    {
        var columns = line.Split(',');
        decimal value = columns switch
        {
            [_, "DEPOSIT", _, var amount] => amount.ToDecimal(),
            [_, "WITHDRAWAL", .., var amount] => amount.ToDecimal(),
            [_, "FEE", var amount, ..] => amount.ToDecimal(),
            _ => throw new InvalidOperationException()
        };

        _logger.WriteLine($"value: {value}");
        Assert.Equal(expected, value);
    }
}

public static class PatternMatchingExpressionExtensions
{
    public static decimal ToDecimal(this string? amount) =>
                amount is not null
                ? decimal.Parse(amount)
                : 0;
}