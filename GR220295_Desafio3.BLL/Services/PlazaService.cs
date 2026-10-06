using GR220295_Desafio3.BLL.Interfaces;
using GR220295_Desafio3.DAL.Interfaces;
using GR220295_Desafio3.Entities.Constantes;
using GR220295_Desafio3.Entities.Dtos;
using GR220295_Desafio3.Entities.Models;

namespace GR220295_Desafio3.BLL.Services;

public class PlazaService(IPlazaRepository repositorio) : IPlazaService
{
    public async Task<List<PlazaDto>> ObtenerPlazasAsync(bool soloDisponibles = false)
    {
        var plazas = await repositorio.ObtenerTodasAsync(soloDisponibles);
        return plazas.Select(Mapear).ToList();
    }

    public async Task<PlazaDto?> ObtenerPlazaAsync(int id)
    {
        var plaza = await repositorio.ObtenerPorIdAsync(id);
        return plaza is null ? null : Mapear(plaza);
    }

    public async Task<(bool Exito, string Mensaje, PlazaDto? Plaza)> CrearAsync(CrearPlazaDto dto, string idAgente)
    {
        var validacion = ValidarDatos(dto.Titulo, dto.Descripcion, dto.Institucion, dto.FechaPublicacion, dto.FechaCierre);
        if (!validacion.Exito) return (false, validacion.Mensaje, null);

        var agenteExiste = await repositorio.AgenteValidoAsync(idAgente);
        if (!agenteExiste) return (false, "El usuario actual no es un agente de selección válido.", null);

        var plaza = new Plaza
        {
            Titulo = dto.Titulo.Trim(),
            Descripcion = dto.Descripcion.Trim(),
            FechaPublicacion = dto.FechaPublicacion,
            FechaCierre = dto.FechaCierre,
            Institucion = dto.Institucion.Trim(),
            Estado = EstadosPlaza.Abierta,
            IdAgente = idAgente
        };

        await repositorio.AgregarAsync(plaza);
        await repositorio.GuardarCambiosAsync();

        return (true, "La plaza fue creada correctamente.", await ObtenerPlazaAsync(plaza.IdPlaza));
    }

    public async Task<(bool Exito, string Mensaje, PlazaDto? Plaza)> ActualizarAsync(int id, ActualizarPlazaDto dto, string idAgente)
    {
        var validacion = ValidarDatos(dto.Titulo, dto.Descripcion, dto.Institucion, dto.FechaPublicacion, dto.FechaCierre);
        if (!validacion.Exito) return (false, validacion.Mensaje, null);

        var plaza = await repositorio.ObtenerPorIdAsync(id);
        if (plaza is null) return (false, "La plaza no existe.", null);
        if (plaza.IdAgente != idAgente) return (false, "Solo el agente que publicó la plaza puede modificarla.", null);
        if (plaza.Estado == EstadosPlaza.Cerrada) return (false, "Una plaza cerrada no puede editarse.", null);

        plaza.Titulo = dto.Titulo.Trim();
        plaza.Descripcion = dto.Descripcion.Trim();
        plaza.FechaPublicacion = dto.FechaPublicacion;
        plaza.FechaCierre = dto.FechaCierre;
        plaza.Institucion = dto.Institucion.Trim();

        await repositorio.GuardarCambiosAsync();
        return (true, "La plaza fue actualizada correctamente.", await ObtenerPlazaAsync(plaza.IdPlaza));
    }

    public async Task<(bool Exito, string Mensaje)> EliminarAsync(int id, string idAgente)
    {
        var plaza = await repositorio.ObtenerPorIdAsync(id, true);
        if (plaza is null) return (false, "La plaza no existe.");
        if (plaza.IdAgente != idAgente) return (false, "Solo el agente que publicó la plaza puede eliminarla.");
        if (plaza.Postulaciones.Count > 0) return (false, "No se puede eliminar una plaza que ya tiene postulaciones.");

        repositorio.Eliminar(plaza);
        await repositorio.GuardarCambiosAsync();
        return (true, "La plaza fue eliminada correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> PublicarAsync(int id, string idAgente)
    {
        var plaza = await repositorio.ObtenerPorIdAsync(id);
        if (plaza is null) return (false, "La plaza no existe.");
        if (plaza.IdAgente != idAgente) return (false, "Solo el agente que publicó la plaza puede publicarla.");
        if (plaza.FechaCierre <= DateTime.Now) return (false, "La fecha de cierre ya pasó.");

        plaza.Estado = EstadosPlaza.Abierta;
        await repositorio.GuardarCambiosAsync();
        return (true, "La plaza fue publicada correctamente.");
    }

    public async Task<(bool Exito, string Mensaje)> CambiarEstadoAsync(int id, string estado, string idUsuario, bool esAdministrador)
    {
        if (!EstadosPlaza.Todos.Contains(estado)) return (false, "El estado de plaza no es válido.");

        var plaza = await repositorio.ObtenerPorIdAsync(id);
        if (plaza is null) return (false, "La plaza no existe.");
        if (!esAdministrador && plaza.IdAgente != idUsuario) return (false, "No tiene permisos para modificar esta plaza.");

        if (estado == EstadosPlaza.Abierta && plaza.FechaCierre <= DateTime.Now)
        {
            return (false, "No se puede abrir una plaza cuya fecha de cierre ya pasó.");
        }

        plaza.Estado = estado;
        await repositorio.GuardarCambiosAsync();
        return (true, "El estado de la plaza fue actualizado correctamente.");
    }

    private static PlazaDto Mapear(Plaza plaza)
    {
        return new PlazaDto
        {
            IdPlaza = plaza.IdPlaza,
            Titulo = plaza.Titulo,
            Descripcion = plaza.Descripcion,
            FechaPublicacion = plaza.FechaPublicacion,
            FechaCierre = plaza.FechaCierre,
            Institucion = plaza.Institucion,
            Estado = plaza.Estado,
            IdAgente = plaza.IdAgente,
            NombreAgente = plaza.Agente?.NombreCompleto ?? string.Empty
        };
    }

    private static (bool Exito, string Mensaje) ValidarDatos(
        string titulo,
        string descripcion,
        string institucion,
        DateTime fechaPublicacion,
        DateTime fechaCierre)
    {
        if (string.IsNullOrWhiteSpace(titulo)) return (false, "El título de la plaza es obligatorio.");
        if (string.IsNullOrWhiteSpace(descripcion)) return (false, "La descripción de la plaza es obligatoria.");
        if (string.IsNullOrWhiteSpace(institucion)) return (false, "La institución es obligatoria.");
        if (titulo.Length > 200) return (false, "El título no puede exceder 200 caracteres.");
        if (institucion.Length > 200) return (false, "La institución no puede exceder 200 caracteres.");
        if (fechaPublicacion >= fechaCierre) return (false, "La fecha de publicación debe ser anterior a la fecha de cierre.");
        return (true, string.Empty);
    }
}
