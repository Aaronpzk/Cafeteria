using System.ComponentModel.DataAnnotations;

namespace Cafeteria.WEB.Models;

public class LoginViewModel
{
    [Required(ErrorMessage = "Escribe el usuario")]
    public string Usuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "Escribe la contraseña")]
    [DataType(DataType.Password)]
    public string Contrasena { get; set; } = string.Empty;
}
