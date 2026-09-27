using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model;
using Model.NewModel;
using Test.NewTests;

namespace Test;

[TestClass]
public sealed class TestSuite
{
    private static IHost host;
    private static Executor executor;

    private static IEnumerable<TestDataRow<string>> TestCases =>
        Executor.Tests.Select(test =>
            new TestDataRow<string>(test.Name)
            {
                DisplayName = test.Name
                // TODO: ignore
                // TODO: categories
            });

    [AssemblyInitialize]
    public static async Task Initialize(TestContext _)
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = AppContext.BaseDirectory,
        });

        builder.Services.AddSuite(builder.Configuration);
        builder.Logging.AddTestContext();

        host = builder.Build();
        executor = host.Services.GetRequiredService<Executor>();
    }

    [AssemblyCleanup]
    public static void Cleanup()
    {
        host.Dispose();
    }

    [TestMethod]
    [DynamicData(nameof(TestCases))]
    public async Task Entry(string testCase)
    {
        var context = host.Services.GetRequiredService<Context>();

        var result = await executor.Execute(testCase, context);

        switch (result.Outcome)
        {
            case "Fail":
                Assert.Fail(result.Info);
                break;
            case "Inconclusive":
                Assert.Inconclusive(result.Info);
                break;
        }
    }
}
