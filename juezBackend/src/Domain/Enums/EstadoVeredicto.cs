namespace Domain.Enums;

/// <summary>
/// Representa los estados posibles de un envío en el motor de evaluación.
/// Basado en los IDs de estado de Judge0.
/// </summary>
public enum EstadoVeredicto
{
    EnCola = 1,
    Procesando = 2,
    Aceptado = 3,
    RespuestaIncorrecta = 4,
    TiempoLimiteExcedido = 5,
    ErrorCompilacion = 6,
    ErrorEjecucionSigsegv = 7,
    ErrorEjecucionSigfpe = 8,
    ErrorEjecucionSigabrt = 9,
    ErrorEjecucionNnz = 10,
    ErrorEjecucionOtros = 11,
    MemoriaExcedida = 12,
    SalidaExcedida = 13,
    ErrorInterno = 14,
    ErrorEjecucionExecvp = 15
}
