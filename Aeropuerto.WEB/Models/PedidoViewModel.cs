using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Cafeteria.WEB.Models;

public class PedidoViewModel
{
    [Required(ErrorMessage = "Escribe tu nombre")]
    [Display(Name = "Nombre")]
    public string Cliente { get; set; } = string.Empty;

    [Required(ErrorMessage = "Selecciona un café")]
    [Display(Name = "Café")]
    public int CafeId { get; set; }

    [Range(1, 20, ErrorMessage = "La cantidad debe estar entre 1 y 20")]
    public int Cantidad { get; set; } = 1;

    [StringLength(250)]
    public string? Observaciones { get; set; }

    public IEnumerable<SelectListItem> Cafes { get; set; } = Enumerable.Empty<SelectListItem>();
}
