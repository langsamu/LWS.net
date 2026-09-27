namespace Model.NewModel.Expressions;

public partial class ResolveExpression : Expression
{
    protected ResolveExpression(INode node, IGraph graph) : base(node, graph) { }

    public Expression Base => this.Singular(Vocabulary.Base, Expression.Choose);

    public Expression Relative => this.Singular(Vocabulary.Relative, Expression.Choose);

    public static ResolveExpression Wrap(INode node, IGraph graph) => new(node, graph);

    public static ResolveExpression? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
