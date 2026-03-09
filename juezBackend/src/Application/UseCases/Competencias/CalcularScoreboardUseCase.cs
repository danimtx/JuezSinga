using Application.Persistence;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Competencias;

public class CalcularScoreboardUseCase(ApplicationDbContext context)
{
    public async Task<IEnumerable<FilaScoreboardDto>> EjecutarAsync(Guid competenciaId)
    {
        var competencia = await context.Competencias
            .Include(c => c.Participantes)
            .ThenInclude(p => p.Usuario)
            .FirstOrDefaultAsync(c => c.Id == competenciaId)
            ?? throw new KeyNotFoundException("Competencia no encontrada");

        var ahora = DateTime.UtcNow;
        var esFreeze = competencia.FechaCongelamiento.HasValue && ahora >= competencia.FechaCongelamiento.Value;

        // Obtener envíos válidos de la competencia
        var envios = await context.Envios
            .Where(e => e.CompetenciaId == competenciaId && e.FechaEnvio <= (esFreeze ? competencia.FechaCongelamiento : competencia.FechaFin))
            .ToListAsync();

        var scoreboard = new List<FilaScoreboardDto>();

        foreach (var participante in competencia.Participantes)
        {
            var enviosUsuario = envios.Where(e => e.UsuarioId == participante.UsuarioId).ToList();
            var problemasResueltos = 0;
            var penalizacionTotal = 0;

            // Agrupar por problema para calcular penalizaciones
            var enviosPorProblema = enviosUsuario.GroupBy(e => e.ProblemaId);

            foreach (var grupo in enviosPorProblema)
            {
                var aceptado = grupo.FirstOrDefault(e => e.VeredictoGlobal == "Accepted");
                if (aceptado != null)
                {
                    problemasResueltos++;
                    
                    // Minutos desde el inicio hasta el Accepted
                    var minutos = (int)(aceptado.FechaEnvio - competencia.FechaInicio).TotalMinutes;
                    
                    // Cantidad de intentos fallidos ANTES del Accepted (excluyendo Compilation Error)
                    var intentosFallidos = grupo.Count(e => 
                        e.FechaEnvio < aceptado.FechaEnvio && 
                        e.VeredictoGlobal != "Accepted" && 
                        e.VeredictoGlobal != "Compilation Error");

                    penalizacionTotal += minutos + (intentosFallidos * 20);
                }
            }

            scoreboard.Add(new FilaScoreboardDto(
                participante.Usuario.Nombre,
                participante.Usuario.UserName,
                problemasResueltos,
                penalizacionTotal
            ));
        }

        // Ordenar: 1. Más problemas, 2. Menos penalización
        return scoreboard
            .OrderByDescending(s => s.Resueltos)
            .ThenBy(s => s.Penalizacion);
    }
}

public record FilaScoreboardDto(
    string Nombre,
    string UserName,
    int Resueltos,
    int Penalizacion
);
