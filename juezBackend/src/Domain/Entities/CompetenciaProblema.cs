namespace Domain.Entities;

public class CompetenciaProblema
{
    public Guid CompetenciaId { get; set; }
    public Competencia Competencia { get; set; } = null!;

    public Guid ProblemaId { get; set; }
    public Problema Problema { get; set; } = null!;

    public string Letra { get; set; } = string.Empty; // A, B, C...
    public string? ColorGlobo { get; set; }
}
