namespace Domain.Entities;

/// <summary>
/// Representa los datos flexibles almacenados como JSONB en la base de datos.
/// </summary>
public class MetadatosUsuario
{
    // Datos comunes
    public string? Universidad { get; set; }
    public string? Departamento { get; set; }
    public string? Pais { get; set; }
    public string? FotoUrl { get; set; }

    // Datos específicos de Equipo
    public string? NombreEquipo { get; set; }
    public List<string>? Integrantes { get; set; }
}
