using Domain.Entities;

namespace Application.Interfaces;

/// <summary>
/// Interfaz para abstraer la comunicación con el motor de evaluación externo (Judge0).
/// </summary>
public interface IServicioJuez
{
    Task<IEnumerable<Lenguaje>> ObtenerLenguajesAsync();
    Task<string> CrearEnvioAsync(Envio envio, float? limiteTiempo = null, int? limiteMemoriaKB = null, string? salidaEsperada = null);
    Task<ResultadoEnvio?> ConsultarVeredictoAsync(string token);
}
