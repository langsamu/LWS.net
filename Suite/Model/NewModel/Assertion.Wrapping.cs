using Model.NewModel.Assertions;

namespace Model.NewModel;

public abstract partial class Assertion : GraphWrapperNode
{
    protected Assertion(INode node, IGraph graph) : base(node, graph) { }

    public string Param => this.Singular(Vocabulary.Param, ValueMappings.As<string>);

    public string Value => this.Singular(Vocabulary.Value, ValueMappings.As<string>);

    // TODO: Diversify
    internal static Assertion? Choose(GraphWrapperNode node) => EqualityAssertion.Wrap(node);
}
