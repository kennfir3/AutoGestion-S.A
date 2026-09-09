using System.ComponentModel.DataAnnotations;

namespace Personas.Api.Contracts;

public sealed class PersonaRequest
{
    [Required(AllowEmptyStrings = false), StringLength(120)]
    public string Nombre { get; set; } = string.Empty;
    [Required, RegularExpression(@"^\d{8}-\d$", ErrorMessage = "DUI must use format 00000000-0.")]
    public string DUI { get; set; } = string.Empty;
}
