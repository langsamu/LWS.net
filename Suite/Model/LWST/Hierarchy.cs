namespace Model.LWST;

public class Hierarchy : GraphWrapperNode
{
    protected Hierarchy(INode node, IGraph graph) : base(node, graph) { }

    public static Hierarchy Wrap(INode node, IGraph graph) => new(node, graph);

    public static Hierarchy Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);
}
