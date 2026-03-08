using Domain.Enums;

namespace Application.DTOs.Auth;

public record LoginPeticionDto(string UserName, string Password);

public record UsuarioRespuestaDto(
    Guid Id,
    string UserName,
    string Nombre,
    string Apellidos,
    RolUsuario Rol,
    string Token,
    string RefreshToken
);

public record RefreshTokenPeticionDto(
    string TokenExpirado,
    string RefreshToken
);

public record RegistroEstudianteDto(
    string UserName,
    string Password,
    string Nombre,
    string Apellidos,
    string? Correo,
    string? Universidad,
    string? Departamento
);

public record CrearEquipoDto(
    string NombreEquipo,
    List<string> Integrantes,
    string? Universidad
);

public record CredencialesEquipoDto(
    string NombreEquipo,
    string UserName,
    string PasswordClaro
);
