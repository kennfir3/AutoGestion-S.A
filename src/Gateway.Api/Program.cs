using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.OpenApi.Models;
using Gateway.Api;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AutoGestionSA Gateway", Version = "v1" });
    options.DocumentFilter<GatewayRoutesDocumentFilter>();
});

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.Use(async (context, next) =>
{
    if (!context.Request.Headers.ContainsKey("ClientId"))
        context.Request.Headers["ClientId"] = "anonymous";
    await next();
});
app.UseWhen(context => context.Request.Path == "/", landing => landing.Run(WriteLandingPageAsync));
await app.UseOcelot();
app.Run();

static async Task WriteLandingPageAsync(HttpContext context)
{
    var client = context.RequestServices.GetRequiredService<IHttpClientFactory>().CreateClient();
    var services = new[]
    {
        new GatewayService("Productos", "/productos", "http://productos.api/api/productos"),
        new GatewayService("Libros", "/libros", "http://libros.api/api/libros"),
        new GatewayService("Vehiculos", "/vehiculos", "http://vehiculos.api/api/vehiculos"),
        new GatewayService("Personas", "/personas/{id}", "http://personas.api/api/personas/0"),
        new GatewayService("Auth register", "/auth/register", "http://vehiculos.api/api/auth/register"),
        new GatewayService("Auth login", "/auth/login", "http://vehiculos.api/api/auth/login")
    };
    var rows = await Task.WhenAll(services.Select(async service =>
    {
        try
        {
            using var response = await client.GetAsync(service.HealthUrl, context.RequestAborted);
            return (service, response.StatusCode < System.Net.HttpStatusCode.InternalServerError ? "UP" : "DOWN");
        }
        catch
        {
            return (service, "DOWN");
        }
    }));

    context.Response.ContentType = "text/html; charset=utf-8";
    var rowsHtml = string.Join(string.Empty, rows.Select(row =>
        $"<tr><td>{row.service.Name}</td><td><a href=\"{row.service.PublicRoute}\">{row.service.PublicRoute}</a></td><td class=\"{row.Item2.ToLowerInvariant()}\">{row.Item2}</td></tr>"));
    var html = "<!doctype html><html><head><title>AutoGestionSA Gateway</title>"
        + "<style>body{font-family:Arial,sans-serif;margin:2rem;max-width:860px}table{border-collapse:collapse;width:100%}th,td{padding:.75rem;border-bottom:1px solid #ddd;text-align:left}.up{color:#167c2d;font-weight:bold}.down{color:#b42318;font-weight:bold}a{color:#0759a5}</style>"
        + "</head><body><h1>AutoGestionSA API Gateway</h1><p>Gateway activo. <a href=\"/swagger\">Abrir Swagger del Gateway</a></p>"
        + "<table><thead><tr><th>Servicio</th><th>Ruta publica</th><th>Estado</th></tr></thead><tbody>"
        + rowsHtml + "</tbody></table></body></html>";
    await context.Response.WriteAsync(html);
}

internal sealed record GatewayService(string Name, string PublicRoute, string HealthUrl);
