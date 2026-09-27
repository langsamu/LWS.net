using Microsoft.Extensions.Logging;

namespace Model.NewModel.Expressions;

public partial class ConstantExpression
{
    public override async Task<string> Evaluate(Context context)
    {
        var logger = context.LoggerFactory.CreateLogger<ConstantExpression>();
        logger.LogInformation("Evaluate [{0}]", Value);
        return Value;
    }
}
