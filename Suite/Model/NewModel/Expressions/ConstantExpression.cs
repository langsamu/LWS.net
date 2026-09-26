using Microsoft.Extensions.Logging;

namespace Model.NewModel.Expressions;

public class ConstantExpression : Expression
{
    protected ConstantExpression(INode node, IGraph graph) : base(node, graph) { }

    public static ConstantExpression Wrap(INode node, IGraph graph) => new(node, graph);

    public static ConstantExpression? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    public string Value { get; set; }

    public override async Task<string> Evaluate(Context context)
    {
        var logger = context.LoggerFactory.CreateLogger<ConstantExpression>();
        logger.LogInformation("Evaluate [{0}]", Value);
        return Value;
    }
}
