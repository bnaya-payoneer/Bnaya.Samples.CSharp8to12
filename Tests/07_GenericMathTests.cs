using System.Numerics;

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
        int[] values = [2, 3, 4];
        Assert.Equal(2 * 3 * 4, values.Mult());
    }
}
public static class GenericMathTestsExtensions
{
    public static T Mult<T>(this IEnumerable<T> values)
        where T : INumber<T>
    {
        T result = T.Zero;
        foreach (var item in values)
        {
            if (T.IsZero(result))
                result = item;
            else
                result *= item;
        }
        return result;
    }
}