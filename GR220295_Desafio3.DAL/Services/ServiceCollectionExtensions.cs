using GR220295_Desafio3.DAL.Data;
using GR220295_Desafio3.DAL.Interfaces;
using GR220295_Desafio3.DAL.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GR220295_Desafio3.DAL.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AgregarCapaDatos(
        this IServiceCollection servicios,
        IConfiguration configuracion)
    {
        var cadenaConexion = configuracion.GetConnectionString("DefaultConnection")
            ?? "Data Source=GR220295_Desafio3.db";

        servicios.AddDbContext<ApplicationDbContext>(opciones =>
            opciones.UseSqlite(cadenaConexion, sqlite =>
                sqlite.MigrationsAssembly("GR220295_Desafio3.DAL")));

        servicios.AddScoped<IPlazaRepository, PlazaRepository>();
        servicios.AddScoped<IPostulacionRepository, PostulacionRepository>();
        servicios.AddScoped<IHojaDeVidaRepository, HojaDeVidaRepository>();

        return servicios;
    }
}
