using Microsoft.Extensions.Logging;

namespace Test.NewTests;

// Writes to the TestContext of whichever test is executing, so one provider serves the whole host
#pragma warning disable MSTESTEXP // TestContext.Current is experimental
sealed class TestContextLoggerProvider : ILoggerProvider
{
    ILogger ILoggerProvider.CreateLogger(string categoryName) => new TestContextLogger(categoryName);

    void IDisposable.Dispose() { }

    private sealed class TestContextLogger(string category) : ILogger
    {
        IDisposable? ILogger.BeginScope<TState>(TState state) => null;

        bool ILogger.IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None && TestContext.Current is not null;

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            TestContext.Current?.WriteLine("[{0}] {1}: {2}", logLevel, category, formatter(state, exception));
        }
    }
}
