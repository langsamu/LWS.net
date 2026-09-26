using Microsoft.Extensions.Logging;

namespace Test.NewTests;

internal static class TestContextLoggingExtensions
{
    public static ILoggingBuilder AddTestContext(this ILoggingBuilder builder, TestContext testContext) =>
        builder.AddProvider(new TestContextLoggerProvider(testContext));
}
