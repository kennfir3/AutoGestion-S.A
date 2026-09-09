using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Productos.Api.Contracts;
using Productos.Api.Data;
using Productos.Api.Models;

namespace Productos.Api.Controllers;

[ApiController]
[Route("api/productos")]
public sealed class ProductosController(ProductosDbContext db, IDistributedCache cache) : ControllerBase
{
    private const string CacheKey = "productos:lista";
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Producto>>> GetAll()
    {
        var cached = await cache.GetStringAsync(CacheKey);
        if (cached is not null)
        {
            Response.Headers["X-Cache"] = "HIT";
            return Ok(JsonSerializer.Deserialize<List<Producto>>(cached, JsonOptions) ?? []);
        }

        var productos = await db.Productos.AsNoTracking().OrderBy(x => x.Id).ToListAsync();
        await cache.SetStringAsync(CacheKey, JsonSerializer.Serialize(productos, JsonOptions),
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) });
        Response.Headers["X-Cache"] = "MISS";
        return Ok(productos);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Producto>> GetById(int id)
    {
        var producto = await db.Productos.FindAsync(id);
        return producto is null ? NotFound() : Ok(producto);
    }

    [HttpPost]
    public async Task<ActionResult<Producto>> Create(ProductoRequest request)
    {
        var producto = new Producto { Nombre = request.Nombre, Precio = request.Precio, Stock = request.Stock };
        db.Productos.Add(producto);
        await db.SaveChangesAsync();
        await cache.RemoveAsync(CacheKey);
        return CreatedAtAction(nameof(GetById), new { id = producto.Id }, producto);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, ProductoRequest request)
    {
        var producto = await db.Productos.FindAsync(id);
        if (producto is null) return NotFound();
        producto.Nombre = request.Nombre;
        producto.Precio = request.Precio;
        producto.Stock = request.Stock;
        await db.SaveChangesAsync();
        await cache.RemoveAsync(CacheKey);
        return Ok(producto);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var producto = await db.Productos.FindAsync(id);
        if (producto is null) return NotFound();
        db.Productos.Remove(producto);
        await db.SaveChangesAsync();
        await cache.RemoveAsync(CacheKey);
        return NoContent();
    }
}
