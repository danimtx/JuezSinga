using Domain.Enums;

namespace Application.DTOs.Problema;

public record CrearProblemaDto(
    string Titulo,
    string Descripcion,
    float LimiteTiempo,
    int LimiteMemoria,
    UnidadMemoria Unidad = UnidadMemoria.Kilobytes
);

public record ProblemaResumenDto(Guid Id, string Titulo);

public record ProblemaDto(
    Guid Id,
    string Titulo,
    string Descripcion,
    float LimiteTiempo,
    int LimiteMemoriaKB,
    IEnumerable<CasoDePruebaDto> CasosPublicos
);

public record CrearCasoDePruebaDto(
    string Entrada,
    string SalidaEsperada,
    bool EsOculto = true
);

public record CasoDePruebaDto(
    Guid Id,
    string Entrada,
    string SalidaEsperada,
    bool EsOculto
);
