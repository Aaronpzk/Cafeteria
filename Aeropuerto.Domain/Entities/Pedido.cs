namespace Cafeteria.Domain.Entities;

public class Pedido
{
    public int Id { get; set; }
    public string Cliente { get; set; } = string.Empty;
    public int CafeId { get; set; }
    public Cafe Cafe { get; set; } = null!;
    public int Cantidad { get; set; }
    public decimal PrecioUnitario { get; set; }
    public string? Observaciones { get; set; }
    public DateTime Fecha { get; set; } = DateTime.UtcNow;
    public string Estado { get; set; } = "Pendiente";
}
