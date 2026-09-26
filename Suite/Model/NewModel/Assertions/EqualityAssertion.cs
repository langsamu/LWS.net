namespace Model.NewModel.Assertions;

public class EqualityAssertion : Assertion
{
    protected EqualityAssertion(INode node, IGraph graph) : base(node, graph) { }

    public static EqualityAssertion Wrap(INode node, IGraph graph) => new(node, graph);

    public static EqualityAssertion? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    public override Result Execute(Context context)
    {
        var actual = context.Get(Param)?.ToString();
        if (actual is null)
        {
            return new Result { Outcome = "Inconclusive", Info = $"Parameter '{Param}' not found in context." };
        }
        return actual == Value.ToString()
            ? new Result { Outcome = "Pass", Info = $"Parameter '{Param}' equals '{Value}'." }
            : new Result { Outcome = "Fail", Info = $"Parameter '{Param}' expected '{Value}', but got '{actual}'." };
    }
}
