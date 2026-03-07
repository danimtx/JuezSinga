using Application.DTOs.Envio;
using Application.Interfaces;

namespace Application.UseCases.Envios;

/// <summary>
/// Orquesta la obtención del veredicto final a partir de un token.
/// </summary>
public class ConsultarVeredictoCasoDeUso(IServicioJuez servicioJuez)
{
    public async Task<VeredictoRespuestaDto?> EjecutarAsync(string token)
    {
        var resultado = await servicioJuez.ConsultarVeredictoAsync(token);
        
        if (resultado == null) return null;

        return new VeredictoRespuestaDto(
            SalidaEstandar: resultado.SalidaEstandar,
            MensajeError: resultado.MensajeError,
            Tiempo: resultado.Tiempo,
            Memoria: resultado.Memoria,
            Estado: resultado.Estado.ToString(),
            EstadoId: (int)resultado.Estado
        );
    }
}
