namespace iLiniumTech.Backend.Domain.Polizas;

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int Total);
