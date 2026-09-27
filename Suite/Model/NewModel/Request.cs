namespace Model.NewModel;

public partial class Request
{
    public async Task<HttpResponseMessage> Execute(Context context)
    {
        var u = await Uri.Evaluate(context);
        var re = new HttpRequestMessage(new HttpMethod(Method), u);

        context.Log<Request>("Execute: [{0}] [{1}]", Method, u);

        return await context.Client.SendAsync(re);
    }
}
