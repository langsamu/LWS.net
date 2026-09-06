namespace Model.EARL;

public class TestCriterion : Typed
{
    protected TestCriterion(INode node, IGraph graph) : base(node, graph) { }

    public static new TestCriterion Wrap(INode node, IGraph graph) => new(node, graph);

    public static new TestCriterion Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public string Title
    {
        get => this.Singular(Vocabulary.Title, ValueMappings.As<string>);

        set => this.Overwrite(Vocabulary.Title, value);
    }

    public INode IsPartOf
    {
        get => this.Singular(Vocabulary.IsPartOf, ValueMappings.AsIs);

        set => this.Overwrite(Vocabulary.IsPartOf, value);
    }
}
