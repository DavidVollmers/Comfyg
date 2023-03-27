using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace Comfyg.Tests.Common;

internal class TestLoggerProvider : ILoggerProvider
{
    private readonly ITestOutputHelper _testOutputHelper;

    public TestLoggerProvider(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    public void Dispose()
    {
    }

    public ILogger CreateLogger(string categoryName)
    {
        return new TestLogger(_testOutputHelper);
    }
}
