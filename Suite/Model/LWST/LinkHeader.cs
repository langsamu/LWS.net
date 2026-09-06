namespace Model.LWST;

public class LinkHeader : GraphWrapperNode
{
    protected LinkHeader(INode node, IGraph graph) : base(node, graph) { }

    public static LinkHeader Wrap(INode node, IGraph graph) => new(node, graph);

    public static LinkHeader Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public string Rel => this.Singular(Vocabulary.Rel, ValueMappings.As<string>);

    public string HrefTemplate => this.Singular(Vocabulary.HrefTemplate, ValueMappings.As<string>);
}
