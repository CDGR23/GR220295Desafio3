using System.ComponentModel.DataAnnotations;

namespace GR220295_Desafio3.Entities.Dtos;

public class ActualizarPerfilDto
{
    [Required, StringLength(150)]
    public string NombreCompleto { get; set; } = string.Empty;

    [StringLength(20)]
    public string Telefono { get; set; } = string.Empty;
}
