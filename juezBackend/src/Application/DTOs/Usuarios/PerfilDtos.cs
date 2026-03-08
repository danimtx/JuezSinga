namespace Application.DTOs.Usuarios;

public record ActualizarPerfilDto(
    string Nombre,
    string Apellidos,
    string? Correo,
    string? Universidad,
    string? Departamento,
    string? Pais,
    string? FotoUrl
);

public record CambiarPasswordDto(
    string PasswordActual,
    string PasswordNueva
);
