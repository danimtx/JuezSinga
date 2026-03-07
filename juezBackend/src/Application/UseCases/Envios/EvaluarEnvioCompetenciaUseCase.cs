using Application.DTOs.Envio;
using Application.Interfaces;
using Application.Persistence;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Envios;

/// <summary>
/// Orquesta la evaluación de un envío contra múltiples casos de prueba de la base de datos.
/// </summary>
public class EvaluarEnvioCompetenciaUseCase(
    IServicioJuez servicioJuez, 
    ApplicationDbContext context)
{
    // Semáforo para no saturar la API gratuita durante las pruebas (máximo 2 peticiones concurrentes)
    private static readonly SemaphoreSlim Semaphore = new(2);

    public async Task<Guid> EjecutarAsync(CrearEnvioCompetenciaDto peticion)
    {
        // 1. Validar existencia del Problema
        var problema = await context.Problemas
            .Include(p => p.CasosDePrueba)
            .FirstOrDefaultAsync(p => p.Id == peticion.ProblemaId)
            ?? throw new KeyNotFoundException($"El problema con ID {peticion.ProblemaId} no existe.");

        if (!problema.CasosDePrueba.Any())
            throw new InvalidOperationException("El problema no tiene casos de prueba configurados.");

        // 2. Validar existencia del Lenguaje (en nuestra tabla local)
        var lenguajeExiste = await context.Lenguajes
            .AnyAsync(l => l.CodigoJudge0 == peticion.LenguajeId);
        
        if (!lenguajeExiste)
            throw new KeyNotFoundException($"El lenguaje con código {peticion.LenguajeId} no está soportado o no ha sido sincronizado.");

        // 3. Crear el registro del Envío
        var envio = new Envio
        {
            ProblemaId = peticion.ProblemaId,
            CodigoFuente = peticion.CodigoFuente,
            LenguajeId = peticion.LenguajeId,
            VeredictoGlobal = "Processing"
        };
        context.Envios.Add(envio);
        await context.SaveChangesAsync();

        // 2. Disparar evaluaciones en paralelo
        var tareas = problema.CasosDePrueba.Select(async caso =>
        {
            await Semaphore.WaitAsync();
            try
            {
                var envioTecnico = new Envio { CodigoFuente = peticion.CodigoFuente, LenguajeId = peticion.LenguajeId, EntradaEstandar = caso.Entrada };
                var token = await servicioJuez.CrearEnvioAsync(envioTecnico, problema.LimiteTiempo, problema.LimiteMemoria, caso.SalidaEsperada);

                // Guardar detalle individual
                var detalle = new DetalleEnvio
                {
                    EnvioId = envio.Id,
                    CasoDePruebaId = caso.Id,
                    Token = token,
                    Veredicto = "In Queue"
                };
                return detalle;
            }
            finally
            {
                Semaphore.Release();
            }
        });

        var detalles = await Task.WhenAll(tareas);
        context.DetalleEnvios.AddRange(detalles);
        await context.SaveChangesAsync();

        return envio.Id;
    }
}
