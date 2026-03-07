namespace Application.DTOs.Envio;

/// <summary>
/// Información detallada del veredicto retornado por el juez.
/// </summary>
public record VeredictoRespuestaDto(
    string? SalidaEstandar,
    string? MensajeError,
    float? Tiempo,
    int? Memoria,
    string Estado,
    int EstadoId
);
