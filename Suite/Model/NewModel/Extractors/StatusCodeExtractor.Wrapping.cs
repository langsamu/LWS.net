namespace Model.NewModel.Extractors;

public partial class StatusCodeExtractor : Extractor
{
    protected StatusCodeExtractor(INode node, IGraph graph) : base(node, graph) { }

    public static StatusCodeExtractor Wrap(INode node, IGraph graph) => new(node, graph);

    public static StatusCodeExtractor? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
