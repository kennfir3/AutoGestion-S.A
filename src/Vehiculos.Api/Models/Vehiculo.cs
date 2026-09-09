using System.ComponentModel.DataAnnotations;

namespace Vehiculos.Api.Models;

public sealed class Vehiculo
{
    public int Id { get; set; }
    [Required, StringLength(12)]
    public string Placa { get; set; } = string.Empty;
    [Required, StringLength(80)]
    public string Marca { get; set; } = string.Empty;
    [Required, StringLength(80)]
    public string Modelo { get; set; } = string.Empty;
}
