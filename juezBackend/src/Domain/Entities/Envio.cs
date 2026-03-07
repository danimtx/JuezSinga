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

    public ICollection<DetalleEnvio> DetalleEnvios { get; set; } = new List<DetalleEnvio>();
}
