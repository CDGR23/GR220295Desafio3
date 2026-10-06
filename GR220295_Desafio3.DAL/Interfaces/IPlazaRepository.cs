using GR220295_Desafio3.Entities.Models;

namespace GR220295_Desafio3.DAL.Interfaces;

public interface IPlazaRepository
{
    Task<List<Plaza>> ObtenerTodasAsync(bool soloDisponibles = false);
    Task<Plaza?> ObtenerPorIdAsync(int id, bool incluirPostulaciones = false);
    Task<bool> AgenteValidoAsync(string idAgente);
    Task AgregarAsync(Plaza plaza);
    void Eliminar(Plaza plaza);
    Task GuardarCambiosAsync();
}
