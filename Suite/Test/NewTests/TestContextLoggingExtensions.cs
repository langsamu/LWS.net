using Microsoft.Extensions.Logging;

namespace Test.NewTests;

internal static class TestContextLoggingExtensions
{
    public static ILoggingBuilder AddTestContext(this ILoggingBuilder builder) =>
        builder.AddProvider(new TestContextLoggerProvider());
}
