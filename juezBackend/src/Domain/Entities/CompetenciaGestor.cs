using Domain.Enums;

namespace Domain.Entities;

public class CompetenciaGestor
{
    public Guid CompetenciaId { get; set; }
    public Competencia Competencia { get; set; } = null!;

    public Guid UsuarioId { get; set; }
    public Usuario Usuario { get; set; } = null!;

    public RolGestor Rol { get; set; }
}
