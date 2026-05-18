namespace iLiniumTech.Backend.Domain.Polizas;

public sealed record PolizaCreateRequest(
    string? Numero,
    string? Aplicacion,
    int? CiaId,
    int? ClienteId,
    string? Estado,
    string? Ramo,
    string? TipoPoliza,
    DateOnly? FechaEfecto,
    DateOnly? FechaVencimiento,
    decimal? PrimaAnual);

public sealed record PolizaUpdateRequest(
    string? Numero = null,
    string? Aplicacion = null,
    string? Estado = null,
    string? Ramo = null,
    string? TipoPoliza = null,
    DateOnly? FechaEfecto = null,
    DateOnly? FechaVencimiento = null,
    decimal? PrimaAnual = null);
