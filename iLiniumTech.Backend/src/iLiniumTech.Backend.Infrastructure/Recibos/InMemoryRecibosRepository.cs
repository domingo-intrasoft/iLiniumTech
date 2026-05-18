using iLiniumTech.Backend.Application.Recibos;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Recibos;

namespace iLiniumTech.Backend.Infrastructure.Recibos;

public sealed class InMemoryRecibosRepository : IRecibosRepository
{
    private static readonly IReadOnlyList<ReciboListItem> Items =
    [
        new(
            Id: "REC-MVP-1001",
            Recibo: "REC-2026-0001",
            Poliza: "POL-2026-0001",
            Cliente: "Cliente anonimo 1",
            Compania: "Compania demo norte",
            Tipo: "Prima",
            Situacion: "Pendiente",
            FechaEfecto: new DateOnly(2026, 1, 1),
            FechaVencimiento: new DateOnly(2026, 2, 1),
            EstadoCobro: "No operativo",
            Canal: "Canal demo"),
        new(
            Id: "REC-MVP-1002",
            Recibo: "REC-2026-0002",
            Poliza: "POL-2026-0002",
            Cliente: "Cliente anonimo 2",
            Compania: "Compania demo sur",
            Tipo: "Regularizacion",
            Situacion: "Cobrado",
            FechaEfecto: new DateOnly(2026, 2, 15),
            FechaVencimiento: new DateOnly(2026, 3, 15),
            EstadoCobro: "Cobro demo confirmado",
            Canal: "Canal demo mediador"),
        new(
            Id: "REC-MVP-1003",
            Recibo: "REC-2026-0003",
            Poliza: "POL-2026-0003",
            Cliente: "Cliente anonimo 3",
            Compania: "Compania demo este",
            Tipo: "Extorno",
            Situacion: "Anulado",
            FechaEfecto: new DateOnly(2026, 4, 1),
            FechaVencimiento: new DateOnly(2026, 5, 1),
            EstadoCobro: "No operativo",
            Canal: "Canal demo compania")
    ];

    private static readonly RecibosCatalogs Catalogs = new(
        Situaciones: ["Pendiente", "Cobrado", "Anulado"],
        Tipos: ["Prima", "Extorno", "Regularizacion"],
        Canales: ["Canal demo", "Canal demo mediador", "Canal demo compania"]);

    public Task<PagedResult<ReciboListItem>> SearchAsync(
        RecibosSearchRequest request,
        RecibosSort sort,
        CancellationToken cancellationToken)
    {
        var query = Items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Recibo))
        {
            query = query.Where(item =>
                item.Recibo.Contains(request.Recibo, StringComparison.OrdinalIgnoreCase) ||
                item.Cliente.Contains(request.Recibo, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Poliza))
        {
            query = query.Where(item => item.Poliza.Contains(request.Poliza, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Situacion))
        {
            query = query.Where(item => item.Situacion.Equals(request.Situacion, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            query = query.Where(item => item.Tipo.Equals(request.Tipo, StringComparison.OrdinalIgnoreCase));
        }

        if (request.FechaVencimientoDesde.HasValue)
        {
            query = query.Where(item => item.FechaVencimiento >= request.FechaVencimientoDesde.Value);
        }

        if (request.FechaVencimientoHasta.HasValue)
        {
            query = query.Where(item => item.FechaVencimiento <= request.FechaVencimientoHasta.Value);
        }

        query = ApplySort(query, sort);

        var total = query.Count();
        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();

        return Task.FromResult(new PagedResult<ReciboListItem>(items, request.Page, request.PageSize, total));
    }

    public Task<RecibosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Catalogs);

    private static IEnumerable<ReciboListItem> ApplySort(IEnumerable<ReciboListItem> query, RecibosSort sort)
    {
        Func<ReciboListItem, object> keySelector = sort.Field.ToLowerInvariant() switch
        {
            "recibo" => item => item.Recibo,
            "poliza" => item => item.Poliza,
            "situacion" => item => item.Situacion,
            "tipo" => item => item.Tipo,
            "compania" => item => item.Compania,
            "fechaefecto" => item => item.FechaEfecto,
            "fechavencimiento" => item => item.FechaVencimiento,
            _ => item => item.FechaVencimiento
        };

        return sort.Descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
