using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using iLiniumTech.Backend.Api.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace iLiniumTech.Backend.Tests.Clientes;

public sealed class ClientesApiTests
{
    [Fact]
    public async Task Clientes_requires_authentication()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/clientes");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Legacy_api_key_does_not_grant_clientes_read()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/clientes");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        body.Should().Contain("POLIZAS_ACCESS_DENIED");
        body.Should().NotContain("CLI-2026");
        body.Should().NotContain("ConnectionStrings");
    }

    [Fact]
    public async Task Clientes_catalogs_require_exact_permission()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = ClientesPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var response = await client.GetAsync("/api/clientes/catalogs");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        body.Should().Contain("POLIZAS_ACCESS_DENIED");
    }

    [Fact]
    public async Task Clientes_search_returns_minimized_fixture_contract_without_pii_fields()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = ClientesPermissions.Catalogs,
            ["Auth:Demo:Permissions:1"] = ClientesPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var catalogs = await client.GetAsync("/api/clientes/catalogs");
        var catalogsBody = await catalogs.Content.ReadAsStringAsync();
        var response = await client.GetAsync("/api/clientes?page=1&pageSize=2&estado=Activo%20demo");
        var body = await response.Content.ReadAsStringAsync();

        catalogs.StatusCode.Should().Be(HttpStatusCode.OK);
        catalogsBody.Should().Contain("\"estados\"");
        catalogsBody.Should().Contain("\"segmentos\"");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"page\":1");
        body.Should().Contain("\"pageSize\":2");
        body.Should().Contain("\"total\":2");
        body.Should().Contain("CLI-2026-0001");
        body.Should().Contain("CLI-2026-0004");
        body.Should().Contain("Alias anonimo");
        body.Contains("documento", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("email", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("telefono", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("direccion", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("iban", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("cuenta", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Should().NotContain("QueryStatic");
        body.Should().NotContain("AppBuilder");
    }

    [Fact]
    public async Task Clientes_rejects_sort_fields_outside_whitelist()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = ClientesPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "clientes-sort-validation");

        var response = await client.GetAsync("/api/clientes?sort=SELECT%20*%20FROM%20SecretTable:desc");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("clientes-sort-validation");
        body.Should().Contain("CLIENTES_VALIDATION_ERROR");
        body.Should().Contain("\"correlationId\":\"clientes-sort-validation\"");
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
