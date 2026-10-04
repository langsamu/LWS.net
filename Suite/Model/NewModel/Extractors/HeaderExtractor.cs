namespace Model.NewModel.Extractors;

public partial class HeaderExtractor
{
    protected override async Task<string> Extract(Context context, HttpResponseMessage response)
    {
        var values = response.Headers.TryGetValues(HeaderName, out var responseValues)
            ? responseValues
            : response.Content.Headers.GetValues(HeaderName);

        // TODO: <1?
        // TODO: >1?
        var value = values.Single();

        context.Log<HeaderExtractor>("Extract: [{HeaderName}] = [{Value}]", HeaderName, value);

        return value;
    }
}
