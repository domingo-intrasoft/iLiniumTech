namespace iLiniumTech.Backend.Domain.Clientes;

public sealed record ClientesSearchRequest(
    int Page,
    int PageSize,
    string? Sort,
    string? Texto,
    string? Estado,
    string? Segmento,
    DateOnly? FechaAltaDesde,
    DateOnly? FechaAltaHasta);
