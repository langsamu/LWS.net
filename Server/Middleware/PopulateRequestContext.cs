namespace Server.Middleware;

public class PopulateRequestContext(ILwsStorage storage, RequestContext request) : IMiddleware
{
    async Task IMiddleware.InvokeAsync(HttpContext context, RequestDelegate next)
    {
        request.Metadata = await storage.GetMetadataAsync(context);

        await next(context);
    }
}
