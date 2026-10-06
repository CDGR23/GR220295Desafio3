namespace GR220295_Desafio3.Entities.Dtos;

public class PlazaDto
{
    public int IdPlaza { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public DateTime FechaCierre { get; set; }
    public string Institucion { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string IdAgente { get; set; } = string.Empty;
    public string NombreAgente { get; set; } = string.Empty;
}
