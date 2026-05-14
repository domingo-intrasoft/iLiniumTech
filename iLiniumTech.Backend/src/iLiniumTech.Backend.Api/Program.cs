using iLiniumTech.Backend.Api.Security;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Infrastructure;
using iLiniumTech.Backend.Infrastructure.Polizas.Connections;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

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
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:5173"];
        policy.WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddScoped<IPolizasService, PolizasService>();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("DefaultCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new
{
    status = "ok",
    application = "iLiniumTech.Backend"
}));

var polizas = app.MapGroup("/api/polizas")
    .RequireAuthorization();

polizas.MapGet("/metadata", () => Results.Problem(
        statusCode: StatusCodes.Status410Gone,
        title: "Polizas metadata endpoint is deprecated.",
        detail: "Runtime screen design metadata is no longer served by the backend. Use /api/polizas/catalogs for policy catalogs."))
    .WithName("GetPolizasMetadataDeprecated");

polizas.MapGet("/catalogs", async ([FromServices] IPolizasService service, CancellationToken cancellationToken) =>
        Results.Ok(await service.GetCatalogsAsync(cancellationToken)))
    .WithName("GetPolizasCatalogs")
    .AddEndpointFilter(RequirePolizasExecutionContextAsync);

polizas.MapGet("/", async (
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
            CorrelationId: null)));
    }
})
.WithName("SearchPolizas")
.AddEndpointFilter(RequirePolizasExecutionContextAsync);

polizas.MapGet("/{id}", async ([FromServices] IPolizasService service, string id, CancellationToken cancellationToken) =>
{
    try
    {
        var result = await service.GetByIdAsync(id, cancellationToken);
        return result is null ? Results.NotFound() : Results.Ok(result);
    }
    catch (PolizasValidationException exception)
    {
        return Results.BadRequest(new ErrorResponse(new ErrorBody(
            Code: "POLIZAS_VALIDATION_ERROR",
            Message: exception.Message,
            CorrelationId: null)));
    }
})
.WithName("GetPolizaById")
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
            CorrelationId: null)));
    }

    return Results.BadRequest(new ErrorResponse(new ErrorBody(
        Code: "POLIZAS_CONTEXT_REQUIRED",
        Message: "Broker context is required for SQL polizas requests.",
        CorrelationId: null)));
}

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

public partial class Program;

public sealed record ErrorResponse(ErrorBody Error);

public sealed record ErrorBody(string Code, string Message, string? CorrelationId);
