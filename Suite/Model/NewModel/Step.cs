namespace Model.NewModel;

public class Step : GraphWrapperNode
{
    protected Step(INode node, IGraph graph) : base(node, graph) { }

    public static Step Wrap(INode node, IGraph graph) => new(node, graph);

    public static Step? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    public Request Request => this.Singular(Vocabulary.Request, Request.Wrap);
    public IList<Extractor> Extractors => this.List(Vocabulary.Extractors, (value, graph)=>null!, Extractor.Choose);

    public async Task Execute(Context context, HttpClient client)
    {
        var response = await Request.Execute(context, client);
        foreach (var extractor in Extractors)
        {
            await extractor.Execute(context, response);
        }
    }
}
