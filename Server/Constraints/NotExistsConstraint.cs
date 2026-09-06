namespace Server.Constraints;

public class NotExistsConstraint(RequestContext context) : IRouteConstraint
{
    public const string Name = "NotExists";

    public bool Match(HttpContext? _, IRouter? __, string ___, RouteValueDictionary ____, RouteDirection _____) =>
        context.Metadata is null;
}
