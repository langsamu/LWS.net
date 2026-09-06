using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Model;

public static class SuiteServiceCollectionExtensions
{
    public static IServiceCollection AddSuite(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSuiteOptions(configuration)
            .Services
            .AddHttpClient<Executor>()
            .Services;
}
