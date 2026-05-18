using iLiniumTech.Backend.Application.Siniestros;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Siniestros;

namespace iLiniumTech.Backend.Infrastructure.Siniestros;

public sealed class InMemorySiniestrosRepository : ISiniestrosRepository
{
    private static readonly IReadOnlyList<SiniestroListItem> Items =
    [
        new(
            Id: "SIN-MVP-1001",
            Referencia: "SIN-2026-0001",
            Poliza: "POL-2026-0001",
            Cliente: "Cliente anonimo 1",
            Compania: "Compania demo norte",
            Situacion: "Pendiente de documentacion",
            Estado: "En revision",
            Prioridad: "Alta",
            FechaSiniestro: new DateOnly(2026, 2, 4),
            FechaParte: new DateOnly(2026, 2, 5),
            Tramitador: "Equipo tramitacion A"),
        new(
            Id: "SIN-MVP-1002",
            Referencia: "SIN-2026-0002",
            Poliza: "POL-2026-0002",
            Cliente: "Cliente anonimo 2",
            Compania: "Compania demo sur",
            Situacion: "Peritacion solicitada",
            Estado: "Abierto",
            Prioridad: "Media",
            FechaSiniestro: new DateOnly(2026, 3, 12),
            FechaParte: new DateOnly(2026, 3, 13),
            Tramitador: "Equipo tramitacion B"),
        new(
            Id: "SIN-MVP-1003",
            Referencia: "SIN-2026-0003",
            Poliza: "POL-2026-0003",
            Cliente: "Cliente anonimo 3",
            Compania: "Compania demo este",
            Situacion: "Cierre tecnico validado",
            Estado: "Cerrado",
            Prioridad: "Baja",
            FechaSiniestro: new DateOnly(2026, 1, 18),
            FechaParte: new DateOnly(2026, 1, 20),
            Tramitador: "Equipo tramitacion C")
    ];

    private static readonly SiniestrosCatalogs Catalogs = new(
        Estados: ["En revision", "Abierto", "Cerrado"],
        Prioridades: ["Alta", "Media", "Baja"]);

    public Task<PagedResult<SiniestroListItem>> SearchAsync(
        SiniestrosSearchRequest request,
        SiniestrosSort sort,
        CancellationToken cancellationToken)
    {
        var query = Items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Referencia))
        {
            query = query.Where(item =>
                item.Referencia.Contains(request.Referencia, StringComparison.OrdinalIgnoreCase) ||
                item.Cliente.Contains(request.Referencia, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Poliza))
        {
            query = query.Where(item => item.Poliza.Contains(request.Poliza, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            query = query.Where(item => item.Estado.Equals(request.Estado, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Prioridad))
        {
            query = query.Where(item => item.Prioridad.Equals(request.Prioridad, StringComparison.OrdinalIgnoreCase));
        }

        if (request.FechaSiniestroDesde.HasValue)
        {
            query = query.Where(item => item.FechaSiniestro >= request.FechaSiniestroDesde.Value);
        }

        if (request.FechaSiniestroHasta.HasValue)
        {
            query = query.Where(item => item.FechaSiniestro <= request.FechaSiniestroHasta.Value);
        }

        query = ApplySort(query, sort);

        var total = query.Count();
        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();

        return Task.FromResult(new PagedResult<SiniestroListItem>(items, request.Page, request.PageSize, total));
    }

    public Task<SiniestrosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Catalogs);

    private static IEnumerable<SiniestroListItem> ApplySort(IEnumerable<SiniestroListItem> query, SiniestrosSort sort)
    {
        Func<SiniestroListItem, object> keySelector = sort.Field.ToLowerInvariant() switch
        {
            "referencia" => item => item.Referencia,
            "poliza" => item => item.Poliza,
            "estado" => item => item.Estado,
            "prioridad" => item => item.Prioridad,
            "compania" => item => item.Compania,
            "fechasiniestro" => item => item.FechaSiniestro,
            "fechaparte" => item => item.FechaParte,
            _ => item => item.FechaSiniestro
        };

        return sort.Descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
