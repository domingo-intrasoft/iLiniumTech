using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iLiniumTech.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var repositoryMode = configuration["Polizas:Repository"];
        if (string.Equals(repositoryMode, "Sql", StringComparison.OrdinalIgnoreCase))
        {
            var connectionString = configuration.GetConnectionString("PolizasReadOnly")
                ?? configuration["ILINIUMTECH:POLIZAS_CONNECTION"];

            if (string.IsNullOrWhiteSpace(connectionString) ||
                string.Equals(connectionString, "__SET_IN_ENVIRONMENT__", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "Polizas SQL repository requires ConnectionStrings:PolizasReadOnly or ILINIUMTECH__POLIZAS_CONNECTION.");
            }

            services.AddSingleton<IPolizasRepository>(_ => new SqlPolizasRepository(connectionString));
            return services;
        }

        services.AddSingleton<IPolizasRepository, InMemoryPolizasRepository>();
        return services;
    }
}
