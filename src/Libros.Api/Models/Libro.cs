using System.ComponentModel.DataAnnotations;

namespace Libros.Api.Models;

public sealed class Libro
{
    public int Id { get; set; }
    [Required, StringLength(200)]
    public string Titulo { get; set; } = string.Empty;
    [Required, StringLength(160)]
    public string Autor { get; set; } = string.Empty;
    [Required, StringLength(20)]
    public string ISBN { get; set; } = string.Empty;
}
