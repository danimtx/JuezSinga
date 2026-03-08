using System.Security.Claims;
using Application.DTOs.Envio;
using Application.UseCases.Envios;
using Domain.Enums;
using Microsoft.AspNetCore.Authorization;
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
    /// <returns>Retorna el ID (GUID) para el envío. Use este ID para consultar el resultado global.</returns>
    /// <response code="200">Envío recibido y comenzando evaluación.</response>
    /// <response code="404">Si el problema o el lenguaje no existen.</response>
    /// <response code="400">Si el problema no tiene casos de prueba configurados.</response>
    /// <response code="429">Límite de peticiones al motor de evaluación excedido (Cuota o Rate Limit).</response>
    [HttpPost("Problema")]
    [Authorize]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<Guid>> CrearCompetencia([FromBody] CrearEnvioCompetenciaDto peticion)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try 
        {
            var idEnvio = await evaluarCompetencia.EjecutarAsync(peticion, userId);
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
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        {
            return StatusCode(429, "El motor de evaluación ha alcanzado su límite. Intente más tarde.");
        }
    }

    /// <summary>
    /// Obtiene el veredicto consolidado de un envío (Modo Competencia).
    /// </summary>
    /// <param name="idEnvio">ID (GUID) retornado por el endpoint de creación.</param>
    /// <returns>Veredicto global en inglés (Accepted, Wrong Answer, etc.) y detalle de casos.</returns>
    [HttpGet("Resultado/{idEnvio}")]
    [Authorize]
    [ProducesResponseType(typeof(VeredictoCompetenciaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<VeredictoCompetenciaDto>> ConsultarResultadoGlobal(Guid idEnvio)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var userRol = Enum.Parse<RolUsuario>(User.FindFirstValue(ClaimTypes.Role)!);

        try
        {
            var resultado = await consultarConsolidado.EjecutarAsync(idEnvio, userId, userRol);
            if (resultado == null) return NotFound("No se encontró el envío.");
            return Ok(resultado);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
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
