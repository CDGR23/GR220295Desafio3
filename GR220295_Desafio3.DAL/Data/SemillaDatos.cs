using GR220295_Desafio3.Entities.Constantes;
using GR220295_Desafio3.Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace GR220295_Desafio3.DAL.Data;

public static class SemillaDatos
{
    public static async Task InicializarAsync(
        ApplicationDbContext contexto,
        UserManager<Usuario> administradorUsuarios,
        RoleManager<IdentityRole> administradorRoles)
    {
        await contexto.Database.MigrateAsync();

        foreach (var nombreRol in Roles.Todos)
        {
            if (!await administradorRoles.RoleExistsAsync(nombreRol))
            {
                await administradorRoles.CreateAsync(new IdentityRole(nombreRol));
            }
        }

        var administrador = await CrearUsuarioAsync(
            administradorUsuarios,
            "Administrador del Sistema",
            "admin@desafio3.local",
            "Admin123!",
            Roles.Administrador);

        var agente = await CrearUsuarioAsync(
            administradorUsuarios,
            "Agente de Selección",
            "agente@desafio3.local",
            "Agente123!",
            Roles.Agente);

        var candidato = await CrearUsuarioAsync(
            administradorUsuarios,
            "Candidato de Prueba",
            "candidato@desafio3.local",
            "Candidato123!",
            Roles.Candidato);

        if (!await contexto.HojasDeVida.AnyAsync(h => h.IdUsuario == candidato.Id))
        {
            contexto.HojasDeVida.Add(new HojaDeVida
            {
                IdUsuario = candidato.Id,
                FormacionAcademica = "Ingeniería de Sistemas - Universidad de ejemplo",
                ExperienciaLaboral = "2 años de experiencia en desarrollo de software.",
                Competencias = "C#, .NET, SQL, Git y trabajo en equipo.",
                ArchivoCV = string.Empty
            });
            await contexto.SaveChangesAsync();
        }

        if (!await contexto.Plazas.AnyAsync())
        {
            contexto.Plazas.AddRange(
                new Plaza
                {
                    Titulo = "Desarrollador/a .NET",
                    Descripcion = "Desarrollo y mantenimiento de aplicaciones empresariales.",
                    FechaPublicacion = DateTime.Now.AddDays(-8),
                    FechaCierre = DateTime.Now.AddDays(15),
                    Institucion = "Institución Pública de Ejemplo",
                    Estado = EstadosPlaza.Abierta,
                    IdAgente = agente.Id
                },
                new Plaza
                {
                    Titulo = "Analista de Sistemas",
                    Descripcion = "Análisis de requerimientos y apoyo en proyectos tecnológicos.",
                    FechaPublicacion = DateTime.Now.AddDays(-20),
                    FechaCierre = DateTime.Now.AddDays(5),
                    Institucion = "Ministerio de Ejemplo",
                    Estado = EstadosPlaza.EnEvaluacion,
                    IdAgente = agente.Id
                },
                new Plaza
                {
                    Titulo = "Soporte de Aplicaciones",
                    Descripcion = "Atención y seguimiento de incidentes de aplicaciones institucionales.",
                    FechaPublicacion = DateTime.Now.AddDays(-45),
                    FechaCierre = DateTime.Now.AddDays(-10),
                    Institucion = "Institución Pública de Ejemplo",
                    Estado = EstadosPlaza.Cerrada,
                    IdAgente = agente.Id
                });

            await contexto.SaveChangesAsync();
        }

        var primeraPlaza = await contexto.Plazas.OrderBy(p => p.IdPlaza).FirstAsync();

        if (!await contexto.Postulaciones.AnyAsync(p => p.IdPlaza == primeraPlaza.IdPlaza && p.IdCandidato == candidato.Id))
        {
            contexto.Postulaciones.Add(new Postulacion
            {
                IdPlaza = primeraPlaza.IdPlaza,
                IdCandidato = candidato.Id,
                FechaPostulacion = DateTime.Now.AddDays(-2),
                Estado = EstadosPostulacion.EnRevision
            });
            await contexto.SaveChangesAsync();
        }
    }

    private static async Task<Usuario> CrearUsuarioAsync(
        UserManager<Usuario> administradorUsuarios,
        string nombre,
        string email,
        string password,
        string rol)
    {
        var usuario = await administradorUsuarios.FindByEmailAsync(email);

        if (usuario is null)
        {
            usuario = new Usuario
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                NombreCompleto = nombre,
                Rol = rol
            };

            var resultado = await administradorUsuarios.CreateAsync(usuario, password);
            if (!resultado.Succeeded)
            {
                var errores = string.Join(" | ", resultado.Errors.Select(e => e.Description));
                throw new Exception("No se pudo crear el usuario de semilla: " + errores);
            }
        }

        if (!await administradorUsuarios.IsInRoleAsync(usuario, rol))
        {
            await administradorUsuarios.AddToRoleAsync(usuario, rol);
        }

        if (usuario.Rol != rol)
        {
            usuario.Rol = rol;
            await administradorUsuarios.UpdateAsync(usuario);
        }

        return usuario;
    }
}
