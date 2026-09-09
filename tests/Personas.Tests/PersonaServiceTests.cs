using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Personas.Api.Contracts;
using Personas.Api.Controllers;
using Personas.Api.Data;
using Personas.Api.Models;
using Personas.Api.Services;

namespace Personas.Tests;

public sealed class PersonaServiceTests
{
    [Fact]
    public async Task CrearPersona_DuiInvalido_DevuelveBadRequest()
    {
        await using var db = CreateContext();
        var controller = CreateController(db);
        var request = new PersonaRequest { Nombre = "Ana", DUI = "123" };
        AddValidationErrors(controller, request);

        var result = await controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task CrearPersona_NombreVacio_DevuelveBadRequest()
    {
        await using var db = CreateContext();
        var controller = CreateController(db);
        var request = new PersonaRequest { Nombre = "", DUI = "12345678-9" };
        AddValidationErrors(controller, request);

        var result = await controller.Create(request);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }

    [Fact]
    public async Task ObtenerPersona_IdInexistente_DevuelveNotFound()
    {
        await using var db = CreateContext();
        var controller = CreateController(db);

        var result = await controller.GetById(404);

        Assert.IsType<NotFoundResult>(result.Result);
    }

    [Fact]
    public async Task CrearPersona_DatosValidos_GuardaCorrectamente()
    {
        await using var db = CreateContext();
        var controller = CreateController(db);
        var request = new PersonaRequest { Nombre = "Maria Lopez", DUI = "12345678-9" };

        var result = await controller.Create(request);

        var created = Assert.IsType<CreatedAtActionResult>(result.Result);
        var persona = Assert.IsType<Persona>(created.Value);
        Assert.Equal("Maria Lopez", persona.Nombre);
        Assert.Single(await db.Personas.ToListAsync());
    }

    [Fact]
    public async Task ActualizarPersona_DatosValidos_DevuelveOk()
    {
        await using var db = CreateContext();
        var service = new PersonaService(db);
        var original = await service.CreateAsync(new PersonaRequest { Nombre = "Carlos", DUI = "11111111-1" });
        var controller = new PersonasController(service);

        var result = await controller.Update(original.Id, new PersonaRequest { Nombre = "Carlos Ruiz", DUI = "22222222-2" });

        var ok = Assert.IsType<OkObjectResult>(result);
        var updated = Assert.IsType<Persona>(ok.Value);
        Assert.Equal("Carlos Ruiz", updated.Nombre);
        Assert.Equal("22222222-2", (await db.Personas.FindAsync(original.Id))!.DUI);
    }

    private static PersonasDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PersonasDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new PersonasDbContext(options);
    }

    private static PersonasController CreateController(PersonasDbContext db) =>
        new(new PersonaService(db));

    private static void AddValidationErrors(ControllerBase controller, object model)
    {
        var results = new List<ValidationResult>();
        Validator.TryValidateObject(model, new ValidationContext(model), results, validateAllProperties: true);
        foreach (var validationResult in results)
        {
            var key = validationResult.MemberNames.FirstOrDefault() ?? string.Empty;
            controller.ModelState.AddModelError(key, validationResult.ErrorMessage ?? "Validation failed.");
        }
    }
}
