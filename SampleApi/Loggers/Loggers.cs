using Bnaya.Samples.Entities;


namespace Bnaya.Samples;

public static partial class Loggers
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Sample Log")]
    public static partial void LogSample(
      this ILogger logger,
      [LogProperties(SkipNullProperties = true)]Person person);
}
