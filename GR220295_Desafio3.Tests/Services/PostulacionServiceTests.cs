using Xunit;
using GR220295_Desafio3.BLL.Services;
using GR220295_Desafio3.DAL.Repositories;
using GR220295_Desafio3.Entities.Constantes;
using GR220295_Desafio3.Entities.Dtos;
using GR220295_Desafio3.Entities.Models;
using GR220295_Desafio3.Tests.Services;

namespace GR220295_Desafio3.Tests;

public class PostulacionServiceTests
{
    private static async Task PrepararAsync(GR220295_Desafio3.DAL.Data.ApplicationDbContext contexto)
    {
        contexto.Users.AddRange(
            new Usuario { Id = "agente-1", UserName = "agente@prueba.local", Email = "agente@prueba.local", NombreCompleto = "Agente", Rol = Roles.Agente },
            new Usuario { Id = "candidato-1", UserName = "candidato@prueba.local", Email = "candidato@prueba.local", NombreCompleto = "Candidato", Rol = Roles.Candidato });
        contexto.Plazas.Add(new Plaza
        {
            IdPlaza = 1, Titulo = "Plaza 1", Descripcion = "Descripción", Institucion = "Institución",
            FechaPublicacion = DateTime.Now.AddDays(-1), FechaCierre = DateTime.Now.AddDays(5), Estado = EstadosPlaza.Abierta, IdAgente = "agente-1"
        });
        await contexto.SaveChangesAsync();
    }

    [Fact]
    public async Task CrearPostulacion_SinUsuarioAutenticado_GeneraError()
    {
        await using var contexto = ContextoPruebas.Crear();
        await PrepararAsync(contexto);
        var servicio = new PostulacionService(new PostulacionRepository(contexto));

        var resultado = await servicio.CrearAsync(1, null);

        Assert.False(resultado.Exito);
        Assert.Contains("iniciar sesión", resultado.Mensaje, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task CrearPostulacion_DosVeces_MismoCandidatoFalla()
    {
        await using var contexto = ContextoPruebas.Crear();
        await PrepararAsync(contexto);
        var servicio = new PostulacionService(new PostulacionRepository(contexto));

        var primera = await servicio.CrearAsync(1, "candidato-1");
        var segunda = await servicio.CrearAsync(1, "candidato-1");

        Assert.True(primera.Exito);
        Assert.False(segunda.Exito);
        Assert.Contains("ya se postuló", segunda.Mensaje, StringComparison.OrdinalIgnoreCase);
    }
}
