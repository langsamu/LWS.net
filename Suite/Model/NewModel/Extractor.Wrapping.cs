using Model.NewModel.Extractors;

namespace Model.NewModel;

public abstract partial class Extractor : GraphWrapperNode
{
    protected Extractor(INode node, IGraph graph) : base(node, graph) { }

    public string ParamName => this.Singular(Vocabulary.Param, ValueMappings.As<string>);

    internal static Extractor? Choose(GraphWrapperNode node)
    {
        if (node.Graph.GetTriplesWithSubjectPredicate(node, Vocabulary.Header).Any())
        {
            return HeaderExtractor.Wrap(node);
        }

        if (node.Graph.GetTriplesWithSubjectPredicate(node, Vocabulary.Path).Any())
        {
            return JsonPathExtractor.Wrap(node);
        }

        // TODO: Match on something
        if (true)
        {
            return StatusCodeExtractor.Wrap(node);
        }

        throw new InvalidOperationException();
    }
}
