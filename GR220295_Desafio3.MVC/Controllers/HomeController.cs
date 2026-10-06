using GR220295_Desafio3.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace GR220295_Desafio3.MVC.Controllers;

public class HomeController(ApiClient api) : Controller
{
    public async Task<IActionResult> Index()
    {
        var plazas = await api.ObtenerPlazasAsync(true);
        return View(plazas);
    }

    public IActionResult Privacidad() => View();
}
