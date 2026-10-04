namespace Model.NewModel.Extractors;

public partial class BodyExtractor
{
    protected override async Task<string> Extract(Context context, HttpResponseMessage response)
    {
        var value = await response.Content.ReadAsStringAsync();

        context.Log<BodyExtractor>("Extract: body = [{Value}]", value);

        return value;
    }
}
