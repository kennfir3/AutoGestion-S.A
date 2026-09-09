using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Gateway.Api;

public sealed class GatewayRoutesDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        AddPath(swaggerDoc, "/productos", "Proxy a Productos.Api");
        AddPath(swaggerDoc, "/productos/{everything}", "Proxy a Productos.Api");
        AddPath(swaggerDoc, "/libros", "Proxy a Libros.Api");
        AddPath(swaggerDoc, "/libros/{everything}", "Proxy a Libros.Api");
        AddPath(swaggerDoc, "/vehiculos", "Proxy protegido a Vehiculos.Api");
        AddPath(swaggerDoc, "/vehiculos/{everything}", "Proxy protegido a Vehiculos.Api");
        AddPath(swaggerDoc, "/personas/{everything}", "Proxy a Personas.Api");
        AddPath(swaggerDoc, "/auth/{everything}", "Proxy de autenticacion a Vehiculos.Api");
    }

    private static void AddPath(OpenApiDocument document, string path, string description)
    {
        document.Paths[path] = new OpenApiPathItem
        {
            Operations =
            {
                [OperationType.Get] = new OpenApiOperation { Summary = description, Responses = SuccessResponse() },
                [OperationType.Post] = new OpenApiOperation { Summary = description, Responses = SuccessResponse() },
                [OperationType.Put] = new OpenApiOperation { Summary = description, Responses = SuccessResponse() },
                [OperationType.Delete] = new OpenApiOperation { Summary = description, Responses = SuccessResponse() }
            }
        };
    }

    private static OpenApiResponses SuccessResponse() =>
        new() { ["200"] = new OpenApiResponse { Description = "Respuesta del servicio downstream." } };
}
