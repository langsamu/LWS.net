using Model.NewModel.Expressions;

namespace Model.NewModel;

public abstract class Expression : GraphWrapperNode
{
    protected Expression(INode node, IGraph graph) : base(node, graph) { }

    public abstract Task<string> Evaluate(Context context);

    internal static Expression? Choose(GraphWrapperNode node)
    {
        if (node.Graph.GetTriplesWithSubjectPredicate(node, Vocabulary.Value).Any())
        {
            return ConstantExpression.Wrap(node);
        }

        if (node.Graph.GetTriplesWithSubjectPredicate(node, Vocabulary.Param).Any())
        {
            return ParamRefExpression.Wrap(node);
        }

        if (node.Graph.GetTriplesWithSubjectPredicate(node, Vocabulary.Base).Any())
        {
            return ResolveExpression.Wrap(node);
        }


        throw new InvalidOperationException();
    }
}
