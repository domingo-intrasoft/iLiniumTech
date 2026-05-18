using FluentAssertions;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Infrastructure;
using iLiniumTech.Backend.Infrastructure.Polizas;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iLiniumTech.Backend.Tests.Polizas;

public sealed class PolizasInfrastructureTests
{
    [Fact]
    public void AddInfrastructure_defaults_to_in_memory_repository()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder().Build();

        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IPolizasRepository>()
            .Should().BeOfType<InMemoryPolizasRepository>();
        provider.GetRequiredService<IPolizasWriteRepository>()
            .Should().BeOfType<InMemoryPolizasRepository>();
    }

    [Fact]
    public void AddInfrastructure_requires_connection_string_for_sql_repository()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Polizas:Repository"] = "Sql"
            })
            .Build();

        services.AddInfrastructure(configuration);
        using var provider = services.BuildServiceProvider();

        var act = () => provider.GetRequiredService<IPolizasConnectionStringProvider>();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*PolizasReadOnly*");
    }

    [Fact]
    public void AddInfrastructure_registers_sql_repository_with_static_connection_provider()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Polizas:Repository"] = "Sql",
                ["ConnectionStrings:PolizasReadOnly"] = "Server=localhost;Database=Polizas;User Id=user;Password=password;TrustServerCertificate=True"
            })
            .Build();

        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IPolizasRepository>()
            .Should().BeOfType<SqlPolizasRepository>();
        provider.GetRequiredService<IPolizasWriteRepository>()
            .Should().BeOfType<SqlPolizasWriteRepository>();
        provider.GetRequiredService<IPolizasConnectionStringProvider>()
            .Should().BeOfType<StaticPolizasConnectionStringProvider>();
    }

    [Fact]
    public async Task Sql_repository_exposes_mvp_catalog_values_compatible_with_model_constraints()
    {
        var repository = new SqlPolizasRepository("Server=localhost;Database=Polizas;User Id=user;Password=password;TrustServerCertificate=True");

        var catalogs = await repository.GetCatalogsAsync(CancellationToken.None);

        catalogs.TipoPoliza.Should().Contain(option =>
            option.Value == "situacionpoliza-EV" &&
            option.Label == "En vigor");
        catalogs.TipoPoliza.Should().NotContain(option => option.Value == "Vigor");
    }

    [Fact]
    public void AddInfrastructure_requires_master_connection_for_appbuilder_master_resolver()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Polizas:Repository"] = "Sql",
                ["Polizas:ConnectionResolver"] = "AppBuilderMaster",
                ["Polizas:BrokerId"] = "42"
            })
            .Build();

        services.AddInfrastructure(configuration);
        using var provider = services.BuildServiceProvider();

        var act = () => provider.GetRequiredService<IPolizasConnectionStringProvider>();

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*AppBuilderMaster*");
    }

    [Fact]
    public void AddInfrastructure_registers_appbuilder_master_connection_provider_without_startup_broker()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Polizas:Repository"] = "Sql",
                ["Polizas:ConnectionResolver"] = "AppBuilderMaster",
                ["ConnectionStrings:AppBuilderMaster"] = "Server=localhost;Database=Master;User Id=user;Password=password;TrustServerCertificate=True"
            })
            .Build();

        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IPolizasConnectionStringProvider>()
            .Should().BeOfType<AppBuilderMasterPolizasConnectionStringProvider>();
        provider.GetRequiredService<IPolizasExecutionContextAccessor>()
            .Should().BeOfType<ConfiguredPolizasExecutionContextAccessor>();
    }

    [Fact]
    public void AddInfrastructure_registers_appbuilder_master_connection_provider()
    {
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Polizas:Repository"] = "Sql",
                ["Polizas:ConnectionResolver"] = "AppBuilderMaster",
                ["Polizas:BrokerId"] = "42",
                ["ConnectionStrings:AppBuilderMaster"] = "Server=localhost;Database=Master;User Id=user;Password=password;TrustServerCertificate=True"
            })
            .Build();

        services.AddInfrastructure(configuration);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IPolizasConnectionStringProvider>()
            .Should().BeOfType<AppBuilderMasterPolizasConnectionStringProvider>();
    }
}
