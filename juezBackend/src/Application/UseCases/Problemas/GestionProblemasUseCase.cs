using Application.DTOs.Problema;
using Application.Persistence;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Problemas;

/// <summary>
/// Caso de uso para crear problemas con conversión de unidades.
/// </summary>
public class GestionProblemasUseCase(ApplicationDbContext context)
{
    public async Task<Guid> CrearProblemaAsync(CrearProblemaDto dto)
    {
        // Conversión a KB si es necesario
        int memoriaKB = dto.Unidad == UnidadMemoria.Megabytes 
            ? dto.LimiteMemoria * 1024 
            : dto.LimiteMemoria;

        var problema = new Problema
        {
            Titulo = dto.Titulo,
            Descripcion = dto.Descripcion,
            LimiteTiempo = dto.LimiteTiempo,
            LimiteMemoria = memoriaKB
        };

        context.Problemas.Add(problema);
        await context.SaveChangesAsync();
        
        return problema.Id;
    }

    public async Task<IEnumerable<ProblemaResumenDto>> ListarResumenAsync()
    {
        return await context.Problemas
            .OrderBy(p => p.Titulo)
            .Select(p => new ProblemaResumenDto(p.Id, p.Titulo))
            .ToListAsync();
    }

    public async Task<ProblemaDto?> ObtenerDetalleAsync(Guid id)
    {
        var problema = await context.Problemas
            .Include(p => p.CasosDePrueba)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (problema == null) return null;

        var casosPublicos = problema.CasosDePrueba
            .Where(c => !c.EsOculto)
            .Select(c => new CasoDePruebaDto(c.Id, c.Entrada, c.SalidaEsperada, c.EsOculto));

        return new ProblemaDto(
            problema.Id, 
            problema.Titulo, 
            problema.Descripcion, 
            problema.LimiteTiempo, 
            problema.LimiteMemoria, 
            casosPublicos);
    }

    public async Task ActualizarProblemaAsync(Guid id, CrearProblemaDto dto)
    {
        var problema = await context.Problemas.FindAsync(id) 
            ?? throw new Exception("Problema no encontrado");

        problema.Titulo = dto.Titulo;
        problema.Descripcion = dto.Descripcion;
        problema.LimiteTiempo = dto.LimiteTiempo;
        problema.LimiteMemoria = dto.Unidad == UnidadMemoria.Megabytes 
            ? dto.LimiteMemoria * 1024 
            : dto.LimiteMemoria;

        await context.SaveChangesAsync();
    }

    public async Task BorrarLogicoAsync(Guid id)
    {
        var problema = await context.Problemas.FindAsync(id) 
            ?? throw new Exception("Problema no encontrado");

        problema.EsEliminado = true;
        await context.SaveChangesAsync();
    }

    public async Task SincronizarCasosAsync(Guid problemaId, IEnumerable<CrearCasoDePruebaDto> nuevosCasos)
    {
        var problema = await context.Problemas
            .Include(p => p.CasosDePrueba)
            .FirstOrDefaultAsync(p => p.Id == problemaId) 
            ?? throw new Exception("Problema no encontrado");

        // Borrar casos anteriores
        context.CasosDePrueba.RemoveRange(problema.CasosDePrueba);

        // Agregar los nuevos
        foreach (var c in nuevosCasos)
        {
            context.CasosDePrueba.Add(new CasoDePrueba
            {
                ProblemaId = problemaId,
                Entrada = c.Entrada,
                SalidaEsperada = c.SalidaEsperada,
                EsOculto = c.EsOculto
            });
        }

        await context.SaveChangesAsync();
    }

    public async Task AgregarCasosDePruebaAsync(Guid problemaId, IEnumerable<CrearCasoDePruebaDto> casos)
    {
        var problema = await context.Problemas.FindAsync(problemaId) 
            ?? throw new Exception("Problema no encontrado");

        foreach (var c in casos)
        {
            context.CasosDePrueba.Add(new CasoDePrueba
            {
                ProblemaId = problemaId,
                Entrada = c.Entrada,
                SalidaEsperada = c.SalidaEsperada,
                EsOculto = c.EsOculto
            });
        }

        await context.SaveChangesAsync();
    }
}
