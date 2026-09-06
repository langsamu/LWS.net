using static Server.Filters.Respond404Attribute;

namespace Server.Filters;

internal class Respond404Attribute : TypeFilterAttribute<Filter>
{
    internal class Filter : IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            context.Result = new NotFoundObjectResult(null);
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}
