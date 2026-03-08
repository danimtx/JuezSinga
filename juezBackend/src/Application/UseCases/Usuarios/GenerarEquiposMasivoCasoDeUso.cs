using Application.DTOs.Auth;
using Application.Interfaces;
using Application.Persistence;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Usuarios;

public class GenerarEquiposMasivoCasoDeUso(
    ApplicationDbContext context,
    IPasswordHasher passwordHasher)
{
    private const string Alfabeto = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const string UserPrefix = "singaDMTX-";
    private const string PassPrefix = "TJ-";
    private readonly Random _random = new();

    public async Task<IEnumerable<CredencialesEquipoDto>> EjecutarAsync(List<CrearEquipoDto> equiposDto)
    {
        var resultados = new List<CredencialesEquipoDto>();

        foreach (var dto in equiposDto)
        {
            string hash = GenerarHashAleatorio(5);
            string user = $"{UserPrefix}{hash}";
            string rawPass = $"{PassPrefix}{GenerarHashAleatorio(5)}";

            var usuario = new Usuario
            {
                UserName = user,
                PasswordHash = passwordHasher.Hash(rawPass),
                Rol = RolUsuario.Equipo,
                Nombre = dto.NombreEquipo,
                Apellidos = "Equipo Competencia",
                Metadatos = new MetadatosUsuario
                {
                    NombreEquipo = dto.NombreEquipo,
                    Integrantes = dto.Integrantes,
                    Universidad = dto.Universidad
                }
            };

            context.Usuarios.Add(usuario);
            resultados.Add(new CredencialesEquipoDto(dto.NombreEquipo, user, rawPass));
        }

        await context.SaveChangesAsync();
        return resultados;
    }

    private string GenerarHashAleatorio(int longitud)
    {
        var chars = new char[longitud];
        for (int i = 0; i < longitud; i++)
        {
            chars[i] = Alfabeto[_random.Next(Alfabeto.Length)];
        }
        return new string(chars);
    }
}
