namespace Server.Services;

public interface ILwsStorage
{
    Task<ResourceMetadata?> GetMetadataAsync(HttpContext context);

    Task<Stream?> GetContentAsync(HttpContext context);

    Task<ResourceMetadata> PutMetadataAsync(string id, string contentType, CancellationToken ct);

    Task PutContentAsync(string id, Stream body, CancellationToken ct);
}
