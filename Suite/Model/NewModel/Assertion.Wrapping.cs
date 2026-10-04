using Model.NewModel.Assertions;

namespace Model.NewModel;

public abstract partial class Assertion : GraphWrapperNode
{
    protected Assertion(INode node, IGraph graph) : base(node, graph) { }

    public string Param => this.Singular(Vocabulary.Param, ValueMappings.As<string>);

    internal static Assertion? Choose(GraphWrapperNode node)
    {
        if (node.Graph.GetTriplesWithSubjectPredicate(node, Vocabulary.Value).Any())
        {
            return EqualityAssertion.Wrap(node);
        }

        if (node.Graph.GetTriplesWithSubjectPredicate(node, Vocabulary.Regex).Any())
        {
            return RegexAssertion.Wrap(node);
        }

        throw new InvalidOperationException();
    }
}
