namespace iLiniumTech.Backend.Domain.Propuestas;

public sealed record PropuestasSearchRequest(
    int Page,
    int PageSize,
    string? Sort,
    string? Referencia,
    string? Estado,
    string? Ramo,
    string? Canal,
    DateOnly? FechaAltaDesde,
    DateOnly? FechaAltaHasta);
