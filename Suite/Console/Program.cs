using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;
using VDS.RDF;
using VDS.RDF.Writing;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSuite(builder.Configuration);

using var host = builder.Build();
var suite = host.Services.GetRequiredService<Executor>();

(await suite.Execute(Resources.ManifestGraph)).SaveToStream(Console.Out, new CompressingTurtleWriter());
