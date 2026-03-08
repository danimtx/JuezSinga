using Domain.Enums;

namespace Domain.Entities;

public class Usuario
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string UserName { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public RolUsuario Rol { get; set; }
    
    public string Nombre { get; set; } = string.Empty;
    public string Apellidos { get; set; } = string.Empty;
    public string? Correo { get; set; }
    
    /// <summary>
    /// Propiedad que EF Core 9 mapeará automáticamente a JSONB.
    /// </summary>
    public MetadatosUsuario Metadatos { get; set; } = new();

    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiracion { get; set; }

    public bool EstaActivo { get; set; } = true;
    public DateTime FechaRegistro { get; set; } = DateTime.UtcNow;

    // Relación con los envíos
    public ICollection<Envio> Envios { get; set; } = new List<Envio>();
}
