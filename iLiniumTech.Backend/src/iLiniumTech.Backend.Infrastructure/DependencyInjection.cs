using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas;
using Microsoft.Extensions.DependencyInjection;

namespace iLiniumTech.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPolizasRepository, InMemoryPolizasRepository>();
        return services;
    }
}
