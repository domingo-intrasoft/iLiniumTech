using iLiniumTech.Backend.Api.Health;
using iLiniumTech.Backend.Api.Security;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Infrastructure;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

const string AutosParticularesRamo = "Autos";
var autosParticularesScope = new AutosParticularesScope(
    Ramo: AutosParticularesRamo,
    DivisionObjetivo: "Particulares",
    DivisionFiltroAplicado: false,
    DivisionPendienteUat: true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = AuthenticationSchemes.ApiKeyOrDemoSession;
    options.DefaultChallengeScheme = AuthenticationSchemes.ApiKeyOrDemoSession;
    options.DefaultForbidScheme = AuthenticationSchemes.ApiKeyOrDemoSession;
})
    .AddPolicyScheme(AuthenticationSchemes.ApiKeyOrDemoSession, "API key or demo session", options =>
    {
        options.ForwardDefaultSelector = context =>
            context.Request.Cookies.ContainsKey(AuthenticationSchemes.DemoSessionCookieName)
                ? AuthenticationSchemes.DemoSession
                : ApiKeyAuthenticationHandler.SchemeName;
    })
    .AddCookie(AuthenticationSchemes.DemoSession, options =>
    {
        options.Cookie.Name = AuthenticationSchemes.DemoSessionCookieName;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return Task.CompletedTask;
            }
        };
    })
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, _ => { });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolizasAuthorizationPolicies.Catalogs, policy =>
        policy.Requirements.Add(new PolizasPermissionRequirement(PolizasPermissions.Catalogs)));
    options.AddPolicy(PolizasAuthorizationPolicies.Read, policy =>
        policy.Requirements.Add(new PolizasPermissionRequirement(PolizasPermissions.Read)));
    options.AddPolicy(PolizasAuthorizationPolicies.Detail, policy =>
        policy.Requirements.Add(new PolizasPermissionRequirement(PolizasPermissions.Detail)));
    options.AddPolicy(PolizasAuthorizationPolicies.Create, policy =>
        policy.Requirements.Add(new PolizasPermissionRequirement(PolizasPermissions.Create)));
    options.AddPolicy(PolizasAuthorizationPolicies.Update, policy =>
        policy.Requirements.Add(new PolizasPermissionRequirement(PolizasPermissions.Update)));
    options.AddPolicy(PolizasAuthorizationPolicies.Delete, policy =>
        policy.Requirements.Add(new PolizasPermissionRequirement(PolizasPermissions.Delete)));
});
builder.Services.AddSingleton<IAuthorizationHandler, PolizasPermissionAuthorizationHandler>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, SanitizedAuthorizationMiddlewareResultHandler>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IPolizasExecutionContextAccessor, HeaderPolizasExecutionContextAccessor>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCors", policy =>
    {
        policy.WithOrigins(GetAllowedCorsOrigins(builder.Configuration))
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
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

app.MapPost("/api/auth/login", async (
    HttpContext httpContext,
    [FromBody] LoginRequest request,
    [FromServices] IConfiguration configuration,
    [FromServices] IHostEnvironment environment) =>
{
    if (!IsDemoAuthEnabled(configuration, environment))
    {
        return ErrorResult(
            httpContext,
            StatusCodes.Status403Forbidden,
            "AUTH_DEMO_DISABLED",
            "Demo authentication is not enabled for this environment.");
    }

    var username = request.Username?.Trim();
    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrEmpty(request.Password))
    {
        return ErrorResult(
            httpContext,
            StatusCodes.Status400BadRequest,
            "AUTH_VALIDATION_ERROR",
            "Username and password are required.");
    }

    if (request.BrokerId is 0)
    {
        return ErrorResult(
            httpContext,
            StatusCodes.Status400BadRequest,
            "AUTH_BROKER_VALIDATION_ERROR",
            "A non-zero broker identifier is required.");
    }

    var expectedPassword = ReadDemoPassword(configuration, environment);
    if (expectedPassword is null)
    {
        return ErrorResult(
            httpContext,
            StatusCodes.Status403Forbidden,
            "AUTH_DEMO_PASSWORD_REQUIRED",
            "Demo authentication requires explicit non-default credentials in this environment.");
    }

    if (!string.Equals(request.Password, expectedPassword, StringComparison.Ordinal))
    {
        return ErrorResult(
            httpContext,
            StatusCodes.Status401Unauthorized,
            "AUTH_INVALID_CREDENTIALS",
            "Invalid credentials.");
    }

    var session = CreateDemoSession(username, request.BrokerId, configuration);
    if (session.CurrentBrokerId is not null &&
        session.AllowedBrokerIds.Count > 0 &&
        !session.AllowedBrokerIds.Contains(session.CurrentBrokerId.Value))
    {
        return ErrorResult(
            httpContext,
            StatusCodes.Status403Forbidden,
            "AUTH_BROKER_FORBIDDEN",
            "The selected broker is not available for this session.");
    }

    await httpContext.SignInAsync(
        AuthenticationSchemes.DemoSession,
        CreateDemoPrincipal(session),
        new AuthenticationProperties
        {
            IsPersistent = false,
            IssuedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        });

    return Results.Ok(session);
})
.AllowAnonymous()
.WithName("LoginDemo");

