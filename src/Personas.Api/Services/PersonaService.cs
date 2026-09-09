using Microsoft.EntityFrameworkCore;
using Personas.Api.Contracts;
using Personas.Api.Data;
using Personas.Api.Models;

namespace Personas.Api.Services;

public sealed class PersonaService(PersonasDbContext db) : IPersonaService
{
    public async Task<Persona> CreateAsync(PersonaRequest request)
    {
        var persona = new Persona { Nombre = request.Nombre, DUI = request.DUI };
        db.Personas.Add(persona);
        await db.SaveChangesAsync();
        return persona;
    }

    public Task<Persona?> GetByIdAsync(int id) =>
        db.Personas.AsNoTracking().SingleOrDefaultAsync(persona => persona.Id == id);

    public async Task<Persona?> UpdateAsync(int id, PersonaRequest request)
    {
        var persona = await db.Personas.FindAsync(id);
        if (persona is null) return null;
        persona.Nombre = request.Nombre;
        persona.DUI = request.DUI;
        await db.SaveChangesAsync();
        return persona;
    }
}
