namespace Model.NewModel.Expressions;

public partial class ParamRefExpression
{
    public override async Task<string> Evaluate(Context context)
    {
        var value = context.Get(Param);

        context.Log<ParamRefExpression>("Evaluate: [{0}] = [{1}]", Param, value);

        return value;
    }
}
