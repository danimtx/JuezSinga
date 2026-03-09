using Application.DTOs.Lenguaje;
using Application.UseCases.Lenguajes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LenguajesController(ObtenerLenguajesCasoDeUso obtenerLenguajes) : ControllerBase
{
    /// <summary>
    /// Lista todos los lenguajes soportados por el motor de evaluación.
    /// </summary>
    /// <returns>Una lista de lenguajes disponibles con sus respectivos IDs.</returns>
    /// <response code="200">Retorna la lista de lenguajes exitosamente.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<LenguajeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<LenguajeDto>>> Listar()
    {
        var lenguajes = await obtenerLenguajes.EjecutarAsync();
        return Ok(lenguajes);
    }
}
