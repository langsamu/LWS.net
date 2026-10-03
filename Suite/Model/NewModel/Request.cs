using System.Net.Http.Headers;

namespace Model.NewModel;

public partial class Request
{
    public async Task<HttpResponseMessage> Execute(Context context)
    {
        var u = await Uri.Evaluate(context);
        var re = new HttpRequestMessage(new HttpMethod(Method), u);

        if (Body is { } body)
        {
            re.Content = new StringContent(body, null as MediaTypeHeaderValue);
        }

        foreach (var header in Headers)
        {
            if (!re.Headers.TryAddWithoutValidation(header.Name, header.Value))
            {
                re.Content?.Headers.TryAddWithoutValidation(header.Name, header.Value);
            }
        }

        context.Log<Request>("Execute: [{Method}] [{Uri}]", Method, u);

        return await context.Client.SendAsync(re);
    }
}
