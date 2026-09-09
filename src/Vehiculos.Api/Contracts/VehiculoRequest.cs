using System.ComponentModel.DataAnnotations;

namespace Vehiculos.Api.Contracts;

public sealed class VehiculoRequest
{
    [Required, StringLength(12)]
    public string Placa { get; set; } = string.Empty;
    [Required, StringLength(80)]
    public string Marca { get; set; } = string.Empty;
    [Required, StringLength(80)]
    public string Modelo { get; set; } = string.Empty;
}
