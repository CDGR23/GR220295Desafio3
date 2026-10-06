using GR220295_Desafio3.DAL.Data;
using GR220295_Desafio3.DAL.Interfaces;
using GR220295_Desafio3.Entities.Constantes;
using GR220295_Desafio3.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace GR220295_Desafio3.DAL.Repositories;

public class PlazaRepository(ApplicationDbContext contexto) : IPlazaRepository
{
    public async Task<List<Plaza>> ObtenerTodasAsync(bool soloDisponibles = false)
    {
        var consulta = contexto.Plazas
            .Include(p => p.Agente)
            .AsQueryable();

        if (soloDisponibles)
        {
            consulta = consulta.Where(p => p.Estado == EstadosPlaza.Abierta && p.FechaCierre >= DateTime.Now);
        }

        return await consulta.OrderByDescending(p => p.FechaPublicacion).ToListAsync();
    }

    public async Task<Plaza?> ObtenerPorIdAsync(int id, bool incluirPostulaciones = false)
    {
        var consulta = contexto.Plazas.Include(p => p.Agente).AsQueryable();
        if (incluirPostulaciones) consulta = consulta.Include(p => p.Postulaciones);
        return await consulta.FirstOrDefaultAsync(p => p.IdPlaza == id);
    }

    public Task<bool> AgenteValidoAsync(string idAgente)
        => contexto.Users.AnyAsync(u => u.Id == idAgente && u.Rol == Roles.Agente);

    public async Task AgregarAsync(Plaza plaza) => await contexto.Plazas.AddAsync(plaza);

    public void Eliminar(Plaza plaza) => contexto.Plazas.Remove(plaza);

    public Task GuardarCambiosAsync() => contexto.SaveChangesAsync();
}