app.MapPost("/api/auth/broker", async (
    HttpContext httpContext,
    [FromBody] BrokerSelectionRequest? request) =>
{
    if (request?.BrokerId is null or 0)
    {
        return ErrorResult(
            httpContext,
            StatusCodes.Status400BadRequest,
            "AUTH_BROKER_VALIDATION_ERROR",
            "A non-zero broker identifier is required.");
    }

    var requestedBrokerId = request.BrokerId.Value;
    var allowedBrokerIds = ReadBrokerIdClaims(httpContext.User, PolizasContextClaimTypes.AllowedBrokerId);
    if (allowedBrokerIds.Count == 0 || !allowedBrokerIds.Contains(requestedBrokerId))
    {
        return ErrorResult(
            httpContext,
            StatusCodes.Status403Forbidden,
            "AUTH_BROKER_FORBIDDEN",
            "The selected broker is not available for this session.");
    }

    var session = CreateDemoSessionFromCurrentPrincipal(httpContext.User, requestedBrokerId);
    await httpContext.SignInAsync(
        AuthenticationSchemes.DemoSession,
        CreateDemoPrincipal(session),
        new AuthenticationProperties
        {
            IsPersistent = false,
            IssuedUtc = DateTimeOffset.UtcNow,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        });

    return Results.Ok(session);
})
.RequireAuthorization(policy =>
{
    policy.AddAuthenticationSchemes(AuthenticationSchemes.DemoSession);
    policy.RequireAuthenticatedUser();
})
.WithName("SwitchDemoBroker");

app.MapPost("/api/auth/logout", async (HttpContext httpContext) =>
{
    await httpContext.SignOutAsync(AuthenticationSchemes.DemoSession);
    return Results.NoContent();
})
.AllowAnonymous()
.WithName("LogoutDemo");

