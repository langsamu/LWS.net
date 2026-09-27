namespace Model.NewModel;

public partial class Step : GraphWrapperNode
{
    protected Step(INode node, IGraph graph) : base(node, graph) { }

    public Request Request => this.Singular(Vocabulary.Request, Request.Wrap);

    public IList<Extractor> Extractors => this.List(Vocabulary.Extractors, (value, graph) => null!, Extractor.Choose);

    public static Step Wrap(INode node, IGraph graph) => new(node, graph);

    public static Step? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
