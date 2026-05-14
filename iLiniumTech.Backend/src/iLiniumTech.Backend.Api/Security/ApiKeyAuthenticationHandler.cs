using System.Security.Cryptography;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace iLiniumTech.Backend.Api.Security;

public sealed class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "ApiKey";
    private const string HeaderName = "X-ILiniumTech-Api-Key";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var configuredApiKey = configuration["ApiSecurity:ApiKey"];
        if (string.IsNullOrWhiteSpace(configuredApiKey) || configuredApiKey == "__SET_IN_ENVIRONMENT__")
        {
            return Task.FromResult(AuthenticateResult.Fail("API key is not configured."));
        }

        if (!Request.Headers.TryGetValue(HeaderName, out var apiKey) || apiKey.Count != 1)
        {
            return Task.FromResult(AuthenticateResult.Fail("API key header is missing."));
        }

        if (!ApiKeysMatch(apiKey[0], configuredApiKey))
        {
            return Task.FromResult(AuthenticateResult.Fail("API key is invalid."));
        }

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, "local-api-key-user")],
            SchemeName);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, SchemeName);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    private static bool ApiKeysMatch(string? providedApiKey, string configuredApiKey)
    {
        if (providedApiKey is null)
        {
            return false;
        }

        var providedBytes = Encoding.UTF8.GetBytes(providedApiKey);
        var configuredBytes = Encoding.UTF8.GetBytes(configuredApiKey);
        try
        {
            return providedBytes.Length == configuredBytes.Length &&
                CryptographicOperations.FixedTimeEquals(providedBytes, configuredBytes);
        }
        finally
        {
            CryptographicOperations.ZeroMemory(providedBytes);
            CryptographicOperations.ZeroMemory(configuredBytes);
        }
    }
}
