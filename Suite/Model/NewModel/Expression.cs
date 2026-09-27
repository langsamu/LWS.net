namespace Model.NewModel;

public abstract partial class Expression
{
    public abstract Task<string> Evaluate(Context context);
}
