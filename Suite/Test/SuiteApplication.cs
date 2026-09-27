using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Testing.Platform.Services;
using Model;
using Model.NewModel;
using System.Diagnostics.CodeAnalysis;
using Test.NewTests;

namespace Test;

internal static class SuiteApplication
{
    private static IHost? host;

    private static IServiceProvider Services => host?.Services ?? throw new InvalidOperationException("Host not initialised.");

    internal static async Task<Result> Test(string testCase)
    {
        var executor = Services.GetRequiredService<Executor>();
        var context = Services.GetRequiredService<Context>();

        return await executor.Execute(testCase, context);
    }

    [AssemblyInitialize]
    [SuppressMessage("Usage", "MSTEST0012:AssemblyInitialize methods should have valid layout", Justification = "Analyzer doesn't know AssemblyFixtureProvider")]
    public static async Task Initialize(TestContext testContext)
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = AppContext.BaseDirectory,
        });

        builder.Services.AddSuite(builder.Configuration);
        builder.Logging.ClearProviders().AddTestContext();

        host = builder.Build();
        await host.StartAsync(testContext.CancellationToken);
    }

    [AssemblyCleanup]
    [SuppressMessage("Usage", "MSTEST0013:AssemblyCleanup methods should have valid layout", Justification = "Analyzer doesn't know AssemblyFixtureProvider")]
    public static async Task Cleanup(TestContext testContext)
    {
        if (host is null)
        {
            return;
        }

        await host.StopAsync(testContext.CancellationToken);
        host.Dispose();
    }
}
