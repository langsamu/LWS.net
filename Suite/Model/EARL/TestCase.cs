namespace Model.EARL;

public class TestCase : TestCriterion
{
    protected TestCase(INode node, IGraph graph) : base(node, graph) { }

    public static new TestCase Wrap(INode node, IGraph graph) => new(node, graph);

    public static new TestCase Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public static TestCase Create(IGraph g)
    {
        var result = Wrap(g.CreateBlankNode(), g);
        result.Types.Add(Vocabulary.TestCase);

        return result;
    }

}
