using Microsoft.Extensions.Logging;

namespace Model.NewModel.Extractors;

public class HeaderExtractor : Extractor
{
    protected HeaderExtractor(INode node, IGraph graph) : base(node, graph) { }

    public static HeaderExtractor Wrap(INode node, IGraph graph) => new(node, graph);

    public static HeaderExtractor? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    public string HeaderName => this.Singular(Vocabulary.Header, ValueMappings.As<string>);

    protected override async Task<string> Extract(Context context, HttpResponseMessage response)
    {
        // TODO: <1?
        // TODO: >1?
        var value = response.Headers.GetValues(HeaderName).Single();
        var logger = context.LoggerFactory.CreateLogger<HeaderExtractor>();
        logger.LogInformation("Extract: [{0}] = [{1}]", HeaderName, value);

        return value;
    }
}
