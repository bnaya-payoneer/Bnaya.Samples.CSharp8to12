using FakeItEasy;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Xunit.Abstractions;

namespace Tests;

public class AsyncStreamTests
{
    private readonly ITestOutputHelper _logger;
    
    public AsyncStreamTests(ITestOutputHelper logger)
    {
        _logger = logger;
    }

    [Theory]
    [InlineData(100 * 3 + 50, 3)]
    [InlineData(100 * 5 + 50, 5)]
    public async Task AsynStreamTest(int duration, int expectedLength)
    {
        _logger.WriteLine($"duration: {duration}");
        var sw = Stopwatch.StartNew();
        var ct = new CancellationTokenSource(duration).Token;
        var result = await AsyncStreamExtensiosn.CreateAsyncStream(ct) // pass cancellation
                                                .Log(_logger, sw)
                                                .Count();

        _logger.WriteLine($"result: {result}");
        Assert.Equal(expectedLength, result);
    }

    [Theory]
    [InlineData(100 * 3 + 50, 3)]
    [InlineData(100 * 5 + 50, 5)]
    public async Task AsynStreamsTest(int duration, int expectedLength)
    {
        _logger.WriteLine($"duration: {duration}");
        var stream = AsyncStreamExtensiosn.CreateAsyncStream();

        for (int i = 0; i < 3; i++)
        {
            _logger.WriteLine($"Iteration: {i}");
            var sw = Stopwatch.StartNew();
            var ct = new CancellationTokenSource(duration).Token;

            var result = await stream
                                .WithCancellation(ct) // pass cancellation
                                .Log(_logger, sw)
                                .Count();
            _logger.WriteLine($"result: {result}");
            Assert.Equal(expectedLength, result);
            sw.Stop();
        }         
    }
}

public static class AsyncStreamExtensiosn
{
    public static async IAsyncEnumerable<int> CreateAsyncStream(
                        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        int i = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(100, cancellationToken);
            }
            catch (OperationCanceledException cancelled)
            {
                break;
            }
            yield return i++;
        }
    }

    public static async IAsyncEnumerable<int> Log(
                this IAsyncEnumerable<int> enumerable,
                ITestOutputHelper logger,
                Stopwatch sw)
    {
        await foreach (var i in enumerable)
        {
            logger.WriteLine($"{i}: {sw.ElapsedMilliseconds}");
            yield return i;
        }
    }

    public static async IAsyncEnumerable<int> Log(
                this ConfiguredCancelableAsyncEnumerable<int> enumerable,
                ITestOutputHelper logger,
                Stopwatch sw)
    {
        await foreach (var i in enumerable)
        {
            logger.WriteLine($"{i}: {sw.ElapsedMilliseconds}");
            yield return i;
        }
    }

    public static async Task<int> Count(this IAsyncEnumerable<int> enumerable)
    {
        int i = 0;
        await foreach (var item in enumerable)
        {
            i++;
        }
        return i;
    }

    public static async Task<int> Count(this ConfiguredCancelableAsyncEnumerable<int> enumerable)
    {
        int i = 0;
        await foreach (var item in enumerable)
        {
            i++;
        }
        return i;
    }
}