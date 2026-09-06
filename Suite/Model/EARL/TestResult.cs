namespace Model.EARL;

public class TestResult : GraphWrapperNode
{
    protected TestResult(INode node, IGraph graph) : base(node, graph) { }

    public static TestResult Wrap(INode node, IGraph graph) => new(node, graph);

    public static TestResult Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public static TestResult Create(IGraph g) => Wrap(g.CreateBlankNode(), g);

    public DateTimeOffset Date => this.Singular(Vocabulary.Date, ValueMappings.As<DateTimeOffset>);

    public INode Outcome
    {
        get => this.Singular(Vocabulary.Outcome, ValueMappings.AsIs);

        set => this.Overwrite(Vocabulary.Outcome, value);
    }

    public string Info
    {
        get => this.Singular(Vocabulary.Info, ValueMappings.As<string>);

        set => this.Overwrite(Vocabulary.Info, value);
    }
}
