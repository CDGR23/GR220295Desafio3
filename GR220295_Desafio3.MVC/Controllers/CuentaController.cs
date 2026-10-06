using System.Security.Claims;
using GR220295_Desafio3.Entities.Dtos;
using GR220295_Desafio3.MVC.Models;
using GR220295_Desafio3.MVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GR220295_Desafio3.MVC.Controllers;

public class CuentaController(ApiClient api) : Controller
{
    [AllowAnonymous]
    public IActionResult Login(string? retorno = null)
    {
        ViewBag.Retorno = retorno;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginViewModel modelo, string? retorno = null)
    {
        if (!ModelState.IsValid) return View(modelo);

        var resultado = await api.IniciarSesionAsync(new IniciarSesionDto
        {
            Email = modelo.Email,
            Password = modelo.Password
        });

        if (!resultado.Exito || resultado.Datos is null)
        {
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return View(modelo);
        }

        var datos = resultado.Datos;
        var reclamos = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, datos.Id),
            new(ClaimTypes.Name, datos.NombreCompleto),
            new(ClaimTypes.Email, datos.Email),
            new(ClaimTypes.Role, datos.Rol),
            new("Token", datos.Token),
            new("Rol", datos.Rol)
        };

        var identidad = new ClaimsIdentity(reclamos, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identidad);
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

        return !string.IsNullOrWhiteSpace(retorno) && Url.IsLocalUrl(retorno)
            ? Redirect(retorno)
            : RedirectToAction("Index", "Home");
    }


    [Authorize]
    public async Task<IActionResult> Perfil()
    {
        var perfil = await api.ObtenerPerfilAsync();
        if (perfil is null) return RedirectToAction(nameof(Login));

        return View(new PerfilViewModel
        {
            NombreCompleto = perfil.NombreCompleto,
            Email = perfil.Email,
            Rol = perfil.Rol
        });
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Perfil(PerfilViewModel modelo)
    {
        if (!ModelState.IsValid) return View(modelo);

        var resultado = await api.ActualizarPerfilAsync(new ActualizarPerfilDto
        {
            NombreCompleto = modelo.NombreCompleto,
            Telefono = modelo.Telefono
        });

        if (!resultado.Exito)
        {
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return View(modelo);
        }

        if (resultado.Perfil is not null)
        {
            var token = User.FindFirst("Token")?.Value ?? string.Empty;
            var reclamos = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, resultado.Perfil.Id),
                new(ClaimTypes.Name, resultado.Perfil.NombreCompleto),
                new(ClaimTypes.Email, resultado.Perfil.Email),
                new(ClaimTypes.Role, resultado.Perfil.Rol),
                new("Token", token),
                new("Rol", resultado.Perfil.Rol)
            };
            var identidad = new ClaimsIdentity(reclamos, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identidad));
        }

        TempData["Mensaje"] = resultado.Mensaje;
        return RedirectToAction(nameof(Perfil));
    }

    [AllowAnonymous]
    public IActionResult Registro() => View();

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Registro(RegistroViewModel modelo)
    {
        if (modelo.Password != modelo.ConfirmarPassword) ModelState.AddModelError(nameof(modelo.ConfirmarPassword), "Las contraseñas no coinciden.");
        if (!ModelState.IsValid) return View(modelo);

        var resultado = await api.RegistrarAsync(new RegistrarUsuarioDto
        {
            NombreCompleto = modelo.NombreCompleto,
            Email = modelo.Email,
            Password = modelo.Password
        });

        if (!resultado.Exito)
        {
            ModelState.AddModelError(string.Empty, resultado.Mensaje);
            return View(modelo);
        }

        TempData["Mensaje"] = "Registro realizado. Ahora puede iniciar sesión.";
        return RedirectToAction(nameof(Login));
    }

    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Home");
    }

    public IActionResult Denegado() => View();

}
