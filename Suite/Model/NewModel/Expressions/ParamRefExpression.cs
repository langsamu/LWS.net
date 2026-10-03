namespace Model.NewModel.Expressions;

public partial class ParamRefExpression
{
    public override async Task<string> Evaluate(Context context)
    {
        var value = context.Get(Param);

        context.Log<ParamRefExpression>("Evaluate: [{Param}] = [{Value}]", Param, value);

        return value;
    }
}
