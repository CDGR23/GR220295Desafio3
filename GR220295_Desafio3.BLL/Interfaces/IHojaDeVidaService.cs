using GR220295_Desafio3.Entities.Dtos;

namespace GR220295_Desafio3.BLL.Interfaces;

public interface IHojaDeVidaService
{
    Task<HojaDeVidaDto?> ObtenerAsync(string idUsuario);
    Task<HojaDeVidaDto> GuardarAsync(string idUsuario, string formacionAcademica, string experienciaLaboral, string competencias, string archivo);
}
