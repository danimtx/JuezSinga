namespace Domain.Entities;

public class Envio
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string CodigoFuente { get; set; } = string.Empty;
    public int LenguajeId { get; set; } // ID numérico de Judge0
    public string? EntradaEstandar { get; set; }
    public string? Token { get; set; }
    public string? VeredictoGlobal { get; set; }

    public Guid? ProblemaId { get; set; }
    public Problema? Problema { get; set; }

    public Guid? UsuarioId { get; set; }
    public Usuario? Usuario { get; set; }

    public Guid? CompetenciaId { get; set; }
    public Competencia? Competencia { get; set; }

    public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;

    public ICollection<DetalleEnvio> DetalleEnvios { get; set; } = new List<DetalleEnvio>();
}
