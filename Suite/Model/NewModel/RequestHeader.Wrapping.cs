namespace Model.NewModel;

public class RequestHeader : GraphWrapperNode
{
    protected RequestHeader(INode node, IGraph graph) : base(node, graph) { }

    public string Name => this.Singular(Vocabulary.Header, ValueMappings.As<string>);

    public string Value => this.Singular(Vocabulary.Value, ValueMappings.As<string>);

    public static RequestHeader Wrap(INode node, IGraph graph) => new(node, graph);

    public static RequestHeader? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
