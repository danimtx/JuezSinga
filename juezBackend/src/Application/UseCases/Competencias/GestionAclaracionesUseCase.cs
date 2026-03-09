using Application.DTOs.Competencias;
using Application.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Competencias;

public class GestionAclaracionesUseCase(
    ApplicationDbContext context,
    INotificacionService notificacionService)
{
    public async Task EnviarDudaAsync(Guid competenciaId, Guid usuarioId, EnviarAclaracionDto dto)
    {
        var aclaracion = new Aclaracion
        {
            CompetenciaId = competenciaId,
            UsuarioId = usuarioId,
            ProblemaId = dto.ProblemaId,
            Pregunta = dto.Pregunta,
            FechaCreacion = DateTime.UtcNow
        };

        context.Aclaraciones.Add(aclaracion);
        await context.SaveChangesAsync();

        // Notificar a los admins sobre la nueva duda
        await notificacionService.NotificarNuevaPreguntaAdminAsync(competenciaId, new { 
            aclaracionId = aclaracion.Id, 
            pregunta = aclaracion.Pregunta 
        });
    }

    public async Task ResponderDudaAsync(Guid aclaracionId, ResponderAclaracionDto dto)
    {
        var aclaracion = await context.Aclaraciones
            .Include(a => a.Usuario)
            .FirstOrDefaultAsync(a => a.Id == aclaracionId)
            ?? throw new KeyNotFoundException("Aclaración no encontrada");

        aclaracion.Respuesta = dto.Respuesta;
        aclaracion.EsGlobal = dto.EsGlobal;

        await context.SaveChangesAsync();

        // Notificar al equipo (o a todos si es global)
        await notificacionService.NotificarAclaracionAsync(
            aclaracion.CompetenciaId, 
            aclaracion.UsuarioId, 
            new { aclaracionId, respuesta = aclaracion.Respuesta, esGlobal = aclaracion.EsGlobal }, 
            aclaracion.EsGlobal);
    }

    public async Task CrearAclaracionGlobalAsync(Guid competenciaId, Guid adminId, CrearAclaracionGlobalDto dto)
    {
        var aclaracion = new Aclaracion
        {
            CompetenciaId = competenciaId,
            UsuarioId = adminId,
            ProblemaId = dto.ProblemaId,
            Pregunta = "AVISO DEL SISTEMA / ANNOUNCEMENT",
            Respuesta = dto.Mensaje,
            EsGlobal = true,
            FechaCreacion = DateTime.UtcNow
        };

        context.Aclaraciones.Add(aclaracion);
        await context.SaveChangesAsync();

        // Notificar a todos sobre el aviso global
        await notificacionService.NotificarAclaracionAsync(
            competenciaId, 
            null, 
            new { aclaracionId = aclaracion.Id, mensaje = aclaracion.Respuesta }, 
            true);
    }

    public async Task<IEnumerable<AclaracionDto>> ListarParaUsuarioAsync(Guid competenciaId, Guid usuarioId, bool esAdmin)
    {
        var query = context.Aclaraciones
            .Include(a => a.Usuario)
            .Include(a => a.Competencia)
            .ThenInclude(c => c.Problemas)
            .Where(a => a.CompetenciaId == competenciaId);

        if (!esAdmin)
        {
            // Un usuario normal solo ve las globales o sus propias preguntas
            query = query.Where(a => a.EsGlobal || a.UsuarioId == usuarioId);
        }

        var aclaraciones = await query
            .OrderByDescending(a => a.FechaCreacion)
            .ToListAsync();

        return aclaraciones.Select(a => new AclaracionDto(
            a.Id,
            a.UsuarioId,
            a.Usuario.UserName,
            a.Competencia.Problemas.FirstOrDefault(p => p.ProblemaId == a.ProblemaId)?.Letra,
            a.Pregunta,
            a.Respuesta,
            a.EsGlobal,
            a.FechaCreacion
        ));
    }
}
