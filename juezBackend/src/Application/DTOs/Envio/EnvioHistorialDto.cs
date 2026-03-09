namespace Application.DTOs.Envio;

public record EnvioHistorialDto(
    Guid Id,
    Guid ProblemaId,
    string ProblemaTitulo,
    DateTime FechaEnvio,
    string VeredictoGlobal,
    int LenguajeId
);
