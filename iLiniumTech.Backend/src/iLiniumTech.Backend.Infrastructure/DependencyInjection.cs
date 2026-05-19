using iLiniumTech.Backend.Application.Agenda;
using iLiniumTech.Backend.Application.Clientes;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Application.Propuestas;
using iLiniumTech.Backend.Application.Recibos;
using iLiniumTech.Backend.Application.Siniestros;
using iLiniumTech.Backend.Application.Suplementos;
using iLiniumTech.Backend.Infrastructure.Agenda;
using iLiniumTech.Backend.Infrastructure.Clientes;
using iLiniumTech.Backend.Infrastructure.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using iLiniumTech.Backend.Infrastructure.Propuestas;
using iLiniumTech.Backend.Infrastructure.Recibos;
using iLiniumTech.Backend.Infrastructure.Siniestros;
using iLiniumTech.Backend.Infrastructure.Suplementos;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace iLiniumTech.Backend.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<InMemorySiniestrosRepository>();
        services.AddScoped<ISiniestrosRepository>(provider => provider.GetRequiredService<InMemorySiniestrosRepository>());
        services.AddSingleton<InMemoryRecibosRepository>();
        services.AddScoped<IRecibosRepository>(provider => provider.GetRequiredService<InMemoryRecibosRepository>());
        services.AddSingleton<InMemoryAgendaRepository>();
        services.AddScoped<IAgendaRepository>(provider => provider.GetRequiredService<InMemoryAgendaRepository>());
        services.AddScoped<IAgendaWriteRepository>(provider => provider.GetRequiredService<InMemoryAgendaRepository>());
        services.AddSingleton<InMemoryPropuestasRepository>();
        services.AddScoped<IPropuestasRepository>(provider => provider.GetRequiredService<InMemoryPropuestasRepository>());
        services.AddSingleton<InMemorySuplementosRepository>();
        services.AddScoped<ISuplementosRepository>(provider => provider.GetRequiredService<InMemorySuplementosRepository>());

        var repositoryMode = configuration["Polizas:Repository"];
        if (string.Equals(repositoryMode, "Sql", StringComparison.OrdinalIgnoreCase))
        {
            services.TryAddScoped<IPolizasExecutionContextAccessor>(_ => new ConfiguredPolizasExecutionContextAccessor(configuration));
            services.AddScoped<IPolizasConnectionStringProvider>(provider =>
                CreateConnectionStringProvider(
                    configuration,
                    provider.GetRequiredService<IPolizasExecutionContextAccessor>(),
                    "Polizas"));
            services.AddScoped<IPolizasRepository>(provider => new SqlPolizasRepository(
                provider.GetRequiredService<IPolizasConnectionStringProvider>(),
                provider.GetRequiredService<IPolizasExecutionContextAccessor>()));
            services.AddScoped<IPolizasWriteRepository>(provider => new SqlPolizasWriteRepository(
                provider.GetRequiredService<IPolizasConnectionStringProvider>(),
                provider.GetRequiredService<IPolizasExecutionContextAccessor>()));
        }
        else
        {
            services.AddSingleton<InMemoryPolizasRepository>();
            services.AddScoped<IPolizasRepository>(provider => provider.GetRequiredService<InMemoryPolizasRepository>());
            services.AddScoped<IPolizasWriteRepository>(provider => provider.GetRequiredService<InMemoryPolizasRepository>());
        }

        var clientesRepositoryMode = configuration["Clientes:Repository"];
        if (string.Equals(clientesRepositoryMode, "Sql", StringComparison.OrdinalIgnoreCase))
        {
            services.TryAddScoped<IPolizasExecutionContextAccessor>(_ => new ConfiguredPolizasExecutionContextAccessor(configuration));
            services.AddScoped<IClientesRepository>(provider => new SqlClientesRepository(
                CreateConnectionStringProvider(
                    configuration,
                    provider.GetRequiredService<IPolizasExecutionContextAccessor>(),
                    "Clientes"),
                provider.GetRequiredService<IPolizasExecutionContextAccessor>()));
        }
        else
        {
            services.AddSingleton<InMemoryClientesRepository>();
            services.AddScoped<IClientesRepository>(provider => provider.GetRequiredService<InMemoryClientesRepository>());
        }

        var agendaRepositoryMode = configuration["Agenda:Repository"];
        if (string.Equals(agendaRepositoryMode, "Sql", StringComparison.OrdinalIgnoreCase))
        {
            services.TryAddScoped<IPolizasExecutionContextAccessor>(_ => new ConfiguredPolizasExecutionContextAccessor(configuration));
            services.AddScoped<IAgendaRepository>(provider => new SqlAgendaRepository(
                CreateConnectionStringProvider(
                    configuration,
                    provider.GetRequiredService<IPolizasExecutionContextAccessor>(),
                    "Agenda"),
                provider.GetRequiredService<IPolizasExecutionContextAccessor>()));
            services.AddScoped<IAgendaWriteRepository>(provider => new SqlAgendaWriteRepository(
                CreateConnectionStringProvider(
                    configuration,
                    provider.GetRequiredService<IPolizasExecutionContextAccessor>(),
                    "Agenda"),
                provider.GetRequiredService<IPolizasExecutionContextAccessor>()));
        }

        return services;
    }

    private static IPolizasConnectionStringProvider CreateConnectionStringProvider(
        IConfiguration configuration,
        IPolizasExecutionContextAccessor executionContextAccessor,
        string sectionName)
    {
        var resolverMode = configuration[$"{sectionName}:ConnectionResolver"]
            ?? configuration["Polizas:ConnectionResolver"];
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
            var databaseTypeId = configuration[$"{sectionName}:ModelDatabaseTypeId"]
                ?? configuration["Polizas:ModelDatabaseTypeId"]
                ?? AppBuilderMasterPolizasConnectionStringProvider.DefaultModelDatabaseTypeId;
            var trustServerCertificate = bool.TryParse(
                    configuration[$"{sectionName}:AppBuilderMaster:TrustServerCertificate"]
                    ?? configuration["Polizas:AppBuilderMaster:TrustServerCertificate"]
                    ?? configuration["ILINIUMTECH:APPBUILDER_MASTER_TRUST_SERVER_CERTIFICATE"],
                    out var configuredTrustServerCertificate)
                && configuredTrustServerCertificate;

            return new AppBuilderMasterPolizasConnectionStringProvider(
                masterConnectionString,
                executionContextAccessor,
                new AppBuilderConnectionValueProtector(encryptionKey),
                databaseTypeId,
                trustServerCertificate);
        }

        var connectionString = configuration.GetConnectionString($"{sectionName}Model")
            ?? configuration.GetConnectionString($"{sectionName}ReadWrite")
            ?? configuration.GetConnectionString($"{sectionName}ReadOnly")
            ?? configuration[$"ILINIUMTECH:{sectionName.ToUpperInvariant()}_CONNECTION"]
            ?? configuration.GetConnectionString("PolizasReadOnly")
            ?? configuration["ILINIUMTECH:POLIZAS_CONNECTION"];
        if (string.IsNullOrWhiteSpace(connectionString) ||
            string.Equals(connectionString, "__SET_IN_ENVIRONMENT__", StringComparison.Ordinal))
        {
            var message = string.Equals(sectionName, "Polizas", StringComparison.OrdinalIgnoreCase)
                ? "Polizas SQL repository requires ConnectionStrings:PolizasReadOnly or ILINIUMTECH__POLIZAS_CONNECTION."
                : $"{sectionName} SQL repository requires a model database connection string.";
            throw new InvalidOperationException(message);
        }

        return new StaticPolizasConnectionStringProvider(connectionString);
    }
}
