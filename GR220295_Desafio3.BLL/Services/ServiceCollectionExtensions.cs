using GR220295_Desafio3.BLL.Interfaces;
using GR220295_Desafio3.BLL.Services;
using Microsoft.Extensions.DependencyInjection;

namespace GR220295_Desafio3.BLL.Services;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AgregarCapaNegocio(this IServiceCollection servicios)
    {
        servicios.AddScoped<IPlazaService, PlazaService>();
        servicios.AddScoped<IPostulacionService, PostulacionService>();
        servicios.AddScoped<IHojaDeVidaService, HojaDeVidaService>();
        return servicios;
    }
}
