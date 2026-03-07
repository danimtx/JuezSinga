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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuración de Problema
        modelBuilder.Entity<Problema>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Titulo).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Descripcion).IsRequired();
            entity.HasQueryFilter(p => !p.EsEliminado); // Filtro global para borrado lógico
        });

        // Configuración de CasoDePrueba
        modelBuilder.Entity<CasoDePrueba>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Problema)
                  .WithMany(p => p.CasosDePrueba)
                  .HasForeignKey(e => e.ProblemaId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuración de Envio
        modelBuilder.Entity<Envio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Problema)
                  .WithMany(p => p.Envios)
                  .HasForeignKey(e => e.ProblemaId)
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
