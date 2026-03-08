using Application.DTOs.Auth;
using Application.UseCases.Auth;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(
    LoginCasoDeUso loginCaso,
    RegistroEstudianteCasoDeUso registroCaso) : ControllerBase
{
    /// <summary>
    /// Inicia sesión en el sistema y devuelve un token JWT.
    /// </summary>
    [HttpPost("Login")]
    [ProducesResponseType(typeof(UsuarioRespuestaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UsuarioRespuestaDto>> Login([FromBody] LoginPeticionDto peticion)
    {
        var respuesta = await loginCaso.EjecutarAsync(peticion);
        if (respuesta == null) return Unauthorized("Credenciales inválidas o cuenta inactiva.");
        return Ok(respuesta);
    }

    /// <summary>
    /// Renueva el token de acceso usando un Refresh Token válido.
    /// </summary>
    [HttpPost("RefreshToken")]
    [ProducesResponseType(typeof(UsuarioRespuestaDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<UsuarioRespuestaDto>> RefreshToken([FromBody] RefreshTokenPeticionDto peticion, [FromServices] RefreshTokenCasoDeUso refreshCaso)
    {
        var respuesta = await refreshCaso.EjecutarAsync(peticion);
        if (respuesta == null) return Unauthorized("Token o Refresh Token inválido.");
        return Ok(respuesta);
    }

    /// <summary>
    /// Registra a un nuevo estudiante en la plataforma.
    /// </summary>
    [HttpPost("Registro")]
    [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Guid>> Registro([FromBody] RegistroEstudianteDto dto)
    {
        try
        {
            var id = await registroCaso.EjecutarAsync(dto);
            return Ok(id);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
