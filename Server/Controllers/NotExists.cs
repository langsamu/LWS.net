namespace Server.Controllers;

[Route("{*_:NotExists}")] // Any path that signifies a non-existent resource
[ApiController]
public class NotExists
{
    [HttpPost]
    [CreateResource]
    [Respond201]
    public void Post() { }

    [Respond404]
    public void AnyOtherMethod() { }
}
