namespace GR220295_Desafio3.Entities.Models;

public class HojaDeVida
{
    public int IdCV { get; set; }
    public string IdUsuario { get; set; } = string.Empty;
    public string FormacionAcademica { get; set; } = string.Empty;
    public string ExperienciaLaboral { get; set; } = string.Empty;
    public string Competencias { get; set; } = string.Empty;
    public string ArchivoCV { get; set; } = string.Empty;

    public Usuario? Usuario { get; set; }
}
