namespace Model;

public class Typed : GraphWrapperNode
{
    protected Typed(INode node, IGraph graph) : base(node, graph) { }

    internal static Typed Wrap(INode node, IGraph graph) => new(node, graph);

    internal static Typed Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    //internal ISet<Uri> Types => this.Objects(Vocabulary.RdfType, NodeMappings.From, ValueMappings.As<Uri>);
    internal ISet<INode> Types => this.Objects(Vocabulary.RdfType, NodeMappings.From, ValueMappings.As<INode>);
}
