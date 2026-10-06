using GR220295_Desafio3.Entities.Constantes;
using GR220295_Desafio3.Entities.Dtos;
using GR220295_Desafio3.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR220295_Desafio3.MVC.Controllers;

[Authorize(Roles = Roles.Administrador)]
public class AdministradorController(ApiClient api) : Controller
{
    public async Task<IActionResult> Index()
    {
        var usuarios = await api.ObtenerUsuariosAsync();
        var postulaciones = await api.ObtenerPostulacionesAsync();
        ViewBag.Postulaciones = postulaciones;
        return View(usuarios);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarRol(string id, string rol)
    {
        var resultado = await api.CambiarRolAsync(id, rol);
        TempData[resultado.Exito ? "Mensaje" : "Error"] = resultado.Mensaje;
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Eliminar(string id)
    {
        var resultado = await api.EliminarUsuarioAsync(id);
        TempData[resultado.Exito ? "Mensaje" : "Error"] = resultado.Mensaje;
        return RedirectToAction(nameof(Index));
    }

    public IActionResult CrearUsuario() => View();

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CrearUsuario(RegistrarUsuarioDto modelo, string rol)
    {
        var resultado = await api.CrearUsuarioAdminAsync(modelo, rol);
        if (!resultado.Exito)
        {
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return View(modelo);
        }

        TempData["Mensaje"] = resultado.Mensaje;
        return RedirectToAction(nameof(Index));
    }
}
