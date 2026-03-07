using Application.DTOs.Lenguaje;
using Application.Interfaces;
using Application.Persistence;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Lenguajes;

/// <summary>
/// Orquesta la obtención de lenguajes soportados, sincronizándolos con la base de datos local.
/// </summary>
public class ObtenerLenguajesCasoDeUso(IServicioJuez servicioJuez, ApplicationDbContext context)
{
    public async Task<IEnumerable<LenguajeDto>> EjecutarAsync()
    {
        // 1. Intentar obtener lenguajes de la base de datos local
        var lenguajesLocal = await context.Lenguajes
            .OrderBy(l => l.Nombre)
            .ToListAsync();

        // 2. Si la tabla está vacía, sincronizar desde la API externa
        if (!lenguajesLocal.Any())
        {
            var lenguajesExternos = await servicioJuez.ObtenerLenguajesAsync();
            
            var nuevosLenguajes = lenguajesExternos.Select(l => new Lenguaje
            {
                Id = Guid.NewGuid(),
                CodigoJudge0 = l.CodigoJudge0,
                Nombre = l.Nombre
            }).ToList();

            context.Lenguajes.AddRange(nuevosLenguajes);
            await context.SaveChangesAsync();
            
            lenguajesLocal = nuevosLenguajes;
        }

        // 3. Retornar los datos desde la base de datos local
        return lenguajesLocal.Select(l => new LenguajeDto(l.Id, l.CodigoJudge0, l.Nombre));
    }
}
