namespace Cafeteria.Domain.Entities;

public class Cafe
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public bool Disponible { get; set; } = true;

    public ICollection<Pedido> Pedidos { get; set; } = new List<Pedido>();
}
