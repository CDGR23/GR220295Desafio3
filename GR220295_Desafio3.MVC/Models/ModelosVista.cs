using System.ComponentModel.DataAnnotations;

namespace GR220295_Desafio3.MVC.Models;

public class LoginViewModel
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}

public class RegistroViewModel
{
    [Required, StringLength(150)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string ConfirmarPassword { get; set; } = string.Empty;
}

public class PlazaViewModel
{
    public int IdPlaza { get; set; }

    [Required, StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Descripcion { get; set; } = string.Empty;

    public DateTime FechaPublicacion { get; set; } = DateTime.Now;
    public DateTime FechaCierre { get; set; } = DateTime.Now.AddDays(15);

    [Required, StringLength(200)]
    public string Institucion { get; set; } = string.Empty;

    public string Estado { get; set; } = string.Empty;
    public string IdAgente { get; set; } = string.Empty;
    public string NombreAgente { get; set; } = string.Empty;
}

public class HojaDeVidaViewModel
{
    [Required, StringLength(2000)]
    public string FormacionAcademica { get; set; } = string.Empty;

    [Required, StringLength(3000)]
    public string ExperienciaLaboral { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Competencias { get; set; } = string.Empty;

    public IFormFile? ArchivoCV { get; set; }
    public string ArchivoExistente { get; set; } = string.Empty;
}


public class PerfilViewModel
{
    [Required, StringLength(150)]
    public string NombreCompleto { get; set; } = string.Empty;

    [StringLength(20)]
    public string Telefono { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
}
