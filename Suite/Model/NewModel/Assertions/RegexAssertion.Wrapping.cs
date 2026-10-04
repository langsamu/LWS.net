using System.Text.RegularExpressions;

namespace Model.NewModel.Assertions;

public partial class RegexAssertion : Assertion
{
    protected RegexAssertion(INode node, IGraph graph) : base(node, graph) { }

    public Regex Pattern => this.Singular(Vocabulary.Regex, ValueMappings.RegexFromStringLiteral);

    public static RegexAssertion Wrap(INode node, IGraph graph) => new(node, graph);

    public static RegexAssertion? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
