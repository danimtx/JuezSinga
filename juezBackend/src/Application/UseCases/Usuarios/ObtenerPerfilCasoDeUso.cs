using Application.DTOs.Usuarios;
using Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Usuarios;

public class ObtenerPerfilCasoDeUso(ApplicationDbContext context)
{
    public async Task<UsuarioPerfilDto?> EjecutarAsync(Guid usuarioId)
    {
        var usuario = await context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == usuarioId);

        if (usuario == null) return null;

        return new UsuarioPerfilDto(
            usuario.Id,
            usuario.UserName,
            usuario.Nombre,
            usuario.Apellidos,
            usuario.Correo,
            usuario.Rol,
            usuario.Metadatos,
            usuario.EstaActivo,
            usuario.FechaRegistro
        );
    }
}
