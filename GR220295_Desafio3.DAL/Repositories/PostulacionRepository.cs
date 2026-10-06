using GR220295_Desafio3.DAL.Data;
using GR220295_Desafio3.DAL.Interfaces;
using GR220295_Desafio3.Entities.Constantes;
using GR220295_Desafio3.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace GR220295_Desafio3.DAL.Repositories;

public class PostulacionRepository(ApplicationDbContext contexto) : IPostulacionRepository
{
    public Task<List<Postulacion>> ObtenerTodasAsync()
        => contexto.Postulaciones
            .Include(p => p.Plaza)
            .Include(p => p.Candidato)
            .OrderByDescending(p => p.FechaPostulacion)
            .ToListAsync();

    public Task<List<Postulacion>> ObtenerPorCandidatoAsync(string idCandidato)
        => contexto.Postulaciones
            .Include(p => p.Plaza)
            .Include(p => p.Candidato)
            .Where(p => p.IdCandidato == idCandidato)
            .OrderByDescending(p => p.FechaPostulacion)
            .ToListAsync();

    public Task<List<Postulacion>> ObtenerPorPlazaAsync(int idPlaza)
        => contexto.Postulaciones
            .Include(p => p.Plaza)
            .Include(p => p.Candidato)
            .Where(p => p.IdPlaza == idPlaza)
            .OrderByDescending(p => p.FechaPostulacion)
            .ToListAsync();

    public Task<Postulacion?> ObtenerPorIdAsync(int idPostulacion)
        => contexto.Postulaciones.Include(p => p.Plaza).FirstOrDefaultAsync(p => p.IdPostulacion == idPostulacion);

    public Task<bool> ExisteAsync(int idPlaza, string idCandidato)
        => contexto.Postulaciones.AnyAsync(p => p.IdPlaza == idPlaza && p.IdCandidato == idCandidato);

    public Task<bool> CandidatoValidoAsync(string idCandidato)
        => contexto.Users.AnyAsync(u => u.Id == idCandidato && u.Rol == Roles.Candidato);

    public Task<Plaza?> ObtenerPlazaAsync(int idPlaza)
        => contexto.Plazas.FirstOrDefaultAsync(p => p.IdPlaza == idPlaza);

    public async Task AgregarAsync(Postulacion postulacion) => await contexto.Postulaciones.AddAsync(postulacion);

    public Task GuardarCambiosAsync() => contexto.SaveChangesAsync();
}
