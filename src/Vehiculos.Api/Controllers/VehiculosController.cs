using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vehiculos.Api.Contracts;
using Vehiculos.Api.Data;
using Vehiculos.Api.Models;

namespace Vehiculos.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/vehiculos")]
public sealed class VehiculosController(ApplicationDbContext db) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Vehiculo>>> GetAll() =>
        Ok(await db.Vehiculos.AsNoTracking().OrderBy(x => x.Id).ToListAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<Vehiculo>> GetById(int id)
    {
        var vehiculo = await db.Vehiculos.FindAsync(id);
        return vehiculo is null ? NotFound() : Ok(vehiculo);
    }

    [HttpPost]
    public async Task<ActionResult<Vehiculo>> Create(VehiculoRequest request)
    {
        var vehiculo = new Vehiculo { Placa = request.Placa, Marca = request.Marca, Modelo = request.Modelo };
        db.Vehiculos.Add(vehiculo);
        await db.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = vehiculo.Id }, vehiculo);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, VehiculoRequest request)
    {
        var vehiculo = await db.Vehiculos.FindAsync(id);
        if (vehiculo is null) return NotFound();
        vehiculo.Placa = request.Placa;
        vehiculo.Marca = request.Marca;
        vehiculo.Modelo = request.Modelo;
        await db.SaveChangesAsync();
        return Ok(vehiculo);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var vehiculo = await db.Vehiculos.FindAsync(id);
        if (vehiculo is null) return NotFound();
        db.Vehiculos.Remove(vehiculo);
        await db.SaveChangesAsync();
        return NoContent();
    }
}
