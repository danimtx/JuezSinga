using Application.Interfaces;
using Application.Persistence;
using Infrastructure.ExternalServices.Judge0;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Registro de DbContext con PostgreSQL (Supabase)
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

        // Configuración de Judge0
        services.Configure<ConfiguracionJudge0>(
            configuration.GetSection(ConfiguracionJudge0.Seccion));

        // Registro del Cliente HTTP y el Servicio
        services.AddHttpClient<IServicioJuez, ServicioJuezRapidApi>();
        
        return services;
    }
}
