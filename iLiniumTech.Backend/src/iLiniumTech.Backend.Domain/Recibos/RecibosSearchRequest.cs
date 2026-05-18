namespace iLiniumTech.Backend.Domain.Recibos;

public sealed record RecibosSearchRequest(
    int Page,
    int PageSize,
    string? Sort,
    string? Recibo,
    string? Poliza,
    string? Situacion,
    string? Tipo,
    DateOnly? FechaVencimientoDesde,
    DateOnly? FechaVencimientoHasta);
