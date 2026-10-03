namespace Model.NewModel;

public partial class Request : GraphWrapperNode
{
    protected Request(INode node, IGraph graph) : base(node, graph) { }

    public Expression Uri => this.Singular(Vocabulary.Uri, Expression.Choose);

    public IList<RequestHeader> Headers => this.List(Vocabulary.Headers, RequestHeader.Wrap, RequestHeader.Wrap);

    public string? Body => this.Singular(Vocabulary.Body, ValueMappings.As<string>);

    public string Method => this.Singular(Vocabulary.Method, ValueMappings.As<string>);

    public static Request Wrap(INode node, IGraph graph) => new(node, graph);

    public static Request? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
