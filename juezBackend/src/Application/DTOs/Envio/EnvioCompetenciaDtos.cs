namespace Application.DTOs.Envio;

/// <summary>
/// Datos para realizar un envío vinculado a un problema específico (Modo Competencia).
/// </summary>
public record CrearEnvioCompetenciaDto(
    Guid ProblemaId,
    string CodigoFuente,
    int LenguajeId
);

/// <summary>
/// Resultado detallado de la evaluación de un problema con múltiples casos.
/// </summary>
public record VeredictoCompetenciaDto(
    string VeredictoGlobal,
    int CasosPasados,
    int TotalCasos,
    IEnumerable<string> DetalleTokens
);
