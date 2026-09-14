namespace Model.LWST;

public class Response : GraphWrapperNode
{
    protected Response(INode node, IGraph graph) : base(node, graph) { }

    public static Response Wrap(INode node, IGraph graph) => new(node, graph);

    public static Response Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public long? StatusCode => this.Singular(Vocabulary.StatusCode, ValueMappings.As<long>);

    public string? ContentType => this.Singular(Vocabulary.ContentType, ValueMappings.As<string>);

    public IList<LinkHeader> LinkHeaders => this.List(Vocabulary.LinkHeaders, LinkHeader.Wrap, LinkHeader.Wrap);

    public string? Body
    {
        get => this.Singular(Vocabulary.Body, ValueMappings.As<string>);

        set => this.Overwrite(Vocabulary.Body, value);
    }

    public Uri BodyUrl => this.Singular(Vocabulary.BodyURL, ValueMappings.As<Uri>);

    public AuthenticationChallenge AuthenticationChallenge => this.Singular(Vocabulary.AuthenticationChallenge, AuthenticationChallenge.Wrap);

    public IList<Header> OtherHeaders => this.List(Vocabulary.OtherHeaders, Header.Wrap, Header.Wrap);
}
