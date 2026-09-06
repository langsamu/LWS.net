using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Model;
using VDS.RDF;
using VDS.RDF.Writing;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSuiteOptions(builder.Configuration);

using var host = builder.Build();
var options = host.Services.GetRequiredService<IOptions<SuiteOptions>>().Value;

(await Executor.Execute(Resources.ManifestGraph, options.BaseUri)).SaveToStream(Console.Out, new CompressingTurtleWriter());
