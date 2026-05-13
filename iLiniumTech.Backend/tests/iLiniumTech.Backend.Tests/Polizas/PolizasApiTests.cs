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
    public async Task Metadata_exposes_appbuilder_polizas_reference()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/polizas/metadata");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"rootComponentId\":2824");
        body.Should().Contain("\"dataSourceId\":146");
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

    private sealed class TestApiFactory : WebApplicationFactory<Program>
    {
        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(configuration =>
            {
                configuration.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ApiSecurity:ApiKey"] = "test-key",
                    ["Cors:AllowedOrigins:0"] = "http://localhost:5173"
                });
            });

            return base.CreateHost(builder);
        }
    }
}
