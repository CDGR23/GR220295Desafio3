using GR220295_Desafio3.Entities.Models;

namespace GR220295_Desafio3.DAL.Interfaces;

public interface IPostulacionRepository
{
    Task<List<Postulacion>> ObtenerTodasAsync();
    Task<List<Postulacion>> ObtenerPorCandidatoAsync(string idCandidato);
    Task<List<Postulacion>> ObtenerPorPlazaAsync(int idPlaza);
    Task<Postulacion?> ObtenerPorIdAsync(int idPostulacion);
    Task<bool> ExisteAsync(int idPlaza, string idCandidato);
    Task<bool> CandidatoValidoAsync(string idCandidato);
    Task<Plaza?> ObtenerPlazaAsync(int idPlaza);
    Task AgregarAsync(Postulacion postulacion);
    Task GuardarCambiosAsync();
}
