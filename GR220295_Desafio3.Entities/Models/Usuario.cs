using Microsoft.AspNetCore.Identity;

namespace GR220295_Desafio3.Entities.Models;

public class Usuario : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public ICollection<Plaza> PlazasPublicadas { get; set; } = new List<Plaza>();
    public ICollection<Postulacion> Postulaciones { get; set; } = new List<Postulacion>();
    public HojaDeVida? HojaDeVida { get; set; }
}