app.MapGet("/api/me", (
    HttpContext httpContext,
    [FromServices] IPolizasExecutionContextAccessor executionContextAccessor,
    [FromServices] IConfiguration configuration,
    [FromServices] IHostEnvironment environment) =>
{
    try
    {
        var executionContext = executionContextAccessor.Current;
        if (executionContext is not null &&
            !IsBrokerAllowedForAuthenticatedContext(httpContext.User, executionContext.BrokerId))
        {
            return ErrorResult(
                httpContext,
                StatusCodes.Status403Forbidden,
                "POLIZAS_BROKER_FORBIDDEN",
                "The active broker is not available for this session.");
        }

        return Results.Ok(new MeResponse(
            BrokerId: executionContext?.BrokerId,
            EntityMainId: executionContext?.EntityMainId,
            UserId: executionContext?.UserId,
            ProfileId: executionContext?.ProfileId,
            ProfileTypeId: executionContext?.ProfileTypeId,
            IsAdmin: executionContext?.IsAdmin,
            HeaderExecutionContextEnabled: IsHeaderExecutionContextEnabled(configuration, environment),
            PolizasExecutionContextRequired: RequiresPolizasExecutionContext(configuration),
            User: CreateAuthUserResponse(httpContext.User),
            Application: CreateApplicationResponse(httpContext.User),
            AllowedBrokerIds: ReadBrokerIdClaims(httpContext.User, PolizasContextClaimTypes.AllowedBrokerId),
            Permissions: ReadStringClaims(httpContext.User, PolizasContextClaimTypes.Permission),
            AuthMode: httpContext.User.Identity?.AuthenticationType ?? "unknown"));
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
    .RequireAuthorization(PolizasAuthorizationPolicies.Catalogs)
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
.RequireAuthorization(PolizasAuthorizationPolicies.Read)
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
.RequireAuthorization(PolizasAuthorizationPolicies.Detail)
.AddEndpointFilter(RequirePolizasExecutionContextAsync);

polizas.MapPost("/", async (
    HttpContext httpContext,
    [FromServices] IPolizasService service,
    [FromBody] PolizaCreateRequest? request,
    CancellationToken cancellationToken) =>
{
    try
    {
        var result = await service.CreateAsync(request!, cancellationToken);
        return Results.Created($"/api/polizas/{result.Id}", result);
    }
    catch (PolizasValidationException exception)
    {
        return PolizasValidationErrorResult(httpContext, exception);
    }
})
    .WithName("CreatePoliza")
    .RequireAuthorization(PolizasAuthorizationPolicies.Create)
    .AddEndpointFilter(RequirePolizasWritesEnabledAsync)
    .AddEndpointFilter(RequirePolizasExecutionContextAsync);

polizas.MapPut("/{id}", async (
    HttpContext httpContext,
    [FromServices] IPolizasService service,
    string id,
    [FromBody] PolizaUpdateRequest? request,
    CancellationToken cancellationToken) =>
{
    try
    {
        var updated = await service.UpdateAsync(id, request!, cancellationToken);
        return updated
            ? Results.NoContent()
            : PolizasNotFoundOrNotWritableResult(httpContext);
    }
    catch (PolizasValidationException exception)
    {
        return PolizasValidationErrorResult(httpContext, exception);
    }
})
    .WithName("UpdatePoliza")
    .RequireAuthorization(PolizasAuthorizationPolicies.Update)
    .AddEndpointFilter(RequirePolizasWritesEnabledAsync)
    .AddEndpointFilter(RequirePolizasExecutionContextAsync);

polizas.MapDelete("/{id}", async (
    HttpContext httpContext,
    [FromServices] IPolizasService service,
    string id,
    CancellationToken cancellationToken) =>
{
    try
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted
            ? Results.NoContent()
            : PolizasNotFoundOrNotWritableResult(httpContext);
    }
    catch (PolizasValidationException exception)
    {
        return PolizasValidationErrorResult(httpContext, exception);
    }
})
    .WithName("DeletePoliza")
    .RequireAuthorization(PolizasAuthorizationPolicies.Delete)
    .AddEndpointFilter(RequirePolizasWritesEnabledAsync)
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
.RequireAuthorization(PolizasAuthorizationPolicies.Catalogs)
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
            Items: result.Items.Select(SanitizeAutosParticularesListItem).ToArray(),
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
.RequireAuthorization(PolizasAuthorizationPolicies.Read)
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
.RequireAuthorization(PolizasAuthorizationPolicies.Detail)
.AddEndpointFilter(RequirePolizasExecutionContextAsync);

app.Run();

static ValueTask<object?> RequirePolizasWritesEnabledAsync(
    EndpointFilterInvocationContext context,
    EndpointFilterDelegate next)
{
    var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
    var environment = context.HttpContext.RequestServices.GetRequiredService<IHostEnvironment>();

    return IsPolizasWritesEnabled(configuration, environment)
        ? next(context)
        : ValueTask.FromResult<object?>(ErrorResult(
            context.HttpContext,
            StatusCodes.Status403Forbidden,
            "POLIZAS_WRITES_DISABLED",
            "Polizas write operations are disabled for this environment."));
}

static async ValueTask<object?> RequirePolizasExecutionContextAsync(
    EndpointFilterInvocationContext context,
    EndpointFilterDelegate next)
{
    var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
    var requiresExecutionContext = RequiresPolizasExecutionContext(configuration);
    var executionContextAccessor = context.HttpContext.RequestServices
        .GetRequiredService<IPolizasExecutionContextAccessor>();
    PolizasExecutionContext? executionContext;
    try
    {
        executionContext = executionContextAccessor.Current;
    }
    catch (PolizasExecutionContextException exception)
    {
        return Results.BadRequest(new ErrorResponse(new ErrorBody(
            Code: "POLIZAS_CONTEXT_INVALID",
            Message: exception.Message,
            CorrelationId: EnsureCorrelationId(context.HttpContext))));
    }

    if (executionContext is null)
    {
        return requiresExecutionContext
            ? Results.BadRequest(new ErrorResponse(new ErrorBody(
                Code: "POLIZAS_CONTEXT_REQUIRED",
                Message: "Broker context is required for SQL polizas requests.",
                CorrelationId: EnsureCorrelationId(context.HttpContext))))
            : await next(context);
    }

    if (!IsBrokerAllowedForAuthenticatedContext(context.HttpContext.User, executionContext.BrokerId))
    {
        return ErrorResult(
            context.HttpContext,
            StatusCodes.Status403Forbidden,
            "POLIZAS_BROKER_FORBIDDEN",
            "The active broker is not available for this session.");
    }

    return await next(context);
}

static bool IsBrokerAllowedForAuthenticatedContext(ClaimsPrincipal user, int brokerId)
{
    if (string.Equals(
            user.Identity?.AuthenticationType,
            ApiKeyAuthenticationHandler.SchemeName,
            StringComparison.Ordinal))
    {
        return true;
    }

    var allowedBrokerIds = ReadBrokerIdClaims(user, PolizasContextClaimTypes.AllowedBrokerId);
    return allowedBrokerIds.Contains(brokerId);
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

static PolizaListItem SanitizeAutosParticularesListItem(PolizaListItem item) =>
    item with
    {
        ClienteId = string.Empty,
        Documento = string.Empty,
        Riesgo = string.IsNullOrWhiteSpace(item.Riesgo) ? string.Empty : "Vehiculo asegurado"
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

static IResult PolizasNotFoundOrNotWritableResult(HttpContext context) =>
    ErrorResult(
        context,
        StatusCodes.Status404NotFound,
        "POLIZAS_NOT_FOUND_OR_NOT_WRITABLE",
        "Poliza no encontrada o no modificable.");

static IResult PolizasValidationErrorResult(HttpContext context, PolizasValidationException exception) =>
    Results.BadRequest(new ErrorResponse(new ErrorBody(
        Code: "POLIZAS_VALIDATION_ERROR",
        Message: exception.Message,
        CorrelationId: EnsureCorrelationId(context))));

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

static bool IsPolizasWritesEnabled(IConfiguration configuration, IHostEnvironment environment)
{
    var configured = bool.TryParse(
            configuration["Polizas:WritesEnabled"]
            ?? configuration["ILINIUMTECH:POLIZAS_WRITES_ENABLED"],
            out var enabled)
        && enabled;
    if (!configured)
    {
        return false;
    }

    return environment.IsDevelopment() ||
        string.Equals(
            configuration["Polizas:WritesEnabledDemoOptIn"],
            HeaderExecutionContextPolicy.DemoOptInRequiredValue,
            StringComparison.Ordinal);
}

static bool IsHeaderExecutionContextEnabled(IConfiguration configuration, IHostEnvironment environment) =>
    HeaderExecutionContextPolicy.IsEnabled(configuration, environment);

static bool IsDemoAuthEnabled(IConfiguration configuration, IHostEnvironment environment)
{
    var configured = bool.TryParse(configuration["Auth:Demo:Enabled"], out var enabled) && enabled;
    if (environment.IsDevelopment())
    {
        return configuration["Auth:Demo:Enabled"] is null || configured;
    }

    return configured &&
        string.Equals(
            configuration["Auth:Demo:OptIn"],
            HeaderExecutionContextPolicy.DemoOptInRequiredValue,
            StringComparison.Ordinal);
}

static string? ReadDemoPassword(IConfiguration configuration, IHostEnvironment environment)
{
    var configuredPassword = configuration["Auth:Demo:Password"];
    if (environment.IsDevelopment())
    {
        return configuredPassword ?? "demo";
    }

    return string.IsNullOrWhiteSpace(configuredPassword) ||
        string.Equals(configuredPassword, "demo", StringComparison.Ordinal) ||
        string.Equals(configuredPassword, "__SET_IN_ENVIRONMENT__", StringComparison.Ordinal)
            ? null
            : configuredPassword;
}

static LoginResponse CreateDemoSession(string username, int? requestedBrokerId, IConfiguration configuration)
{
    var brokerId = requestedBrokerId is not null and not 0
        ? requestedBrokerId
        : ReadNonZeroIntConfiguration(configuration, "Auth:Demo:BrokerId")
            ?? ReadNonZeroIntConfiguration(configuration, "Polizas:BrokerId");
    var userId = ReadPositiveIntConfiguration(configuration, "Auth:Demo:UserId")
        ?? ReadPositiveIntConfiguration(configuration, "Polizas:UserId")
        ?? 1;
    var profileId = ReadPositiveIntConfiguration(configuration, "Auth:Demo:ProfileId")
        ?? ReadPositiveIntConfiguration(configuration, "Polizas:ProfileId");
    var profileTypeId = configuration["Auth:Demo:ProfileTypeId"]
        ?? configuration["Polizas:ProfileTypeId"];
    var isAdmin = bool.TryParse(configuration["Auth:Demo:IsAdmin"] ?? configuration["Polizas:IsAdmin"], out var parsedIsAdmin)
        ? parsedIsAdmin
        : false;

    return new LoginResponse(
        Session: new LoginSessionResponse(
            Mode: "demo",
            ExpiresAt: DateTimeOffset.UtcNow.AddHours(8)),
        User: new AuthUserResponse(
            Id: $"demo:{SanitizeIdentifier(DisplayNameFromUsername(username))}",
            DisplayName: DisplayNameFromUsername(username)),
        Application: new AuthApplicationResponse(
            Key: "iliniumtech",
            Name: "iLiniumTech"),
        CurrentBrokerId: brokerId,
        UserId: userId,
        ProfileId: profileId,
        ProfileTypeId: string.IsNullOrWhiteSpace(profileTypeId) ? null : profileTypeId,
        IsAdmin: isAdmin,
        AllowedBrokerIds: ReadDemoAllowedBrokerIds(configuration, brokerId),
        Permissions: ReadDemoPermissions(configuration));
}

static LoginResponse CreateDemoSessionFromCurrentPrincipal(ClaimsPrincipal user, int brokerId)
{
    return new LoginResponse(
        Session: new LoginSessionResponse(
            Mode: "demo",
            ExpiresAt: DateTimeOffset.UtcNow.AddHours(8)),
        User: CreateAuthUserResponse(user) ?? new AuthUserResponse("demo:user", "demo"),
        Application: CreateApplicationResponse(user) ?? new AuthApplicationResponse("iliniumtech", "iLiniumTech"),
        CurrentBrokerId: brokerId,
        UserId: ReadFirstIntClaim(user, PolizasContextClaimTypes.UserId) ?? 1,
        ProfileId: ReadFirstIntClaim(user, PolizasContextClaimTypes.ProfileId),
        ProfileTypeId: ReadFirstStringClaim(user, PolizasContextClaimTypes.ProfileTypeId),
        IsAdmin: ReadBoolClaim(user, PolizasContextClaimTypes.IsAdmin) ?? false,
        AllowedBrokerIds: ReadBrokerIdClaims(user, PolizasContextClaimTypes.AllowedBrokerId),
        Permissions: ReadStringClaims(user, PolizasContextClaimTypes.Permission));
}

static ClaimsPrincipal CreateDemoPrincipal(LoginResponse session)
{
    var claims = new List<Claim>
    {
        new(ClaimTypes.NameIdentifier, session.User.Id),
        new(ClaimTypes.Name, session.User.DisplayName),
        new(PolizasContextClaimTypes.ApplicationKey, session.Application.Key),
        new(PolizasContextClaimTypes.ApplicationName, session.Application.Name),
        new(PolizasContextClaimTypes.UserId, session.UserId.ToString()),
        new(PolizasContextClaimTypes.IsAdmin, session.IsAdmin.ToString())
    };

    if (session.CurrentBrokerId is not null)
    {
        claims.Add(new Claim(PolizasContextClaimTypes.BrokerId, session.CurrentBrokerId.Value.ToString()));
    }

    if (session.ProfileId is not null)
    {
        claims.Add(new Claim(PolizasContextClaimTypes.ProfileId, session.ProfileId.Value.ToString()));
    }

    if (!string.IsNullOrWhiteSpace(session.ProfileTypeId))
    {
        claims.Add(new Claim(PolizasContextClaimTypes.ProfileTypeId, session.ProfileTypeId));
    }

    foreach (var brokerId in session.AllowedBrokerIds)
    {
        claims.Add(new Claim(PolizasContextClaimTypes.AllowedBrokerId, brokerId.ToString()));
    }

    foreach (var permission in session.Permissions)
    {
        claims.Add(new Claim(PolizasContextClaimTypes.Permission, permission));
    }

    return new ClaimsPrincipal(new ClaimsIdentity(claims, AuthenticationSchemes.DemoSession));
}

static AuthUserResponse? CreateAuthUserResponse(ClaimsPrincipal user)
{
    var id = user.FindFirstValue(ClaimTypes.NameIdentifier);
    var displayName = user.FindFirstValue(ClaimTypes.Name);
    return string.IsNullOrWhiteSpace(id) || string.IsNullOrWhiteSpace(displayName)
        ? null
        : new AuthUserResponse(id, displayName);
}

static AuthApplicationResponse? CreateApplicationResponse(ClaimsPrincipal user)
{
    var key = user.FindFirstValue(PolizasContextClaimTypes.ApplicationKey);
    var name = user.FindFirstValue(PolizasContextClaimTypes.ApplicationName);
    return string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(name)
        ? null
        : new AuthApplicationResponse(key, name);
}

static IReadOnlyList<int> ReadIntClaims(ClaimsPrincipal user, string claimType) =>
    user.FindAll(claimType)
        .Select(claim => int.TryParse(claim.Value, out var parsed) ? parsed : (int?)null)
        .Where(value => value is > 0)
        .Select(value => value!.Value)
        .Distinct()
        .ToArray();

static IReadOnlyList<int> ReadBrokerIdClaims(ClaimsPrincipal user, string claimType) =>
    user.FindAll(claimType)
        .Select(claim => int.TryParse(claim.Value, out var parsed) ? parsed : (int?)null)
        .Where(value => value is not null and not 0)
        .Select(value => value!.Value)
        .Distinct()
        .ToArray();

static int? ReadFirstIntClaim(ClaimsPrincipal user, string claimType)
{
    var values = ReadIntClaims(user, claimType);
    return values.Count == 0 ? null : values[0];
}

static bool? ReadBoolClaim(ClaimsPrincipal user, string claimType)
{
    var value = ReadFirstStringClaim(user, claimType);
    return bool.TryParse(value, out var parsed) ? parsed : null;
}

static string? ReadFirstStringClaim(ClaimsPrincipal user, string claimType) =>
    user.FindFirstValue(claimType);

static IReadOnlyList<string> ReadStringClaims(ClaimsPrincipal user, string claimType) =>
    user.FindAll(claimType)
        .Select(claim => claim.Value)
        .Where(value => !string.IsNullOrWhiteSpace(value))
        .Distinct(StringComparer.Ordinal)
        .ToArray();

static IReadOnlyList<string> ReadDemoPermissions(IConfiguration configuration)
{
    var configuredPermissions = configuration.GetSection("Auth:Demo:Permissions");
    if (configuredPermissions.Exists())
    {
        return configuredPermissions.Get<string[]>()?
            .Where(permission => !string.IsNullOrWhiteSpace(permission))
            .Select(permission => permission.Trim())
            .Where(PolizasPermissions.IsActive)
            .Distinct(StringComparer.Ordinal)
            .ToArray() ?? [];
    }

    return
    [
        PolizasPermissions.Catalogs,
        PolizasPermissions.Read,
        PolizasPermissions.Detail
    ];
}

static IReadOnlyList<int> ReadDemoAllowedBrokerIds(IConfiguration configuration, int? brokerId)
{
    var configuredBrokerIds = configuration.GetSection("Auth:Demo:AllowedBrokerIds");
    if (configuredBrokerIds.Exists())
    {
        return configuredBrokerIds.Get<int[]>()?
            .Where(value => value != 0)
            .Distinct()
            .ToArray() ?? [];
    }

    return brokerId is null ? [] : [brokerId.Value];
}

static int? ReadPositiveIntConfiguration(IConfiguration configuration, string key) =>
    int.TryParse(configuration[key], out var parsed) && parsed > 0 ? parsed : null;

static int? ReadNonZeroIntConfiguration(IConfiguration configuration, string key) =>
    int.TryParse(configuration[key], out var parsed) && parsed != 0 ? parsed : null;

static string DisplayNameFromUsername(string username)
{
    var atIndex = username.IndexOf('@', StringComparison.Ordinal);
    return atIndex > 0 ? username[..atIndex] : username;
}

static string SanitizeIdentifier(string value)
{
    var cleaned = new string(value
        .Where(character => char.IsLetterOrDigit(character) || character is '-' or '_')
        .ToArray());
    return string.IsNullOrWhiteSpace(cleaned) ? "user" : cleaned.ToLowerInvariant();
}

static string[] GetAllowedCorsOrigins(IConfiguration configuration)
{
    var origins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
        ?? [
            "http://localhost:5173",
            "http://127.0.0.1:5173",
            "http://localhost:5174",
            "http://127.0.0.1:5174"
        ];
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
    bool PolizasExecutionContextRequired,
    AuthUserResponse? User,
    AuthApplicationResponse? Application,
    IReadOnlyList<int> AllowedBrokerIds,
    IReadOnlyList<string> Permissions,
    string AuthMode);

public sealed record LoginRequest(string? Username, string? Password, int? BrokerId);

public sealed record BrokerSelectionRequest(int? BrokerId);

public sealed record LoginSessionResponse(string Mode, DateTimeOffset ExpiresAt);

public sealed record AuthUserResponse(string Id, string DisplayName);

public sealed record AuthApplicationResponse(string Key, string Name);

public sealed record LoginResponse(
    LoginSessionResponse Session,
    AuthUserResponse User,
    AuthApplicationResponse Application,
    int? CurrentBrokerId,
    int UserId,
    int? ProfileId,
    string? ProfileTypeId,
    bool IsAdmin,
    IReadOnlyList<int> AllowedBrokerIds,
    IReadOnlyList<string> Permissions);

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
