using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace iLiniumTech.Backend.Api.Security;

public static class PolizasPermissions
{
    public const string Catalogs = "polizas.catalogs";
    public const string Read = "polizas.read";
    public const string Detail = "polizas.detail";

    private static readonly HashSet<string> ActivePermissions = new(StringComparer.Ordinal)
    {
        Catalogs,
        Read,
        Detail
    };

    public static bool IsActive(string permission) => ActivePermissions.Contains(permission);
}

public static class PolizasAuthorizationPolicies
{
    public const string Catalogs = PolizasPermissions.Catalogs;
    public const string Read = PolizasPermissions.Read;
    public const string Detail = PolizasPermissions.Detail;
}

public sealed record PolizasPermissionRequirement(string Permission) : IAuthorizationRequirement;

public sealed class PolizasPermissionAuthorizationHandler(IHostEnvironment environment)
    : AuthorizationHandler<PolizasPermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PolizasPermissionRequirement requirement)
    {
        if (context.User.Identity?.IsAuthenticated != true)
        {
            return Task.CompletedTask;
        }

        if (HasPermission(context.User, requirement.Permission) ||
            IsLegacyApiKeyCompatibility(context.User, environment))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static bool HasPermission(ClaimsPrincipal user, string permission) =>
        user.FindAll(PolizasContextClaimTypes.Permission)
            .Any(claim => string.Equals(claim.Value, permission, StringComparison.Ordinal));

    private static bool IsLegacyApiKeyCompatibility(ClaimsPrincipal user, IHostEnvironment environment) =>
        environment.IsDevelopment() &&
        string.Equals(
            user.Identity?.AuthenticationType,
            ApiKeyAuthenticationHandler.SchemeName,
            StringComparison.Ordinal);
}
