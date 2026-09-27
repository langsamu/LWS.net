namespace Model.NewModel;

public class Executor
{
    public static IEnumerable<TestCase> Tests => Resources.Graph.Manifest.Tests;

    public async static Task<Result> Execute(string name, Context context)
    {
        var test = Tests.Single(test => test.Name == name);
        return await test.Execute(context, new HttpClient());
    }
}
