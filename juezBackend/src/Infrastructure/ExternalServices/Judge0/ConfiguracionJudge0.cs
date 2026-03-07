namespace Infrastructure.ExternalServices.Judge0;

/// <summary>
/// Clase para mapear las configuraciones de Judge0 y RapidAPI.
/// </summary>
public class ConfiguracionJudge0
{
    public const string Seccion = "Judge0Settings";
    
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Host { get; set; } = string.Empty;
}
