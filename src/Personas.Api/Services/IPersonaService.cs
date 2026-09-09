using Personas.Api.Contracts;
using Personas.Api.Models;

namespace Personas.Api.Services;

public interface IPersonaService
{
    Task<Persona> CreateAsync(PersonaRequest request);
    Task<Persona?> GetByIdAsync(int id);
    Task<Persona?> UpdateAsync(int id, PersonaRequest request);
}
