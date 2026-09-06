namespace Model.EARL;

public class Assertion : GraphWrapperNode
{
    protected Assertion(INode node, IGraph graph) : base(node, graph) { }

    public static Assertion Wrap(INode node, IGraph graph) => new(node, graph);

    public static Assertion Wrap(GraphWrapperNode node) => Wrap(node, node.Graph);

    public static Assertion Create(IGraph g) => Wrap(g.CreateBlankNode(), g);

    public Assertor AssertedBy
    {
        get => this.Singular(Vocabulary.AssertedBy, Assertor.Wrap);

        set => this.Overwrite(Vocabulary.AssertedBy, value);
    }

    public TestSubject Subject => this.Singular(Vocabulary.Subject, TestSubject.Wrap);

    public TestCriterion Test
    {
        get => this.Singular(Vocabulary.Test, TestCriterion.Wrap);

        set => this.Overwrite(Vocabulary.Test, value);
    }

    public string Title
    {
        get => this.Singular(Vocabulary.Title, ValueMappings.As<string>);

        set => this.Overwrite(Vocabulary.Title, value);
    }

    public TestResult Result
    {
        get => this.Singular(Vocabulary.Result, TestResult.Wrap);

        set => this.Overwrite(Vocabulary.Result, value);
    }
}
