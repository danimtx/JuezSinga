using Domain.Entities;

namespace Domain.Entities;

public class Competencia
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public DateTime? FechaCongelamiento { get; set; }
    
    public bool VerVeredictoDuranteFreeze { get; set; } = true;
    public bool EsPublica { get; set; } = false;
    
    public Guid PropietarioId { get; set; }
    public Usuario Propietario { get; set; } = null!;

    // Relaciones
    public ICollection<CompetenciaProblema> Problemas { get; set; } = new List<CompetenciaProblema>();
    public ICollection<CompetenciaParticipante> Participantes { get; set; } = new List<CompetenciaParticipante>();
    public ICollection<CompetenciaGestor> Gestores { get; set; } = new List<CompetenciaGestor>();
    public ICollection<Aclaracion> Aclaraciones { get; set; } = new List<Aclaracion>();
    public ICollection<Envio> Envios { get; set; } = new List<Envio>();
}
