using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace iLiniumTech.Backend.Api.Security;

public sealed class SanitizedAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
    private const string CorrelationIdHeaderName = "X-Correlation-Id";
    private readonly AuthorizationMiddlewareResultHandler defaultHandler = new();

    public Task HandleAsync(
        RequestDelegate next,
        HttpContext context,
        AuthorizationPolicy policy,
        PolicyAuthorizationResult authorizeResult)
    {
        if (authorizeResult.Challenged)
        {
            return WriteErrorAsync(
                context,
                StatusCodes.Status401Unauthorized,
                "POLIZAS_AUTH_REQUIRED",
                "Authentication is required.");
        }

        if (authorizeResult.Forbidden)
        {
            return WriteErrorAsync(
                context,
                StatusCodes.Status403Forbidden,
                "POLIZAS_ACCESS_DENIED",
                "Access denied.");
        }

        return defaultHandler.HandleAsync(next, context, policy, authorizeResult);
    }

    private static Task WriteErrorAsync(HttpContext context, int statusCode, string code, string message)
    {
        context.Response.StatusCode = statusCode;
        return context.Response.WriteAsJsonAsync(new
        {
            error = new
            {
                code,
                message,
                correlationId = EnsureCorrelationId(context)
            }
        });
    }

    private static string EnsureCorrelationId(HttpContext context)
    {
        if (context.Items.TryGetValue(CorrelationIdHeaderName, out var existing) &&
            existing is string existingCorrelationId)
        {
            return existingCorrelationId;
        }

        var requestedCorrelationId = context.Request.Headers[CorrelationIdHeaderName].Count == 1
            ? context.Request.Headers[CorrelationIdHeaderName][0]
            : null;
        var correlationId = !string.IsNullOrWhiteSpace(requestedCorrelationId) &&
            requestedCorrelationId.Length <= 100
                ? requestedCorrelationId.Trim()
                : context.TraceIdentifier;

        context.Items[CorrelationIdHeaderName] = correlationId;
        context.Response.Headers[CorrelationIdHeaderName] = correlationId;
        return correlationId;
    }
}
