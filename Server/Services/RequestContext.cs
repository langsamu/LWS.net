namespace Server.Services;

public class RequestContext(IHttpContextAccessor httpContextAccessor)
{
    private static readonly object resourceKey = new();
    private static readonly object streamKey = new();

    private HttpContext HttpContext =>
        httpContextAccessor.HttpContext ??
        throw new InvalidOperationException("HTTP context unavailable");

    private object? this[object key]
    {
        get => HttpContext.Items.TryGetValue(key, out var value) ? value : null;
        set => HttpContext.Items[key] = value;
    }

    public ResourceMetadata? Metadata
    {
        get => this[resourceKey] as ResourceMetadata;
        set => this[resourceKey] = value;
    }

    public Stream? Stream
    {
        get => this[streamKey] as Stream;
        set => this[streamKey] = value;
    }
}
