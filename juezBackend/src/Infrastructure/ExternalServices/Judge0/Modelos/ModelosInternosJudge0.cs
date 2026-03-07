using System.Text.Json.Serialization;

namespace Infrastructure.ExternalServices.Judge0.Modelos;

/// <summary>
/// Modelo de respuesta para la lista de lenguajes de Judge0.
/// </summary>
internal class RespuestaLenguajeJudge0
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("name")]
    public string Nombre { get; set; } = string.Empty;
}

/// <summary>
/// Modelo de respuesta al crear un envío.
/// </summary>
internal class RespuestaEnvioCreadoJudge0
{
    [JsonPropertyName("token")]
    public string Token { get; set; } = string.Empty;
}

/// <summary>
/// Modelo de respuesta al consultar un envío.
/// </summary>
internal class RespuestaVeredictoJudge0
{
    [JsonPropertyName("stdout")]
    public string? SalidaEstandar { get; set; }

    [JsonPropertyName("stderr")]
    public string? ErrorEjecucion { get; set; }

    [JsonPropertyName("compile_output")]
    public string? SalidaCompilacion { get; set; }

    [JsonPropertyName("time")]
    public string? Tiempo { get; set; }

    [JsonPropertyName("memory")]
    public int? Memoria { get; set; }

    [JsonPropertyName("status")]
    public EstadoJudge0? Estado { get; set; }
}

internal class EstadoJudge0
{
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("description")]
    public string Descripcion { get; set; } = string.Empty;
}
