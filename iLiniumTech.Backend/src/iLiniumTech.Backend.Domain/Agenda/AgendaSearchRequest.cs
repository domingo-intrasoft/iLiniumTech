namespace iLiniumTech.Backend.Domain.Agenda;

public sealed record AgendaSearchRequest(
    int Page,
    int PageSize,
    string? Sort,
    string? Texto,
    string? Estado,
    string? Prioridad,
    string? Origen,
    DateOnly? FechaDesde,
    DateOnly? FechaHasta);
