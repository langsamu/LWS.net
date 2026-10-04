using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Model;
using VDS.RDF;
using VDS.RDF.Writing;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSuite(builder.Configuration);

using var host = builder.Build();
await host.StartAsync();

var earlReport = await host.Services.GetRequiredService<Executor>().Execute();
earlReport.SaveToStream(Console.Out, new CompressingTurtleWriter());

await host.StopAsync();
