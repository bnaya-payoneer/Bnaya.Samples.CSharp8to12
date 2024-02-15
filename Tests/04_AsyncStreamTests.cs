using FakeItEasy;
using System;
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
        var result = await AsyncStreamExtensiosn.CreateAsyncStream(cancellationToken: ct) // pass cancellation
                                                .Log(_logger, sw)
                                                .Count();

        _logger.WriteLine($"result: {result}");
        Assert.Equal(expectedLength, result);
    }

    [Theory]
    [InlineData(100 * 3 + 50, 3)]
    [InlineData(100 * 5 + 50, 5)]
    public async Task AsynStreamsest(int duration, int expectedLength)
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

    #region TimeManagement

    sealed record TimeManagement: IAsyncDisposable
    {
        private readonly Task _triggering;

        #region Ctor

        public TimeManagement(int expectedLength)
        {
            TimeProvider = A.Fake<TimeProvider>();

            #region A.CallTo(() => timeProvider.CreateTimer(...)

            A.CallTo(() => TimeProvider.CreateTimer(A<TimerCallback>.Ignored, A<object?>.Ignored, A<TimeSpan>.Ignored, A<TimeSpan>.Ignored))
                .Invokes((TimerCallback cb, object? state, TimeSpan dutime, TimeSpan period) =>
                {
                    if (state is CancellationTokenSource)
                    {
                        CancellationTimerCallback = cb;
                        CancellationState = state;
                    }
                    else
                    {
                        StreamTimerCallback = cb;
                        StreamState = state;
                        Sync.Release();
                    }
                });

            #endregion // A.CallTo(() => timeProvider.CreateTimer(...)

            _triggering = InitAsync(expectedLength);
        }

        #endregion // Ctor

        #region InitAsync

        private Task InitAsync(int expectedLength)
        { 
            return Task.Run(async () =>
            {
                await Sync.WaitAsync();
                for (int i = 0; i < expectedLength; i++)
                {
                    StreamTimerCallback?.Invoke(StreamState); // async stream
                    await Task.Delay(1);
                }
                CancellationTimerCallback?.Invoke(CancellationState); // cancellation
                await Task.Delay(1);
                StreamTimerCallback?.Invoke(StreamState); // async stream
            });
        }

        #endregion // InitAsync

        public TimerCallback? StreamTimerCallback { get; set; }
        public object? StreamState { get; set; }
        public TimerCallback? CancellationTimerCallback { get; set; }
        public object? CancellationState { get; set; }
        public TimeProvider TimeProvider { get; }
        public SemaphoreSlim Sync { get; } = new SemaphoreSlim(1);

        public async ValueTask DisposeAsync()
        {
            await _triggering;
        }
    }

    #endregion // TimeManagement

    [Theory]
    [InlineData(100 * 3 + 50, 3)]
    [InlineData(100 * 5 + 50, 5)]
    public async Task AsynStream_TimeAbstraction_Test(int duration, int expectedLength)
    {
        await using var timeManagement = new TimeManagement(expectedLength); // IAsyncDisposable
        await timeManagement.Sync.WaitAsync();

        _logger.WriteLine($"duration: {duration}");
        var sw = Stopwatch.StartNew();
        var ct = new CancellationTokenSource(
                        TimeSpan.FromMilliseconds(duration),
                        timeManagement.TimeProvider).Token;
        var result = await AsyncStreamExtensiosn.CreateAsyncStream(timeManagement.TimeProvider, ct) // pass cancellation
                                                .Log(_logger, sw)
                                                .Count();

        _logger.WriteLine($"result: {result}");
        Assert.Equal(expectedLength, result);
    }
}

public static class AsyncStreamExtensiosn
{
    public static async IAsyncEnumerable<int> CreateAsyncStream(
                        TimeProvider? timeProvider = default,
                        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        timeProvider = timeProvider ?? TimeProvider.System;
        int i = 0;
        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(
                            TimeSpan.FromMilliseconds(100),
                            timeProvider,
                            cancellationToken);
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