using GR220295_Desafio3.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR220295_Desafio3.MVC.Controllers;

[Authorize]
public class PostulacionesController(ApiClient api) : Controller
{
    [Authorize(Roles = "Candidato")]
    public async Task<IActionResult> Index() => View(await api.ObtenerMisPostulacionesAsync());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Candidato")]
    public async Task<IActionResult> Postularse(int idPlaza)
    {
        var resultado = await api.CrearPostulacionAsync(idPlaza);
        TempData[resultado.Exito ? "Mensaje" : "Error"] = resultado.Mensaje;
        return RedirectToAction("Details", "Plazas", new { id = idPlaza });
    }

    [Authorize(Roles = "Agente de Selección,Administrador")]
    public async Task<IActionResult> Gestionar() => View(await api.ObtenerPostulacionesAsync());


    [Authorize(Roles = "Agente de Selección,Administrador")]
    public async Task<IActionResult> DescargarCv(string idUsuario)
    {
        var resultado = await api.DescargarCvAsync(idUsuario);
        return resultado.Archivo is null ? NotFound("No se encontró el CV.") : File(resultado.Archivo, "application/pdf", resultado.Nombre);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Agente de Selección,Administrador")]
    public async Task<IActionResult> CambiarEstado(int id, string estado)
    {
        var resultado = await api.CambiarEstadoPostulacionAsync(id, estado);
        TempData[resultado.Exito ? "Mensaje" : "Error"] = resultado.Mensaje;
        return RedirectToAction(nameof(Gestionar));
    }
}
