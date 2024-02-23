using Xunit.Abstractions;

namespace Tests;

public class CollectionExpressionTests
{
    private readonly ITestOutputHelper _logger;

    public CollectionExpressionTests(ITestOutputHelper logger)
    {
        _logger = logger;
    }

    [Fact]
    public void Sampl1Test()
    {
        int[] ids = [1, 2, 3, 4];
        IEnumerable<string> names = ["Lory", "Bob", "Marry", "Sammuel"];
        List<double> scores = [32.4, 56.3, 89.4, 90];
        _logger.WriteLine($"{ids.GetType().Name}: {string.Join(", ", ids)}");
        _logger.WriteLine($"{names.GetType().Name}: {string.Join(", ", names)}");
        _logger.WriteLine($"{scores.GetType().Name}: {string.Join(", ", scores)}");
    }

    [Fact]
    public void SpreadTest()
    {
        ICollection<string> lista = ["Lory", "Bob", "Marry", "Samuel"];
        string[] listb = ["Funny", "Alberta", "Calmy", "Paul"];
        IEnumerable<string> spreed = [.. lista, .. listb];
        _logger.WriteLine($"lista is: {lista.GetType().Name}");
        _logger.WriteLine($"{spreed.GetType().Name} {string.Join(", ", spreed)}");
    }
}