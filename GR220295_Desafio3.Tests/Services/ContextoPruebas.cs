using GR220295_Desafio3.DAL.Data;
using Microsoft.EntityFrameworkCore;

namespace GR220295_Desafio3.Tests.Services;

public static class ContextoPruebas
{
    public static ApplicationDbContext Crear()
    {
        var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(opciones);
    }
}
