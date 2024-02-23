using System.Net.Http.Json;

using FakeItEasy;

using Xunit.Abstractions;

namespace Tests;

public class TimeAbsractionTests
{
    private readonly ITestOutputHelper _logger;
    private readonly TimeProvider _timeProvider = A.Fake<TimeProvider>();

    public TimeAbsractionTests(ITestOutputHelper logger)
    {
        _logger = logger;
    }

    [Fact]
    public async Task TimeAbsractionTest()
    {
        var facts = new Facts(_timeProvider);
        var date = DateTimeOffset.UtcNow;
        A.CallTo(() => _timeProvider.GetUtcNow()).Returns(date);
        string text1 = await facts.GetAsync();
        _logger.WriteLine(text1);
        A.CallTo(() => _timeProvider.GetUtcNow()).Returns(date + TimeSpan.FromSeconds(5));
        string text2 = await facts.GetAsync();
        _logger.WriteLine(text2);
        A.CallTo(() => _timeProvider.GetUtcNow()).Returns(date + TimeSpan.FromSeconds(11));
        string text3 = await facts.GetAsync();
        _logger.WriteLine(text3);
        Assert.Equal(text1, text2);
        Assert.NotEqual(text1, text3);
    }
    public record Result(string text);

    public class Facts
    {
        private string _text;
        private DateTimeOffset _date = DateTimeOffset.MinValue;
        private readonly HttpClient _http = new HttpClient();
        private readonly TimeProvider _timeProvider;
        private int index = 0;

        public Facts(TimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public async Task<string> GetAsync()
        {
            DateTimeOffset now = _timeProvider.GetUtcNow();
            if (now - _date < TimeSpan.FromSeconds(10))
                return _text;
            var http = new HttpClient();
            Result[] results = await _http.GetFromJsonAsync<Result[]>("https://cat-fact.herokuapp.com/facts/");
            _date = now;
            string text = results[index++ % results.Length].text;
            _text = text;
            return text;
        }
    }
}

