using GR220295_Desafio3.Entities.Models;

namespace GR220295_Desafio3.DAL.Interfaces;

public interface IHojaDeVidaRepository
{
    Task<HojaDeVida?> ObtenerPorUsuarioAsync(string idUsuario);
    Task AgregarAsync(HojaDeVida hoja);
    Task GuardarCambiosAsync();
}
