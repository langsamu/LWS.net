using Microsoft.Extensions.Logging;

namespace Model.NewModel;

public class Request : GraphWrapperNode
{
    protected Request(INode node, IGraph graph) : base(node, graph) { }

    public static Request Wrap(INode node, IGraph graph) => new(node, graph);

    public static Request? Wrap(GraphWrapperNode node) => node switch { null => default, _ => Wrap(node, node.Graph) };

    public Expression Uri => this.Singular(Vocabulary.Uri, Expression.Choose);
    public string Method => this.Singular(Vocabulary.Method, ValueMappings.As<string>);

    public async Task<HttpResponseMessage> Execute(Context context, HttpClient client)
    {
        var u = await Uri.Evaluate(context);
        var re = new HttpRequestMessage(new HttpMethod(Method), u);

        var logger = context.LoggerFactory.CreateLogger<Request>();
        logger.LogInformation("Execute: [{0}] [{1}]", Method, u);
        return await client.SendAsync(re);
    }
}
