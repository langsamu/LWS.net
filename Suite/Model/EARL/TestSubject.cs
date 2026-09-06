namespace Model.EARL;

public class TestSubject : GraphWrapperNode
{
    protected TestSubject(INode node, IGraph graph) : base(node, graph) { }

    public static TestSubject Wrap(INode node, IGraph graph) => new(node, graph);

    public static TestSubject Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public string Title => this.Singular(Vocabulary.Title, ValueMappings.As<string>);
}
