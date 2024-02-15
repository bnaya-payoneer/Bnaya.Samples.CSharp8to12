using Xunit.Abstractions;

using Acc = (string Value, int Index);

namespace Tests;

public class LocalFunctionTests
{
    private readonly ITestOutputHelper _logger;

    public LocalFunctionTests(ITestOutputHelper logger)
    {
        _logger = logger;
    }


    [Theory]
    [InlineData('*', 2, 5)]
    [InlineData('#', 3)]
    [InlineData('@', 4)]
    public void FormatStringLiteralsTest(char sign, int gap, int? ignoredGap = 100)
    {
        string text = "Lorem ipsum dolor sit amet, consectetur adipiscing.";
        (string chars, _) = text.Aggregate(default(Acc), Strategy);

        _logger.WriteLine(chars);

        Acc Strategy(Acc acc, char current)
        {
            var (value, index) = acc;
            char c = index switch
            {
                var x when x % ignoredGap == 0 => current,
                var x when x % gap == 0 => sign,
                _ => current
            };
            return ($"{value}{c}", index + 1);
        }
    }
}