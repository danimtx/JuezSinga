using Application.DTOs.Envio;
using Application.Interfaces;
using Application.Persistence;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Envios;

/// <summary>
/// VERSION DE PRODUCCIÓN (Sin Semáforos).
/// Úsela cuando el motor Judge0 CE esté corriendo en un servidor local/propio sin límites de API.
/// </summary>
public class EvaluarEnvioCompetenciaUseCaseSinSemaforo(
    IServicioJuez servicioJuez, 
    ApplicationDbContext context)
{
    public async Task<Guid> EjecutarAsync(CrearEnvioCompetenciaDto peticion, Guid usuarioId)
    {
        // 1. Validar existencia del Problema
        var problema = await context.Problemas
            .Include(p => p.CasosDePrueba)
            .FirstOrDefaultAsync(p => p.Id == peticion.ProblemaId)
            ?? throw new KeyNotFoundException($"El problema con ID {peticion.ProblemaId} no existe.");

        if (!problema.CasosDePrueba.Any())
            throw new InvalidOperationException("El problema no tiene casos de prueba configurados.");

        // 2. Validar existencia del Lenguaje
        var lenguajeExiste = await context.Lenguajes
            .AnyAsync(l => l.CodigoJudge0 == peticion.LenguajeId);

        if (!lenguajeExiste)
            throw new KeyNotFoundException($"El lenguaje con código {peticion.LenguajeId} no está soportado.");

        // 3. Lógica de Competencia: Buscar si el problema pertenece a un contest activo para este usuario
        var ahora = DateTime.UtcNow;
        var competenciaId = await context.CompetenciaProblemas
            .Where(cp => cp.ProblemaId == peticion.ProblemaId && 
                         cp.Competencia.FechaInicio <= ahora && 
                         cp.Competencia.FechaFin >= ahora &&
                         cp.Competencia.Participantes.Any(p => p.UsuarioId == usuarioId))
            .Select(cp => (Guid?)cp.CompetenciaId)
            .FirstOrDefaultAsync();

        // 4. Crear el registro del Envío
        var envio = new Envio
        {
            ProblemaId = peticion.ProblemaId,
            UsuarioId = usuarioId,
            CompetenciaId = competenciaId, 
            CodigoFuente = peticion.CodigoFuente,
            LenguajeId = peticion.LenguajeId,
            VeredictoGlobal = "Processing",
            FechaEnvio = ahora
        };
        context.Envios.Add(envio);
        await context.SaveChangesAsync();

        // 2. Disparar evaluaciones en paralelo MASIVO (Sin restricciones de semáforo)
        var tareas = problema.CasosDePrueba.Select(async caso =>
        {
            var envioTecnico = new Envio 
            { 
                CodigoFuente = peticion.CodigoFuente, 
                LenguajeId = peticion.LenguajeId, 
                EntradaEstandar = caso.Entrada 
            };

            // Envío directo a la potencia del hardware local
            var token = await servicioJuez.CrearEnvioAsync(
                envioTecnico, 
                problema.LimiteTiempo, 
                problema.LimiteMemoria, 
                caso.SalidaEsperada);

            return new DetalleEnvio
            {
                EnvioId = envio.Id,
                CasoDePruebaId = caso.Id,
                Token = token,
                Veredicto = "In Queue"
            };
        });

        var detalles = await Task.WhenAll(tareas);
        context.DetalleEnvios.AddRange(detalles);
        await context.SaveChangesAsync();

        return envio.Id;
    }
}
