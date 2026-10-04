using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Model.NewModel;
using OpenTelemetry;
using OpenTelemetry.Trace;

namespace Model;

public static class SuiteServiceCollectionExtensions
{
    public static IServiceCollection AddSuite(this IServiceCollection services, IConfiguration configuration) =>
        services
            .AddSuiteOptions(configuration)
            .Services
            .AddHttpClient<Executor>()
            .Services
            .AddOpenTelemetry()
            .WithTracing(builder => builder.AddSource(Executor.ActivitySource.Name).AddHttpClientInstrumentation())
            .WithLogging(configureBuilder: null, configureOptions: options => options.IncludeFormattedMessage = true)
            .UseOtlpExporter()
            .Services;
}
