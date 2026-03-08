using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Auth;

public class LoginCasoDeUso(
    ApplicationDbContext context,
    IPasswordHasher passwordHasher,
    ITokenService tokenService)
{
    public async Task<UsuarioRespuestaDto?> EjecutarAsync(LoginPeticionDto peticion)
    {
        var usuario = await context.Usuarios
            .FirstOrDefaultAsync(u => u.UserName == peticion.UserName && u.EstaActivo);

        if (usuario == null || !passwordHasher.Verify(peticion.Password, usuario.PasswordHash))
        {
            return null;
        }

        var token = tokenService.GenerarToken(usuario);
        var refreshToken = tokenService.GenerarRefreshToken();

        usuario.RefreshToken = refreshToken;
        usuario.RefreshTokenExpiracion = DateTime.UtcNow.AddDays(7); // Refresh token válido por 7 días

        await context.SaveChangesAsync();

        return new UsuarioRespuestaDto(
            usuario.Id,
            usuario.UserName,
            usuario.Nombre,
            usuario.Apellidos,
            usuario.Rol,
            token,
            refreshToken
        );
    }
}
