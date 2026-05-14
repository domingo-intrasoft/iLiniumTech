using iLiniumTech.Backend.Infrastructure.Polizas.Connections;

namespace iLiniumTech.Backend.Api.Security;

public sealed class HeaderPolizasExecutionContextAccessor(
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration)
    : IPolizasExecutionContextAccessor
{
    public const string BrokerIdHeaderName = "X-Broker-Id";
    public const string UserIdHeaderName = "X-User-Id";
    public const string ProfileIdHeaderName = "X-Profile-Id";
    public const string ProfileTypeIdHeaderName = "X-Profile-Type-Id";
    public const string IsAdminHeaderName = "X-Is-Admin";

    public PolizasExecutionContext? Current
    {
        get
        {
            var allowHeaderContext = ReadBoolConfiguration(
                "Polizas:AllowHeaderExecutionContext",
                "ILINIUMTECH:ALLOW_HEADER_EXECUTION_CONTEXT") == true;
            var brokerId = (allowHeaderContext ? ReadPositiveIntHeader(BrokerIdHeaderName) : null)
                ?? ReadIntConfiguration("Polizas:BrokerId", "ILINIUMTECH:BROKER_ID");
            return brokerId is null or <= 0
                ? null
                : new PolizasExecutionContext(
                    BrokerId: brokerId.Value,
                    UserId: (allowHeaderContext ? ReadPositiveIntHeader(UserIdHeaderName) : null)
                        ?? ReadIntConfiguration("Polizas:UserId", "ILINIUMTECH:USER_ID"),
                    ProfileId: (allowHeaderContext ? ReadPositiveIntHeader(ProfileIdHeaderName) : null)
                        ?? ReadIntConfiguration("Polizas:ProfileId", "ILINIUMTECH:PROFILE_ID"),
                    ProfileTypeId: (allowHeaderContext ? ReadStringHeader(ProfileTypeIdHeaderName) : null)
                        ?? ReadStringConfiguration("Polizas:ProfileTypeId", "ILINIUMTECH:PROFILE_TYPE_ID"),
                    IsAdmin: (allowHeaderContext ? ReadBoolHeader(IsAdminHeaderName) : null)
                        ?? ReadBoolConfiguration("Polizas:IsAdmin", "ILINIUMTECH:IS_ADMIN"));
        }
    }

    private int? ReadPositiveIntHeader(string headerName)
    {
        var value = ReadStringHeader(headerName, out var wasPresent);
        if (!wasPresent)
        {
            return null;
        }

        return int.TryParse(value, out var parsed) && parsed > 0
            ? parsed
            : throw new PolizasExecutionContextException($"{headerName} must be a positive integer.");
    }

    private bool? ReadBoolHeader(string headerName)
    {
        var value = ReadStringHeader(headerName, out var wasPresent);
        if (!wasPresent)
        {
            return null;
        }

        return bool.TryParse(value, out var parsed)
            ? parsed
            : throw new PolizasExecutionContextException($"{headerName} must be true or false.");
    }

    private string? ReadStringHeader(string headerName) => ReadStringHeader(headerName, out _);

    private string? ReadStringHeader(string headerName, out bool wasPresent)
    {
        wasPresent = false;
        var headers = httpContextAccessor.HttpContext?.Request.Headers;
        if (headers is null || !headers.TryGetValue(headerName, out var value))
        {
            return null;
        }

        wasPresent = true;
        if (value.Count != 1)
        {
            throw new PolizasExecutionContextException($"{headerName} must have a single value.");
        }

        return string.IsNullOrWhiteSpace(value[0]) ? null : value[0]!.Trim();
    }

    private int? ReadIntConfiguration(string configurationKey, string environmentKey)
    {
        var value = ReadStringConfiguration(configurationKey, environmentKey);
        return int.TryParse(value, out var parsed) ? parsed : null;
    }

    private bool? ReadBoolConfiguration(string configurationKey, string environmentKey)
    {
        var value = ReadStringConfiguration(configurationKey, environmentKey);
        return bool.TryParse(value, out var parsed) ? parsed : null;
    }

    private string? ReadStringConfiguration(string configurationKey, string environmentKey) =>
        configuration[configurationKey] ?? configuration[environmentKey];
}
