namespace Model.NewModel.Assertions;

public partial class GreaterThanAssertion : ValueAssertion
{
    protected GreaterThanAssertion(INode node, IGraph graph) : base(node, graph) { }

    public Expression Value => this.Singular(Vocabulary.GreaterThan, Expression.Choose);

    public static GreaterThanAssertion Wrap(INode node, IGraph graph) => new(node, graph);

    public static GreaterThanAssertion? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
