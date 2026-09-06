using static Server.Filters.CreateResourceAttribute;

namespace Server.Filters;

internal class CreateResourceAttribute : TypeFilterAttribute<Filter>
{
    internal class Filter(ILwsStorage service, RequestContext context) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext action, ActionExecutionDelegate next)
        {
            var id = action.HttpContext.Request.Headers["Slug"].FirstOrDefault() ?? $"/{Guid.NewGuid()}";

            context.Metadata = await service.PutMetadataAsync(id, action.HttpContext.Request.ContentType!, action.HttpContext.RequestAborted);
            await service.PutContentAsync(id, action.HttpContext.Request.Body, action.HttpContext.RequestAborted);

            await next.Invoke();
        }
    }
}
