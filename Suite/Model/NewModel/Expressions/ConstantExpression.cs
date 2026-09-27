namespace Model.NewModel.Expressions;

public partial class ConstantExpression
{
    public override async Task<string> Evaluate(Context context)
    {
        context.Log<ConstantExpression>("Evaluate [{0}]", Value);

        return Value;
    }
}
