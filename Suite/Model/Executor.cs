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

    private static void Process(HttpResponseMessage? response, EarlGraph graph, Assertor assertor, LWST.Entry entry, TestRequirement manifestRequirement)
    {
        var entryRequirement = TestRequirement.Create(entry.Id!, graph);
        entryRequirement.Title = entry.Name;
        entryRequirement.IsPartOf = manifestRequirement;

        if (entry.Response.StatusCode is var statusCode)
        {
            Assert("status code", statusCode, response => (long)response.StatusCode);
        }

        if (entry.Response.ContentType is var contentType)
        {
            Assert("content type", contentType, response => response.Content.Headers.ContentType?.MediaType, StringComparer.OrdinalIgnoreCase);
        }

        void Assert<T>(string aspect, T expected, Func<HttpResponseMessage, T> actual, IEqualityComparer<T>? comparer = null)
        {
            var assertion = Assertion.Create(graph);
            assertion.AssertedBy = assertor;

            var test = assertion.Test = TestCase.Create(graph);
            test.Title = $"{entry.Name} - {aspect}";
            test.IsPartOf = entryRequirement;

            var result = assertion.Result = TestResult.Create(graph);

            if (response is null)
            {
                result.Outcome = EARL.Vocabulary.Failed;
                result.Info = "No response";

                return;
            }

            var value = actual(response);

            if (!(comparer ?? EqualityComparer<T>.Default).Equals(expected, value))
            {
                result.Outcome = EARL.Vocabulary.Failed;
                result.Info = $"Expected {aspect} [{expected}], but got [{value}]";

                return;
            }

            result.Outcome = EARL.Vocabulary.Passed;
        }
    }
}
