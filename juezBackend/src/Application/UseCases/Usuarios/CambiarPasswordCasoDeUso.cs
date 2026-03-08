using Application.DTOs.Usuarios;
using Application.Interfaces;
using Application.Persistence;

namespace Application.UseCases.Usuarios;

public class CambiarPasswordCasoDeUso(
    ApplicationDbContext context, 
    IPasswordHasher passwordHasher)
{
    public async Task EjecutarAsync(Guid usuarioId, CambiarPasswordDto dto)
    {
        var usuario = await context.Usuarios.FindAsync(usuarioId) 
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        if (!passwordHasher.Verify(dto.PasswordActual, usuario.PasswordHash))
        {
            throw new UnauthorizedAccessException("La contraseña actual es incorrecta.");
        }

        usuario.PasswordHash = passwordHasher.Hash(dto.PasswordNueva);
        await context.SaveChangesAsync();
    }
}
