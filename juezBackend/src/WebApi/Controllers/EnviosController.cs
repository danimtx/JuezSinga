using Application.DTOs.Envio;
using Application.UseCases.Envios;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EnviosController(
    ProcesarEnvioCasoDeUso procesarEnvio,
    EvaluarEnvioCompetenciaUseCase evaluarCompetencia,
    ConsultarResultadoConsolidadoUseCase consultarConsolidado,
    ConsultarVeredictoCasoDeUso consultarVeredicto) : ControllerBase
{
    /// <summary>
    /// [INTERNO/DEBUG] Envía código directamente con parámetros personalizados.
    /// </summary>
    /// <remarks>NO USAR EN EL FRONTEND. Solo para pruebas rápidas del motor.</remarks>
    [HttpPost("Prueba")]
    [ProducesResponseType(typeof(CrearEnvioRespuestaDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CrearEnvioRespuestaDto>> CrearPrueba([FromBody] CrearEnvioPeticionDto peticion)
    {
        var respuesta = await procesarEnvio.EjecutarAsync(peticion);
        return Ok(respuesta);
    }

    /// <summary>
    /// Modo Competencia: Inicia la evaluación de un código contra todos los casos de prueba.
    /// </summary>
    /// <param name="peticion">Contiene el ID del problema (Guid), código fuente y ID del lenguaje (int).</param>
    /// <returns>Retorna el ID (GUID) para el envío. Use este ID para consultar el resultado global.</returns>
    /// <response code="200">Envío recibido y comenzando evaluación.</response>
    /// <response code="404">Si el problema o el lenguaje no existen.</response>
    /// <response code="400">Si el problema no tiene casos de prueba configurados.</response>
    [HttpPost("Problema")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> CrearCompetencia([FromBody] CrearEnvioCompetenciaDto peticion)
    {
        try 
        {
            var idEnvio = await evaluarCompetencia.EjecutarAsync(peticion);
            return Ok(idEnvio);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Obtiene el veredicto consolidado de un envío (Modo Competencia).
    /// </summary>
    /// <param name="idEnvio">ID (GUID) retornado por el endpoint de creación.</param>
    /// <returns>Veredicto global en inglés (Accepted, Wrong Answer, etc.) y detalle de casos.</returns>
    [HttpGet("Resultado/{idEnvio}")]
    [ProducesResponseType(typeof(VeredictoCompetenciaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VeredictoCompetenciaDto>> ConsultarResultadoGlobal(Guid idEnvio)
    {
        var resultado = await consultarConsolidado.EjecutarAsync(idEnvio);
        if (resultado == null) return NotFound("No se encontró el envío.");
        return Ok(resultado);
    }

    /// <summary>
    /// [INTERNO/DEBUG] Consulta el veredicto de un token individual de Judge0.
    /// </summary>
    /// <remarks>NO USAR EN EL FRONTEND. Útil para verificar estados crudos.</remarks>
    [HttpGet("Token/{token}")]
    [ProducesResponseType(typeof(VeredictoRespuestaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VeredictoRespuestaDto>> ConsultarPorToken(string token)
    {
        var veredicto = await consultarVeredicto.EjecutarAsync(token);
        if (veredicto == null) return NotFound("No se encontró un envío con el token proporcionado.");
        return Ok(veredicto);
    }
}
