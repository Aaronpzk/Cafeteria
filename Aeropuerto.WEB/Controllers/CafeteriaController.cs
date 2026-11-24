using Cafeteria.Domain.Entities;
using Cafeteria.Infrastructure.Data;
using Cafeteria.WEB.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.WEB.Controllers;

public class CafeteriaController : Controller
{
    private readonly ApplicationDBContext _context;

    public CafeteriaController(ApplicationDBContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cafes = await _context.Cafes
            .Where(c => c.Disponible || User.IsInRole("Administrador"))
            .OrderBy(c => c.Nombre)
            .ToListAsync();

        var model = new CafeteriaPaginaViewModel
        {
            Cafes = cafes,
            EsAdministrador = User.IsInRole("Administrador"),
            NuevoPedido = new PedidoViewModel { Cafes = CrearOpcionesCafe(cafes) }
        };

        if (model.EsAdministrador)
        {
            model.Pedidos = await _context.Pedidos
                .Include(p => p.Cafe)
                .OrderByDescending(p => p.Fecha)
                .ToListAsync();

            model.Ventas = await _context.Pedidos
                .Include(p => p.Cafe)
                .GroupBy(p => new { p.CafeId, Nombre = p.Cafe.Nombre })
                .Select(g => new VentaCafeViewModel
                {
                    Cafe = g.Key.Nombre,
                    CantidadVendida = g.Sum(p => p.Cantidad),
                    Importe = g.Sum(p => p.Cantidad * p.PrecioUnitario)
                })
                .OrderByDescending(v => v.CantidadVendida)
                .ToListAsync();
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearPedido([Bind(Prefix = "NuevoPedido")] PedidoViewModel model)
    {
        var cafe = await _context.Cafes
            .FirstOrDefaultAsync(c => c.Id == model.CafeId && c.Disponible);

        if (cafe is null)
            ModelState.AddModelError(nameof(model.CafeId), "El café seleccionado no está disponible");

        if (!ModelState.IsValid)
        {
            var pagina = await IndexModelAsync();
            pagina.NuevoPedido = model;
            pagina.NuevoPedido.Cafes = CrearOpcionesCafe(pagina.Cafes);
            return View("Index", pagina);
        }

        _context.Pedidos.Add(new Pedido
        {
            Cliente = model.Cliente.Trim(),
            CafeId = cafe!.Id,
            Cantidad = model.Cantidad,
            PrecioUnitario = cafe.Precio,
            Observaciones = string.IsNullOrWhiteSpace(model.Observaciones) ? null : model.Observaciones.Trim()
        });
        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Pedido registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Pedido(int? cafeId)
    {
        var model = new PedidoViewModel { CafeId = cafeId ?? 0 };
        await CargarCafes(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pedido(PedidoViewModel model)
    {
        var cafe = await _context.Cafes
            .FirstOrDefaultAsync(c => c.Id == model.CafeId && c.Disponible);

        if (cafe is null)
            ModelState.AddModelError(nameof(model.CafeId), "El café seleccionado no está disponible");

        if (!ModelState.IsValid)
        {
            await CargarCafes(model);
            return View(model);
        }

        var pedido = new Pedido
        {
            Cliente = model.Cliente.Trim(),
            CafeId = cafe!.Id,
            Cantidad = model.Cantidad,
            PrecioUnitario = cafe.Precio,
            Observaciones = string.IsNullOrWhiteSpace(model.Observaciones)
                ? null
                : model.Observaciones.Trim()
        };

        _context.Pedidos.Add(pedido);
        await _context.SaveChangesAsync();

        TempData["Mensaje"] = $"Pedido #{pedido.Id} registrado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    private async Task CargarCafes(PedidoViewModel model)
    {
        model.Cafes = await _context.Cafes
            .Where(c => c.Disponible)
            .OrderBy(c => c.Nombre)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = $"{c.Nombre} - {c.Precio:C}"
            })
            .ToListAsync();
    }

    private async Task<CafeteriaPaginaViewModel> IndexModelAsync()
    {
        var cafes = await _context.Cafes
            .Where(c => c.Disponible || User.IsInRole("Administrador"))
            .OrderBy(c => c.Nombre)
            .ToListAsync();
        return new CafeteriaPaginaViewModel
        {
            Cafes = cafes,
            EsAdministrador = User.IsInRole("Administrador"),
            NuevoPedido = new PedidoViewModel { Cafes = CrearOpcionesCafe(cafes) }
        };
    }

    private static IEnumerable<SelectListItem> CrearOpcionesCafe(IEnumerable<Cafe> cafes) => cafes.Select(c => new SelectListItem
    {
        Value = c.Id.ToString(),
        Text = $"{c.Nombre} - {c.Precio:C}"
    });
}
