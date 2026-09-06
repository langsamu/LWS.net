using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;
using Model.EARL;

namespace Test;

[TestClass]
public sealed class TestSuite
{
    private static IEnumerable<TestDataRow<Assertion>> Assertions;

    [AssemblyInitialize]
    public static async Task Initialize(TestContext _)
    {
        var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
        {
            ContentRootPath = AppContext.BaseDirectory,
        });

        builder.Services.AddSuite(builder.Configuration);

        using var host = builder.Build();
        var suite = host.Services.GetRequiredService<Executor>();

        var report = await suite.Execute(Resources.ManifestGraph);
        Assertions = report.Assertions.Select(assertion =>
            new TestDataRow<Assertion>(assertion)
            {
                DisplayName = assertion.Test.Title,
                //TestCategories = [.. entry.Traits.Select(t => t.ToString())],
                //IgnoreMessage =assertion.Result entry.Status == Status.Pending ? "Test ignored due to pending status" : null, // TODO: What's the real logic?
            }
        );
    }

    [TestMethod]
    //[Something]
    [DynamicData(nameof(Assertions))]
    public void Entry(Assertion assertion)
    {
        if (assertion.Result.Outcome.Equals(Vocabulary.Failed))
        {
            Assert.Fail(assertion.Result.Info);
        }
        else if (assertion.Result.Outcome.Equals(Vocabulary.CantTell))
        {
            Assert.Inconclusive(assertion.Result.Info);
        }

        // TODO: Fail/pass based on assertion.Result.Outcome
        //Console.WriteLine(assertion);
    }
}
