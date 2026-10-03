namespace Model.NewModel.Extractors;

public partial class BodyExtractor : Extractor
{
    protected BodyExtractor(INode node, IGraph graph) : base(node, graph) { }

    public static BodyExtractor Wrap(INode node, IGraph graph) => new(node, graph);

    public static BodyExtractor? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
