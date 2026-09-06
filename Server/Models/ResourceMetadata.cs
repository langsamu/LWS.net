using Microsoft.Net.Http.Headers;

namespace Server.Models;

public class ResourceMetadata
{
    public PathString Id { get; set; }

    public bool IsContainer { get; set; }

    public string ContentType { get; set; }

    public EntityTagHeaderValue ETag { get; set; }

    public DateTimeOffset LastModified { get; set; }
}
