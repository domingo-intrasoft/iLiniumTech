using iLiniumTech.Backend.Api.Health;
using iLiniumTech.Backend.Api.Security;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Infrastructure;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

const string AutosParticularesRamo = "Autos";
var autosParticularesScope = new AutosParticularesScope(
    Ramo: AutosParticularesRamo,
    DivisionObjetivo: "Particulares",
    DivisionFiltroAplicado: false,
    DivisionPendienteUat: true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, _ => { });
builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPolizasExecutionContextAccessor, HeaderPolizasExecutionContextAccessor>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.WithOrigins(GetAllowedCorsOrigins(builder.Configuration))
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IPolizasService, PolizasService>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

app.Use(async (context, next) =>
{
    EnsureCorrelationId(context);

    try
    {
        await next(context);
    }
    catch (InvalidOperationException exception) when (IsKnownConfigurationException(exception))
    {
        app.Logger.LogError(
            "Known backend configuration error {CorrelationId} ({ExceptionType})",
            EnsureCorrelationId(context),
            exception.GetType().Name);

        await WriteErrorAsync(
            context,
            StatusCodes.Status500InternalServerError,
            "POLIZAS_CONFIGURATION_ERROR",
            "Polizas backend configuration is incomplete.");
    }
    catch (Exception exception)
    {
        app.Logger.LogError(
            "Unhandled backend error {CorrelationId} ({ExceptionType})",
            EnsureCorrelationId(context),
            exception.GetType().Name);

        await WriteErrorAsync(
            context,
            StatusCodes.Status500InternalServerError,
            "POLIZAS_UNEXPECTED_ERROR",
            "Unexpected backend error.");
    }
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.Use(async (context, next) =>
{
    context.Response.Headers.TryAdd("X-Content-Type-Options", "nosniff");
    context.Response.Headers.TryAdd("Referrer-Policy", "no-referrer");
    context.Response.Headers.TryAdd("X-Frame-Options", "DENY");
    await next(context);
});

app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    application = "iLiniumTech.Backend"
}))
.AllowAnonymous();

app.MapGet("/ready", ([FromServices] IConfiguration configuration, [FromServices] IHostEnvironment environment) =>
{
    var readiness = BackendReadiness.Check(configuration, environment);
    return readiness.IsReady
        ? Results.Ok(readiness)
        : Results.Json(readiness, statusCode: StatusCodes.Status503ServiceUnavailable);
})
.AllowAnonymous();

app.MapGet("/api/me", (
    HttpContext httpContext,
    [FromServices] IPolizasExecutionContextAccessor executionContextAccessor,
    [FromServices] IConfiguration configuration,
    [FromServices] IHostEnvironment environment) =>
{
    try
    {
        var executionContext = executionContextAccessor.Current;
        return Results.Ok(new MeResponse(
            BrokerId: executionContext?.BrokerId,
            EntityMainId: executionContext?.EntityMainId,
            UserId: executionContext?.UserId,
            ProfileId: executionContext?.ProfileId,
            ProfileTypeId: executionContext?.ProfileTypeId,
            IsAdmin: executionContext?.IsAdmin,
            HeaderExecutionContextEnabled: IsHeaderExecutionContextEnabled(configuration, environment),
            PolizasExecutionContextRequired: RequiresPolizasExecutionContext(configuration)));
    }
    catch (PolizasExecutionContextException exception)
    {
        return Results.BadRequest(new ErrorResponse(new ErrorBody(
            Code: "POLIZAS_CONTEXT_INVALID",
            Message: exception.Message,
            CorrelationId: EnsureCorrelationId(httpContext))));
    }
})
.RequireAuthorization()
.WithName("GetMe");

var polizas = app.MapGroup("/api/polizas")
    .RequireAuthorization();

var autosParticulares = app.MapGroup("/api/autos-particulares")
    .RequireAuthorization();

