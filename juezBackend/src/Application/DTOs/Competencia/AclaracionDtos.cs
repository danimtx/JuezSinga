namespace Application.DTOs.Competencias;

public record EnviarAclaracionDto(
    Guid? ProblemaId,
    string Pregunta
);

public record ResponderAclaracionDto(
    string Respuesta,
    bool EsGlobal
);

public record CrearAclaracionGlobalDto(
    Guid? ProblemaId,
    string Mensaje
);

public record AclaracionDto(
    Guid Id,
    Guid UsuarioId,
    string UserName,
    string? ProblemaLetra,
    string Pregunta,
    string? Respuesta,
    bool EsGlobal,
    DateTime FechaCreacion
);
