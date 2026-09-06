namespace Model.TestManifest;

public class Entry : GraphWrapperNode
{
    protected Entry(INode node, IGraph graph) : base(node, graph) { }

    public static Entry Wrap(INode node, IGraph graph) => new(node, graph);

    public static Entry Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public string Name => this.Singular(Vocabulary.Name, ValueMappings.As<string>);

    public Status Status => this.Singular(Vocabulary.Status, ValueMappings.EnumFromUri<Status>(Vocabulary.NS));
}
