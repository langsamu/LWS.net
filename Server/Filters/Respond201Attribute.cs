using static Server.Filters.Respond201Attribute;

namespace Server.Filters;

internal class Respond201Attribute : TypeFilterAttribute<Filter>
{
    internal class Filter(RequestContext someContext) : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.Result = new CreatedResult(someContext.Metadata!.Id, null);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
