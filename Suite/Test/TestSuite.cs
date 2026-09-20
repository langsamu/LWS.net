using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;
using Model.EARL;
using Model.TestManifest;

namespace Test;

[TestClass]
public sealed class TestSuite
{
    private static IEnumerable<Assertion> Assertions;

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
        Assertions = report.Assertions;
    }

    public static IEnumerable<TestDataRow<string>> TestCases
    {
        get
        {
            foreach (var manifest in Resources.ManifestGraph.Manifests)
            {
                foreach (var entry in manifest.Entries)
                {
                    var categories = entry.Traits.Select(static t => t.ToString()).ToList();
                    var ignore = entry.Status == Status.Pending ? "Test ignored due to pending status" : null;

                    if (entry.Response.StatusCode is not null)
                    {
                        var name = Executor.TestName(manifest, entry, "status code");
                        yield return new TestDataRow<string>(name) { DisplayName = name, TestCategories = categories, IgnoreMessage = ignore };
                    }

                    if (entry.Response.ContentType is not null)
                    {
                        var name = Executor.TestName(manifest, entry, "content type");
                        yield return new TestDataRow<string>(name) { DisplayName = name, TestCategories = categories, IgnoreMessage = ignore };
                    }

                    if (entry.Response.Body is not null)
                    {
                        var name = Executor.TestName(manifest, entry, "body");
                        yield return new TestDataRow<string>(name) { DisplayName = name, TestCategories = categories, IgnoreMessage = ignore };
                    }

                    foreach (var header in entry.Response.OtherHeaders)
                    {
                        var name = Executor.TestName(manifest, entry, $"header {header.HeaderName}");
                        yield return new TestDataRow<string>(name) { DisplayName = name, TestCategories = categories, IgnoreMessage = ignore };
                    }

                    if (entry.Response.AuthenticationChallenge is not null)
                    {
                        var name = Executor.TestName(manifest, entry, "authentication challenge");
                        yield return new TestDataRow<string>(name) { DisplayName = name, TestCategories = categories, IgnoreMessage = ignore };
                    }

                    // TODO: linkHeaders
                }
            }
        }
    }

    [TestMethod]
    [DynamicData(nameof(TestCases))]
    public void Entry(string testCase)
    {
        var assertion = Assertions.Single(assertion => assertion.Test.Title == testCase);

        if (assertion.Result.Outcome.Equals(Vocabulary.Failed))
        {
            Assert.Fail(assertion.Result.Info);
        }
        else if (assertion.Result.Outcome.Equals(Vocabulary.CantTell))
        {
            Assert.Inconclusive(assertion.Result.Info);
        }
    }
}
