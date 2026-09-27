using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.NewModel;
using OpenTelemetry.Trace;

namespace Model;

public static class SuiteServiceCollectionExtensions
{
    public static IServiceCollection AddSuite(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSuiteOptions(configuration)
            .Services
            .AddHttpClient<Context>()
            .Services
            .AddOpenTelemetry()
            .WithTracing(builder => builder.AddHttpClientInstrumentation().AddOtlpExporter())
            .Services;
}
