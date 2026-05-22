namespace iLiniumTech.Backend.Domain.Polizas;

public sealed record PolizasSearchRequest(
    int Page = 1,
    int PageSize = 25,
    string? Sort = null,
    string? Numero = null,
    string? Cliente = null,
    string? Estado = null,
    DateOnly? FechaEfectoDesde = null,
    DateOnly? FechaEfectoHasta = null,
    string? Compania = null,
    string? Ramo = null,
    string? Documento = null);

public sealed record PolizasSort(
    string Field,
    bool Descending);
