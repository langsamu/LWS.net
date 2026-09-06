namespace Model.TestManifest;

public class Manifest(INode node, IGraph graph) : GraphWrapperNode(node, graph)
{
    internal static Manifest Wrap(INode node, IGraph graph) => new(node, graph);

    internal static Manifest Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public Uri? Id => (this as IUriNode)?.Uri;

    public string Label => this.Singular(Vocabulary.Label, ValueMappings.As<string>);

    public IList<LWST.Entry> Entries => this.List(Vocabulary.Entries, LWST.Entry.Wrap, LWST.Entry.Wrap);

    public IList<Uri> Include => this.List(Vocabulary.Include, NodeMappings.From, ValueMappings.As<Uri>);
}
