namespace Model.NewModel.Assertions;

public partial class ContainsAssertion : ValueAssertion
{
    protected ContainsAssertion(INode node, IGraph graph) : base(node, graph) { }

    public Expression Value => this.Singular(Vocabulary.Contains, Expression.Choose);

    public static ContainsAssertion Wrap(INode node, IGraph graph) => new(node, graph);

    public static ContainsAssertion? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
