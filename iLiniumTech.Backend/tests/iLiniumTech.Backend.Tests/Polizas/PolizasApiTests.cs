using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace iLiniumTech.Backend.Tests.Polizas;

public sealed class PolizasApiTests
{
    [Fact]
    public async Task Health_is_anonymous()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Polizas_requires_api_key()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/polizas");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Metadata_is_deprecated_without_appbuilder_reference()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/polizas/metadata");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Gone);
        body.Should().Contain("/api/polizas/catalogs");
        body.Should().NotContain("rootComponentId");
        body.Should().NotContain("dataSourceId");
    }

    [Fact]
    public async Task Catalogs_exposes_policy_select_data()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/polizas/catalogs");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"tipoPoliza\"");
        body.Should().Contain("\"ramo\"");
        body.Should().Contain("\"compania\"");
        body.Should().Contain("\"oficina\"");
        body.Should().Contain("\"gestor\"");
        body.Should().Contain("Compania demo");
    }

    [Fact]
    public async Task Search_returns_polizas_data()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/polizas");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"items\"");
        body.Should().Contain("POL-2026-0001");
    }

    [Fact]
    public async Task GetById_returns_poliza_detail()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/polizas/POL-1001");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"clienteNombre\"");
        body.Should().Contain("POL-2026-0001");
    }

    [Fact]
    public async Task Search_rejects_sort_fields_outside_whitelist()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/polizas?sort=rawSql:desc");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("POLIZAS_VALIDATION_ERROR");
        body.Contains("SELECT", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public async Task Sql_appbuilder_mode_requires_broker_context()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Polizas:Repository"] = "Sql",
            ["Polizas:ConnectionResolver"] = "AppBuilderMaster",
            ["ConnectionStrings:AppBuilderMaster"] = "Server=localhost;Database=Master;User Id=user;Password=password;TrustServerCertificate=True"
        });
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/polizas");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("POLIZAS_CONTEXT_REQUIRED");
    }

    [Fact]
    public async Task Sql_appbuilder_mode_rejects_invalid_header_broker_context()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Polizas:Repository"] = "Sql",
            ["Polizas:ConnectionResolver"] = "AppBuilderMaster",
            ["Polizas:AllowHeaderExecutionContext"] = "true",
            ["ConnectionStrings:AppBuilderMaster"] = "Server=localhost;Database=Master;User Id=user;Password=password;TrustServerCertificate=True"
        });
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");
        client.DefaultRequestHeaders.Add("X-Broker-Id", "not-a-broker");

        var response = await client.GetAsync("/api/polizas");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("POLIZAS_CONTEXT_INVALID");
    }

    private sealed class TestApiFactory(Dictionary<string, string?>? configurationOverrides = null)
        : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(configuration =>
            {
                var values = new Dictionary<string, string?>
                {
                    ["ApiSecurity:ApiKey"] = "test-key",
                    ["Cors:AllowedOrigins:0"] = "http://localhost:5173"
                };
                if (configurationOverrides is not null)
                {
                    foreach (var (key, value) in configurationOverrides)
                    {
                        values[key] = value;
                    }
                }

                configuration.AddInMemoryCollection(values);
            });

            return base.CreateHost(builder);
        }
    }
}
