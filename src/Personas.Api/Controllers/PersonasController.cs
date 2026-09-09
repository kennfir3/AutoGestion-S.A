using Microsoft.AspNetCore.Mvc;
using Personas.Api.Contracts;
using Personas.Api.Models;
using Personas.Api.Services;

namespace Personas.Api.Controllers;

[ApiController]
[Route("api/personas")]
public sealed class PersonasController(IPersonaService service) : ControllerBase
{
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Persona>> GetById(int id)
    {
        var persona = await service.GetByIdAsync(id);
        return persona is null ? NotFound() : Ok(persona);
    }

    [HttpPost]
    public async Task<ActionResult<Persona>> Create(PersonaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var persona = await service.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = persona.Id }, persona);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PersonaRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var persona = await service.UpdateAsync(id, request);
        return persona is null ? NotFound() : Ok(persona);
    }
}
