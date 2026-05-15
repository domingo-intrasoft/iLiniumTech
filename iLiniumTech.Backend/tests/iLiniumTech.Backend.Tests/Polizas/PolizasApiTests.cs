using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using FluentAssertions;
using iLiniumTech.Backend.Api.Security;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ApiAuthenticationSchemes = iLiniumTech.Backend.Api.Security.AuthenticationSchemes;

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
    public async Task Ready_is_anonymous_and_reports_configured_backend()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/ready");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"status\":\"ready\"");
        body.Should().Contain("\"name\":\"apiKey\"");
        body.Should().Contain("\"name\":\"polizasRepository\"");
        body.Should().Contain("\"mode\":\"InMemory\"");
    }

    [Fact]
    public async Task Ready_reports_unavailable_when_api_key_is_not_configured()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["ApiSecurity:ApiKey"] = "__SET_IN_ENVIRONMENT__"
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/ready");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        body.Should().Contain("\"status\":\"not_ready\"");
        body.Should().Contain("\"name\":\"apiKey\"");
        body.Should().Contain("API key is not configured");
        body.Should().NotContain("__SET_IN_ENVIRONMENT__");
    }

    [Fact]
    public async Task Ready_reports_sql_configuration_gap_without_secret_values()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Polizas:Repository"] = "Sql"
        });
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/ready");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        body.Should().Contain("\"status\":\"not_ready\"");
        body.Should().Contain("Polizas SQL connection is not configured");
        body.Should().Contain("\"connectionResolver\":\"Static\"");
        body.Should().NotContain("ConnectionStrings");
        body.Should().NotContain("Password");
        body.Should().NotContain("Server=");
    }

    [Fact]
    public async Task Ready_rejects_header_context_outside_development_without_demo_opt_in()
    {
        await using var factory = new TestApiFactory(
            new Dictionary<string, string?>
            {
                ["ApiSecurity:ApiKey"] = "test-key-that-is-long-enough-for-production",
                ["Polizas:AllowHeaderExecutionContext"] = "true",
                ["Polizas:AllowHeaderExecutionContextOutsideDevelopment"] = "true"
            },
            environmentName: "Production");
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/ready");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        body.Should().Contain("\"status\":\"not_ready\"");
        body.Should().Contain("\"name\":\"headerExecutionContext\"");
        body.Should().Contain("Temporary header execution context cannot be enabled outside Development");
        body.Should().Contain("\"requiresDemoOptIn\":true");
        body.Should().NotContain(HeaderExecutionContextPolicy.DemoOptInRequiredValue);
    }

    [Fact]
    public async Task Ready_rejects_outside_development_header_override_without_demo_opt_in()
    {
        await using var factory = new TestApiFactory(
            new Dictionary<string, string?>
            {
                ["ApiSecurity:ApiKey"] = "test-key-that-is-long-enough-for-production",
                ["Polizas:AllowHeaderExecutionContextOutsideDevelopment"] = "true"
            },
            environmentName: "Production");
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/ready");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.ServiceUnavailable);
        body.Should().Contain("\"status\":\"not_ready\"");
        body.Should().Contain("\"name\":\"headerExecutionContext\"");
        body.Should().Contain("\"requiresDemoOptIn\":true");
    }

    [Fact]
    public async Task Ready_allows_header_context_outside_development_with_explicit_demo_opt_in()
    {
        await using var factory = new TestApiFactory(
            new Dictionary<string, string?>
            {
                ["ApiSecurity:ApiKey"] = "test-key-that-is-long-enough-for-production",
                ["Polizas:AllowHeaderExecutionContext"] = "true",
                ["Polizas:AllowHeaderExecutionContextOutsideDevelopment"] = "true",
                [HeaderExecutionContextPolicy.DemoOptInKey] = HeaderExecutionContextPolicy.DemoOptInRequiredValue
            },
            environmentName: "Production");
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/ready");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"status\":\"ready\"");
        body.Should().Contain("Temporary header execution context is enabled for demo outside Development");
        body.Should().Contain("\"demoOptIn\":true");
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
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "anonymous-polizas");

        var response = await client.GetAsync("/api/polizas");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("anonymous-polizas");
        body.Should().Contain("POLIZAS_AUTH_REQUIRED");
        body.Should().Contain("\"correlationId\":\"anonymous-polizas\"");
        body.Should().NotContain("ApiSecurity");
        body.Should().NotContain("API key header");
    }

    [Fact]
    public async Task Autos_particulares_polizas_requires_api_key()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/autos-particulares/polizas");

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
    public async Task Demo_login_creates_cookie_session_and_me_context_without_api_key()
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

        var login = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "demo@iliniumtech.local",
            password = "demo",
            brokerId = 42
        });
        var loginBody = await login.Content.ReadAsStringAsync();

        login.StatusCode.Should().Be(HttpStatusCode.OK);
        login.Headers.GetValues("Set-Cookie")
            .Should().Contain(cookie => cookie.Contains(ApiAuthenticationSchemes.DemoSessionCookieName));
        loginBody.Should().Contain("\"currentBrokerId\":42");
        loginBody.Should().Contain("\"displayName\":\"demo\"");
        loginBody.Should().Contain("polizas.read");
        loginBody.Should().NotContain("demo@iliniumtech.local");
        loginBody.Should().NotContain("password");

        var me = await client.GetAsync("/api/me");
        var meBody = await me.Content.ReadAsStringAsync();

        me.StatusCode.Should().Be(HttpStatusCode.OK);
        meBody.Should().Contain("\"brokerId\":42");
        meBody.Should().Contain("\"entityMainId\":42");
        meBody.Should().Contain("\"userId\":10");
        meBody.Should().Contain("\"profileId\":11");
        meBody.Should().Contain("\"profileTypeId\":\"configured-profile\"");
        meBody.Should().Contain("\"displayName\":\"demo\"");
        meBody.Should().Contain("\"application\":{\"key\":\"iliniumtech\",\"name\":\"iLiniumTech\"}");
        meBody.Should().Contain("polizas.detail");
        meBody.Should().Contain("\"authMode\":\"DemoSession\"");
    }

    [Fact]
    public async Task Demo_login_rejects_invalid_credentials_without_setting_session_cookie()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "demo",
            password = "wrong"
        });
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        response.Headers.TryGetValues("Set-Cookie", out _).Should().BeFalse();
        body.Should().Contain("AUTH_INVALID_CREDENTIALS");
        body.Should().NotContain("wrong");
    }

    [Fact]
    public async Task Demo_login_rejects_requested_broker_outside_allowed_brokers()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:AllowedBrokerIds:0"] = "84"
        });
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "demo",
            password = "demo",
            brokerId = 42
        });
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        response.Headers.TryGetValues("Set-Cookie", out _).Should().BeFalse();
        body.Should().Contain("AUTH_BROKER_FORBIDDEN");
        body.Should().NotContain("84");
        body.Should().NotContain("42");
    }

    [Fact]
    public async Task Demo_login_is_disabled_outside_development_without_explicit_opt_in()
    {
        await using var factory = new TestApiFactory(
            new Dictionary<string, string?>
            {
                ["Auth:Demo:Enabled"] = "true"
            },
            environmentName: "Production");
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "demo",
            password = "demo"
        });
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        body.Should().Contain("AUTH_DEMO_DISABLED");
        response.Headers.TryGetValues("Set-Cookie", out _).Should().BeFalse();
    }

    [Fact]
    public async Task Demo_logout_clears_cookie_session()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();

        var login = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "demo",
            password = "demo"
        });
        login.StatusCode.Should().Be(HttpStatusCode.OK);

        var logout = await client.PostAsync("/api/auth/logout", null);

        logout.StatusCode.Should().Be(HttpStatusCode.NoContent);
        logout.Headers.GetValues("Set-Cookie")
            .Should().Contain(cookie =>
                cookie.Contains(ApiAuthenticationSchemes.DemoSessionCookieName) &&
                cookie.Contains("expires=", StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task Demo_session_without_required_permission_returns_forbidden()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = PolizasPermissions.Catalogs
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "permission-denied");

        var response = await client.GetAsync("/api/polizas");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        body.Should().Contain("POLIZAS_ACCESS_DENIED");
        body.Should().Contain("\"correlationId\":\"permission-denied\"");
        body.Should().NotContain(PolizasPermissions.Read);
    }

    [Fact]
    public async Task Demo_session_with_catalogs_permission_can_read_catalogs()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = PolizasPermissions.Catalogs
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var response = await client.GetAsync("/api/polizas/catalogs");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"tipoPoliza\"");
    }

    [Fact]
    public async Task Demo_session_without_catalogs_permission_cannot_read_catalogs()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = PolizasPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var response = await client.GetAsync("/api/polizas/catalogs");

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Demo_session_read_permission_does_not_allow_detail()
    {
        await using var factory = new TestApiFactory(new Dictionary<string, string?>
        {
            ["Auth:Demo:Permissions:0"] = PolizasPermissions.Read
        });
        using var client = factory.CreateClient();
        await LoginDemoAsync(client);

        var search = await client.GetAsync("/api/polizas");
        var detail = await client.GetAsync("/api/polizas/POL-1001");

        search.StatusCode.Should().Be(HttpStatusCode.OK);
        detail.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Me_rejects_demo_session_when_current_broker_is_not_allowed()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        var cookie = CreateDemoSessionCookie(
            factory,
            currentBrokerId: 42,
            allowedBrokerIds: [84],
            permissions: [PolizasPermissions.Catalogs, PolizasPermissions.Read, PolizasPermissions.Detail]);
        client.DefaultRequestHeaders.Add(
            "Cookie",
            $"{ApiAuthenticationSchemes.DemoSessionCookieName}={cookie}");
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "broker-not-allowed");

        var response = await client.GetAsync("/api/me");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("broker-not-allowed");
        body.Should().Contain("POLIZAS_BROKER_FORBIDDEN");
        body.Should().Contain("\"correlationId\":\"broker-not-allowed\"");
        body.Should().NotContain("42");
        body.Should().NotContain("84");
        body.Should().NotContain(PolizasPermissions.Read);
    }

    [Fact]
    public async Task Api_key_mvp_remains_legacy_compatibility_for_policy_protected_polizas()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/polizas/POL-1001");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("POL-2026-0001");
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
    public async Task Me_ignores_header_context_outside_development_without_explicit_override()
    {
        await using var factory = new TestApiFactory(
            new Dictionary<string, string?>
            {
                ["ApiSecurity:ApiKey"] = "test-key-that-is-long-enough-for-production",
                ["Polizas:AllowHeaderExecutionContext"] = "true",
                ["Polizas:BrokerId"] = "84"
            },
            environmentName: "Production");
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key-that-is-long-enough-for-production");
        client.DefaultRequestHeaders.Add("X-Broker-Id", "42");

        var response = await client.GetAsync("/api/me");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"brokerId\":84");
        body.Should().Contain("\"headerExecutionContextEnabled\":false");
    }

    [Fact]
    public async Task Me_can_use_header_context_outside_development_with_explicit_override()
    {
        await using var factory = new TestApiFactory(
            new Dictionary<string, string?>
            {
                ["ApiSecurity:ApiKey"] = "test-key-that-is-long-enough-for-production",
                ["Polizas:AllowHeaderExecutionContext"] = "true",
                ["Polizas:AllowHeaderExecutionContextOutsideDevelopment"] = "true",
                ["Polizas:BrokerId"] = "84"
            },
            environmentName: "Production");
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key-that-is-long-enough-for-production");
        client.DefaultRequestHeaders.Add("X-Broker-Id", "42");

        var response = await client.GetAsync("/api/me");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"brokerId\":42");
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
    public async Task Autos_particulares_search_returns_only_autos_fixture_data()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/autos-particulares/polizas");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"items\"");
        body.Should().Contain("\"scope\"");
        body.Should().Contain("POL-2026-0001");
        body.Should().Contain("\"ramo\":\"Autos\"");
        body.Should().Contain("\"divisionObjetivo\":\"Particulares\"");
        body.Should().Contain("\"divisionFiltroAplicado\":false");
        body.Should().Contain("\"divisionPendienteUat\":true");
        body.Should().Contain("\"total\":1");
        body.Should().NotContain("POL-2026-0002");
        body.Should().NotContain("\"ramo\":\"Hogar\"");
        body.Should().NotContain("QueryStatic");
        body.Should().NotContain("Pantalla_Polizas");
    }

    [Fact]
    public async Task Autos_particulares_catalogs_are_minimized_and_scoped()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/autos-particulares/catalogs");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"estado\"");
        body.Should().Contain("\"compania\"");
        body.Should().Contain("\"scope\"");
        body.Should().Contain("\"ramo\":\"Autos\"");
        body.Should().Contain("\"divisionPendienteUat\":true");
        body.Should().NotContain("\"oficina\"");
        body.Should().NotContain("\"gestor\"");
        body.Should().NotContain("connectionString");
        body.Should().NotContain("Pantalla_Polizas");
    }

    [Fact]
    public async Task Autos_particulares_search_supports_pagination_and_basic_filters()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/autos-particulares/polizas?page=1&pageSize=1&numero=0001");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("\"page\":1");
        body.Should().Contain("\"pageSize\":1");
        body.Should().Contain("\"total\":1");
        body.Should().Contain("POL-2026-0001");
        body.Should().NotContain("POL-2026-0002");
    }

    [Fact]
    public async Task Autos_particulares_search_keeps_autos_filter_even_when_query_asks_for_other_ramo()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/autos-particulares/polizas?ramo=Hogar");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        body.Should().Contain("POL-2026-0001");
        body.Should().NotContain("POL-2026-0002");
        body.Should().NotContain("\"ramo\":\"Hogar\"");
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
    public async Task GetById_returns_sanitized_not_found_error_with_correlation_id()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "test-correlation-polizas-not-found");

        var response = await client.GetAsync("/api/polizas/POL-NO-EXISTE");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("test-correlation-polizas-not-found");
        body.Should().Contain("POLIZAS_NOT_FOUND");
        body.Should().Contain("\"correlationId\":\"test-correlation-polizas-not-found\"");
        body.Should().NotContain("SELECT");
        body.Should().NotContain("Pantalla_Polizas");
    }

    [Fact]
    public async Task Autos_particulares_detail_returns_autos_and_hides_other_ramos()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var autosResponse = await client.GetAsync("/api/autos-particulares/polizas/POL-1001");
        var autosBody = await autosResponse.Content.ReadAsStringAsync();
        var hogarResponse = await client.GetAsync("/api/autos-particulares/polizas/POL-1002");

        autosResponse.StatusCode.Should().Be(HttpStatusCode.OK);
        autosBody.Should().Contain("\"item\"");
        autosBody.Should().Contain("\"ramo\":\"Autos\"");
        autosBody.Should().Contain("\"scope\"");
        autosBody.Should().Contain("\"riesgo\":\"Vehiculo asegurado\"");
        autosBody.Should().NotContain("1234 ABC");
        autosBody.Should().NotContain("00000001A");
        autosBody.Should().NotContain("+34 600");
        hogarResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Autos_particulares_does_not_expose_runtime_metadata_route()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");

        var response = await client.GetAsync("/api/autos-particulares/polizas/metadata");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        body.Should().NotContain("AppBuilder");
        body.Should().NotContain("QueryStatic");
        body.Should().NotContain("Pantalla_Polizas");
        body.Should().NotContain("rootComponentId");
        body.Should().NotContain("dataSourceId");
    }

    [Fact]
    public async Task Search_rejects_sort_fields_outside_whitelist()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "test-correlation-polizas-validation");

        var response = await client.GetAsync("/api/polizas?sort=SELECT%20*%20FROM%20SecretTable:desc");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("test-correlation-polizas-validation");
        body.Should().Contain("POLIZAS_VALIDATION_ERROR");
        body.Should().Contain("\"correlationId\":\"test-correlation-polizas-validation\"");
        body.Contains("SELECT", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("SecretTable", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public async Task Autos_particulares_validation_errors_are_sanitized()
    {
        await using var factory = new TestApiFactory();
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "test-correlation-autos-validation");

        var response = await client.GetAsync("/api/autos-particulares/polizas?sort=SELECT%20*%20FROM%20SecretTable:desc");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("test-correlation-autos-validation");
        body.Should().Contain("AUTOS_PARTICULARES_VALIDATION_ERROR");
        body.Should().Contain("\"correlationId\":\"test-correlation-autos-validation\"");
        body.Should().NotContain("ConnectionStrings");
        body.Should().NotContain("AppBuilder");
        body.Should().NotContain("QueryStatic");
        body.Contains("SELECT", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
        body.Contains("SecretTable", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
    }

    [Fact]
    public async Task Autos_particulares_configuration_errors_are_sanitized()
    {
        await using var factory = new TestApiFactory(configureServices: services =>
        {
            services.RemoveAll<IPolizasService>();
            services.AddScoped<IPolizasService, ThrowingConfigurationPolizasService>();
        });
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-ILiniumTech-Api-Key", "test-key");
        client.DefaultRequestHeaders.Add("X-Correlation-Id", "test-correlation-autos-config");

        var response = await client.GetAsync("/api/autos-particulares/polizas");
        var body = await response.Content.ReadAsStringAsync();

        response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        response.Headers.GetValues("X-Correlation-Id").Should().Contain("test-correlation-autos-config");
        body.Should().Contain("POLIZAS_CONFIGURATION_ERROR");
        body.Should().Contain("\"correlationId\":\"test-correlation-autos-config\"");
        body.Should().NotContain("ConnectionStrings");
        body.Should().NotContain("PolizasReadOnly");
        body.Should().NotContain("ILINIUMTECH");
        body.Contains("Server=", StringComparison.OrdinalIgnoreCase).Should().BeFalse();
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

    private static async Task LoginDemoAsync(HttpClient client)
    {
        var login = await client.PostAsJsonAsync("/api/auth/login", new
        {
            username = "demo",
            password = "demo"
        });

        login.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    private static string CreateDemoSessionCookie(
        TestApiFactory factory,
        int currentBrokerId,
        IReadOnlyCollection<int> allowedBrokerIds,
        IReadOnlyCollection<string> permissions)
    {
        var options = factory.Services
            .GetRequiredService<IOptionsMonitor<CookieAuthenticationOptions>>()
            .Get(ApiAuthenticationSchemes.DemoSession);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "demo:broker-test"),
            new(ClaimTypes.Name, "broker-test"),
            new(PolizasContextClaimTypes.ApplicationKey, "iliniumtech"),
            new(PolizasContextClaimTypes.ApplicationName, "iLiniumTech"),
            new(PolizasContextClaimTypes.BrokerId, currentBrokerId.ToString()),
            new(PolizasContextClaimTypes.UserId, "10"),
            new(PolizasContextClaimTypes.IsAdmin, bool.FalseString)
        };
        claims.AddRange(allowedBrokerIds
            .Select(brokerId => new Claim(PolizasContextClaimTypes.AllowedBrokerId, brokerId.ToString())));
        claims.AddRange(permissions
            .Select(permission => new Claim(PolizasContextClaimTypes.Permission, permission)));

        var principal = new ClaimsPrincipal(new ClaimsIdentity(claims, ApiAuthenticationSchemes.DemoSession));
        var ticket = new AuthenticationTicket(
            principal,
            new AuthenticationProperties
            {
                IssuedUtc = DateTimeOffset.UtcNow,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
            },
            ApiAuthenticationSchemes.DemoSession);

        return options.TicketDataFormat.Protect(ticket);
    }

    private sealed class TestApiFactory(
        Dictionary<string, string?>? configurationOverrides = null,
        Action<IServiceCollection>? configureServices = null,
        string environmentName = "Development")
        : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment(environmentName);
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

        public Task<PolizaDetail?> GetByIdAsync(
            string id,
            CancellationToken cancellationToken,
            string? ramo = null) =>
            throw new InvalidOperationException("ConnectionStrings:PolizasReadOnly is missing.");

        public Task<PolizasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
            throw new InvalidOperationException("ConnectionStrings:PolizasReadOnly is missing.");
    }
}
