using Microsoft.Extensions.Logging;

namespace Model.NewModel.Expressions;

public class ParamRefExpression : Expression
{
    protected ParamRefExpression(INode node, IGraph graph) : base(node, graph) { }

    public static ParamRefExpression Wrap(INode node, IGraph graph) => new(node, graph);

    public static ParamRefExpression? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    public string Param => this.Singular(Vocabulary.Param, ValueMappings.As<string>);

    public override async Task<string> Evaluate(Context context)
    {
        var value = context.Get(Param);
        var logger = context.LoggerFactory.CreateLogger<ParamRefExpression>();
        logger.LogInformation("Evaluate: [{0}] = [{1}]", Param, value);

        return value;
    }
}
