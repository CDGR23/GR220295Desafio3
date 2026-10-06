using System.Security.Claims;
using GR220295_Desafio3.BLL.Interfaces;
using GR220295_Desafio3.Entities.Constantes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR220295_Desafio3.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class HojasDeVidaController(
    IHojaDeVidaService servicio,
    IWebHostEnvironment entorno) : ControllerBase
{
    [HttpGet("mia")]
    [Authorize(Roles = Roles.Candidato)]
    public async Task<IActionResult> ObtenerMia()
    {
        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idUsuario is null) return Unauthorized();
        var hoja = await servicio.ObtenerAsync(idUsuario);
        return hoja is null ? NotFound("Todavía no ha registrado una hoja de vida.") : Ok(hoja);
    }

    [HttpGet("{idUsuario}")]
    [Authorize(Roles = "Administrador," + Roles.Agente)]
    public async Task<IActionResult> ObtenerPorUsuario(string idUsuario)
    {
        var hoja = await servicio.ObtenerAsync(idUsuario);
        return hoja is null ? NotFound("No se encontró la hoja de vida.") : Ok(hoja);
    }

    [HttpPost]
    [Authorize(Roles = Roles.Candidato)]
    [RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> Guardar([FromForm] FormularioHojaDeVida formulario)
    {
        var idUsuario = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (idUsuario is null) return Unauthorized();

        try
        {
            var archivo = await GuardarArchivoAsync(formulario.ArchivoCV);
            var hoja = await servicio.GuardarAsync(
                idUsuario,
                formulario.FormacionAcademica,
                formulario.ExperienciaLaboral,
                formulario.Competencias,
                archivo);

            return Ok(hoja);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut]
    [Authorize(Roles = Roles.Candidato)]
    [RequestSizeLimit(5_000_000)]
    public async Task<IActionResult> Actualizar([FromForm] FormularioHojaDeVida formulario)
    {
        return await Guardar(formulario);
    }

    [HttpGet("archivo/{idUsuario}")]
    [Authorize(Roles = "Administrador," + Roles.Agente + "," + Roles.Candidato)]
    public async Task<IActionResult> DescargarArchivo(string idUsuario)
    {
        var idActual = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var puedeVer = User.IsInRole(Roles.Administrador) || User.IsInRole(Roles.Agente) || idActual == idUsuario;
        if (!puedeVer) return Forbid();

        var hoja = await servicio.ObtenerAsync(idUsuario);
        if (hoja is null || string.IsNullOrWhiteSpace(hoja.ArchivoCV)) return NotFound("No hay un archivo CV registrado.");

        var ruta = Path.Combine(entorno.WebRootPath ?? Path.Combine(entorno.ContentRootPath, "wwwroot"), hoja.ArchivoCV);
        if (!System.IO.File.Exists(ruta)) return NotFound("El archivo CV no se encuentra en el servidor.");

        return PhysicalFile(ruta, "application/pdf", Path.GetFileName(ruta));
    }

    private async Task<string> GuardarArchivoAsync(IFormFile? archivo)
    {
        if (archivo is null || archivo.Length == 0) return string.Empty;

        var extension = Path.GetExtension(archivo.FileName);
        if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
        {
            throw new ArgumentException("El archivo CV debe estar en formato PDF.");
        }

        var carpeta = Path.Combine(entorno.WebRootPath ?? Path.Combine(entorno.ContentRootPath, "wwwroot"), "cv");
        Directory.CreateDirectory(carpeta);

        var nombreArchivo = $"cv_{Guid.NewGuid():N}.pdf";
        var ruta = Path.Combine(carpeta, nombreArchivo);

        await using var stream = System.IO.File.Create(ruta);
        await archivo.CopyToAsync(stream);
        return Path.Combine("cv", nombreArchivo).Replace("\\", "/");
    }
}

public class FormularioHojaDeVida
{
    public string FormacionAcademica { get; set; } = string.Empty;
    public string ExperienciaLaboral { get; set; } = string.Empty;
    public string Competencias { get; set; } = string.Empty;
    public IFormFile? ArchivoCV { get; set; }
}
