using Domain.Enums;

namespace Application.DTOs.Envio;

/// <summary>
/// Datos recibidos para realizar un nuevo envío al juez (Modo Prueba).
/// </summary>
public record CrearEnvioPeticionDto(
    /// <summary>Código fuente en texto plano (ej: "#include <iostream>...").</summary>
    string CodigoFuente,
    
    /// <summary>ID del lenguaje en Judge0 (ej: 54 para C++, 51 para C#, 71 para Python).</summary>
    int LenguajeId,
    
    /// <summary>Opcional: Entrada estándar para el programa. Enviar null o string vacía si no se requiere.</summary>
    string? EntradaEstandar = null,
    
    /// <summary>Opcional: Salida esperada para comparación automática. Enviar null si solo se desea ejecutar sin comparar.</summary>
    string? SalidaEsperada = null,
    
    /// <summary>Opcional: Límite de tiempo en segundos (ej: 1.5). Si es null, se usa el valor por defecto del juez.</summary>
    float? LimiteTiempo = null,
    
    /// <summary>Opcional: Límite de memoria (ej: 256). Si es null, se usa el valor por defecto del juez.</summary>
    int? LimiteMemoria = null,
    
    /// <summary>Unidad de la memoria: 1 = Kilobytes (KB), 2 = Megabytes (MB). Por defecto es 1.</summary>
    UnidadMemoria Unidad = UnidadMemoria.Kilobytes
);
