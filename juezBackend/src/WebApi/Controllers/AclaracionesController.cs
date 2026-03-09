using System.Security.Claims;
using Application.DTOs.Competencias;
using Application.UseCases.Competencias;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AclaracionesController(GestionAclaracionesUseCase gestionAclaraciones) : ControllerBase
{
    /// <summary>
    /// Lista las aclaraciones de una competencia. Los equipos solo ven las globales o las suyas.
    /// </summary>
    [HttpGet("Competencia/{competenciaId}")]
    public async Task<ActionResult<IEnumerable<AclaracionDto>>> Listar(Guid competenciaId)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var esAdmin = User.IsInRole("Admin");
        
        var resultados = await gestionAclaraciones.ListarParaUsuarioAsync(competenciaId, userId, esAdmin);
        return Ok(resultados);
    }

    /// <summary>
    /// Envía una duda sobre un problema o sobre la competencia en general.
    /// </summary>
    [HttpPost("Competencia/{competenciaId}")]
    public async Task<ActionResult> Preguntar(Guid competenciaId, [FromBody] EnviarAclaracionDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await gestionAclaraciones.EnviarDudaAsync(competenciaId, userId, dto);
        return Ok();
    }

    /// <summary>
    /// Responde a una duda enviada por un equipo (Solo Admin).
    /// </summary>
    [HttpPut("{aclaracionId}/responder")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Responder(Guid aclaracionId, [FromBody] ResponderAclaracionDto dto)
    {
        try
        {
            await gestionAclaraciones.ResponderDudaAsync(aclaracionId, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// Emite un aviso global proactivo para todos los competidores (Solo Admin).
    /// </summary>
    [HttpPost("Competencia/{competenciaId}/Global")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> EmitirAvisoGlobal(Guid competenciaId, [FromBody] CrearAclaracionGlobalDto dto)
    {
        var adminId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        await gestionAclaraciones.CrearAclaracionGlobalAsync(competenciaId, adminId, dto);
        return Ok();
    }
}
