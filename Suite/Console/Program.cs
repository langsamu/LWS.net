using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;
using Model.NewModel;
using VDS.RDF;
using VDS.RDF.Writing;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSuite(builder.Configuration);

using var host = builder.Build();

var context = host.Services.GetRequiredService<Context>();

var earlReport = await Executor.Execute(context);
earlReport.SaveToStream(Console.Out, new CompressingTurtleWriter());
