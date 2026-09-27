using Microsoft.Extensions.Logging;

namespace Model.NewModel.Expressions;

public partial class ResolveExpression
{
    public override async Task<string> Evaluate(Context context)
    {
        var baseUri = await Base.Evaluate(context);
        var uri = await Relative.Evaluate(context);
        var result = new Uri(new Uri(baseUri), uri);

        var logger = context.LoggerFactory.CreateLogger<ResolveExpression>();
        logger.LogInformation("Evaluate: baseUri = [{0}], relativeUri = [{1}], result = [{2}]", baseUri, uri, result);
        return result.ToString();
    }
}
