using FluentAssertions;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Infrastructure;
using iLiniumTech.Backend.Infrastructure.Polizas;
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

        var act = () => services.AddInfrastructure(configuration);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*PolizasReadOnly*");
    }
}
