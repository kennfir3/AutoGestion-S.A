using Microsoft.EntityFrameworkCore;
using Personas.Api.Data;
using Personas.Api.Services;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Server=(localdb)\\MSSQLLocalDB;Database=PersonasDb;Trusted_Connection=True;TrustServerCertificate=True";
builder.Services.AddDbContext<PersonasDbContext>(options => options.UseSqlServer(connectionString));
builder.Services.AddScoped<IPersonaService, PersonaService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
await InitializeDatabaseAsync(app.Services, app.Logger);
app.UseHttpsRedirection();
app.MapGet("/", () => Results.Redirect("/swagger"));
app.MapControllers();
app.Run();

static async Task InitializeDatabaseAsync(IServiceProvider services, ILogger logger)
{
    for (var attempt = 1; attempt <= 12; attempt++)
    {
        try
        {
            using var scope = services.CreateScope();
            await scope.ServiceProvider.GetRequiredService<PersonasDbContext>().Database.EnsureCreatedAsync();
            return;
        }
        catch (Exception exception) when (attempt < 12)
        {
            logger.LogWarning(exception, "Database unavailable, retry {Attempt}/12", attempt);
            await Task.Delay(TimeSpan.FromSeconds(3));
        }
    }
}
