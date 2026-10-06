using Xunit;
using GR220295_Desafio3.BLL.Services;
using GR220295_Desafio3.DAL.Repositories;
using GR220295_Desafio3.Entities.Dtos;
using GR220295_Desafio3.Entities.Models;
using GR220295_Desafio3.Tests.Services;

namespace GR220295_Desafio3.Tests;

public class PlazaServiceTests
{
    [Fact]
    public async Task CrearPlaza_Valida_AumentaConteo()
    {
        await using var contexto = ContextoPruebas.Crear();
        var agente = new Usuario
        { Id = "agente-1", UserName = "agente@prueba.local", Email = "agente@prueba.local", NombreCompleto = "Agente", Rol = "Agente de Selección" };
        contexto.Users.Add(agente);
        await contexto.SaveChangesAsync();

        var servicio = new PlazaService(new PlazaRepository(contexto));
        var antes = contexto.Plazas.Count();

        var resultado = await servicio.CrearAsync(new CrearPlazaDto
        {
            Titulo = "Nueva plaza",
            Descripcion = "Descripción de prueba",
            Institucion = "Institución de prueba",
            FechaPublicacion = DateTime.Now,
            FechaCierre = DateTime.Now.AddDays(10)
        }, agente.Id);

        var despues = contexto.Plazas.Count();

        Assert.True(resultado.Exito);
        Assert.Equal(antes + 1, despues);
    }

    [Fact]
    public async Task CrearPlaza_FechaCierreAnteriorFalla()
    {
        await using var contexto = ContextoPruebas.Crear();
        contexto.Users.Add(new Usuario
        { Id = "agente-1", UserName = "agente@prueba.local", Email = "agente@prueba.local", NombreCompleto = "Agente", Rol = "Agente de Selección" });
        await contexto.SaveChangesAsync();

        var servicio = new PlazaService(new PlazaRepository(contexto));
        var resultado = await servicio.CrearAsync(new CrearPlazaDto
        {
            Titulo = "Nueva plaza",
            Descripcion = "Descripción",
            Institucion = "Institución",
            FechaPublicacion = DateTime.Now.AddDays(5),
            FechaCierre = DateTime.Now
        }, "agente-1");

        Assert.False(resultado.Exito);
        Assert.Contains("anterior", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CrearPlaza_FechaPublicacionIgualACierreFalla()
    {
        await using var contexto = ContextoPruebas.Crear();
        contexto.Users.Add(new Usuario
        { Id = "agente-1", UserName = "agente@prueba.local", Email = "agente@prueba.local", NombreCompleto = "Agente", Rol = "Agente de Selección" });
        await contexto.SaveChangesAsync();

        var fecha = DateTime.Now;
        var servicio = new PlazaService(new PlazaRepository(contexto));
        var resultado = await servicio.CrearAsync(new CrearPlazaDto
        {
            Titulo = "Nueva plaza",
            Descripcion = "Descripción",
            Institucion = "Institución",
            FechaPublicacion = fecha,
            FechaCierre = fecha
        }, "agente-1");

        Assert.False(resultado.Exito);
    }
}
