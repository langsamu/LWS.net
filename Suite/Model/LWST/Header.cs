namespace Model.LWST;

public class Header : GraphWrapperNode
{
    protected Header(INode node, IGraph graph) : base(node, graph) { }

    public static Header Wrap(INode node, IGraph graph) => new(node, graph);

    public static Header Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public string HeaderName => this.Singular(Vocabulary.HeaderName, ValueMappings.As<string>);

    public string HeaderValue => this.Singular(Vocabulary.HeaderValue, ValueMappings.As<string>);
}
