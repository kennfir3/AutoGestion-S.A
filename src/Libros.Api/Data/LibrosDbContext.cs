using Libros.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Libros.Api.Data;

public sealed class LibrosDbContext(DbContextOptions<LibrosDbContext> options) : DbContext(options)
{
    public DbSet<Libro> Libros => Set<Libro>();
}
