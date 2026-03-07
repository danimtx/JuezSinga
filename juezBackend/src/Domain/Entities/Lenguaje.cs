namespace Domain.Entities;

public class Lenguaje
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// El ID numérico real que usa Judge0 (ej: 54 para C++).
    /// </summary>
    public int CodigoJudge0 { get; set; }
    
    public string Nombre { get; set; } = string.Empty;
}
