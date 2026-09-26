using Model.NewModel.Extractors;

namespace Model.NewModel;

public abstract class Extractor : GraphWrapperNode
{
    protected Extractor(INode node, IGraph graph) : base(node, graph) { }

    public string ParamName => this.Singular(Vocabulary.Param, ValueMappings.As<string>);

    public async Task Execute(Context context, HttpResponseMessage response)
    {
        context.Set(ParamName, await Extract(context, response));
    }
    protected abstract Task<string> Extract(Context context, HttpResponseMessage response);

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

        if (true)
        {
            return StatusCodeExtractor.Wrap(node);
        }

        throw new InvalidOperationException();
    }
}
