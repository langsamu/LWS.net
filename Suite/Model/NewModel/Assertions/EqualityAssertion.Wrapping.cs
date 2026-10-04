namespace Model.NewModel.Assertions;

public partial class EqualityAssertion : Assertion
{
    protected EqualityAssertion(INode node, IGraph graph) : base(node, graph) { }

    public string Value => this.Singular(Vocabulary.Value, ValueMappings.As<string>);

    public static EqualityAssertion Wrap(INode node, IGraph graph) => new(node, graph);

    public static EqualityAssertion? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
