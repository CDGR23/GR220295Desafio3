using Xunit;
using System.Security.Claims;
using GR220295_Desafio3.API.Controllers;
using GR220295_Desafio3.BLL.Services;
using GR220295_Desafio3.DAL.Repositories;
using GR220295_Desafio3.Entities.Models;
using GR220295_Desafio3.Tests.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace GR220295_Desafio3.Tests;

public class PostulacionesControllerTests
{
    [Fact]
    public async Task Crear_SinIdUsuario_EstableceErrorDeNegocio()
    {
        await using var contexto = ContextoPruebas.Crear();
        contexto.Users.Add(new Usuario
        { Id = "candidato-1", UserName = "candidato@prueba.local", Email = "candidato@prueba.local", NombreCompleto = "Candidato", Rol = "Candidato" });
        await contexto.SaveChangesAsync();

        var controlador = new PostulacionesController(new PostulacionService(new PostulacionRepository(contexto)));
        controlador.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity())
            }
        };

        var resultado = await controlador.Crear(new GR220295_Desafio3.Entities.Dtos.CrearPostulacionDto { IdPlaza = 1 });

        Assert.IsType<BadRequestObjectResult>(resultado);
    }
}
