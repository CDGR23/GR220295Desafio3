namespace GR220295_Desafio3.Entities.Models;

public class Postulacion
{
    public int IdPostulacion { get; set; }
    public int IdPlaza { get; set; }
    public string IdCandidato { get; set; } = string.Empty;
    public DateTime FechaPostulacion { get; set; }
    public string Estado { get; set; } = Constantes.EstadosPostulacion.EnRevision;

    public Plaza? Plaza { get; set; }
    public Usuario? Candidato { get; set; }
}
