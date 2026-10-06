using GR220295_Desafio3.DAL.Data;
using GR220295_Desafio3.DAL.Interfaces;
using GR220295_Desafio3.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace GR220295_Desafio3.DAL.Repositories;

public class HojaDeVidaRepository(ApplicationDbContext contexto) : IHojaDeVidaRepository
{
    public Task<HojaDeVida?> ObtenerPorUsuarioAsync(string idUsuario)
        => contexto.HojasDeVida.FirstOrDefaultAsync(h => h.IdUsuario == idUsuario);

    public async Task AgregarAsync(HojaDeVida hoja) => await contexto.HojasDeVida.AddAsync(hoja);

    public Task GuardarCambiosAsync() => contexto.SaveChangesAsync();
}
