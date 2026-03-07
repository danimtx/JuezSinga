namespace Domain.Entities;

public class Problema
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public float LimiteTiempo { get; set; }
    public int LimiteMemoria { get; set; }
    public bool EsEliminado { get; set; } = false;

    public ICollection<CasoDePrueba> CasosDePrueba { get; set; } = new List<CasoDePrueba>();
    public ICollection<Envio> Envios { get; set; } = new List<Envio>();
}
