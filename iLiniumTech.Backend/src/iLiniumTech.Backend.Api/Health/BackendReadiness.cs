using iLiniumTech.Backend.Api.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace iLiniumTech.Backend.Api.Health;

public static class BackendReadiness
{
    private const string PlaceholderValue = "__SET_IN_ENVIRONMENT__";

    public static BackendReadinessResult Check(IConfiguration configuration, IHostEnvironment environment)
    {
        var checks = new List<ReadinessCheck>
        {
            CheckApiKey(configuration, environment),
            CheckCors(configuration),
            CheckPolizasRepository(configuration),
            CheckHeaderExecutionContext(configuration, environment)
        };

        var isReady = checks.All(check => check.Status == ReadinessStatus.Ok);
        return new BackendReadinessResult(
            Status: isReady ? "ready" : "not_ready",
            Application: "iLiniumTech.Backend",
            Checks: checks);
    }

    private static ReadinessCheck CheckApiKey(IConfiguration configuration, IHostEnvironment environment)
    {
        var apiKey = configuration["ApiSecurity:ApiKey"];
        if (IsMissingOrPlaceholder(apiKey))
        {
            return ReadinessCheck.Fail(
                "apiKey",
                "API key is not configured.",
                new Dictionary<string, object?>
                {
                    ["source"] = "configuration",
                    ["environment"] = environment.EnvironmentName
                });
        }

        if (environment.IsProduction() && apiKey!.Length < 32)
        {
            return ReadinessCheck.Fail(
                "apiKey",
                "API key is too short for production.",
                new Dictionary<string, object?>
                {
                    ["minimumLength"] = 32
                });
        }

        return ReadinessCheck.Ok("apiKey", "API key is configured.");
    }

    private static ReadinessCheck CheckCors(IConfiguration configuration)
    {
        var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:5173"];
        var normalizedOrigins = origins
            .Where(origin => !string.IsNullOrWhiteSpace(origin))
            .Select(origin => origin.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (normalizedOrigins.Length == 0)
        {
            return ReadinessCheck.Fail("cors", "At least one explicit CORS origin is required.");
        }

        foreach (var origin in normalizedOrigins)
        {
            if (origin.Contains('*', StringComparison.Ordinal))
            {
                return ReadinessCheck.Fail("cors", "Wildcard CORS origins are not allowed.");
            }

            if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) ||
                (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                return ReadinessCheck.Fail("cors", "CORS origins must be absolute http or https origins.");
            }
        }

        return ReadinessCheck.Ok(
            "cors",
            "CORS origins are explicit.",
            new Dictionary<string, object?>
            {
                ["originCount"] = normalizedOrigins.Length
            });
    }

