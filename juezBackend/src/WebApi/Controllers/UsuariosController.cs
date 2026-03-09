using System.Security.Claims;
using Application.DTOs.Auth;
using Application.DTOs.Usuarios;
using Application.UseCases.Usuarios;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Domain.Enums;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController(
    ActualizarPerfilCasoDeUso actualizarPerfil,
    CambiarPasswordCasoDeUso cambiarPassword,
    ObtenerPerfilCasoDeUso obtenerPerfil,
    ListarUsuariosCasoDeUso listarUsuarios) : ControllerBase
{
    /// <summary>
    /// [Admin, Estudiante, Equipo] Obtiene los datos del perfil del usuario autenticado.
    /// </summary>
    [HttpGet("Perfil")]
    [Authorize]
    [ProducesResponseType(typeof(UsuarioPerfilDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UsuarioPerfilDto>> GetPerfil()
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var perfil = await obtenerPerfil.EjecutarAsync(userId);
        if (perfil == null) return NotFound("Usuario no encontrado.");
        return Ok(perfil);
    }

    /// <summary>
    /// [Solo Admin] Lista todos los usuarios con un filtro opcional por rol.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(IEnumerable<UsuarioPerfilDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UsuarioPerfilDto>>> Listar([FromQuery] RolUsuario? rol)
    {
        var usuarios = await listarUsuarios.EjecutarAsync(rol);
        return Ok(usuarios);
    }

    /// <summary>
    /// [Admin, Estudiante] Actualiza los datos del perfil del usuario autenticado.
    /// </summary>
    [HttpPut("Perfil")]
    [Authorize(Roles = "Admin,Estudiante")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> ActualizarPerfil([FromBody] ActualizarPerfilDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            await actualizarPerfil.EjecutarAsync(userId, dto);
            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    /// <summary>
    /// [Admin, Estudiante] Cambia la contraseña del usuario autenticado.
    /// </summary>
    [HttpPut("Perfil/Password")]
    [Authorize(Roles = "Admin,Estudiante")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult> CambiarPassword([FromBody] CambiarPasswordDto dto)
    {
        var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        try
        {
            await cambiarPassword.EjecutarAsync(userId, dto);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }
}
