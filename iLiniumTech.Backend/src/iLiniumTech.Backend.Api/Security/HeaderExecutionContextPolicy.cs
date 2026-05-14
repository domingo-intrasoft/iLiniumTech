using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace iLiniumTech.Backend.Api.Security;

public static class HeaderExecutionContextPolicy
{
    public const string AllowHeaderExecutionContextKey = "Polizas:AllowHeaderExecutionContext";
    public const string AllowHeaderExecutionContextEnvironmentKey = "ILINIUMTECH:ALLOW_HEADER_EXECUTION_CONTEXT";
    public const string AllowOutsideDevelopmentKey = "Polizas:AllowHeaderExecutionContextOutsideDevelopment";
    public const string AllowOutsideDevelopmentEnvironmentKey = "ILINIUMTECH:ALLOW_HEADER_EXECUTION_CONTEXT_OUTSIDE_DEVELOPMENT";
    public const string DemoOptInKey = "Polizas:AllowHeaderExecutionContextDemoOptIn";
    public const string DemoOptInEnvironmentKey = "ILINIUMTECH:ALLOW_HEADER_EXECUTION_CONTEXT_DEMO_OPT_IN";
    public const string DemoOptInRequiredValue = "DEMO_ONLY_NOT_FOR_REAL_DATA";

    public static bool IsConfigured(IConfiguration configuration) =>
        ReadBoolean(configuration, AllowHeaderExecutionContextKey, AllowHeaderExecutionContextEnvironmentKey);

    public static bool IsEnabled(IConfiguration configuration, IHostEnvironment environment)
    {
        if (!IsConfigured(configuration))
        {
            return false;
        }

        return environment.IsDevelopment() ||
            ReadBoolean(configuration, AllowOutsideDevelopmentKey, AllowOutsideDevelopmentEnvironmentKey);
    }

    public static bool RequiresDemoOptInForReadiness(IConfiguration configuration, IHostEnvironment environment) =>
        !environment.IsDevelopment() &&
        ReadBoolean(configuration, AllowOutsideDevelopmentKey, AllowOutsideDevelopmentEnvironmentKey);

    public static bool HasDemoOptIn(IConfiguration configuration) =>
        string.Equals(
            configuration[DemoOptInKey] ?? configuration[DemoOptInEnvironmentKey],
            DemoOptInRequiredValue,
            StringComparison.Ordinal);

    private static bool ReadBoolean(IConfiguration configuration, string configurationKey, string environmentKey) =>
        bool.TryParse(configuration[configurationKey] ?? configuration[environmentKey], out var value) && value;
}
