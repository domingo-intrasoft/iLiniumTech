namespace iLiniumTech.Backend.Domain.Suplementos;

public sealed record SuplementosSearchRequest(
    int Page,
    int PageSize,
    string? Sort,
    string? Referencia,
    string? Poliza,
    string? Tipo,
    string? Situacion,
    DateOnly? FechaEfectoDesde,
    DateOnly? FechaEfectoHasta);
