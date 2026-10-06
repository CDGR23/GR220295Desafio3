using GR220295_Desafio3.BLL.Interfaces;
using GR220295_Desafio3.DAL.Interfaces;
using GR220295_Desafio3.Entities.Dtos;
using GR220295_Desafio3.Entities.Models;

namespace GR220295_Desafio3.BLL.Services;

public class HojaDeVidaService(IHojaDeVidaRepository repositorio) : IHojaDeVidaService
{
    public async Task<HojaDeVidaDto?> ObtenerAsync(string idUsuario)
    {
        var hoja = await repositorio.ObtenerPorUsuarioAsync(idUsuario);
        return hoja is null ? null : Mapear(hoja);
    }

    public async Task<HojaDeVidaDto> GuardarAsync(
        string idUsuario,
        string formacionAcademica,
        string experienciaLaboral,
        string competencias,
        string archivo)
    {
        if (string.IsNullOrWhiteSpace(formacionAcademica)) throw new ArgumentException("La formación académica es obligatoria.");
        if (string.IsNullOrWhiteSpace(experienciaLaboral)) throw new ArgumentException("La experiencia laboral es obligatoria.");
        if (string.IsNullOrWhiteSpace(competencias)) throw new ArgumentException("Las competencias son obligatorias.");

        var hoja = await repositorio.ObtenerPorUsuarioAsync(idUsuario);

        if (hoja is null)
        {
            hoja = new HojaDeVida
            {
                IdUsuario = idUsuario,
                FormacionAcademica = formacionAcademica.Trim(),
                ExperienciaLaboral = experienciaLaboral.Trim(),
                Competencias = competencias.Trim(),
                ArchivoCV = archivo
            };

            await repositorio.AgregarAsync(hoja);
        }
        else
        {
            hoja.FormacionAcademica = formacionAcademica.Trim();
            hoja.ExperienciaLaboral = experienciaLaboral.Trim();
            hoja.Competencias = competencias.Trim();
            if (!string.IsNullOrWhiteSpace(archivo)) hoja.ArchivoCV = archivo;
        }

        await repositorio.GuardarCambiosAsync();
        return Mapear(hoja);
    }

    private static HojaDeVidaDto Mapear(HojaDeVida hoja)
    {
        return new HojaDeVidaDto
        {
            IdCV = hoja.IdCV,
            IdUsuario = hoja.IdUsuario,
            FormacionAcademica = hoja.FormacionAcademica,
            ExperienciaLaboral = hoja.ExperienciaLaboral,
            Competencias = hoja.Competencias,
            ArchivoCV = hoja.ArchivoCV
        };
    }
}
