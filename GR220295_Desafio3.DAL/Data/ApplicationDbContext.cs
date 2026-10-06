using GR220295_Desafio3.Entities.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GR220295_Desafio3.DAL.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<Usuario>(options)
{
    public DbSet<Plaza> Plazas => Set<Plaza>();
    public DbSet<Postulacion> Postulaciones => Set<Postulacion>();
    public DbSet<HojaDeVida> HojasDeVida => Set<HojaDeVida>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entidad =>
        {
            entidad.Property(u => u.NombreCompleto).HasMaxLength(150);
            entidad.Property(u => u.Rol).HasMaxLength(50);
        });

        modelBuilder.Entity<Plaza>(entidad =>
        {
            entidad.HasKey(p => p.IdPlaza);
            entidad.Property(p => p.Titulo).IsRequired().HasMaxLength(200);
            entidad.Property(p => p.Descripcion).IsRequired().HasMaxLength(2000);
            entidad.Property(p => p.Institucion).IsRequired().HasMaxLength(200);
            entidad.Property(p => p.Estado).IsRequired().HasMaxLength(30);
            entidad.Property(p => p.IdAgente).IsRequired();
            entidad.HasIndex(p => p.IdAgente);
            entidad.HasOne(p => p.Agente)
                .WithMany(u => u.PlazasPublicadas)
                .HasForeignKey(p => p.IdAgente)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Postulacion>(entidad =>
        {
            entidad.HasKey(p => p.IdPostulacion);
            entidad.Property(p => p.IdCandidato).IsRequired();
            entidad.Property(p => p.Estado).IsRequired().HasMaxLength(30);
            entidad.HasIndex(p => new { p.IdPlaza, p.IdCandidato }).IsUnique();
            entidad.HasOne(p => p.Plaza)
                .WithMany(p => p.Postulaciones)
                .HasForeignKey(p => p.IdPlaza)
                .OnDelete(DeleteBehavior.Cascade);
            entidad.HasOne(p => p.Candidato)
                .WithMany(u => u.Postulaciones)
                .HasForeignKey(p => p.IdCandidato)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<HojaDeVida>(entidad =>
        {
            entidad.HasKey(h => h.IdCV);
            entidad.Property(h => h.IdUsuario).IsRequired();
            entidad.Property(h => h.FormacionAcademica).IsRequired().HasMaxLength(2000);
            entidad.Property(h => h.ExperienciaLaboral).IsRequired().HasMaxLength(3000);
            entidad.Property(h => h.Competencias).IsRequired().HasMaxLength(2000);
            entidad.Property(h => h.ArchivoCV).IsRequired().HasMaxLength(500);
            entidad.HasIndex(h => h.IdUsuario).IsUnique();
            entidad.HasOne(h => h.Usuario)
                .WithOne(u => u.HojaDeVida)
                .HasForeignKey<HojaDeVida>(h => h.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
