namespace Model.NewModel;

public partial class TestCase
{
    public async Task<Result> Execute(Context context, HttpClient client)
    {
        foreach (var step in Steps)
        {
            await step.Execute(context, client);
        }

        return Assertion.Execute(context);
    }
}
