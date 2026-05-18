using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using iLiniumTech.Backend.Api.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace iLiniumTech.Backend.Tests.Siniestros;

public sealed class SiniestrosApiTests
{
    [Fact]
    public async Task Siniestros_requires_authentication()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/siniestros");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Legacy_api_key_does_not_grant_siniestros_read()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/siniestros");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        body.Should().Contain("POLIZAS_ACCESS_DENIED");
        body.Should().NotContain("Siniestro");
        body.Should().NotContain("ConnectionStrings");
    }

    [Fact]
    public async Task Siniestros_catalogs_require_exact_permission()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = SiniestrosPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var response = await client.GetAsync("/api/siniestros/catalogs");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        body.Should().Contain("POLIZAS_ACCESS_DENIED");
    }

    [Fact]
    public async Task Siniestros_search_returns_minimized_fixture_contract()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = SiniestrosPermissions.Catalogs,
            ["Auth:Demo:Permissions:1"] = SiniestrosPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var catalogs = await client.GetAsync("/api/siniestros/catalogs");
        var catalogsBody = await catalogs.Content.ReadAsStringAsync();
        var response = await client.GetAsync("/api/siniestros?page=1&pageSize=2&estado=Abierto");
        var body = await response.Content.ReadAsStringAsync();

        catalogs.StatusCode.Should().Be(HttpStatusCode.OK);
        catalogsBody.Should().Contain("\"estados\"");
        catalogsBody.Should().Contain("\"prioridades\"");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"page\":1");
        body.Should().Contain("\"pageSize\":2");
        body.Should().Contain("\"total\":1");
        body.Should().Contain("SIN-2026-0002");
        body.Should().Contain("POL-2026-0002");
        body.Should().NotContain("SIN-2026-0001");
        body.Contains("telefono", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("email", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("documento", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("importe", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Should().NotContain("QueryStatic");
        body.Should().NotContain("AppBuilder");
    }

    [Fact]
    public async Task Siniestros_rejects_sort_fields_outside_whitelist()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = SiniestrosPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "siniestros-sort-validation");

        var response = await client.GetAsync("/api/siniestros?sort=SELECT%20*%20FROM%20SecretTable:desc");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("siniestros-sort-validation");
        body.Should().Contain("SINIESTROS_VALIDATION_ERROR");
        body.Should().Contain("\"correlationId\":\"siniestros-sort-validation\"");
        body.Contains("SELECT", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("SecretTable", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    private static async Task LoginDemoAsync(HttpClient client)
    {
        var login = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "demo",
            password = "demo",
            brokerId = 42
        });

        login.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private sealed class TestApiFactory(
        Dictionary<string, string?>? configurationOverrides = null,
        string environmentName = "Development")
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(environmentName);
        }

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
