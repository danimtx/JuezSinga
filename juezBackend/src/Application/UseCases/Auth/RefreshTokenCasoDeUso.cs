using System.Security.Claims;
using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Auth;

public class RefreshTokenCasoDeUso(
    ApplicationDbContext context,
    ITokenService tokenService)
{
    public async Task<UsuarioRespuestaDto?> EjecutarAsync(RefreshTokenPeticionDto peticion)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(peticion.TokenExpirado);
        var username = principal?.Identity?.Name;

        if (string.IsNullOrEmpty(username)) return null;

        var usuario = await context.Usuarios
            .FirstOrDefaultAsync(u => u.UserName == username && u.EstaActivo);

        if (usuario == null || usuario.RefreshToken != peticion.RefreshToken || usuario.RefreshTokenExpiracion <= DateTime.UtcNow)
        {
            return null;
        }

        var nuevoToken = tokenService.GenerarToken(usuario);
        var nuevoRefreshToken = tokenService.GenerarRefreshToken();

        usuario.RefreshToken = nuevoRefreshToken;
        // La expiración del refresh token se puede renovar o mantener la original (Sliding vs Absolute)
        usuario.RefreshTokenExpiracion = DateTime.UtcNow.AddDays(7); 

        await context.SaveChangesAsync();

        return new UsuarioRespuestaDto(
            usuario.Id,
            usuario.UserName,
            usuario.Nombre,
            usuario.Apellidos,
            usuario.Rol,
            nuevoToken,
            nuevoRefreshToken
        );
    }
}
