namespace Model.NewModel;

public partial class TestCase : GraphWrapperNode
{
    protected TestCase(INode node, IGraph graph) : base(node, graph) { }

    public Assertion Assertion => this.Singular(Vocabulary.Assertion, Assertion.Choose);

    public string Name => this.Singular(Vocabulary.Name, ValueMappings.As<string>);

    public IList<Step> Steps => this.List(Vocabulary.Steps, Step.Wrap, Step.Wrap);

    public static TestCase Wrap(INode node, IGraph graph) => new(node, graph);

    public static TestCase? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
