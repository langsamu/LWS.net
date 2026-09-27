namespace Model.NewModel.Expressions;

public partial class ConstantExpression : Expression
{
    protected ConstantExpression(INode node, IGraph graph) : base(node, graph) { }

    public string Value => this.Singular(Vocabulary.Value, ValueMappings.As<string>);

    public static ConstantExpression Wrap(INode node, IGraph graph) => new(node, graph);

    public static ConstantExpression? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
