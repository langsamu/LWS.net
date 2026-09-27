using Microsoft.Extensions.Logging;

namespace Test;

internal static class TestContextLoggingExtensions
{
    public static ILoggingBuilder AddTestContext(this ILoggingBuilder builder) =>
        builder.AddProvider(new TestContextLoggerProvider());
}
