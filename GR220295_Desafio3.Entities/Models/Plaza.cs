namespace GR220295_Desafio3.Entities.Models;

public class Plaza
{
    public int IdPlaza { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime FechaPublicacion { get; set; }
    public DateTime FechaCierre { get; set; }
    public string Institucion { get; set; } = string.Empty;
    public string Estado { get; set; } = Constantes.EstadosPlaza.Abierta;
    public string IdAgente { get; set; } = string.Empty;

    public Usuario? Agente { get; set; }
    public ICollection<Postulacion> Postulaciones { get; set; } = new List<Postulacion>();
}
