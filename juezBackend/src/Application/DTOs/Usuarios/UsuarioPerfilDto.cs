using Domain.Enums;
using Domain.Entities;

namespace Application.DTOs.Usuarios;

public record UsuarioPerfilDto(
    Guid Id,
    string UserName,
    string Nombre,
    string Apellidos,
    string? Correo,
    RolUsuario Rol,
    MetadatosUsuario Metadatos,
    bool EstaActivo,
    DateTime FechaRegistro
);
