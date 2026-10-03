using System.Diagnostics;

namespace Model.NewModel;

public partial class Step
{
    public async Task Execute(Context context)
    {
        using var activity = Executor.ActivitySource.StartActivity(Name, ActivityKind.Internal, Activity.Current?.Context ?? default);

        try
        {
            var response = await Request.Execute(context);

            foreach (var extractor in Extractors)
            {
                await extractor.Execute(context, response);
            }
        }
        catch (Exception exception)
        {
            activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
            throw;
        }
    }
}
