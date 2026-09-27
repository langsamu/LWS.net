using Microsoft.Extensions.Logging;
using Model.NewModel;

namespace Test.NewTests;

[TestClass]
public class TestSuite(TestContext testContext)
{
    private static IEnumerable<TestDataRow<string>> TestNames =>
        Executor.Tests.Select(test =>
            new TestDataRow<string>(test.Name)
            {
                DisplayName = test.Name
                // TODO: ignore
                // TODO: categories
            });

    [TestMethod]
    [DynamicData(nameof(TestNames))]
    public async Task NewTest(string name)
    {
        using var loggerFactory = LoggerFactory.Create(b => b.AddTestContext(testContext));

        var context = new Context(loggerFactory);
        context.Set("baseUri", "http://localhost:8080"); // TODO: make this configurable

        var result = await Executor.Execute(name, context);

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
