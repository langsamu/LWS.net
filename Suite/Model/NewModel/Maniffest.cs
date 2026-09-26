namespace Model.NewModel;

public class Manifest : GraphWrapperNode
{
    protected Manifest(INode node, IGraph graph) : base(node, graph) { }

    public static Manifest Wrap(INode node, IGraph graph) => new(node, graph);

    public static Manifest? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    public IList<TestCase> Tests => this.List(Vocabulary.Tests, TestCase.Wrap, TestCase.Wrap);
}
