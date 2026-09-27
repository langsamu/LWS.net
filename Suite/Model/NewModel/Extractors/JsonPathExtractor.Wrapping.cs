namespace Model.NewModel.Extractors;

public partial class JsonPathExtractor : Extractor
{
    protected JsonPathExtractor(INode node, IGraph graph) : base(node, graph) { }

    public string Path => this.Singular(Vocabulary.Path, ValueMappings.As<string>);

    public static JsonPathExtractor Wrap(INode node, IGraph graph) => new(node, graph);

    public static JsonPathExtractor? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
