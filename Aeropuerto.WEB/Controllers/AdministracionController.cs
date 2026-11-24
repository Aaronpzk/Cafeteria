using Cafeteria.Domain.Entities;
using Cafeteria.Infrastructure.Data;
using Cafeteria.WEB.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Cafeteria.WEB.Controllers;

[Authorize(Roles = "Administrador")]
public class AdministracionController : Controller
{
    private readonly ApplicationDBContext _context;

    public AdministracionController(ApplicationDBContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var cafes = await _context.Cafes.OrderBy(c => c.Nombre).ToListAsync();
        return View(cafes);
    }

    [HttpGet]
    public IActionResult CrearCafe() => View(new CafeAdminViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearCafe([Bind(Prefix = "NuevoCafe")] CafeAdminViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        _context.Cafes.Add(new Cafe
        {
            Nombre = model.Nombre.Trim(),
            Descripcion = model.Descripcion.Trim(),
            Precio = model.Precio,
            Disponible = model.Disponible
        });
        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Producto creado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditarCafe(int id)
    {
        var cafe = await _context.Cafes.FindAsync(id);
        if (cafe is null)
            return NotFound();

        return View(new CafeAdminViewModel
        {
            Id = cafe.Id,
            Nombre = cafe.Nombre,
            Descripcion = cafe.Descripcion,
            Precio = cafe.Precio,
            Disponible = cafe.Disponible
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditarCafe(CafeAdminViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var cafe = await _context.Cafes.FindAsync(model.Id);
        if (cafe is null)
            return NotFound();

        cafe.Nombre = model.Nombre.Trim();
        cafe.Descripcion = model.Descripcion.Trim();
        cafe.Precio = model.Precio;
        cafe.Disponible = model.Disponible;
        await _context.SaveChangesAsync();
        TempData["Mensaje"] = "Producto y precio actualizados correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarDisponibilidad(int id)
    {
        var cafe = await _context.Cafes.FindAsync(id);
        if (cafe is null)
            return NotFound();

        cafe.Disponible = !cafe.Disponible;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Pedidos()
    {
        var pedidos = await _context.Pedidos
            .Include(p => p.Cafe)
            .OrderByDescending(p => p.Fecha)
            .ToListAsync();
        return View(pedidos);
    }

    public async Task<IActionResult> Ventas()
    {
        var ventas = await _context.Pedidos
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

        return View(ventas);
    }
}
