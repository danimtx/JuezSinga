namespace Domain.Entities;

public class CompetenciaParticipante
{
    public Guid CompetenciaId { get; set; }
    public Competencia Competencia { get; set; } = null!;

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public DateTime FechaInscripcion { get; set; } = DateTime.UtcNow;
}
