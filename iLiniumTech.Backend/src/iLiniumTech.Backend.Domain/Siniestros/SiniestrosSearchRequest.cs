namespace iLiniumTech.Backend.Domain.Siniestros;

public sealed record SiniestrosSearchRequest(
    int Page,
    int PageSize,
    string? Sort,
    string? Referencia,
    string? Poliza,
    string? Estado,
    string? Prioridad,
    DateOnly? FechaSiniestroDesde,
    DateOnly? FechaSiniestroHasta);
