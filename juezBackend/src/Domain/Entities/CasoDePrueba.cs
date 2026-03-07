namespace Domain.Entities;

public class CasoDePrueba
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Entrada { get; set; } = string.Empty;
    public string SalidaEsperada { get; set; } = string.Empty;
    public bool EsOculto { get; set; }

    public Guid ProblemaId { get; set; }
    public Problema Problema { get; set; } = null!;
}
