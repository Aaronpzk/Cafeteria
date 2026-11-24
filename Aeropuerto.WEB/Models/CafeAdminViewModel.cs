using System.ComponentModel.DataAnnotations;

namespace Cafeteria.WEB.Models;

public class CafeAdminViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Escribe el nombre")]
    [StringLength(100)]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(500)]
    public string Descripcion { get; set; } = string.Empty;

    [Range(0.01, 100000, ErrorMessage = "El precio debe ser mayor que cero")]
    public decimal Precio { get; set; }

    public bool Disponible { get; set; } = true;
}
