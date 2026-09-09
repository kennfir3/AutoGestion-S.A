using Libros.Api.Contracts;
using Libros.Api.Data;
using Libros.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Libros.Api.Controllers;

[ApiController]
[Route("api/libros")]
public sealed class LibrosController(LibrosDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Libro>>> GetAll() =>
        Ok(await db.Libros.AsNoTracking().OrderBy(x => x.Id).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Libro>> GetById(int id)
    {
        var libro = await db.Libros.FindAsync(id);
        return libro is null ? NotFound() : Ok(libro);
    }

    [HttpPost]
    public async Task<ActionResult<Libro>> Create(LibroRequest request)
    {
        var libro = new Libro { Titulo = request.Titulo, Autor = request.Autor, ISBN = request.ISBN };
        db.Libros.Add(libro);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = libro.Id }, libro);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, LibroRequest request)
    {
        var libro = await db.Libros.FindAsync(id);
        if (libro is null) return NotFound();
        libro.Titulo = request.Titulo;
        libro.Autor = request.Autor;
        libro.ISBN = request.ISBN;
        await db.SaveChangesAsync();
        return Ok(libro);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var libro = await db.Libros.FindAsync(id);
        if (libro is null) return NotFound();
        db.Libros.Remove(libro);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
