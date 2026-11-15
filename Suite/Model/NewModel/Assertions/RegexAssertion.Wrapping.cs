namespace Model.NewModel.Assertions;

public partial class RegexAssertion : ValueAssertion
{
    protected RegexAssertion(INode node, IGraph graph) : base(node, graph) { }

    public Expression Matches => this.Singular(Vocabulary.Matches, Expression.Choose);

    public static RegexAssertion Wrap(INode node, IGraph graph) => new(node, graph);

    public static RegexAssertion? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
