namespace Model.LWST;

public class Request : GraphWrapperNode
{
    protected Request(INode node, IGraph graph) : base(node, graph) { }

    public static Request Wrap(INode node, IGraph graph) => new(node, graph);

    public static Request Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public Uri Url => this.Singular(Vocabulary.Url, ValueMappings.As<Uri>);

    public string Method => this.Singular(Vocabulary.Method, ValueMappings.As<string>);

    public IList<Header> OtherHeaders => this.List(Vocabulary.OtherHeaders, Header.Wrap, Header.Wrap);

    public string? ContentType => this.Singular(Vocabulary.ContentType, ValueMappings.As<string>);

    public string? Body
    {
        get => this.Singular(Vocabulary.Body, ValueMappings.As<string>);

        set => this.Overwrite(Vocabulary.Body, value);
    }

    public Uri? BodyUrl => this.Singular(Vocabulary.BodyURL, ValueMappings.As<Uri>);

    public string? Slug => this.Singular(Vocabulary.Slug, ValueMappings.As<string>);
}
