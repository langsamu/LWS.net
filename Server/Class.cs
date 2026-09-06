using Microsoft.AspNetCore.Mvc.Routing;

namespace Server;

public class Class : HttpMethodAttribute
{
    private static readonly IEnumerable<string> supportedMethods = ["QUERY"];

    public Class() : base(supportedMethods)
    {

    }

    public Class(string? template) : base(supportedMethods, template)
    {

    }
}
