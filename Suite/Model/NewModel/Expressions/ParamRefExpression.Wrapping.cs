namespace Model.NewModel.Expressions;

public partial class ParamRefExpression : Expression
{
    protected ParamRefExpression(INode node, IGraph graph) : base(node, graph) { }

    public string Param => this.Singular(Vocabulary.Param, ValueMappings.As<string>);

    public static ParamRefExpression Wrap(INode node, IGraph graph) => new(node, graph);

    public static ParamRefExpression? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };
}
