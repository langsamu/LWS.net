namespace Model.LWST;

public class Entry : TestManifest.Entry
{
    protected Entry(INode node, IGraph graph) : base(node, graph) { }

    public static new Entry Wrap(INode node, IGraph graph) => new(node, graph);

    public static new Entry Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public Uri? Id => (this as IUriNode)?.Uri;

    public Request Request => this.Singular(Vocabulary.Request, Request.Wrap);

    public Response Response => this.Singular(Vocabulary.Response, Response.Wrap);

    public Prereqs Prereqs => this.Singular(Vocabulary.Prereqs, Prereqs.Wrap);

    public ISet<Uri> Traits => this.Objects(Vocabulary.Traits, NodeMappings.From, ValueMappings.As<Uri>);
}
