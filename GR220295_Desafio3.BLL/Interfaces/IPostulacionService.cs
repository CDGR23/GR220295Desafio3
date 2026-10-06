using GR220295_Desafio3.Entities.Dtos;

namespace GR220295_Desafio3.BLL.Interfaces;

public interface IPostulacionService
{
    Task<List<PostulacionDto>> ObtenerMisPostulacionesAsync(string idCandidato);
    Task<List<PostulacionDto>> ObtenerTodasAsync();
    Task<List<PostulacionDto>> ObtenerPorPlazaAsync(int idPlaza);
    Task<(bool Exito, string Mensaje, PostulacionDto? Postulacion)> CrearAsync(int idPlaza, string? idCandidato);
    Task<(bool Exito, string Mensaje)> CambiarEstadoAsync(int idPostulacion, string estado, string idAgente, bool esAdministrador);
}
