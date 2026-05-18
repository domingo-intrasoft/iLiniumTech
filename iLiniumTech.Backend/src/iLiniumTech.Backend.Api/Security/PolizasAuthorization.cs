using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;

namespace iLiniumTech.Backend.Api.Security;

public static class PolizasPermissions
{
    public const string Catalogs = "polizas.catalogs";
    public const string Read = "polizas.read";
    public const string Detail = "polizas.detail";
    public const string Create = "polizas.create";
    public const string Update = "polizas.update";
    public const string Delete = "polizas.delete";

    private static readonly HashSet<string> ActivePermissions = new(StringComparer.Ordinal)
    {
        Catalogs,
        Read,
        Detail,
        Create,
        Update,
        Delete
    };

    private static readonly HashSet<string> LegacyApiKeyPermissions = new(StringComparer.Ordinal)
    {
        Catalogs,
        Read,
        Detail
    };

    public static bool IsActive(string permission) => ActivePermissions.Contains(permission);

    public static bool IsLegacyApiKeyCompatible(string permission) => LegacyApiKeyPermissions.Contains(permission);
}

public static class PolizasAuthorizationPolicies
{
    public const string Catalogs = PolizasPermissions.Catalogs;
    public const string Read = PolizasPermissions.Read;
    public const string Detail = PolizasPermissions.Detail;
    public const string Create = PolizasPermissions.Create;
    public const string Update = PolizasPermissions.Update;
    public const string Delete = PolizasPermissions.Delete;
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
            IsLegacyApiKeyCompatibility(context.User, environment, requirement.Permission))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }

    private static bool HasPermission(ClaimsPrincipal user, string permission) =>
        user.FindAll(PolizasContextClaimTypes.Permission)
            .Any(claim => string.Equals(claim.Value, permission, StringComparison.Ordinal));

    private static bool IsLegacyApiKeyCompatibility(
        ClaimsPrincipal user,
        IHostEnvironment environment,
        string permission) =>
        environment.IsDevelopment() &&
        PolizasPermissions.IsLegacyApiKeyCompatible(permission) &&
        string.Equals(
            user.Identity?.AuthenticationType,
            ApiKeyAuthenticationHandler.SchemeName,
            StringComparison.Ordinal);
}
