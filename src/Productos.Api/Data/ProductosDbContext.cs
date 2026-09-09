using Microsoft.EntityFrameworkCore;
using Productos.Api.Models;

namespace Productos.Api.Data;

public sealed class ProductosDbContext(DbContextOptions<ProductosDbContext> options) : DbContext(options)
{
    public DbSet<Producto> Productos => Set<Producto>();
}
