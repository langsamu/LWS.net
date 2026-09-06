namespace Server.Controllers;

[Route("{*_:Exists:DataResource}")] // Any path signifying an existing data resource
[ApiController]
public class DataResource
{
    [HttpGet]
    [PopulateContents]
    [SetActionResultFromContext]
    public void Get() { }
}
