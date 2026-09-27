namespace Model.NewModel.Extractors;

public partial class HeaderExtractor
{
    protected override async Task<string> Extract(Context context, HttpResponseMessage response)
    {
        // TODO: <1?
        // TODO: >1?
        var value = response.Headers.GetValues(HeaderName).Single();

        context.Log<HeaderExtractor>("Extract: [{0}] = [{1}]", HeaderName, value);

        return value;
    }
}
