using Application.DTOs.Problema;
using Application.UseCases.Problemas;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProblemasController(GestionProblemasUseCase gestionProblemas) : ControllerBase
{
    /// <summary>
    /// Crea un nuevo problema con límites de tiempo y memoria.
    /// </summary>
    /// <param name="dto">Datos del problema. 
    /// - **limiteTiempo**: Segundos (ej: 1.0). 
    /// - **limiteMemoria**: Valor numérico.
    /// - **unidad**: 1 = Kilobytes (KB), 2 = Megabytes (MB).
    /// </param>
    /// <returns>El ID (GUID) del problema creado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    public async Task<ActionResult<Guid>> Crear([FromBody] CrearProblemaDto dto)
    {
        var id = await gestionProblemas.CrearProblemaAsync(dto);
        return Ok(id);
    }

    /// <summary>
    /// Lista el resumen de todos los problemas (ID y Título solamente).
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProblemaResumenDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ProblemaResumenDto>>> Listar()
    {
        var problemas = await gestionProblemas.ListarResumenAsync();
        return Ok(problemas);
    }

    /// <summary>
    /// Obtiene el detalle completo de un problema incluyendo casos de prueba públicos.
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ProblemaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProblemaDto>> Obtener(Guid id)
    {
        var problema = await gestionProblemas.ObtenerDetalleAsync(id);
        if (problema == null) return NotFound();
        return Ok(problema);
    }

    /// <summary>
    /// Actualiza los datos básicos de un problema.
    /// </summary>
    /// <param name="id">Guid del problema.</param>
    /// <param name="dto">Datos a actualizar.
    /// - **limiteTiempo**: Segundos (ej: 1.0). 
    /// - **limiteMemoria**: Valor numérico.
    /// - **unidad**: 1 = Kilobytes (KB), 2 = Megabytes (MB).
    /// </param>
    [HttpPut("{id}")]
    public async Task<ActionResult> Actualizar(Guid id, [FromBody] CrearProblemaDto dto)
    {
        try {
            await gestionProblemas.ActualizarProblemaAsync(id, dto);
            return NoContent();
        } catch (Exception ex) {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Elimina lógicamente un problema.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Eliminar(Guid id)
    {
        try {
            await gestionProblemas.BorrarLogicoAsync(id);
            return NoContent();
        } catch (Exception ex) {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Reemplaza todos los casos de prueba de un problema por una lista nueva.
    /// </summary>
    [HttpPost("{id}/casos/sincronizar")]
    public async Task<ActionResult> SincronizarCasos(Guid id, [FromBody] IEnumerable<CrearCasoDePruebaDto> casos)
    {
        try {
            await gestionProblemas.SincronizarCasosAsync(id, casos);
            return Ok();
        } catch (Exception ex) {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Agrega casos de prueba adicionales sin borrar los anteriores.
    /// </summary>
    [HttpPost("{id}/casos")]
    public async Task<ActionResult> AgregarCasos(Guid id, [FromBody] IEnumerable<CrearCasoDePruebaDto> casos)
    {
        try {
            await gestionProblemas.AgregarCasosDePruebaAsync(id, casos);
            return Ok();
        } catch (Exception ex) {
            return NotFound(ex.Message);
        }
    }
}
