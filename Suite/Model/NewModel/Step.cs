namespace Model.NewModel;

public partial class Step
{
    public async Task Execute(Context context, HttpClient client)
    {
        var response = await Request.Execute(context, client);
        foreach (var extractor in Extractors)
        {
            await extractor.Execute(context, response);
        }
    }
}
