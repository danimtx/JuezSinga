using Application.DTOs.Usuarios;
using Application.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Usuarios;

public class ActualizarPerfilCasoDeUso(ApplicationDbContext context)
{
    public async Task EjecutarAsync(Guid usuarioId, ActualizarPerfilDto dto)
    {
        var usuario = await context.Usuarios.FindAsync(usuarioId) 
            ?? throw new KeyNotFoundException("Usuario no encontrado.");

        usuario.Nombre = dto.Nombre;
        usuario.Apellidos = dto.Apellidos;
        usuario.Correo = dto.Correo;
        
        // Actualizar Metadatos (JSONB)
        usuario.Metadatos.Universidad = dto.Universidad;
        usuario.Metadatos.Departamento = dto.Departamento;
        usuario.Metadatos.Pais = dto.Pais;
        usuario.Metadatos.FotoUrl = dto.FotoUrl;

        await context.SaveChangesAsync();
    }
}
