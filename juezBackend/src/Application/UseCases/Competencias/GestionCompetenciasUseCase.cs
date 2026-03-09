using Application.DTOs.Competencias;
using Application.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Competencias;

public class GestionCompetenciasUseCase(ApplicationDbContext context)
{
    public async Task<Guid> CrearAsync(CrearCompetenciaDto dto, Guid propietarioId)
    {
        var competencia = new Competencia
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            FechaInicio = dto.FechaInicio.ToUniversalTime(),
            FechaFin = dto.FechaFin.ToUniversalTime(),
            FechaCongelamiento = dto.FechaCongelamiento?.ToUniversalTime(),
            VerVeredictoDuranteFreeze = dto.VerVeredictoDuranteFreeze,
            EsPublica = dto.EsPublica,
            PropietarioId = propietarioId
        };

        context.Competencias.Add(competencia);
        await context.SaveChangesAsync();
        return competencia.Id;
    }

    public async Task<IEnumerable<CompetenciaResumenDto>> ListarDisponiblesAsync()
    {
        var ahora = DateTime.UtcNow;
        var competencias = await context.Competencias
            .OrderByDescending(c => c.FechaInicio)
            .ToListAsync();

        return competencias.Select(c => new CompetenciaResumenDto(
            c.Id,
            c.Titulo,
            c.FechaInicio,
            c.FechaFin,
            c.EsPublica,
            CalcularEstado(c, ahora)
        ));
    }

    public async Task<CompetenciaDetalleDto?> ObtenerDetalleAsync(Guid id)
    {
        var competencia = await context.Competencias
            .Include(c => c.Problemas)
            .ThenInclude(cp => cp.Problema)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (competencia == null) return null;

        var ahora = DateTime.UtcNow;
        var puedeVerProblemas = ahora >= competencia.FechaInicio;

        return new CompetenciaDetalleDto(
            competencia.Id,
            competencia.Titulo,
            competencia.Descripcion,
            competencia.FechaInicio,
            competencia.FechaFin,
            competencia.FechaCongelamiento,
            competencia.VerVeredictoDuranteFreeze,
            competencia.EsPublica,
            puedeVerProblemas 
                ? competencia.Problemas.OrderBy(p => p.Letra).Select(p => new ProblemaContestDto(p.ProblemaId, p.Problema.Titulo, p.Letra, p.ColorGlobo))
                : Enumerable.Empty<ProblemaContestDto>()
        );
    }

    public async Task AsignarProblemasAsync(Guid competenciaId, List<AsignarProblemaDto> problemasDto)
    {
        var competencia = await context.Competencias
            .Include(c => c.Problemas)
            .FirstOrDefaultAsync(c => c.Id == competenciaId)
            ?? throw new KeyNotFoundException("Competencia no encontrada");

        // Limpiar problemas anteriores
        context.CompetenciaProblemas.RemoveRange(competencia.Problemas);

        foreach (var p in problemasDto)
        {
            context.CompetenciaProblemas.Add(new CompetenciaProblema
            {
                CompetenciaId = competenciaId,
                ProblemaId = p.ProblemaId,
                Letra = p.Letra,
                ColorGlobo = p.ColorGlobo
            });
        }

        await context.SaveChangesAsync();
    }

    private string CalcularEstado(Competencia c, DateTime ahora)
    {
        if (ahora < c.FechaInicio) return "Programada";
        if (ahora > c.FechaFin) return "Finalizada";
        return "En Curso";
    }
}
