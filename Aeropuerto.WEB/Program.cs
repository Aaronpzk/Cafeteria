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
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });
builder.Services.AddAuthorization();

// Render ejecuta Linux y debe usar PostgreSQL. SQL Server se mantiene para desarrollo local.
var connectionString = builder.Configuration.GetConnectionString("CafeteriaDB")
    ?? throw new InvalidOperationException(
        "No se encontró la cadena de conexión 'CafeteriaDB'. Configúrala como ConnectionStrings__CafeteriaDB.");

var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "SqlServer";

builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    if (databaseProvider.Equals("Postgres", StringComparison.OrdinalIgnoreCase) ||
        databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
    {
        options.UseNpgsql(connectionString);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

if (!builder.Environment.IsDevelopment() &&
    (string.IsNullOrWhiteSpace(builder.Configuration["AdminSettings:Username"]) ||
     string.IsNullOrWhiteSpace(builder.Configuration["AdminSettings:Password"])))
{
    throw new InvalidOperationException(
        "En producción deben configurarse AdminSettings__Username y AdminSettings__Password.");
}

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

// Render termina HTTPS en su proxy y entrega HTTP al contenedor.
// No activar UseHttpsRedirection aquí porque provoca bucles o avisos de puerto HTTPS.
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cafeteria}/{action=Index}/{id?}");

app.Run();