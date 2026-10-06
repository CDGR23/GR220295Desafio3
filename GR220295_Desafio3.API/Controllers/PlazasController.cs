using System.Security.Claims;
using GR220295_Desafio3.BLL.Interfaces;
using GR220295_Desafio3.Entities.Constantes;
using GR220295_Desafio3.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR220295_Desafio3.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlazasController(IPlazaService servicio) : ControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Obtener([FromQuery] bool soloDisponibles = false)
        => Ok(await servicio.ObtenerPlazasAsync(soloDisponibles));

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> Obtener(int id)
    {
        var plaza = await servicio.ObtenerPlazaAsync(id);
        return plaza is null ? NotFound("La plaza no existe.") : Ok(plaza);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Agente)]
    public async Task<IActionResult> Crear(CrearPlazaDto dto)
    {
        var idAgente = ObtenerIdUsuario();
        if (idAgente is null) return Unauthorized();

        var resultado = await servicio.CrearAsync(dto, idAgente);
        return resultado.Exito
            ? CreatedAtAction(nameof(Obtener), new { id = resultado.Plaza!.IdPlaza }, resultado.Plaza)
            : BadRequest(resultado.Mensaje);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = Roles.Agente)]
    public async Task<IActionResult> Actualizar(int id, ActualizarPlazaDto dto)
    {
        var idAgente = ObtenerIdUsuario();
        if (idAgente is null) return Unauthorized();

        var resultado = await servicio.ActualizarAsync(id, dto, idAgente);
        return resultado.Exito ? Ok(resultado.Plaza) : BadRequest(resultado.Mensaje);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = Roles.Agente)]
    public async Task<IActionResult> Eliminar(int id)
    {
        var idAgente = ObtenerIdUsuario();
        if (idAgente is null) return Unauthorized();

        var resultado = await servicio.EliminarAsync(id, idAgente);
        return resultado.Exito ? NoContent() : BadRequest(resultado.Mensaje);
    }

    [HttpPut("{id}/publicar")]
    [Authorize(Roles = Roles.Agente)]
    public async Task<IActionResult> Publicar(int id)
    {
        var idAgente = ObtenerIdUsuario();
        if (idAgente is null) return Unauthorized();

        var resultado = await servicio.PublicarAsync(id, idAgente);
        return resultado.Exito ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
    }

    [HttpPut("{id}/estado")]
    [Authorize(Roles = "Administrador," + Roles.Agente)]
    public async Task<IActionResult> CambiarEstado(int id, EstadoPlazaDto dto)
    {
        var idUsuario = ObtenerIdUsuario();
        if (idUsuario is null) return Unauthorized();

        var esAdministrador = User.IsInRole(Roles.Administrador);
        var resultado = await servicio.CambiarEstadoAsync(id, dto.Estado, idUsuario, esAdministrador);
        return resultado.Exito ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
    }

    private string? ObtenerIdUsuario() => User.FindFirstValue(ClaimTypes.NameIdentifier);
}
