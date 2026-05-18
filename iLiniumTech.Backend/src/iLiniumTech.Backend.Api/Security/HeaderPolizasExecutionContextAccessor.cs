using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using Microsoft.Extensions.Hosting;

namespace iLiniumTech.Backend.Api.Security;

public sealed class HeaderPolizasExecutionContextAccessor(
    IHttpContextAccessor httpContextAccessor,
    IConfiguration configuration,
    IHostEnvironment environment)
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
            var claimsContext = ReadClaimsContext();
            if (claimsContext is not null)
            {
                return claimsContext;
            }

            var allowHeaderContext = HeaderExecutionContextPolicy.IsEnabled(configuration, environment);
            var brokerId = (allowHeaderContext ? ReadNonZeroIntHeader(BrokerIdHeaderName) : null)
                ?? ReadIntConfiguration("Polizas:BrokerId", "ILINIUMTECH:BROKER_ID");
            return brokerId is null or 0
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

    private PolizasExecutionContext? ReadClaimsContext()
    {
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return null;
        }

        var brokerId = ReadNonZeroIntClaim(PolizasContextClaimTypes.BrokerId);
        return brokerId is null
            ? null
            : new PolizasExecutionContext(
                BrokerId: brokerId.Value,
                UserId: ReadPositiveIntClaim(PolizasContextClaimTypes.UserId),
                ProfileId: ReadPositiveIntClaim(PolizasContextClaimTypes.ProfileId),
                ProfileTypeId: ReadStringClaim(PolizasContextClaimTypes.ProfileTypeId),
                IsAdmin: ReadBoolClaim(PolizasContextClaimTypes.IsAdmin));
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

    private int? ReadNonZeroIntHeader(string headerName)
    {
        var value = ReadStringHeader(headerName, out var wasPresent);
        if (!wasPresent)
        {
            return null;
        }

        return int.TryParse(value, out var parsed) && parsed != 0
            ? parsed
            : throw new PolizasExecutionContextException($"{headerName} must be a non-zero integer.");
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

    private int? ReadPositiveIntClaim(string claimType)
    {
        var value = ReadStringClaim(claimType);
        return int.TryParse(value, out var parsed) && parsed > 0 ? parsed : null;
    }

    private int? ReadNonZeroIntClaim(string claimType)
    {
        var value = ReadStringClaim(claimType);
        return int.TryParse(value, out var parsed) && parsed != 0 ? parsed : null;
    }

    private bool? ReadBoolClaim(string claimType)
    {
        var value = ReadStringClaim(claimType);
        return bool.TryParse(value, out var parsed) ? parsed : null;
    }

    private string? ReadStringClaim(string claimType) =>
        httpContextAccessor.HttpContext?.User.FindFirst(claimType)?.Value;

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
