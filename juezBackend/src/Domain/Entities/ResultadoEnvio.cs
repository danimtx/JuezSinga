using Domain.Enums;

namespace Domain.Entities;

/// <summary>
/// Representa el resultado detallado de la ejecución de un envío.
/// </summary>
public class ResultadoEnvio
{
    public float? Tiempo { get; set; }
    public int? Memoria { get; set; }
    
    /// <summary>
    /// Salida estándar (stdout) de la ejecución, codificada en Base64.
    /// </summary>
    public string? SalidaEstandar { get; set; }
    
    /// <summary>
    /// Error de compilación o ejecución, codificado en Base64.
    /// </summary>
    public string? MensajeError { get; set; }
    
    public EstadoVeredicto Estado { get; set; }
}
