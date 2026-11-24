using Cafeteria.Domain.Entities;

namespace Cafeteria.WEB.Models;

public class AdminPedidoViewModel
{
    public IReadOnlyList<Pedido> Pedidos { get; set; } = [];
    public IReadOnlyList<VentaCafeViewModel> Ventas { get; set; } = [];
}

public class VentaCafeViewModel
{
    public string Cafe { get; set; } = string.Empty;
    public int CantidadVendida { get; set; }
    public decimal Importe { get; set; }
}
