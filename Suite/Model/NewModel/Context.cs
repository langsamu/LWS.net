using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Model.NewModel;

// TODO: test context cancellation token
public class Context(ILoggerFactory loggerFactory, IOptions<SuiteOptions> options, HttpClient client)
{
    private readonly Dictionary<string, string> data = new()
    {
        ["baseUri"] = options.Value.BaseUri.AbsoluteUri,
    };

    public HttpClient Client { get; } = client;

    public string Get(string name)
    {
        Log<Context>("Get: [{Name}] = [{Value}]", name, data[name]);

        return data[name];
    }

    public void Set(string name, string value) => data[name] = value;

    internal void Log<T>(string? message, params object?[] args) => loggerFactory.CreateLogger<T>().LogInformation(message, args);
}
