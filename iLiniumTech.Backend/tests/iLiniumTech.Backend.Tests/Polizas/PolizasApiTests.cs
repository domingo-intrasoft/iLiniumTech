using System.Net;
using FluentAssertions;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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
    public async Task Responses_include_baseline_security_headers()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/health");

        response.Headers.GetValues("X-Content-Type-Options").Should().ContainSingle("nosniff");
        response.Headers.GetValues("Referrer-Policy").Should().ContainSingle("no-referrer");
        response.Headers.GetValues("X-Frame-Options").Should().ContainSingle("DENY");
    }

    [Fact]
    public void Cors_wildcard_origins_are_rejected_at_startup()
    {
        using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Cors:AllowedOrigins:0"] = "*"
        });

        var act = () => factory.CreateClient();

        act.Should().Throw<InvalidOperationException>()
            .Where(exception => exception.ToString().Contains("wildcard", StringComparison.OrdinalIgnoreCase));
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
    public async Task Me_requires_api_key()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/me");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Me_returns_effective_polizas_context_from_configuration()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Polizas:BrokerId"] = "84",
            ["Polizas:UserId"] = "10",
            ["Polizas:ProfileId"] = "11",
            ["Polizas:ProfileTypeId"] = "configured-profile",
            ["Polizas:IsAdmin"] = "false"
        });
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/me");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"brokerId\":84");
        body.Should().Contain("\"entityMainId\":84");
        body.Should().Contain("\"userId\":10");
        body.Should().Contain("\"profileId\":11");
        body.Should().Contain("\"profileTypeId\":\"configured-profile\"");
        body.Should().Contain("\"isAdmin\":false");
        body.Should().Contain("\"headerExecutionContextEnabled\":false");
    }

    [Fact]
    public async Task Me_returns_effective_polizas_context_from_headers_when_enabled()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Polizas:AllowHeaderExecutionContext"] = "true",
            ["Polizas:BrokerId"] = "84"
        });
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");
        client.DefaultRequestHeaders.Add("X-Broker-Id", "42");
        client.DefaultRequestHeaders.Add("X-User-Id", "7");
        client.DefaultRequestHeaders.Add("X-Profile-Id", "9");
        client.DefaultRequestHeaders.Add("X-Profile-Type-Id", "header-profile");
        client.DefaultRequestHeaders.Add("X-Is-Admin", "true");

        var response = await client.GetAsync("/api/me");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"brokerId\":42");
        body.Should().Contain("\"entityMainId\":42");
        body.Should().Contain("\"userId\":7");
        body.Should().Contain("\"profileId\":9");
        body.Should().Contain("\"profileTypeId\":\"header-profile\"");
        body.Should().Contain("\"isAdmin\":true");
        body.Should().Contain("\"headerExecutionContextEnabled\":true");
    }

    [Fact]
    public async Task Me_rejects_invalid_header_context()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Polizas:AllowHeaderExecutionContext"] = "true"
        });
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");
        client.DefaultRequestHeaders.Add("X-Broker-Id", "not-a-broker");

        var response = await client.GetAsync("/api/me");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("POLIZAS_CONTEXT_INVALID");
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
        body.Should().Contain("POLIZAS_METADATA_DEPRECATED");
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
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "test-correlation-polizas-validation");

        var response = await client.GetAsync("/api/polizas?sort=rawSql:desc");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("test-correlation-polizas-validation");
        body.Should().Contain("POLIZAS_VALIDATION_ERROR");
        body.Should().Contain("\"correlationId\":\"test-correlation-polizas-validation\"");
        body.Contains("SELECT", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public async Task Sql_repository_configuration_errors_are_sanitized()
    {
        await using var factory = new TestApiFactory(configureServices: services =>
        {
            services.RemoveAll<IPolizasService>();
            services.AddScoped<IPolizasService, ThrowingConfigurationPolizasService>();
        });
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "test-correlation-polizas-config");

        var response = await client.GetAsync("/api/polizas");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("test-correlation-polizas-config");
        body.Should().Contain("POLIZAS_CONFIGURATION_ERROR");
        body.Should().Contain("\"correlationId\":\"test-correlation-polizas-config\"");
        body.Should().NotContain("ConnectionStrings");
        body.Should().NotContain("PolizasReadOnly");
        body.Should().NotContain("ILINIUMTECH");
        body.Contains("Server=", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
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

    private sealed class TestApiFactory(
        Dictionary<string, string?>? configurationOverrides = null,
        Action<IServiceCollection>? configureServices = null)
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            if (configureServices is not null)
            {
                builder.ConfigureServices(configureServices);
            }
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

    private sealed class ThrowingConfigurationPolizasService : IPolizasService
    {
        public Task<PagedResult<PolizaListItem>> SearchAsync(
            PolizasSearchRequest request,
            CancellationToken cancellationToken) =>
            throw new InvalidOperationException("ConnectionStrings:PolizasReadOnly is missing.");

        public Task<PolizaDetail?> GetByIdAsync(string id, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("ConnectionStrings:PolizasReadOnly is missing.");

        public Task<PolizasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
            throw new InvalidOperationException("ConnectionStrings:PolizasReadOnly is missing.");
    }
}
