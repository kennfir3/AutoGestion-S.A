using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { service = "Gateway.Api", status = "running" }));
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("ClientId"))
        context.Request.Headers["ClientId"] = "anonymous";
    await next();
});
await app.UseOcelot();
app.Run();
