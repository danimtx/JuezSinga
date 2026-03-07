namespace Domain.Entities;

public class DetalleEnvio
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EnvioId { get; set; }
    public Envio Envio { get; set; } = null!;
    
    public Guid CasoDePruebaId { get; set; }
    public CasoDePrueba CasoDePrueba { get; set; } = null!;
    
    public string Token { get; set; } = string.Empty;
    public string? Veredicto { get; set; }
    public float? Tiempo { get; set; }
    public int? Memoria { get; set; }
}
