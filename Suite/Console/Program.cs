using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;
using Model.NewModel;
using VDS.RDF;
using VDS.RDF.Writing;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSuite(builder.Configuration);

using var host = builder.Build();

var executor = host.Services.GetRequiredService<Executor>();
var context = host.Services.GetRequiredService<Context>();

(await executor.Execute(context)).SaveToStream(Console.Out, new CompressingTurtleWriter());
