using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Comfyg.Tests.Common;

internal class TestLogger : ILogger
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TestLogger(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public IDisposable BeginScope<TState>(TState state) => null!;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        try
        {
            _testOutputHelper.WriteLine($"[{logLevel}]: {formatter(state, exception)}");
            if (exception != null)
            {
                _testOutputHelper.WriteLine(exception.ToString());
            }
        }
        catch
        {
            // ignored
        }
    }
}
