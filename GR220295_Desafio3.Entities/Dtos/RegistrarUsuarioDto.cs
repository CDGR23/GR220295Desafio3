using System.ComponentModel.DataAnnotations;

namespace GR220295_Desafio3.Entities.Dtos;

public class RegistrarUsuarioDto
{
    [Required, StringLength(150)]
    public string NombreCompleto { get; set; } = string.Empty;

    [Required, EmailAddress, StringLength(256)]
    public string Email { get; set; } = string.Empty;

    [Required, MinLength(8)]
    public string Password { get; set; } = string.Empty;
}
