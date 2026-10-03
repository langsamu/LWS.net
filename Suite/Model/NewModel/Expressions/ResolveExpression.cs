namespace Model.NewModel.Expressions;

public partial class ResolveExpression
{
    public override async Task<string> Evaluate(Context context)
    {
        var baseUri = await Base.Evaluate(context);
        var uri = await Relative.Evaluate(context);
        var result = new Uri(new Uri(baseUri), uri);

        context.Log<ResolveExpression>("Evaluate: baseUri = [{BaseUri}], relativeUri = [{RelativeUri}], result = [{Result}]", baseUri, uri, result);

        return result.ToString();
    }
}
