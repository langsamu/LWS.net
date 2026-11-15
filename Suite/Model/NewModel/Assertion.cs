namespace Model.NewModel;

public abstract partial class Assertion
{
    public abstract Task<Result> Execute(Context context);
}
