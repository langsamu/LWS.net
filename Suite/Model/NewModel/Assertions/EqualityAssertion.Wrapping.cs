namespace Model.NewModel.Assertions;

public partial class EqualityAssertion : ValueAssertion
{
    protected EqualityAssertion(INode node, IGraph graph) : base(node, graph) { }

    public Expression EqualsValue => this.Singular(Vocabulary.EqualsValue, Expression.Choose);

    public static EqualityAssertion Wrap(INode node, IGraph graph) => new(node, graph);

    public static EqualityAssertion? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
