using Application.DTOs.Envio;
using Application.Interfaces;
using Application.Persistence;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Envios;

/// <summary>
/// Caso de uso para consultar el estado de todos los casos de prueba y dar un veredicto global.
/// </summary>
public class ConsultarResultadoConsolidadoUseCase(
    IServicioJuez servicioJuez, 
    ApplicationDbContext context)
{
    public async Task<VeredictoCompetenciaDto?> EjecutarAsync(Guid envioId)
    {
        var envio = await context.Envios
            .Include(e => e.DetalleEnvios)
            .FirstOrDefaultAsync(e => e.Id == envioId);

        if (envio == null) return null;

        bool procesando = false;
        int casosPasados = 0;
        string veredictoGlobal = "Accepted";

        foreach (var detalle in envio.DetalleEnvios)
        {
            // Si ya tenemos el veredicto en DB y no es final, o si no lo tenemos, consultamos a Judge0
            if (string.IsNullOrEmpty(detalle.Veredicto) || detalle.Veredicto == "In Queue" || detalle.Veredicto == "Processing")
            {
                var resultado = await servicioJuez.ConsultarVeredictoAsync(detalle.Token);
                if (resultado != null)
                {
                    detalle.Veredicto = TraducirEstado(resultado.Estado);
                    detalle.Tiempo = resultado.Tiempo;
                    detalle.Memoria = resultado.Memoria;
                }
            }

            if (detalle.Veredicto == "Processing" || detalle.Veredicto == "In Queue")
                procesando = true;
            
            if (detalle.Veredicto == "Accepted")
                casosPasados++;
            else if (veredictoGlobal == "Accepted" && !procesando)
                veredictoGlobal = detalle.Veredicto ?? "Unknown Error";
        }

        if (procesando) veredictoGlobal = "Processing";

        // Actualizar veredicto global en DB si ya no está procesando
        if (!procesando)
        {
            envio.VeredictoGlobal = veredictoGlobal;
        }
        
        await context.SaveChangesAsync();

        return new VeredictoCompetenciaDto(
            VeredictoGlobal: veredictoGlobal,
            CasosPasados: casosPasados,
            TotalCasos: envio.DetalleEnvios.Count,
            DetalleTokens: envio.DetalleEnvios.Select(d => d.Token)
        );
    }

    private string TraducirEstado(EstadoVeredicto estado)
    {
        return estado switch
        {
            EstadoVeredicto.Aceptado => "Accepted",
            EstadoVeredicto.RespuestaIncorrecta => "Wrong Answer",
            EstadoVeredicto.TiempoLimiteExcedido => "Time Limit Exceeded",
            EstadoVeredicto.ErrorCompilacion => "Compilation Error",
            EstadoVeredicto.MemoriaExcedida => "Memory Limit Exceeded",
            EstadoVeredicto.EnCola => "In Queue",
            EstadoVeredicto.Procesando => "Processing",
            _ => "Runtime Error"
        };
    }
}
