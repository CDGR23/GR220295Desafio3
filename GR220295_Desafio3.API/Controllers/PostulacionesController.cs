using System.Security.Claims;
using GR220295_Desafio3.BLL.Interfaces;
using GR220295_Desafio3.Entities.Constantes;
using GR220295_Desafio3.Entities.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR220295_Desafio3.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PostulacionesController(IPostulacionService servicio) : ControllerBase
{
    [HttpPost]
    [Authorize(Roles = Roles.Candidato)]
    public async Task<IActionResult> Crear(CrearPostulacionDto dto)
    {
        var idCandidato = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var resultado = await servicio.CrearAsync(dto.IdPlaza, idCandidato);

        return resultado.Exito
            ? CreatedAtAction(nameof(ObtenerMisPostulaciones), null, resultado.Postulacion)
            : BadRequest(resultado.Mensaje);
    }

    [HttpGet("mias")]
    [Authorize(Roles = Roles.Candidato)]
    public async Task<IActionResult> ObtenerMisPostulaciones()
    {
        var idCandidato = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idCandidato is null) return Unauthorized();
        return Ok(await servicio.ObtenerMisPostulacionesAsync(idCandidato));
    }

    [HttpGet]
    [Authorize(Roles = "Administrador," + Roles.Agente)]
    public async Task<IActionResult> ObtenerTodas() => Ok(await servicio.ObtenerTodasAsync());

    [HttpGet("plaza/{idPlaza}")]
    [Authorize(Roles = "Administrador," + Roles.Agente)]
    public async Task<IActionResult> ObtenerPorPlaza(int idPlaza) => Ok(await servicio.ObtenerPorPlazaAsync(idPlaza));

    [HttpPut("{idPostulacion}/estado")]
    [Authorize(Roles = "Administrador," + Roles.Agente)]
    public async Task<IActionResult> CambiarEstado(int idPostulacion, CambiarEstadoPostulacionDto dto)
    {
        var idAgente = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idAgente is null) return Unauthorized();

        var resultado = await servicio.CambiarEstadoAsync(
            idPostulacion,
            dto.Estado,
            idAgente,
            User.IsInRole(Roles.Administrador));

        return resultado.Exito ? Ok(resultado.Mensaje) : BadRequest(resultado.Mensaje);
    }
}
