using static Server.Filters.PopulateContentsAttribute;

namespace Server.Filters;

internal class PopulateContentsAttribute : TypeFilterAttribute<Filter>
{
    internal class Filter(ILwsStorage storage, RequestContext context) : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext action, ActionExecutionDelegate next)
        {
            context.Stream = await storage.GetContentAsync(action.HttpContext);

            await next.Invoke();
        }
    }
}
