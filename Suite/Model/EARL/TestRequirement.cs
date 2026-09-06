namespace Model.EARL;

public class TestRequirement : TestCriterion
{
    protected TestRequirement(INode node, IGraph graph) : base(node, graph) { }

    public static new TestRequirement Wrap(INode node, IGraph graph) => new(node, graph);

    public static new TestRequirement Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public static TestRequirement Create(IGraph g)
    {
        var result = Wrap(g.CreateBlankNode(), g);
        result.Types.Add(Vocabulary.TestRequirement);

        return result;
    }

    public static TestRequirement Create(Uri u, IGraph g)
    {
        var result = Wrap(g.CreateUriNode(u), g);
        result.Types.Add(Vocabulary.TestRequirement);

        return result;
    }
}