polizas.MapGet("/metadata", (HttpContext httpContext) => Results.Json(
        new ErrorResponse(new ErrorBody(
            Code: "POLIZAS_METADATA_DEPRECATED",
            Message: "Runtime screen design metadata is no longer served by the backend. Use /api/polizas/catalogs for policy catalogs.",
            CorrelationId: EnsureCorrelationId(httpContext))),
        statusCode: StatusCodes.Status410Gone))
    .WithName("GetPolizasMetadataDeprecated");

polizas.MapGet("/catalogs", async ([FromServices] IPolizasService service, CancellationToken cancellationToken) =>
        Results.Ok(await service.GetCatalogsAsync(cancellationToken)))
    .WithName("GetPolizasCatalogs")
    .AddEndpointFilter(RequirePolizasExecutionContextAsync);

polizas.MapGet("/", async (
    HttpContext httpContext,
    [FromServices] IPolizasService service,
    [FromQuery] int? page,
    [FromQuery] int? pageSize,
    [FromQuery] string? sort,
    [FromQuery] string? numero,
    [FromQuery] string? cliente,
    [FromQuery] string? estado,
    [FromQuery] string? compania,
    [FromQuery] string? ramo,
    [FromQuery] DateOnly? fechaEfectoDesde,
    [FromQuery] DateOnly? fechaEfectoHasta,
    CancellationToken cancellationToken) =>
{
    var request = new PolizasSearchRequest(
        Page: page is null or 0 ? 1 : page.Value,
        PageSize: pageSize is null or 0 ? 25 : pageSize.Value,
        Sort: sort,
        Numero: numero,
        Cliente: cliente,
        Estado: estado,
        Compania: compania,
        Ramo: ramo,
        FechaEfectoDesde: fechaEfectoDesde,
        FechaEfectoHasta: fechaEfectoHasta);

    try
    {
        return Results.Ok(await service.SearchAsync(request, cancellationToken));
    }
    catch (PolizasValidationException exception)
    {
        return Results.BadRequest(new ErrorResponse(new ErrorBody(
            Code: "POLIZAS_VALIDATION_ERROR",
            Message: exception.Message,
            CorrelationId: EnsureCorrelationId(httpContext))));
    }
})
.WithName("SearchPolizas")
.AddEndpointFilter(RequirePolizasExecutionContextAsync);

polizas.MapGet("/{id}", async (HttpContext httpContext, [FromServices] IPolizasService service, string id, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null
            ? ErrorResult(httpContext, StatusCodes.Status404NotFound, "POLIZAS_NOT_FOUND", "Poliza no encontrada.")
            : Results.Ok(result);
    }
    catch (PolizasValidationException exception)
    {
        return Results.BadRequest(new ErrorResponse(new ErrorBody(
            Code: "POLIZAS_VALIDATION_ERROR",
            Message: exception.Message,
            CorrelationId: EnsureCorrelationId(httpContext))));
    }
})
.WithName("GetPolizaById")
.AddEndpointFilter(RequirePolizasExecutionContextAsync);

autosParticulares.MapGet("/catalogs", async (
    [FromServices] IPolizasService service,
    CancellationToken cancellationToken) =>
{
    var catalogs = await service.GetCatalogsAsync(cancellationToken);
    return Results.Ok(new AutosParticularesCatalogsResponse(
        Estado: catalogs.TipoPoliza,
        Compania: catalogs.Compania,
        Scope: autosParticularesScope));
})
.WithName("GetAutosParticularesCatalogs")
.AddEndpointFilter(RequirePolizasExecutionContextAsync);

