namespace Server.Constraints;

public class DataResourceConstraint(RequestContext context) : IRouteConstraint
{
    public const string Name = "DataResource";

    public bool Match(HttpContext? _, IRouter? __, string ___, RouteValueDictionary ____, RouteDirection _____) =>
        !context.Metadata!.IsContainer;
}
