namespace Model.NewModel;

public partial class Request
{
    public async Task<HttpResponseMessage> Execute(Context context)
    {
        var u = await Uri.Evaluate(context);
        var re = new HttpRequestMessage(new HttpMethod(Method), u);

        foreach (var header in Headers)
        {
            re.Headers.TryAddWithoutValidation(header.Name, header.Value);
        }

        context.Log<Request>("Execute: [{Method}] [{Uri}]", Method, u);

        return await context.Client.SendAsync(re);
    }
}
