using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Persistence;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Auth;

public class RegistroEstudianteCasoDeUso(
    ApplicationDbContext context,
    IPasswordHasher passwordHasher)
{
    public async Task<Guid> EjecutarAsync(RegistroEstudianteDto dto)
    {
        if (await context.Usuarios.AnyAsync(u => u.UserName == dto.UserName))
        {
            throw new Exception("El nombre de usuario ya existe.");
        }

        var usuario = new Usuario
        {
            UserName = dto.UserName,
            PasswordHash = passwordHasher.Hash(dto.Password),
            Rol = RolUsuario.Estudiante,
            Nombre = dto.Nombre,
            Apellidos = dto.Apellidos,
            Correo = dto.Correo,
            Metadatos = new MetadatosUsuario
            {
                Universidad = dto.Universidad,
                Departamento = dto.Departamento
            }
        };

        context.Usuarios.Add(usuario);
        await context.SaveChangesAsync();

        return usuario.Id;
    }
}
