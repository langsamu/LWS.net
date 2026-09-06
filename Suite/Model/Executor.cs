using Microsoft.Extensions.Options;
using Model.EARL;
using Model.LWST;
using Model.TestManifest;

namespace Model;

public sealed class Executor(HttpClient client, IOptions<SuiteOptions> options)
{
    private readonly SuiteOptions options = options.Value;

    public async Task<EarlGraph> Execute(ManifestGraph suite)
    {
        var result = new EarlGraph(new Graph());

        var assertor = Assertor.Create(result);
        assertor.Title = "NAME OF ASSERTOR"; // TODO: Don't hardcode

        var suiteRequirement = TestRequirement.Create(result);
        suiteRequirement.Title = "NAME OF TEST SUITE"; // TODO: Don't hardcode

        foreach (var manifest in suite.Manifests)
        {
            var manifestRequirement = TestRequirement.Create(manifest.Id!, result);
            manifestRequirement.Title = manifest.Label;
            manifestRequirement.IsPartOf = suiteRequirement;

            foreach (var entry in manifest.Entries)
            {
                var response = await Send(entry.Request);
                Process(response, result, assertor, entry, manifestRequirement);
            }
        }

        return result;
    }

    private async Task<HttpResponseMessage?> Send(Request request)
    {
        var requestMessage = new HttpRequestMessage(new HttpMethod(request.Method), new Uri(options.BaseUri, request.Url));

        try
        {
            return await client.SendAsync(requestMessage).ConfigureAwait(false);
        }
        catch (Exception)
        {
            return null;
        }
    }

    private static void Process(HttpResponseMessage? response, EarlGraph result, Assertor assertor, LWST.Entry entry, TestRequirement manifestRequirement)
    {
        var entryRequirement = TestRequirement.Create(entry.Id!, result);
        entryRequirement.Title = entry.Name;
        entryRequirement.IsPartOf = manifestRequirement;

        // TODO: Extract commonalities
        if (entry.Response.StatusCode is not null)
        {
            var assertion = Assertion.Create(result);
            assertion.AssertedBy = assertor;

            assertion.Test = TestCase.Create(result);
            assertion.Test.Title = $"{entry.Name} - status code";
            assertion.Test.IsPartOf = entryRequirement;

            assertion.Result = TestResult.Create(result);

            if (response is null)
            {
                assertion.Result.Outcome = EARL.Vocabulary.Failed;
                assertion.Result.Info = "No response";
                return;
            }

            var correct = (long)response.StatusCode == entry.Response.StatusCode;
            assertion.Result.Outcome = correct ? EARL.Vocabulary.Passed : EARL.Vocabulary.Failed;
            if (!correct)
            {
                assertion.Result.Info = $"Expected status code [{entry.Response.StatusCode}], but got [{(long)response.StatusCode}]";
            }
        }

        if (entry.Response.ContentType is not null)
        {
            var assertion = Assertion.Create(result);
            assertion.AssertedBy = assertor;

            assertion.Test = TestCase.Create(result);
            assertion.Test.Title = $"{entry.Name} - content type";
            assertion.Test.IsPartOf = entryRequirement;

            assertion.Result = TestResult.Create(result);

            if (response is null)
            {
                assertion.Result.Outcome = EARL.Vocabulary.Failed;
                assertion.Result.Info = "No response";
                return;
            }

            var correct = response.Content.Headers.ContentType?.MediaType.Equals(entry.Response.ContentType, StringComparison.OrdinalIgnoreCase) is true;
            assertion.Result.Outcome = correct ? assertion.Result.Outcome = EARL.Vocabulary.Passed : assertion.Result.Outcome = EARL.Vocabulary.Failed;
            if (!correct)
            {
                assertion.Result.Info = $"Expected content type [{entry.Response.ContentType}], but got [{response.Content.Headers.ContentType?.MediaType}]";
            }
        }
    }
}
