namespace Domain.Entities;

public class Aclaracion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid CompetenciaId { get; set; }
    public Competencia Competencia { get; set; } = null!;

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public Guid? ProblemaId { get; set; }
    public Problema? Problema { get; set; }

    public string Pregunta { get; set; } = string.Empty;
    public string? Respuesta { get; set; }
    
    public bool EsGlobal { get; set; } = false;
    public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
}
