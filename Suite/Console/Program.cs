using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Model;
using Model.NewModel;
using VDS.RDF;
using VDS.RDF.Writing;

var context = new Context(new LoggerFactory());
context.Set("baseUri", "http://localhost:8080"); // TODO: make this configurable

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSuite(builder.Configuration);

using var host = builder.Build();
var suite = host.Services.GetRequiredService<Executor>();

(await suite.Execute(context)).SaveToStream(Console.Out, new CompressingTurtleWriter());
