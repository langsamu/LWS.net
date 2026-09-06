namespace Model.LWST;

public class Prereqs : GraphWrapperNode
{
    protected Prereqs(INode node, IGraph graph) : base(node, graph) { }

    public static Prereqs Wrap(INode node, IGraph graph) => new(node, graph);

    public static Prereqs Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public string Authentication => this.Singular(Vocabulary.Authentication, ValueMappings.As<string>);

    public IList<Hierarchy> Hierarchy => this.List(Vocabulary.Hierarchy, LWST.Hierarchy.Wrap, LWST.Hierarchy.Wrap);
}
