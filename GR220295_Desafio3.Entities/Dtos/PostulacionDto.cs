namespace GR220295_Desafio3.Entities.Dtos;

public class PostulacionDto
{
    public int IdPostulacion { get; set; }
    public int IdPlaza { get; set; }
    public string TituloPlaza { get; set; } = string.Empty;
    public string IdCandidato { get; set; } = string.Empty;
    public string NombreCandidato { get; set; } = string.Empty;
    public DateTime FechaPostulacion { get; set; }
    public string Estado { get; set; } = string.Empty;
}