    private static ReadinessCheck CheckPolizasRepository(IConfiguration configuration)
    {
        var repositoryMode = configuration["Polizas:Repository"];
        if (string.IsNullOrWhiteSpace(repositoryMode) ||
            string.Equals(repositoryMode, "InMemory", StringComparison.OrdinalIgnoreCase))
        {
            return ReadinessCheck.Ok(
                "polizasRepository",
                "Polizas repository is configured.",
                new Dictionary<string, object?>
                {
                    ["mode"] = "InMemory"
                });
        }

        if (!string.Equals(repositoryMode, "Sql", StringComparison.OrdinalIgnoreCase))
        {
            return ReadinessCheck.Fail(
                "polizasRepository",
                "Polizas repository mode is not supported.",
                new Dictionary<string, object?>
                {
                    ["mode"] = repositoryMode
                });
        }

        var resolverMode = configuration["Polizas:ConnectionResolver"];
        if (string.Equals(resolverMode, "AppBuilderMaster", StringComparison.OrdinalIgnoreCase))
        {
            var masterConnection = configuration.GetConnectionString("AppBuilderMaster")
                ?? configuration["ILINIUMTECH:APPBUILDER_MASTER_CONNECTION"];
            if (IsMissingOrPlaceholder(masterConnection))
            {
                return ReadinessCheck.Fail(
                    "polizasRepository",
                    "AppBuilder master connection is not configured.",
                    new Dictionary<string, object?>
                    {
                        ["mode"] = "Sql",
                        ["connectionResolver"] = "AppBuilderMaster",
                        ["requiresRuntimeBrokerContext"] = true
                    });
            }

            return ReadinessCheck.Ok(
                "polizasRepository",
                "SQL AppBuilder resolver is configured.",
                new Dictionary<string, object?>
                {
                    ["mode"] = "Sql",
                    ["connectionResolver"] = "AppBuilderMaster",
                    ["requiresRuntimeBrokerContext"] = true
                });
        }

        if (!string.IsNullOrWhiteSpace(resolverMode) &&
            !string.Equals(resolverMode, "Static", StringComparison.OrdinalIgnoreCase))
        {
            return ReadinessCheck.Fail(
                "polizasRepository",
                "Polizas connection resolver is not supported.",
                new Dictionary<string, object?>
                {
                    ["mode"] = "Sql",
                    ["connectionResolver"] = resolverMode
                });
        }

        var connectionString = configuration.GetConnectionString("PolizasReadOnly")
            ?? configuration["ILINIUMTECH:POLIZAS_CONNECTION"];
        if (IsMissingOrPlaceholder(connectionString))
        {
            return ReadinessCheck.Fail(
                "polizasRepository",
                "Polizas SQL connection is not configured.",
                new Dictionary<string, object?>
                {
                    ["mode"] = "Sql",
                    ["connectionResolver"] = "Static"
                });
        }

        return ReadinessCheck.Ok(
            "polizasRepository",
            "SQL static resolver is configured.",
            new Dictionary<string, object?>
            {
                ["mode"] = "Sql",
                ["connectionResolver"] = "Static"
            });
    }

    private static ReadinessCheck CheckHeaderExecutionContext(
        IConfiguration configuration,
        IHostEnvironment environment)
    {
        var configured = HeaderExecutionContextPolicy.IsConfigured(configuration);
        var enabled = HeaderExecutionContextPolicy.IsEnabled(configuration, environment);
        var requiresDemoOptIn = HeaderExecutionContextPolicy.RequiresDemoOptInForReadiness(configuration, environment);
        var hasDemoOptIn = HeaderExecutionContextPolicy.HasDemoOptIn(configuration);
        if (requiresDemoOptIn && !hasDemoOptIn)
        {
            return ReadinessCheck.Fail(
                "headerExecutionContext",
                "Temporary header execution context cannot be enabled outside Development without explicit demo opt-in.",
                new Dictionary<string, object?>
                {
                    ["configured"] = configured,
                    ["enabled"] = enabled,
                    ["environment"] = environment.EnvironmentName,
                    ["requiresDemoOptIn"] = true
                });
        }

        return ReadinessCheck.Ok(
            "headerExecutionContext",
            enabled
                ? hasDemoOptIn && requiresDemoOptIn
                    ? "Temporary header execution context is enabled for demo outside Development."
                    : "Temporary header execution context is enabled."
                : "Temporary header execution context is disabled.",
            new Dictionary<string, object?>
            {
                ["configured"] = configured,
                ["enabled"] = enabled,
                ["environment"] = environment.EnvironmentName,
                ["demoOptIn"] = hasDemoOptIn && requiresDemoOptIn
            });
    }

    private static bool IsMissingOrPlaceholder(string? value) =>
        string.IsNullOrWhiteSpace(value) ||
        string.Equals(value, PlaceholderValue, StringComparison.Ordinal);
}

public sealed record BackendReadinessResult(
    string Status,
    string Application,
    IReadOnlyList<ReadinessCheck> Checks)
{
    public bool IsReady => Status == "ready";
}

public sealed record ReadinessCheck(
    string Name,
    string Status,
    string Message,
    IReadOnlyDictionary<string, object?>? Data = null)
{
    public static ReadinessCheck Ok(
        string name,
        string message,
        IReadOnlyDictionary<string, object?>? data = null) =>
        new(name, ReadinessStatus.Ok, message, data);

    public static ReadinessCheck Fail(
        string name,
        string message,
        IReadOnlyDictionary<string, object?>? data = null) =>
        new(name, ReadinessStatus.Fail, message, data);
}

public static class ReadinessStatus
{
    public const string Ok = "ok";
    public const string Fail = "fail";
}
