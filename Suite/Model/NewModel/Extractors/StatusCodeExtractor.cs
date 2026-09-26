using Microsoft.Extensions.Logging;

namespace Model.NewModel.Extractors;

public class StatusCodeExtractor : Extractor
{
    protected StatusCodeExtractor(INode node, IGraph graph) : base(node, graph) { }

    public static StatusCodeExtractor Wrap(INode node, IGraph graph) => new(node, graph);

    public static StatusCodeExtractor? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    protected override async Task<string> Extract(Context context, HttpResponseMessage response)
    {
        var value = ((int)response.StatusCode).ToString();
        var logger = context.LoggerFactory.CreateLogger<StatusCodeExtractor>();
        logger.LogInformation("Extract: value = [{0}]", value);
        return value;
    }
}
