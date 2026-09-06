using static Server.Filters.SetActionResultFromContextAttribute;

namespace Server.Filters;

internal class SetActionResultFromContextAttribute : TypeFilterAttribute<Filter>
{
    internal class Filter(RequestContext context) : IActionFilter
    {
        void IActionFilter.OnActionExecuted(ActionExecutedContext action)
        {
            if (context.Stream is null || context.Metadata is null)
            {
                throw new InvalidOperationException("Stream or Metadata is missing from context");
            }

            // FileStreamResult supports range requests & conditional requests.
            // TODO: Resource content always loaded though potentially not sent. Compliant but suboptimal.
            action.Result = new FileStreamResult(context.Stream, context.Metadata.ContentType)
            {
                EntityTag = context.Metadata.ETag,
                LastModified = context.Metadata.LastModified,
                EnableRangeProcessing = true
            };
        }

        void IActionFilter.OnActionExecuting(ActionExecutingContext context) { }
    }
}
