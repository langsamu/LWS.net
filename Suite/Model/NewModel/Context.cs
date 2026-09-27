using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Model.NewModel;

// TODO: test context cancellation token
public class Context(ILoggerFactory loggerFactory, IOptions<SuiteOptions> options)
{
    private readonly Dictionary<string, string> data = new()
    {
        ["baseUri"] = options.Value.BaseUri.AbsoluteUri,
    };

    public ILoggerFactory LoggerFactory { get; } = loggerFactory;

    public string Get(string name)
    {
        LoggerFactory.CreateLogger<Context>().LogInformation("Get: [{0}] = [{1}]", name, data[name]);
        return data[name];
    }

    public void Set(string name, string value) => data[name] = value;
}
