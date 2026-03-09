using Application.DTOs.Envio;
using Application.Persistence;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Envios;

public class ListarEnviosUseCase(ApplicationDbContext context)
{
    public async Task<IEnumerable<EnvioHistorialDto>> EjecutarAsync(Guid usuarioId, RolUsuario rol, Guid? competenciaId = null)
    {
        var query = context.Envios
            .Include(e => e.Problema)
            .AsQueryable();

        // 1. Filtro de Seguridad: Estudiantes y Equipos solo ven sus propios envíos
        if (rol != RolUsuario.Admin)
        {
            query = query.Where(e => e.UsuarioId == usuarioId);
        }

        // 2. Filtro opcional por competencia
        if (competenciaId.HasValue)
        {
            query = query.Where(e => e.CompetenciaId == competenciaId.Value);
        }

        var envios = await query
            .OrderByDescending(e => e.FechaEnvio)
            .ToListAsync();

        return envios.Select(e => new EnvioHistorialDto(
            e.Id,
            e.ProblemaId ?? Guid.Empty,
            e.Problema?.Titulo ?? "Problema Desconocido",
            e.FechaEnvio,
            e.VeredictoGlobal ?? "Processing",
            e.LenguajeId
        ));
    }
}
