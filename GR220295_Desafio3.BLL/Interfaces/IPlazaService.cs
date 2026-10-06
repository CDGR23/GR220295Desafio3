using GR220295_Desafio3.Entities.Dtos;

namespace GR220295_Desafio3.BLL.Interfaces;

public interface IPlazaService
{
    Task<List<PlazaDto>> ObtenerPlazasAsync(bool soloDisponibles = false);
    Task<PlazaDto?> ObtenerPlazaAsync(int id);
    Task<(bool Exito, string Mensaje, PlazaDto? Plaza)> CrearAsync(CrearPlazaDto dto, string idAgente);
    Task<(bool Exito, string Mensaje, PlazaDto? Plaza)> ActualizarAsync(int id, ActualizarPlazaDto dto, string idAgente);
    Task<(bool Exito, string Mensaje)> EliminarAsync(int id, string idAgente);
    Task<(bool Exito, string Mensaje)> PublicarAsync(int id, string idAgente);
    Task<(bool Exito, string Mensaje)> CambiarEstadoAsync(int id, string estado, string idUsuario, bool esAdministrador);
}
