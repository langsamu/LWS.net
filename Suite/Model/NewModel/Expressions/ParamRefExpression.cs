using Microsoft.Extensions.Logging;

namespace Model.NewModel.Expressions;

public partial class ParamRefExpression
{
    public override async Task<string> Evaluate(Context context)
    {
        var value = context.Get(Param);
        var logger = context.LoggerFactory.CreateLogger<ParamRefExpression>();
        logger.LogInformation("Evaluate: [{0}] = [{1}]", Param, value);

        return value;
    }
}
