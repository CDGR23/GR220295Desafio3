using GR220295_Desafio3.Entities.Dtos;
using GR220295_Desafio3.MVC.Models;
using GR220295_Desafio3.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR220295_Desafio3.MVC.Controllers;

public class PlazasController(ApiClient api) : Controller
{
    [AllowAnonymous]
    public async Task<IActionResult> Index()
    {
        var plazas = await api.ObtenerPlazasAsync();
        return View(plazas);
    }

    [AllowAnonymous]
    public async Task<IActionResult> Details(int id)
    {
        var plaza = await api.ObtenerPlazaAsync(id);
        return plaza is null ? NotFound() : View(plaza);
    }

    [Authorize(Roles = "Agente de Selección")]
    public IActionResult Crear() => View(new PlazaViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Agente de Selección")]
    public async Task<IActionResult> Crear(PlazaViewModel modelo)
    {
        if (modelo.FechaPublicacion >= modelo.FechaCierre)
        {
            ModelState.AddModelError(string.Empty, "La fecha de publicación debe ser anterior a la fecha de cierre.");
        }

        if (!ModelState.IsValid) return View(modelo);

        var resultado = await api.CrearPlazaAsync(new CrearPlazaDto
        {
            Titulo = modelo.Titulo,
            Descripcion = modelo.Descripcion,
            FechaPublicacion = modelo.FechaPublicacion,
            FechaCierre = modelo.FechaCierre,
            Institucion = modelo.Institucion
        });

        if (!resultado.Exito)
        {
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return View(modelo);
        }

        TempData["Mensaje"] = resultado.Mensaje;
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Agente de Selección")]
    public async Task<IActionResult> Editar(int id)
    {
        var plaza = await api.ObtenerPlazaAsync(id);
        if (plaza is null) return NotFound();
        return View(new PlazaViewModel
        {
            IdPlaza = plaza.IdPlaza,
            Titulo = plaza.Titulo,
            Descripcion = plaza.Descripcion,
            FechaPublicacion = plaza.FechaPublicacion,
            FechaCierre = plaza.FechaCierre,
            Institucion = plaza.Institucion
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Agente de Selección")]
    public async Task<IActionResult> Editar(PlazaViewModel modelo)
    {
        if (modelo.FechaPublicacion >= modelo.FechaCierre) ModelState.AddModelError(string.Empty, "La fecha de publicación debe ser anterior a la fecha de cierre.");
        if (!ModelState.IsValid) return View(modelo);

        var resultado = await api.ActualizarPlazaAsync(modelo.IdPlaza, new ActualizarPlazaDto
        {
            Titulo = modelo.Titulo,
            Descripcion = modelo.Descripcion,
            FechaPublicacion = modelo.FechaPublicacion,
            FechaCierre = modelo.FechaCierre,
            Institucion = modelo.Institucion
        });

        if (!resultado.Exito)
        {
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return View(modelo);
        }

        TempData["Mensaje"] = resultado.Mensaje;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Agente de Selección")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var resultado = await api.EliminarPlazaAsync(id);
        TempData[resultado.Exito ? "Mensaje" : "Error"] = resultado.Mensaje;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Agente de Selección,Administrador")]
    public async Task<IActionResult> CambiarEstado(int id, string estado)
    {
        var resultado = await api.CambiarEstadoPlazaAsync(id, estado);
        TempData[resultado.Exito ? "Mensaje" : "Error"] = resultado.Mensaje;
        return RedirectToAction(nameof(Details), new { id });
    }
}
