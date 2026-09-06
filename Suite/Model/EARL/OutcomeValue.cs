namespace Model.EARL;

public class OutcomeValue : GraphWrapperNode
{
    protected OutcomeValue(INode node, IGraph graph) : base(node, graph) { }

    public static OutcomeValue Wrap(INode node, IGraph graph) => new(node, graph);

    public static OutcomeValue Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public static OutcomeValue Create(IGraph g) => Wrap(g.CreateBlankNode(), g);

    public string Title
    {
        get => this.Singular(Vocabulary.Title, ValueMappings.As<string>);

        set => this.Overwrite(Vocabulary.Title, value);
    }

    public string Description => this.Singular(Vocabulary.Description, ValueMappings.As<string>);
}
