using Microsoft.EntityFrameworkCore;
using Personas.Api.Models;

namespace Personas.Api.Data;

public sealed class PersonasDbContext(DbContextOptions<PersonasDbContext> options) : DbContext(options)
{
    public DbSet<Persona> Personas => Set<Persona>();
}
