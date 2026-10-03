namespace Model.NewModel.Extractors;

public partial class StatusCodeExtractor
{
    protected override async Task<string> Extract(Context context, HttpResponseMessage response)
    {
        var value = ((int)response.StatusCode).ToString();

        context.Log<StatusCodeExtractor>("Extract: value = [{Value}]", value);

        return value;
    }
}
