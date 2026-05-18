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

public static class SiniestrosPermissions
{
    public const string Catalogs = "siniestros.catalogs";
    public const string Read = "siniestros.read";

    private static readonly HashSet<string> ActivePermissions = new(StringComparer.Ordinal)
    {
        Catalogs,
        Read
    };

    public static bool IsActive(string permission) => ActivePermissions.Contains(permission);
}

public static class RecibosPermissions
{
    public const string Catalogs = "recibos.catalogs";
    public const string Read = "recibos.read";

    private static readonly HashSet<string> ActivePermissions = new(StringComparer.Ordinal)
    {
        Catalogs,
        Read
    };

    public static bool IsActive(string permission) => ActivePermissions.Contains(permission);
}

public static class ClientesPermissions
{
    public const string Catalogs = "clientes.catalogs";
    public const string Read = "clientes.read";

    private static readonly HashSet<string> ActivePermissions = new(StringComparer.Ordinal)
    {
        Catalogs,
        Read
    };

    public static bool IsActive(string permission) => ActivePermissions.Contains(permission);
}

public static class AgendaPermissions
{
    public const string Catalogs = "agenda.catalogs";
    public const string Read = "agenda.read";

    private static readonly HashSet<string> ActivePermissions = new(StringComparer.Ordinal)
    {
        Catalogs,
        Read
    };

    public static bool IsActive(string permission) => ActivePermissions.Contains(permission);
}

public static class PropuestasPermissions
{
    public const string Catalogs = "propuestas.catalogs";
    public const string Read = "propuestas.read";

    private static readonly HashSet<string> ActivePermissions = new(StringComparer.Ordinal)
    {
        Catalogs,
        Read
    };

    public static bool IsActive(string permission) => ActivePermissions.Contains(permission);
}

public static class SuplementosPermissions
{
    public const string Catalogs = "suplementos.catalogs";
    public const string Read = "suplementos.read";

    private static readonly HashSet<string> ActivePermissions = new(StringComparer.Ordinal)
    {
        Catalogs,
        Read
    };

    public static bool IsActive(string permission) => ActivePermissions.Contains(permission);
}

public static class IlnPermissions
{
    public static bool IsActive(string permission) =>
        PolizasPermissions.IsActive(permission) ||
        SiniestrosPermissions.IsActive(permission) ||
        RecibosPermissions.IsActive(permission) ||
        ClientesPermissions.IsActive(permission) ||
        AgendaPermissions.IsActive(permission) ||
        PropuestasPermissions.IsActive(permission) ||
        SuplementosPermissions.IsActive(permission);
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

public static class SiniestrosAuthorizationPolicies
{
    public const string Catalogs = SiniestrosPermissions.Catalogs;
    public const string Read = SiniestrosPermissions.Read;
}

public static class RecibosAuthorizationPolicies
{
    public const string Catalogs = RecibosPermissions.Catalogs;
    public const string Read = RecibosPermissions.Read;
}

public static class ClientesAuthorizationPolicies
{
    public const string Catalogs = ClientesPermissions.Catalogs;
    public const string Read = ClientesPermissions.Read;
}

public static class AgendaAuthorizationPolicies
{
    public const string Catalogs = AgendaPermissions.Catalogs;
    public const string Read = AgendaPermissions.Read;
}

public static class PropuestasAuthorizationPolicies
{
    public const string Catalogs = PropuestasPermissions.Catalogs;
    public const string Read = PropuestasPermissions.Read;
}

public static class SuplementosAuthorizationPolicies
{
    public const string Catalogs = SuplementosPermissions.Catalogs;
    public const string Read = SuplementosPermissions.Read;
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
