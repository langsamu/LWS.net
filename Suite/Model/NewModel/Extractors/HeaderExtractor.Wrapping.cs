namespace Model.NewModel.Extractors;

public partial class HeaderExtractor : Extractor
{
    protected HeaderExtractor(INode node, IGraph graph) : base(node, graph) { }

    public string HeaderName => this.Singular(Vocabulary.Header, ValueMappings.As<string>);

    public static HeaderExtractor Wrap(INode node, IGraph graph) => new(node, graph);

    public static HeaderExtractor? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
