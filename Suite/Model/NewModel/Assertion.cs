using Model.NewModel.Assertions;

namespace Model.NewModel;

public abstract class Assertion : GraphWrapperNode
{
    protected Assertion(INode node, IGraph graph) : base(node, graph) { }

    public string Param => this.Singular(Vocabulary.Param, ValueMappings.As<string>);
    public string Value => this.Singular(Vocabulary.Value, ValueMappings.As<string>);

    public abstract Result Execute(Context context);

    internal static Assertion? Choose(GraphWrapperNode node) => EqualityAssertion.Wrap(node);
}
