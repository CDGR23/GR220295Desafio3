using System.ComponentModel.DataAnnotations;

namespace GR220295_Desafio3.Entities.Dtos;

public class ActualizarPlazaDto
{
    [Required, StringLength(200)]
    public string Titulo { get; set; } = string.Empty;

    [Required, StringLength(2000)]
    public string Descripcion { get; set; } = string.Empty;

    [Required, StringLength(200)]
    public string Institucion { get; set; } = string.Empty;

    [Required]
    public DateTime FechaPublicacion { get; set; }

    [Required]
    public DateTime FechaCierre { get; set; }
}
