using Model.LWST;

namespace Model.NewModel;

public class TestCase : GraphWrapperNode
{
    protected TestCase(INode node, IGraph graph) : base(node, graph) { }

    public static TestCase Wrap(INode node, IGraph graph) => new(node, graph);

    public static TestCase? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    public Assertion Assertion => this.Singular(Vocabulary.Assertion, Assertion.Choose);
    public string Name => this.Singular(Vocabulary.Name, ValueMappings.As<string>);
    public IList<Step> Steps => this.List(Vocabulary.Steps, Step.Wrap, Step.Wrap);

    public async Task<Result> Execute(Context context, HttpClient client)
    {
        foreach (var step in Steps)
        {
            await step.Execute(context, client);
        }

        return Assertion.Execute(context);
    }
}
