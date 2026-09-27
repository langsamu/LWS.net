namespace Model.NewModel;

public partial class TestCase
{
    public async Task<Result> Execute(Context context)
    {
        foreach (var step in Steps)
        {
            await step.Execute(context);
        }

        return Assertion.Execute(context);
    }
}
