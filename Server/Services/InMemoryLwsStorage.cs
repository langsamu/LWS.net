using System.Collections.Concurrent;

namespace Server.Services;

public class InMemoryLwsStorage : ILwsStorage
{
    private readonly ConcurrentDictionary<PathString, ResourceMetadata> storage = [];
    private readonly ConcurrentDictionary<PathString, byte[]> contents = [];

    public Task<ResourceMetadata?> GetMetadataAsync(HttpContext context)
    {
        storage.TryGetValue(context.Request.Path, out var resource);
        return Task.FromResult(resource);
    }
    public Task<Stream?> GetContentAsync(HttpContext context)
    {
        contents.TryGetValue(context.Request.Path, out var content);
        return Task.FromResult(content is not null ? new MemoryStream(content) : null as Stream);
    }

    public Task<ResourceMetadata> PutMetadataAsync(string id, string contentType, CancellationToken ct)
    {
        var resource = new ResourceMetadata
        {
            Id = id,
            ContentType = contentType,
            LastModified = DateTimeOffset.UtcNow
        };
        storage.TryAdd(id, resource);

        return Task.FromResult(resource);
    }
    public async Task PutContentAsync(string id, Stream body, CancellationToken ct)
    {
        var stream = new MemoryStream();
        await body.CopyToAsync(stream, ct);
        var bytes = stream.ToArray();


        contents.TryAdd(id, bytes);
    }
}
