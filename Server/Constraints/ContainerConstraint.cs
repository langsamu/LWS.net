namespace Server.Constraints;

public class ContainerConstraint(RequestContext context) : IRouteConstraint
{
    public const string Name = "Container";

    public bool Match(HttpContext? _, IRouter? __, string ___, RouteValueDictionary ____, RouteDirection _____) =>
        context.Metadata!.IsContainer;
}
