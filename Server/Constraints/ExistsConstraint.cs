namespace Server.Constraints;

public class ExistsConstraint(RequestContext context) : IRouteConstraint
{
    public const string Name = "Exists";

    public bool Match(HttpContext? _, IRouter? __, string ___, RouteValueDictionary ____, RouteDirection _____) =>
        context.Metadata is not null;
}
