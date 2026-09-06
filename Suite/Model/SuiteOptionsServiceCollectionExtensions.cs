using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Model;

public static class SuiteOptionsServiceCollectionExtensions
{
    public static OptionsBuilder<SuiteOptions> AddSuiteOptions(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddOptions<SuiteOptions>()
            .Bind(configuration.GetSection(SuiteOptions.SectionName))
            .ValidateDataAnnotations();
}
