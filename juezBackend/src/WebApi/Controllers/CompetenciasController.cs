using System.Security.Claims;
using Application.DTOs.Auth;
using Application.DTOs.Competencias;
using Application.UseCases.Competencias;
using Application.UseCases.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CompetenciasController(
    GestionCompetenciasUseCase gestionCompetencias,
    GenerarEquiposMasivoCasoDeUso generarEquipos,
    CalcularScoreboardUseCase calcularScoreboard) : ControllerBase
{
    /// <summary>
    /// Crea una nueva competencia (Solo Admin).
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> Crear([FromBody] CrearCompetenciaDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var id = await gestionCompetencias.CrearAsync(dto, userId);
        return Ok(id);
    }

    /// <summary>
    /// Lista todas las competencias disponibles.
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CompetenciaResumenDto>>> Listar()
    {
        var resultados = await gestionCompetencias.ListarDisponiblesAsync();
        return Ok(resultados);
    }

    /// <summary>
    /// Obtiene el detalle de una competencia (Problemas visibles solo si ya inició).
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<CompetenciaDetalleDto>> Obtener(Guid id)
    {
        var detalle = await gestionCompetencias.ObtenerDetalleAsync(id);
        if (detalle == null) return NotFound();
        return Ok(detalle);
    }

    /// <summary>
    /// Asigna problemas a una competencia (Solo Admin).
    /// </summary>
    [HttpPost("{id}/problemas")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AsignarProblemas(Guid id, [FromBody] List<AsignarProblemaDto> problemas)
    {
        await gestionCompetencias.AsignarProblemasAsync(id, problemas);
        return NoContent();
    }

    /// <summary>
    /// Genera cuentas de equipo masivas e inscribe a la competencia (Solo Admin).
    /// </summary>
    [HttpPost("{id}/equipos/masivo")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<IEnumerable<CredencialesEquipoDto>>> GenerarEquipos(Guid id, [FromBody] List<CrearEquipoDto> equipos)
    {
        var resultados = await generarEquipos.EjecutarAsync(id, equipos);
        return Ok(resultados);
    }

    /// <summary>
    /// Obtiene el Scoreboard en tiempo real (Respetando el Freeze).
    /// </summary>
    [HttpGet("{id}/scoreboard")]
    public async Task<ActionResult<IEnumerable<FilaScoreboardDto>>> GetScoreboard(Guid id)
    {
        var ranking = await calcularScoreboard.EjecutarAsync(id);
        return Ok(ranking);
    }
}