autosParticulares.MapGet("/polizas", async (
    HttpContext httpContext,
    [FromServices] IPolizasService service,
    [FromQuery] int? page,
    [FromQuery] int? pageSize,
    [FromQuery] string? sort,
    [FromQuery] string? numero,
    [FromQuery] string? cliente,
    [FromQuery] string? estado,
    [FromQuery] string? compania,
    [FromQuery] DateOnly? fechaEfectoDesde,
    [FromQuery] DateOnly? fechaEfectoHasta,
    CancellationToken cancellationToken) =>
{
    var request = new PolizasSearchRequest(
        Page: page is null or 0 ? 1 : page.Value,
        PageSize: pageSize is null or 0 ? 25 : pageSize.Value,
        Sort: sort,
        Numero: numero,
        Cliente: cliente,
        Estado: estado,
        Compania: compania,
        Ramo: AutosParticularesRamo,
        FechaEfectoDesde: fechaEfectoDesde,
        FechaEfectoHasta: fechaEfectoHasta);

    try
    {
        var result = await service.SearchAsync(request, cancellationToken);
        return Results.Ok(new AutosParticularesPolizasResponse(
            Items: result.Items,
            Page: result.Page,
            PageSize: result.PageSize,
            Total: result.Total,
            Scope: autosParticularesScope));
    }
    catch (PolizasValidationException exception)
    {
        return Results.BadRequest(new ErrorResponse(new ErrorBody(
            Code: "AUTOS_PARTICULARES_VALIDATION_ERROR",
            Message: exception.Message,
            CorrelationId: EnsureCorrelationId(httpContext))));
    }
})
.WithName("SearchAutosParticularesPolizas")
.AddEndpointFilter(RequirePolizasExecutionContextAsync);

autosParticulares.MapGet("/polizas/{id}", async (
    HttpContext httpContext,
    [FromServices] IPolizasService service,
    string id,
    CancellationToken cancellationToken) =>
{
    try
    {
        var result = await service.GetByIdAsync(id, cancellationToken, AutosParticularesRamo);
        return result is null
            ? Results.NotFound()
            : Results.Ok(new AutosParticularesPolizaDetailResponse(
                Item: SanitizeAutosParticularesDetail(result),
                Scope: autosParticularesScope));
    }
    catch (PolizasValidationException exception)
    {
        return Results.BadRequest(new ErrorResponse(new ErrorBody(
            Code: "AUTOS_PARTICULARES_VALIDATION_ERROR",
            Message: exception.Message,
            CorrelationId: EnsureCorrelationId(httpContext))));
    }
})
.WithName("GetAutosParticularesPolizaById")
.AddEndpointFilter(RequirePolizasExecutionContextAsync);

app.Run();

static async ValueTask<object?> RequirePolizasExecutionContextAsync(
    EndpointFilterInvocationContext context,
    EndpointFilterDelegate next)
{
    var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
    if (!RequiresPolizasExecutionContext(configuration))
    {
        return await next(context);
    }

    var executionContextAccessor = context.HttpContext.RequestServices
        .GetRequiredService<IPolizasExecutionContextAccessor>();
    try
    {
        if (executionContextAccessor.Current is not null)
        {
            return await next(context);
        }
    }
    catch (PolizasExecutionContextException exception)
    {
        return Results.BadRequest(new ErrorResponse(new ErrorBody(
            Code: "POLIZAS_CONTEXT_INVALID",
            Message: exception.Message,
            CorrelationId: EnsureCorrelationId(context.HttpContext))));
    }

    return Results.BadRequest(new ErrorResponse(new ErrorBody(
        Code: "POLIZAS_CONTEXT_REQUIRED",
        Message: "Broker context is required for SQL polizas requests.",
        CorrelationId: EnsureCorrelationId(context.HttpContext))));
}

static string EnsureCorrelationId(HttpContext context)
{
    const string headerName = "X-Correlation-Id";
    if (context.Items.TryGetValue(headerName, out var existing) &&
        existing is string existingCorrelationId)
    {
        return existingCorrelationId;
    }

    var requestedCorrelationId = context.Request.Headers[headerName].Count == 1
        ? context.Request.Headers[headerName][0]
        : null;
    var correlationId = !string.IsNullOrWhiteSpace(requestedCorrelationId) &&
        requestedCorrelationId.Length <= 100
            ? requestedCorrelationId.Trim()
            : context.TraceIdentifier;

    context.Items[headerName] = correlationId;
    context.Response.Headers[headerName] = correlationId;
    return correlationId;
}

