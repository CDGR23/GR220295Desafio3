using GR220295_Desafio3.MVC.Models;
using GR220295_Desafio3.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR220295_Desafio3.MVC.Controllers;

[Authorize(Roles = "Candidato")]
public class HojaDeVidaController(ApiClient api) : Controller
{
    public async Task<IActionResult> Index()
    {
        var hoja = await api.ObtenerMiHojaDeVidaAsync();
        if (hoja is null) return View(new HojaDeVidaViewModel());

        return View(new HojaDeVidaViewModel
        {
            FormacionAcademica = hoja.FormacionAcademica,
            ExperienciaLaboral = hoja.ExperienciaLaboral,
            Competencias = hoja.Competencias,
            ArchivoExistente = hoja.ArchivoCV
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Guardar(HojaDeVidaViewModel modelo)
    {
        if (!ModelState.IsValid) return View("Index", modelo);

        var resultado = await api.GuardarHojaDeVidaAsync(modelo);
        TempData[resultado.Exito ? "Mensaje" : "Error"] = resultado.Mensaje;
        return RedirectToAction(nameof(Index));
    }
}
