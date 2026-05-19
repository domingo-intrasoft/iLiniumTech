using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using iLiniumTech.Backend.Api.Security;
using iLiniumTech.Backend.Domain.Agenda;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace iLiniumTech.Backend.Tests.Agenda;

public sealed class AgendaApiTests
{
    [Fact]
    public async Task Agenda_events_require_authentication()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/agenda/events");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task Legacy_api_key_does_not_grant_agenda_read()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/agenda/events");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        body.Should().Contain("POLIZAS_ACCESS_DENIED");
        body.Should().NotContain("AGE-2026");
        body.Should().NotContain("ConnectionStrings");
    }

    [Fact]
    public async Task Agenda_catalogs_require_exact_permission()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = AgendaPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var response = await client.GetAsync("/api/agenda/catalogs");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        body.Should().Contain("POLIZAS_ACCESS_DENIED");
    }

    [Fact]
    public async Task Agenda_events_return_minimized_fixture_contract_without_calendar_detail_or_pii()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = AgendaPermissions.Catalogs,
            ["Auth:Demo:Permissions:1"] = AgendaPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var catalogs = await client.GetAsync("/api/agenda/catalogs");
        var catalogsBody = await catalogs.Content.ReadAsStringAsync();
        var response = await client.GetAsync("/api/agenda?page=1&pageSize=2&estado=Programado&origen=Fixture%20local");
        var body = await response.Content.ReadAsStringAsync();

        catalogs.StatusCode.Should().Be(HttpStatusCode.OK);
        catalogsBody.Should().Contain("\"estados\"");
        catalogsBody.Should().Contain("\"prioridades\"");
        catalogsBody.Should().Contain("\"origenes\"");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"page\":1");
        body.Should().Contain("\"pageSize\":2");
        body.Should().Contain("\"total\":2");
        body.Should().Contain("AGE-2026-0002");
        body.Should().Contain("AGE-2026-0004");
        body.Contains("descripcion", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("participante", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("identidadId", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("email", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("telefono", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("direccion", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Should().NotContain("FullCalendar");
        body.Should().NotContain("QueryStatic");
        body.Should().NotContain("AppBuilder");
    }

    [Fact]
    public async Task Agenda_rejects_sort_fields_outside_whitelist()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = AgendaPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "agenda-sort-validation");

        var response = await client.GetAsync("/api/agenda/events?sort=SELECT%20*%20FROM%20SecretTable:desc");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("agenda-sort-validation");
        body.Should().Contain("AGENDA_VALIDATION_ERROR");
        body.Should().Contain("\"correlationId\":\"agenda-sort-validation\"");
        body.Contains("SELECT", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("SecretTable", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public async Task Agenda_rejects_excessive_date_ranges()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = AgendaPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var response = await client.GetAsync("/api/agenda/events?fechaDesde=2026-01-01&fechaHasta=2026-12-31");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("AGENDA_VALIDATION_ERROR");
        body.Contains("SELECT", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public async Task Agenda_writes_are_disabled_by_default()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = AgendaPermissions.Create
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var response = await client.PostAsJsonAsync("/api/agenda", new
        {
            referencia = "ILMVP-AGE-DISABLED",
            titulo = "Alta bloqueada",
            inicio = new DateTime(2026, 6, 1, 10, 0, 0),
            fin = new DateTime(2026, 6, 1, 10, 30, 0),
            prioridad = "Media",
            objetoRelacionadoTipo = "Generico demo"
        });
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        body.Should().Contain("AGENDA_WRITES_DISABLED");
        body.Should().NotContain("ILMVP-AGE-DISABLED");
    }

    [Fact]
    public async Task Agenda_crud_requires_write_flag_and_explicit_permissions()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Agenda:WritesEnabled"] = "true",
            ["Auth:Demo:Permissions:0"] = AgendaPermissions.Read,
            ["Auth:Demo:Permissions:1"] = AgendaPermissions.Create,
            ["Auth:Demo:Permissions:2"] = AgendaPermissions.Update,
            ["Auth:Demo:Permissions:3"] = AgendaPermissions.Delete
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var createResponse = await client.PostAsJsonAsync("/api/agenda", new
        {
            referencia = "ILMVP-AGE-CRUD-001",
            titulo = "Evento CRUD local",
            inicio = new DateTime(2026, 6, 1, 10, 0, 0),
            fin = new DateTime(2026, 6, 1, 10, 30, 0),
            prioridad = "Alta",
            objetoRelacionadoTipo = "Generico demo"
        });

        createResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        var created = await createResponse.Content.ReadFromJsonAsync<AgendaCreateResult>();
        created.Should().NotBeNull();
        created!.Id.Should().NotBeNullOrWhiteSpace();

        var searchAfterCreate = await client.GetAsync("/api/agenda?texto=CRUD%20local");
        var searchAfterCreateBody = await searchAfterCreate.Content.ReadAsStringAsync();
        searchAfterCreate.StatusCode.Should().Be(HttpStatusCode.OK);
        searchAfterCreateBody.Should().Contain("ILMVP-AGE-CRUD-001");
        searchAfterCreateBody.Should().Contain("Evento CRUD local");

        var updateResponse = await client.PutAsJsonAsync($"/api/agenda/{created.Id}", new
        {
            titulo = "Evento CRUD local actualizado",
            inicio = new DateTime(2026, 6, 1, 11, 0, 0),
            fin = new DateTime(2026, 6, 1, 11, 30, 0),
            prioridad = "Media",
            objetoRelacionadoTipo = "Generico demo"
        });

        updateResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var searchAfterUpdate = await client.GetAsync("/api/agenda?texto=actualizado");
        var searchAfterUpdateBody = await searchAfterUpdate.Content.ReadAsStringAsync();
        searchAfterUpdate.StatusCode.Should().Be(HttpStatusCode.OK);
        searchAfterUpdateBody.Should().Contain("Evento CRUD local actualizado");

        var deleteResponse = await client.DeleteAsync($"/api/agenda/{created.Id}");
        deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

        var searchAfterDelete = await client.GetAsync("/api/agenda?texto=actualizado");
        var searchAfterDeleteBody = await searchAfterDelete.Content.ReadAsStringAsync();
        searchAfterDelete.StatusCode.Should().Be(HttpStatusCode.OK);
        searchAfterDeleteBody.Should().NotContain("Evento CRUD local actualizado");
    }

    [Fact]
    public async Task Agenda_create_rejects_non_mvp_reference()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Agenda:WritesEnabled"] = "true",
            ["Auth:Demo:Permissions:0"] = AgendaPermissions.Create
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var response = await client.PostAsJsonAsync("/api/agenda", new
        {
            referencia = "AGE-NO-PREFIX",
            titulo = "No permitido",
            inicio = new DateTime(2026, 6, 1, 10, 0, 0)
        });
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        body.Should().Contain("AGENDA_VALIDATION_ERROR");
        body.Should().Contain("ILMVP-AGE-");
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
