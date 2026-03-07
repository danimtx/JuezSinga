using Application.UseCases.Envios;
using Application.UseCases.Lenguajes;
using Application.UseCases.Problemas;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ObtenerLenguajesCasoDeUso>();
        services.AddScoped<ProcesarEnvioCasoDeUso>();
        services.AddScoped<ConsultarVeredictoCasoDeUso>();
        services.AddScoped<GestionProblemasUseCase>();
        services.AddScoped<EvaluarEnvioCompetenciaUseCase>();
        services.AddScoped<ConsultarResultadoConsolidadoUseCase>();
        
        return services;
    }
}
