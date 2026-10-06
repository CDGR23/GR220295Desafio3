using GR220295_Desafio3.BLL.Interfaces;
using GR220295_Desafio3.DAL.Interfaces;
using GR220295_Desafio3.Entities.Constantes;
using GR220295_Desafio3.Entities.Dtos;
using GR220295_Desafio3.Entities.Models;

namespace GR220295_Desafio3.BLL.Services;

public class PostulacionService(IPostulacionRepository repositorio) : IPostulacionService
{
    public async Task<List<PostulacionDto>> ObtenerMisPostulacionesAsync(string idCandidato)
    {
        var postulaciones = await repositorio.ObtenerPorCandidatoAsync(idCandidato);
        return postulaciones.Select(Mapear).ToList();
    }

    public async Task<List<PostulacionDto>> ObtenerTodasAsync()
    {
        var postulaciones = await repositorio.ObtenerTodasAsync();
        return postulaciones.Select(Mapear).ToList();
    }

    public async Task<List<PostulacionDto>> ObtenerPorPlazaAsync(int idPlaza)
    {
        var postulaciones = await repositorio.ObtenerPorPlazaAsync(idPlaza);
        return postulaciones.Select(Mapear).ToList();
    }

    public async Task<(bool Exito, string Mensaje, PostulacionDto? Postulacion)> CrearAsync(int idPlaza, string? idCandidato)
    {
        if (string.IsNullOrWhiteSpace(idCandidato))
        {
            return (false, "Debe iniciar sesión como candidato para postularse.", null);
        }

        var candidatoValido = await repositorio.CandidatoValidoAsync(idCandidato);
        if (!candidatoValido) return (false, "El usuario actual no es un candidato válido.", null);

        var plaza = await repositorio.ObtenerPlazaAsync(idPlaza);
        if (plaza is null) return (false, "La plaza no existe.", null);
        if (plaza.Estado != EstadosPlaza.Abierta) return (false, "La plaza no está disponible para postulaciones.", null);
        if (plaza.FechaCierre <= DateTime.Now) return (false, "La fecha de cierre de la plaza ya pasó.", null);

        var yaExiste = await repositorio.ExisteAsync(idPlaza, idCandidato);
        if (yaExiste) return (false, "El candidato ya se postuló a esta plaza.", null);

        var postulacion = new Postulacion
        {
            IdPlaza = idPlaza,
            IdCandidato = idCandidato,
            FechaPostulacion = DateTime.Now,
            Estado = EstadosPostulacion.EnRevision
        };

        await repositorio.AgregarAsync(postulacion);
        await repositorio.GuardarCambiosAsync();

        var resultado = await repositorio.ObtenerPorIdAsync(postulacion.IdPostulacion);
        return (true, "La postulación fue registrada correctamente.", resultado is null ? null : Mapear(resultado));
    }

    public async Task<(bool Exito, string Mensaje)> CambiarEstadoAsync(int idPostulacion, string estado, string idAgente, bool esAdministrador)
    {
        if (!EstadosPostulacion.Todos.Contains(estado)) return (false, "El estado de postulación no es válido.");

        var postulacion = await repositorio.ObtenerPorIdAsync(idPostulacion);
        if (postulacion is null) return (false, "La postulación no existe.");
        if (!esAdministrador && postulacion.Plaza?.IdAgente != idAgente)
        {
            return (false, "Solo el agente responsable de la plaza puede evaluar esta postulación.");
        }

        postulacion.Estado = estado;
        if (estado == EstadosPostulacion.Aprobado && postulacion.Plaza is not null)
        {
            postulacion.Plaza.Estado = EstadosPlaza.EnEvaluacion;
        }

        await repositorio.GuardarCambiosAsync();
        return (true, "El estado de la postulación fue actualizado correctamente.");
    }

    private static PostulacionDto Mapear(Postulacion postulacion)
    {
        return new PostulacionDto
        {
            IdPostulacion = postulacion.IdPostulacion,
            IdPlaza = postulacion.IdPlaza,
            TituloPlaza = postulacion.Plaza?.Titulo ?? string.Empty,
            IdCandidato = postulacion.IdCandidato,
            NombreCandidato = postulacion.Candidato?.NombreCompleto ?? string.Empty,
            FechaPostulacion = postulacion.FechaPostulacion,
            Estado = postulacion.Estado
        };
    }
}
