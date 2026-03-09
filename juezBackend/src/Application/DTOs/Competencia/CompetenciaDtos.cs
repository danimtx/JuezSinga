using Domain.Enums;

namespace Application.DTOs.Competencias;

public record CrearCompetenciaDto(
    string Titulo,
    string Descripcion,
    DateTime FechaInicio,
    DateTime FechaFin,
    DateTime? FechaCongelamiento,
    bool VerVeredictoDuranteFreeze,
    bool EsPublica
);

public record CompetenciaResumenDto(
    Guid Id,
    string Titulo,
    DateTime FechaInicio,
    DateTime FechaFin,
    bool EsPublica,
    string Estado // Programada, En Curso, Finalizada
);

public record CompetenciaDetalleDto(
    Guid Id,
    string Titulo,
    string Descripcion,
    DateTime FechaInicio,
    DateTime FechaFin,
    DateTime? FechaCongelamiento,
    bool VerVeredictoDuranteFreeze,
    bool EsPublica,
    IEnumerable<ProblemaContestDto> Problemas
);

public record ProblemaContestDto(
    Guid ProblemaId,
    string Titulo,
    string Letra,
    string? ColorGlobo
);

public record AsignarProblemaDto(
    Guid ProblemaId,
    string Letra,
    string? ColorGlobo
);
