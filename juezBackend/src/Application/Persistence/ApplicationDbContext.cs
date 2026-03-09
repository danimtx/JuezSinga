using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.Persistence;

/// <summary>
/// Contexto de la base de datos para la aplicación JuezSinGa.
/// </summary>
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<Problema> Problemas { get; set; }
    public DbSet<CasoDePrueba> CasosDePrueba { get; set; }
    public DbSet<Envio> Envios { get; set; }
    public DbSet<Lenguaje> Lenguajes { get; set; }
    public DbSet<DetalleEnvio> DetalleEnvios { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Competencia> Competencias { get; set; }
    public DbSet<CompetenciaProblema> CompetenciaProblemas { get; set; }
    public DbSet<CompetenciaParticipante> CompetenciaParticipantes { get; set; }
    public DbSet<CompetenciaGestor> CompetenciaGestores { get; set; }
    public DbSet<Aclaracion> Aclaraciones { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Usuario
        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.UserName).IsUnique();
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Apellidos).IsRequired().HasMaxLength(100);
            
            // Configuración de JSONB para Metadatos (EF Core 9)
            entity.OwnsOne(e => e.Metadatos, builder =>
            {
                builder.ToJson();
            });
        });

        // Configuración de Competencia
        modelBuilder.Entity<Competencia>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            
            entity.HasOne(e => e.Propietario)
                  .WithMany()
                  .HasForeignKey(e => e.PropietarioId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de CompetenciaProblema (Clave Compuesta)
        modelBuilder.Entity<CompetenciaProblema>(entity =>
        {
            entity.HasKey(e => new { e.CompetenciaId, e.ProblemaId });
            
            entity.HasOne(e => e.Competencia)
                  .WithMany(c => c.Problemas)
                  .HasForeignKey(e => e.CompetenciaId);

            entity.HasOne(e => e.Problema)
                  .WithMany()
                  .HasForeignKey(e => e.ProblemaId);
        });

        // Configuración de CompetenciaParticipante (Clave Compuesta)
        modelBuilder.Entity<CompetenciaParticipante>(entity =>
        {
            entity.HasKey(e => new { e.CompetenciaId, e.UsuarioId });

            entity.HasOne(e => e.Competencia)
                  .WithMany(c => c.Participantes)
                  .HasForeignKey(e => e.CompetenciaId);

            entity.HasOne(e => e.Usuario)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId);
        });

        // Configuración de CompetenciaGestor (Clave Compuesta)
        modelBuilder.Entity<CompetenciaGestor>(entity =>
        {
            entity.HasKey(e => new { e.CompetenciaId, e.UsuarioId });

            entity.HasOne(e => e.Competencia)
                  .WithMany(c => c.Gestores)
                  .HasForeignKey(e => e.CompetenciaId);

            entity.HasOne(e => e.Usuario)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId);
        });

        // Configuración de Aclaracion
        modelBuilder.Entity<Aclaracion>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasOne(e => e.Competencia)
                  .WithMany(c => c.Aclaraciones)
                  .HasForeignKey(e => e.CompetenciaId);

            entity.HasOne(e => e.Usuario)
                  .WithMany()
                  .HasForeignKey(e => e.UsuarioId);

            entity.HasOne(e => e.Problema)
                  .WithMany()
                  .HasForeignKey(e => e.ProblemaId)
                  .IsRequired(false);
        });

        // Configuración de Envio
        modelBuilder.Entity<Envio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Problema)
                  .WithMany(p => p.Envios)
                  .HasForeignKey(e => e.ProblemaId)
                  .OnDelete(DeleteBehavior.SetNull);

            // Relación con Usuario
            entity.HasOne(e => e.Usuario)
                  .WithMany(u => u.Envios)
                  .HasForeignKey(e => e.UsuarioId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);

            // Relación con Competencia
            entity.HasOne(e => e.Competencia)
                  .WithMany(c => c.Envios)
                  .HasForeignKey(e => e.CompetenciaId)
                  .IsRequired(false)
                  .OnDelete(DeleteBehavior.SetNull);
        });

        // Configuración de DetalleEnvio
        modelBuilder.Entity<DetalleEnvio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Envio)
                  .WithMany(e => e.DetalleEnvios)
                  .HasForeignKey(e => e.EnvioId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.CasoDePrueba)
                  .WithMany()
                  .HasForeignKey(e => e.CasoDePruebaId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuración de Lenguaje
        modelBuilder.Entity<Lenguaje>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(100);
        });
    }
}
