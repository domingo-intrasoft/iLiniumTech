using iLiniumTech.Backend.Api.Security;
using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Infrastructure;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddAuthentication(ApiKeyAuthenticationHandler.SchemeName)
    .AddScheme<AuthenticationSchemeOptions, ApiKeyAuthenticationHandler>(ApiKeyAuthenticationHandler.SchemeName, _ => { });
builder.Services.AddAuthorization();
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

builder.Services.AddSingleton<IPolizasService, PolizasService>();
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
    .WithName("GetPolizasCatalogs");

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
.WithName("SearchPolizas");

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
.WithName("GetPolizaById");

app.Run();

public partial class Program;

public sealed record ErrorResponse(ErrorBody Error);

public sealed record ErrorBody(string Code, string Message, string? CorrelationId);
