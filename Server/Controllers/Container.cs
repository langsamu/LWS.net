namespace Server.Controllers;

[Route("{*_:Exists:Container}")] // Any path signifying an existing container
[ApiController]
public class Container
{
    [HttpGet]
    public void Get() { }
}
