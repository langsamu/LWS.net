namespace Model.NewModel.Assertions;

public partial class IsomorphicAssertion : ValueAssertion
{
    protected IsomorphicAssertion(INode node, IGraph graph) : base(node, graph) { }

    public Expression IsomorphicWith => this.Singular(Vocabulary.IsomorphicWith, Expression.Choose);

    public static IsomorphicAssertion Wrap(INode node, IGraph graph) => new(node, graph);

    public static IsomorphicAssertion? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
