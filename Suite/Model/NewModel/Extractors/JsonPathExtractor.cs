using Json.Path;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Model.NewModel.Extractors;

public class JsonPathExtractor : Extractor
{
    protected JsonPathExtractor(INode node, IGraph graph) : base(node, graph) { }

    public static JsonPathExtractor Wrap(INode node, IGraph graph) => new(node, graph);

    public static JsonPathExtractor? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    public string Path => this.Singular(Vocabulary.Path, ValueMappings.As<string>);

    protected override async Task<string> Extract(Context context,HttpResponseMessage response)
    {
        var path = JsonPath.Parse(Path);
        var node = await response.Content.ReadFromJsonAsync<JsonNode>();
        var result = path.Evaluate(node);

        // TODO: <1?
        // TODO: >1?
        var value = result.Matches.Single().Value;
        var logger = context.LoggerFactory.CreateLogger<JsonPathExtractor>();
        logger.LogInformation("Extract: path = [{0}], value = [{1}]", Path, value);
        return value.ToString();
    }
}
