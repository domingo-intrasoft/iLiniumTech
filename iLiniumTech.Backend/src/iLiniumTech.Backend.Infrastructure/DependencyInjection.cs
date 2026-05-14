using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace iLiniumTech.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var repositoryMode = configuration["Polizas:Repository"];
        if (string.Equals(repositoryMode, "Sql", StringComparison.OrdinalIgnoreCase))
        {
            services.TryAddScoped<IPolizasExecutionContextAccessor>(_ => new ConfiguredPolizasExecutionContextAccessor(configuration));
            services.AddScoped<IPolizasConnectionStringProvider>(provider =>
                CreateConnectionStringProvider(configuration, provider.GetRequiredService<IPolizasExecutionContextAccessor>()));
            services.AddScoped<IPolizasRepository>(provider => new SqlPolizasRepository(
                provider.GetRequiredService<IPolizasConnectionStringProvider>(),
                provider.GetRequiredService<IPolizasExecutionContextAccessor>()));
            return services;
        }

        services.AddScoped<IPolizasRepository, InMemoryPolizasRepository>();
        return services;
    }

    private static IPolizasConnectionStringProvider CreateConnectionStringProvider(
        IConfiguration configuration,
        IPolizasExecutionContextAccessor executionContextAccessor)
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

            var encryptionKey = configuration["AppBuilder:EncryptionKey"]
                ?? configuration["ILINIUMTECH:APPBUILDER_ENCRYPTION_KEY"];
            var databaseTypeId = configuration["Polizas:ModelDatabaseTypeId"]
                ?? AppBuilderMasterPolizasConnectionStringProvider.DefaultModelDatabaseTypeId;

            return new AppBuilderMasterPolizasConnectionStringProvider(
                masterConnectionString,
                executionContextAccessor,
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
