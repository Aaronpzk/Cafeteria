using Cafeteria.WEB.Models;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.WEB.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly ProductoCafeDto[] Productos = new[]
        {
            new ProductoCafeDto
            {
                Id = 1,
                Nombre = "Café americano",
                Categoria = "Café",
                Precio = 1.80m,
                Disponible = true
            },
            new ProductoCafeDto
            {
                Id = 2,
                Nombre = "Capuchino",
                Categoria = "Café",
                Precio = 2.50m,
                Disponible = true
            },
            new ProductoCafeDto
            {
                Id = 3,
                Nombre = "Té verde",
                Categoria = "Infusión",
                Precio = 1.60m,
                Disponible = true
            },
            new ProductoCafeDto
            {
                Id = 4,
                Nombre = "Croissant",
                Categoria = "Bollería",
                Precio = 2.20m,
                Disponible = false
            }
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetProductos")]
        public ActionResult<IEnumerable<ProductoCafeDto>> Get()
        {
            _logger.LogInformation("Consultando productos de cafetería");
            return Ok(Productos);
        }
    }
}