static bool IsKnownConfigurationException(InvalidOperationException exception) =>
    exception.Message.Contains("connection", StringComparison.OrdinalIgnoreCase) ||
    exception.Message.Contains("AppBuilderMaster", StringComparison.OrdinalIgnoreCase) ||
    exception.Message.Contains("Polizas SQL repository", StringComparison.OrdinalIgnoreCase);

static PolizaDetail SanitizeAutosParticularesDetail(PolizaDetail detail) =>
    detail with
    {
        ClienteId = string.Empty,
        Riesgo = string.IsNullOrWhiteSpace(detail.Riesgo) ? string.Empty : "Vehiculo asegurado",
        Documento = string.Empty,
        Email = string.Empty,
        Telefono = string.Empty
    };

static Task WriteErrorAsync(HttpContext context, int statusCode, string code, string message)
{
    context.Response.StatusCode = statusCode;
    return ErrorResult(context, statusCode, code, message).ExecuteAsync(context);
}

static IResult ErrorResult(HttpContext context, int statusCode, string code, string message) =>
    Results.Json(
        new ErrorResponse(new ErrorBody(
            Code: code,
            Message: message,
            CorrelationId: EnsureCorrelationId(context))),
        statusCode: statusCode);

static bool RequiresPolizasExecutionContext(IConfiguration configuration)
{
    if (!string.Equals(configuration["Polizas:Repository"], "Sql", StringComparison.OrdinalIgnoreCase))
    {
        return false;
    }

    if (string.Equals(configuration["Polizas:ConnectionResolver"], "AppBuilderMaster", StringComparison.OrdinalIgnoreCase))
    {
        return true;
    }

    return bool.TryParse(
            configuration["Polizas:RequireExecutionContext"]
            ?? configuration["ILINIUMTECH:REQUIRE_POLIZAS_EXECUTION_CONTEXT"],
            out var requireExecutionContext)
        && requireExecutionContext;
}

static bool IsHeaderExecutionContextEnabled(IConfiguration configuration, IHostEnvironment environment) =>
    HeaderExecutionContextPolicy.IsEnabled(configuration, environment);

static string[] GetAllowedCorsOrigins(IConfiguration configuration)
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
        throw new InvalidOperationException("Cors:AllowedOrigins must define at least one explicit origin.");
    }

    foreach (var origin in normalizedOrigins)
    {
        if (origin.Contains('*', StringComparison.Ordinal))
        {
            throw new InvalidOperationException("CORS wildcard origins are not allowed. Configure explicit origins.");
        }

        if (!Uri.TryCreate(origin, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
        {
            throw new InvalidOperationException($"CORS origin '{origin}' must be an absolute http or https origin.");
        }
    }

    return normalizedOrigins;
}

public partial class Program;

public sealed record ErrorResponse(ErrorBody Error);

public sealed record ErrorBody(string Code, string Message, string? CorrelationId);

public sealed record MeResponse(
    int? BrokerId,
    int? EntityMainId,
    int? UserId,
    int? ProfileId,
    string? ProfileTypeId,
    bool? IsAdmin,
    bool HeaderExecutionContextEnabled,
    bool PolizasExecutionContextRequired);

public sealed record AutosParticularesScope(
    string Ramo,
    string DivisionObjetivo,
    bool DivisionFiltroAplicado,
    bool DivisionPendienteUat);

public sealed record AutosParticularesPolizasResponse(
    IReadOnlyList<PolizaListItem> Items,
    int Page,
    int PageSize,
    int Total,
    AutosParticularesScope Scope);

public sealed record AutosParticularesPolizaDetailResponse(
    PolizaDetail Item,
    AutosParticularesScope Scope);

public sealed record AutosParticularesCatalogsResponse(
    IReadOnlyList<PolizaCatalogOption> Estado,
    IReadOnlyList<PolizaCatalogOption> Compania,
    AutosParticularesScope Scope);
