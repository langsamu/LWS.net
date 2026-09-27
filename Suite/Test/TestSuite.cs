using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model;
using Model.NewModel;
using Test.NewTests;

namespace Test;

[TestClass]
public sealed class TestSuite(TestContext testContext)
{
    private static IHost host;
    private static Executor executor;
    private static ILoggerFactory loggerFactory;

    private static IEnumerable<TestDataRow<string>> TestCases =>
        Executor.Tests.Select(test =>
            new TestDataRow<string>(test.Name)
            {
                DisplayName = test.Name
                // TODO: ignore
                // TODO: categories
            });

    [AssemblyInitialize]
    public static async Task Initialize(TestContext testContext)
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = AppContext.BaseDirectory,
        });

        builder.Services.AddSuite(builder.Configuration);

        host = builder.Build();
        executor = host.Services.GetRequiredService<Executor>();
        loggerFactory = LoggerFactory.Create(b => b.AddTestContext(testContext));
    }

    [AssemblyCleanup]
    public static void Cleanup()
    {
        host.Dispose();
        loggerFactory.Dispose();
    }

    [TestMethod]
    [DynamicData(nameof(TestCases))]
    public async Task Entry(string testCase)
    {
        var context = new Context(loggerFactory);
        context.Set("baseUri", "http://localhost:8080"); // TODO: make this configurable

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
