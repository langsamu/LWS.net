using Microsoft.Extensions.Logging;

namespace Model.NewModel.Extractors;

public partial class StatusCodeExtractor
{
    protected override async Task<string> Extract(Context context, HttpResponseMessage response)
    {
        var value = ((int)response.StatusCode).ToString();
        var logger = context.LoggerFactory.CreateLogger<StatusCodeExtractor>();
        logger.LogInformation("Extract: value = [{0}]", value);
        return value;
    }
}
