namespace Model.LWST;

public class AuthenticationChallenge : GraphWrapperNode
{
    protected AuthenticationChallenge(INode node, IGraph graph) : base(node, graph) { }

    public static AuthenticationChallenge Wrap(INode node, IGraph graph) => new(node, graph);

    public static AuthenticationChallenge Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);
}
