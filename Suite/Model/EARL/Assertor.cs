namespace Model.EARL;

public class Assertor : GraphWrapperNode
{
    protected Assertor(INode node, IGraph graph) : base(node, graph) { }

    public static Assertor Wrap(INode node, IGraph graph) => new(node, graph);

    public static Assertor Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public static Assertor Create(IGraph g) => Wrap(g.CreateBlankNode(), g);

    public string Title
    {
        get => this.Singular(Vocabulary.Title, ValueMappings.As<string>);

        set => this.Overwrite(Vocabulary.Title, value);
    }
}
