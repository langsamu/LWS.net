using Model.EARL;
using Model.NewModel;
using System.Diagnostics;

namespace Model;

public static class Executor
{
    internal static readonly ActivitySource ActivitySource = new(typeof(Executor).FullName!);

    public static IEnumerable<NewModel.TestCase> Tests => Resources.Graph.Manifest.Tests;

    public static async Task<EarlGraph> Execute(Context context)
    {
        var graph = new EarlGraph(new VDS.RDF.Graph());

        var assertor = Assertor.Create(graph);
        assertor.Title = "NAME OF ASSERTOR"; // TODO: Don't hardcode

        var suiteRequirement = TestRequirement.Create(graph);
        suiteRequirement.Title = "NAME OF TEST SUITE"; // TODO: Don't hardcode

        foreach (var testCase in Tests)
        {
            var result3 = await Execute(testCase, context);
            var entryRequirement = TestRequirement.Create(graph);
            entryRequirement.Title = testCase.Name;
            entryRequirement.IsPartOf = suiteRequirement;

            var assertion = EARL.Assertion.Create(graph);
            assertion.AssertedBy = assertor;

            var test = assertion.Test = EARL.TestCase.Create(graph);
            test.Title = testCase.Name;
            test.IsPartOf = entryRequirement;

            var result = assertion.Result = TestResult.Create(graph);
            result.Info = result3.Info;
            result.Outcome = result3.Outcome switch
            {
                "Pass" => EARL.Vocabulary.Passed,
                "Fail" => EARL.Vocabulary.Failed,
                "Inconclusive" => EARL.Vocabulary.CantTell,
                _ => throw new InvalidOperationException($"Unknown outcome: {result3.Outcome}")
            };
        }

        return graph;
    }

    public static async Task<Result> Execute(string name, Context context)
    {
        var test = Tests.Single(test => test.Name == name);
        return await Execute(test, context);
    }

    private static async Task<Result> Execute(NewModel.TestCase test, Context context)
    {
        using var activity = ActivitySource.StartActivity(test.Name);

        return await test.Execute(context);
    }
}
