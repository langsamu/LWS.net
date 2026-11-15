namespace Model.NewModel.Assertions;

public partial class NotAssertion
{
    public override async Task<Result> Execute(Context context)
    {
        var inner = await Inner.Execute(context);
        return inner.Outcome == "Pass"
            ? new Result { Outcome = "Fail", Info = $"Operand passed, but expected it to fail: {inner.Info}" }
            : new Result { Outcome = "Pass", Info = $"Operand failed as expected: {inner.Info}" };
    }
}
