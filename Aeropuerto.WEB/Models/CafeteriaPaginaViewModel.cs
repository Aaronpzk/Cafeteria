using Cafeteria.Domain.Entities;

namespace Cafeteria.WEB.Models;

public class CafeteriaPaginaViewModel
{
    public IReadOnlyList<Cafe> Cafes { get; set; } = [];
    public IReadOnlyList<Pedido> Pedidos { get; set; } = [];
    public IReadOnlyList<VentaCafeViewModel> Ventas { get; set; } = [];
    public PedidoViewModel NuevoPedido { get; set; } = new();
    public CafeAdminViewModel NuevoCafe { get; set; } = new();
    public bool EsAdministrador { get; set; }
}
