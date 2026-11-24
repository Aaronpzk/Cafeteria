using Cafeteria.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// Soporte para MVC (Controladores y Vistas)
builder.Services.AddControllersWithViews();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Cuenta/Login";
        options.AccessDeniedPath = "/Cuenta/AccesoDenegado";
    });
builder.Services.AddAuthorization();

// Configuración de Entity Framework Core con SQL Server
var connectionString = builder.Configuration.GetConnectionString("CafeteriaDB")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'CafeteriaDB' en appsettings.json.");

builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
    try
    {
        await context.Database.EnsureCreatedAsync();

        if (!await context.Cafes.AnyAsync())
        {
            context.Cafes.AddRange(
                new Cafeteria.Domain.Entities.Cafe
                {
                    Nombre = "Espresso",
                    Descripcion = "Café intenso y aromático en una taza pequeña.",
                    Precio = 1.80m
                },
                new Cafeteria.Domain.Entities.Cafe
                {
                    Nombre = "Cappuccino",
                    Descripcion = "Espresso con leche cremosa y espuma.",
                    Precio = 2.80m
                },
                new Cafeteria.Domain.Entities.Cafe
                {
                    Nombre = "Latte",
                    Descripcion = "Suave combinación de espresso y leche vaporizada.",
                    Precio = 3.00m
                },
                new Cafeteria.Domain.Entities.Cafe
                {
                    Nombre = "Moka",
                    Descripcion = "Café con chocolate y una textura cremosa.",
                    Precio = 3.40m
                },
                new Cafeteria.Domain.Entities.Cafe
                {
                    Nombre = "Café con hielo",
                    Descripcion = "Café refrescante servido con hielo.",
                    Precio = 2.50m
                });
        }

        await context.SaveChangesAsync();
    }
    catch (Exception exception)
    {
        app.Logger.LogWarning(exception, "No se pudo inicializar la base de datos de la cafetería.");
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cafeteria}/{action=Index}/{id?}");

app.Run();