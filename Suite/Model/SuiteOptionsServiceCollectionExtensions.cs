using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Model;

public static class SuiteOptionsServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="SuiteOptions"/>, bound from the <see cref="SuiteOptions.SectionName"/> configuration
    /// section and validated the first time it is resolved.
    /// </summary>
    public static OptionsBuilder<SuiteOptions> AddSuiteOptions(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddOptions<SuiteOptions>()
            .Bind(configuration.GetSection(SuiteOptions.SectionName))
            .ValidateDataAnnotations();
}
