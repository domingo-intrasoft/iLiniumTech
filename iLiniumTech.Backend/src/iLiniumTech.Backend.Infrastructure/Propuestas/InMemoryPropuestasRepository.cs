using iLiniumTech.Backend.Application.Propuestas;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Propuestas;

namespace iLiniumTech.Backend.Infrastructure.Propuestas;

public sealed class InMemoryPropuestasRepository : IPropuestasRepository
{
    private static readonly IReadOnlyList<PropuestaListItem> Items =
    [
        new(
            Id: "PROP-MVP-1001",
            Referencia: "PROP-2026-0001",
            Estado: "Borrador demo",
            Ramo: "Autos demo",
            FechaAlta: new DateOnly(2026, 1, 12),
            Canal: "Canal demo mediador"),
        new(
            Id: "PROP-MVP-1002",
            Referencia: "PROP-2026-0002",
            Estado: "En revision demo",
            Ramo: "Hogar demo",
            FechaAlta: new DateOnly(2026, 2, 20),
            Canal: "Canal demo oficina"),
        new(
            Id: "PROP-MVP-1003",
            Referencia: "PROP-2026-0003",
            Estado: "Caducada demo",
            Ramo: "Comercio demo",
            FechaAlta: new DateOnly(2026, 3, 18),
            Canal: "Canal demo interno"),
        new(
            Id: "PROP-MVP-1004",
            Referencia: "PROP-2026-0004",
            Estado: "Bloqueada demo",
            Ramo: "Salud demo",
            FechaAlta: new DateOnly(2026, 4, 5),
            Canal: "Canal demo compania")
    ];

    private static readonly PropuestasCatalogs Catalogs = new(
        Estados: ["Borrador demo", "En revision demo", "Caducada demo", "Bloqueada demo"],
        Ramos: ["Autos demo", "Hogar demo", "Comercio demo", "Salud demo"],
        Canales: ["Canal demo mediador", "Canal demo oficina", "Canal demo interno", "Canal demo compania"]);

    public Task<PagedResult<PropuestaListItem>> SearchAsync(
        PropuestasSearchRequest request,
        PropuestasSort sort,
        CancellationToken cancellationToken)
    {
        var query = Items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Referencia))
        {
            query = query.Where(item => item.Referencia.Contains(request.Referencia, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            query = query.Where(item => item.Estado.Equals(request.Estado, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Ramo))
        {
            query = query.Where(item => item.Ramo.Equals(request.Ramo, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Canal))
        {
            query = query.Where(item => item.Canal.Equals(request.Canal, StringComparison.OrdinalIgnoreCase));
        }

        if (request.FechaAltaDesde.HasValue)
        {
            query = query.Where(item => item.FechaAlta >= request.FechaAltaDesde.Value);
        }

        if (request.FechaAltaHasta.HasValue)
        {
            query = query.Where(item => item.FechaAlta <= request.FechaAltaHasta.Value);
        }

        query = ApplySort(query, sort);

        var total = query.Count();
        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();

        return Task.FromResult(new PagedResult<PropuestaListItem>(items, request.Page, request.PageSize, total));
    }

    public Task<PropuestasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Catalogs);

    private static IEnumerable<PropuestaListItem> ApplySort(IEnumerable<PropuestaListItem> query, PropuestasSort sort)
    {
        Func<PropuestaListItem, object> keySelector = sort.Field.ToLowerInvariant() switch
        {
            "referencia" => item => item.Referencia,
            "estado" => item => item.Estado,
            "ramo" => item => item.Ramo,
            "canal" => item => item.Canal,
            "fechaalta" => item => item.FechaAlta,
            _ => item => item.FechaAlta
        };

        return sort.Descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
