using System.ComponentModel.DataAnnotations;

namespace GR220295_Desafio3.Entities.Dtos;

public class IniciarSesionDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
}
