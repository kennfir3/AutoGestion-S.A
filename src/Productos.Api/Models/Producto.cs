using System.ComponentModel.DataAnnotations;

namespace Productos.Api.Models;

public sealed class Producto
{
    public int Id { get; set; }
    [Required, StringLength(120)]
    public string Nombre { get; set; } = string.Empty;
    [Range(typeof(decimal), "0.01", "999999999")]
    public decimal Precio { get; set; }
    [Range(0, int.MaxValue)]
    public int Stock { get; set; }
}
