using Application.DTOs.Usuarios;
using Application.Persistence;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Usuarios;

public class ListarUsuariosCasoDeUso(ApplicationDbContext context)
{
    public async Task<IEnumerable<UsuarioPerfilDto>> EjecutarAsync(RolUsuario? filtroRol)
    {
        var query = context.Usuarios.AsQueryable();

        if (filtroRol.HasValue)
        {
            query = query.Where(u => u.Rol == filtroRol.Value);
        }

        var usuarios = await query
            .OrderByDescending(u => u.FechaRegistro)
            .ToListAsync();

        return usuarios.Select(u => new UsuarioPerfilDto(
            u.Id,
            u.UserName,
            u.Nombre,
            u.Apellidos,
            u.Correo,
            u.Rol,
            u.Metadatos,
            u.EstaActivo,
            u.FechaRegistro
        ));
    }
}
