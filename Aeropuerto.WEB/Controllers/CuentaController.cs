using System.Security.Claims;
using Cafeteria.WEB.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;

namespace Cafeteria.WEB.Controllers;

public class CuentaController : Controller
{
    private readonly IConfiguration _configuration;

    public CuentaController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        ViewBag.ReturnUrl = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return View(model);

        var username = _configuration["AdminSettings:Username"];
        var password = _configuration["AdminSettings:Password"];

        if (!string.Equals(model.Usuario, username, StringComparison.OrdinalIgnoreCase) ||
            model.Contrasena != password)
        {
            ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
            return View(model);
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, model.Usuario),
            new Claim(ClaimTypes.Role, "Administrador")
        };
        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(identity));

        return LocalRedirect(returnUrl ?? Url.Action("Index", "Cafeteria")!);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction(nameof(Login));
    }

    public IActionResult AccesoDenegado() => View();
}
