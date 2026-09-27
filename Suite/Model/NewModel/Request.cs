using Microsoft.Extensions.Logging;

namespace Model.NewModel;

public partial class Request
{
    public async Task<HttpResponseMessage> Execute(Context context, HttpClient client)
    {
        var u = await Uri.Evaluate(context);
        var re = new HttpRequestMessage(new HttpMethod(Method), u);

        var logger = context.LoggerFactory.CreateLogger<Request>();
        logger.LogInformation("Execute: [{0}] [{1}]", Method, u);
        return await client.SendAsync(re);
    }
}
