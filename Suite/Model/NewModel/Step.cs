namespace Model.NewModel;

public partial class Step
{
    public async Task Execute(Context context)
    {
        var response = await Request.Execute(context);

        foreach (var extractor in Extractors)
        {
            await extractor.Execute(context, response);
        }
    }
}
