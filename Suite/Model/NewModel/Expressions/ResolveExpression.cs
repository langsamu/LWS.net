using Microsoft.Extensions.Logging;

namespace Model.NewModel.Expressions;

public class ResolveExpression : Expression
{
    protected ResolveExpression(INode node, IGraph graph) : base(node, graph) { }

    public static ResolveExpression Wrap(INode node, IGraph graph) => new(node, graph);

    public static ResolveExpression? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    public Expression Base => this.Singular(Vocabulary.Base, Expression.Choose);

    public Expression Relative => this.Singular(Vocabulary.Relative, Expression.Choose);

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
