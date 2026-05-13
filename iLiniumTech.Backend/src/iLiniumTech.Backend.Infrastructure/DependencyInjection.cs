using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
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
            services.AddSingleton(CreateConnectionStringProvider(configuration));
            services.AddSingleton<IPolizasRepository, SqlPolizasRepository>();
            return services;
        }

        services.AddSingleton<IPolizasRepository, InMemoryPolizasRepository>();
        return services;
    }

    private static IPolizasConnectionStringProvider CreateConnectionStringProvider(IConfiguration configuration)
    {
        var resolverMode = configuration["Polizas:ConnectionResolver"];
        if (string.Equals(resolverMode, "AppBuilderMaster", StringComparison.OrdinalIgnoreCase))
        {
            var masterConnectionString = configuration.GetConnectionString("AppBuilderMaster")
                ?? configuration["ILINIUMTECH:APPBUILDER_MASTER_CONNECTION"];
            if (string.IsNullOrWhiteSpace(masterConnectionString) ||
                string.Equals(masterConnectionString, "__SET_IN_ENVIRONMENT__", StringComparison.Ordinal))
            {
                throw new InvalidOperationException(
                    "AppBuilderMaster resolver requires ConnectionStrings:AppBuilderMaster or ILINIUMTECH__APPBUILDER_MASTER_CONNECTION.");
            }

            var brokerIdValue = configuration["Polizas:BrokerId"]
                ?? configuration["ILINIUMTECH:BROKER_ID"];
            if (!int.TryParse(brokerIdValue, out var brokerId) || brokerId <= 0)
            {
                throw new InvalidOperationException(
                    "AppBuilderMaster resolver requires Polizas:BrokerId or ILINIUMTECH__BROKER_ID.");
            }

            var encryptionKey = configuration["AppBuilder:EncryptionKey"]
                ?? configuration["ILINIUMTECH:APPBUILDER_ENCRYPTION_KEY"];
            var databaseTypeId = configuration["Polizas:ModelDatabaseTypeId"]
                ?? AppBuilderMasterPolizasConnectionStringProvider.DefaultModelDatabaseTypeId;

            return new AppBuilderMasterPolizasConnectionStringProvider(
                masterConnectionString,
                brokerId,
                new AppBuilderConnectionValueProtector(encryptionKey),
                databaseTypeId);
        }

        var connectionString = configuration.GetConnectionString("PolizasReadOnly")
            ?? configuration["ILINIUMTECH:POLIZAS_CONNECTION"];
        if (string.IsNullOrWhiteSpace(connectionString) ||
            string.Equals(connectionString, "__SET_IN_ENVIRONMENT__", StringComparison.Ordinal))
        {
            throw new InvalidOperationException(
                "Polizas SQL repository requires ConnectionStrings:PolizasReadOnly or ILINIUMTECH__POLIZAS_CONNECTION.");
        }

        return new StaticPolizasConnectionStringProvider(connectionString);
    }
}
