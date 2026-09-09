using System.ComponentModel.DataAnnotations;

namespace Libros.Api.Contracts;

public sealed class LibroRequest
{
    [Required, StringLength(200)]
    public string Titulo { get; set; } = string.Empty;
    [Required, StringLength(160)]
    public string Autor { get; set; } = string.Empty;
    [Required, StringLength(20)]
    public string ISBN { get; set; } = string.Empty;
}
