using Application.DTOs.Envio;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Application.UseCases.Envios;

/// <summary>
/// Orquesta el proceso de enviar código fuente al motor de evaluación.
/// </summary>
public class ProcesarEnvioCasoDeUso(IServicioJuez servicioJuez)
{
    public async Task<CrearEnvioRespuestaDto> EjecutarAsync(CrearEnvioPeticionDto peticion)
    {
        var envio = new Envio
        {
            CodigoFuente = peticion.CodigoFuente,
            LenguajeId = peticion.LenguajeId,
            EntradaEstandar = peticion.EntradaEstandar
        };

        // Conversión de memoria si aplica
        int? memoriaKB = peticion.LimiteMemoria.HasValue 
            ? (peticion.Unidad == UnidadMemoria.Megabytes ? peticion.LimiteMemoria * 1024 : peticion.LimiteMemoria)
            : null;

        var token = await servicioJuez.CrearEnvioAsync(
            envio, 
            peticion.LimiteTiempo, 
            memoriaKB, 
            peticion.SalidaEsperada);
        
        return new CrearEnvioRespuestaDto(token);
    }
}
