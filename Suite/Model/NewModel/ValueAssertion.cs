namespace Model.NewModel;

public abstract class ValueAssertion : Assertion
{
    protected ValueAssertion(INode node, IGraph graph) : base(node, graph) { }

    public Expression That => this.Singular(Vocabulary.That, Expression.Choose);

    public override async Task<Result> Execute(Context context) => await ExecuteInternal(await That.Evaluate(context), context);

    protected internal abstract Task<Result> ExecuteInternal(object input, Context context);
}
