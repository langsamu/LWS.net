using Model;

namespace Test;

[TestClass]
public sealed class TestSuite
{
    private static IEnumerable<TestDataRow<string>> TestCases =>
        Executor.Tests.Select(test =>
            new TestDataRow<string>(test.Name)
            {
                DisplayName = test.Name
                // TODO: ignore
                // TODO: categories
            });

    [TestMethod]
    [DynamicData(nameof(TestCases))]
    public async Task Entry(string testCase)
    {
        var result = await SuiteApplication.Test(testCase);

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
