using Application.UseCases.Auth;
using Application.UseCases.Envios;
using Application.UseCases.Lenguajes;
using Application.UseCases.Problemas;
using Application.UseCases.Usuarios;
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

        // Auth y Usuarios
        services.AddScoped<LoginCasoDeUso>();
        services.AddScoped<RegistroEstudianteCasoDeUso>();
        services.AddScoped<RefreshTokenCasoDeUso>();
        services.AddScoped<GenerarEquiposMasivoCasoDeUso>();
        services.AddScoped<ActualizarPerfilCasoDeUso>();
        services.AddScoped<CambiarPasswordCasoDeUso>();
        services.AddScoped<ObtenerPerfilCasoDeUso>();
        services.AddScoped<ListarUsuariosCasoDeUso>();
        
        return services;
    }
}
