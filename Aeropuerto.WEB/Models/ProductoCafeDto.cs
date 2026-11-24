namespace Cafeteria.WEB.Models;

public class ProductoCafeDto
{
    public int Id { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public bool Disponible { get; set; }
}
