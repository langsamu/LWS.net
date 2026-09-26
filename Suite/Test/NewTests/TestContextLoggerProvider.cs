using Microsoft.Extensions.Logging;

namespace Test.NewTests;

sealed class TestContextLoggerProvider(TestContext testContext) : ILoggerProvider
{
    ILogger ILoggerProvider.CreateLogger(string categoryName) => new TestContextLogger(testContext, categoryName);
    void IDisposable.Dispose() { }

    private sealed class TestContextLogger(TestContext testContext, string category) : ILogger
    {
        IDisposable? ILogger.BeginScope<TState>(TState state) => throw new NotImplementedException();

        bool ILogger.IsEnabled(LogLevel logLevel) => throw new NotImplementedException();

        void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            testContext.WriteLine("[{0}] {1}: {2}", logLevel, category, formatter(state, exception));
        }
    }
}
