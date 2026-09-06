var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers();

builder.Services.AddSingleton<ILwsStorage, InMemoryLwsStorage>();
builder.Services.AddSingleton<RequestContext>();
builder.Services.AddTransient<PopulateRequestContext>();

builder.Services.AddRouting(routing =>
{
    routing.SetParameterPolicy<ExistsConstraint>(ExistsConstraint.Name);
    routing.SetParameterPolicy<NotExistsConstraint>(NotExistsConstraint.Name);
    routing.SetParameterPolicy<ContainerConstraint>(ContainerConstraint.Name);
    routing.SetParameterPolicy<DataResourceConstraint>(DataResourceConstraint.Name);
});

var app = builder.Build();

app.UseMiddleware<PopulateRequestContext>();
app.UseRouting();

app.MapControllers();

app.Run();
