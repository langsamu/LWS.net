using Json.Path;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Model.NewModel.Extractors;

public partial class JsonPathExtractor
{
    protected override async Task<string> Extract(Context context, HttpResponseMessage response)
    {
        var path = JsonPath.Parse(Path);
        var node = await response.Content.ReadFromJsonAsync<JsonNode>();
        var result = path.Evaluate(node);

        // TODO: <1?
        // TODO: >1?
        var value = result.Matches.Single().Value;

        context.Log<JsonPathExtractor>("Extract: path = [{Path}], value = [{Value}]", Path, value);

        return value.ToString();
    }
}
