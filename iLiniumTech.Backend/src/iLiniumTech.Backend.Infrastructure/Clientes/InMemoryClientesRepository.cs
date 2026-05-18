using iLiniumTech.Backend.Application.Clientes;
using iLiniumTech.Backend.Domain.Clientes;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Infrastructure.Clientes;

public sealed class InMemoryClientesRepository : IClientesRepository
{
    private static readonly IReadOnlyList<ClienteListItem> Items =
    [
        new(
            Id: "CLI-MVP-1001",
            Referencia: "CLI-2026-0001",
            Alias: "Alias anonimo A",
            Estado: "Activo demo",
            Segmento: "Particular demo",
            FechaAlta: new DateOnly(2026, 1, 12),
            Resultado: "Listado minimizado",
            Datos: "PII bloqueada",
            Relacionadas: "Tabs relacionadas pendientes"),
        new(
            Id: "CLI-MVP-1002",
            Referencia: "CLI-2026-0002",
            Alias: "Alias anonimo B",
            Estado: "En revision",
            Segmento: "Empresa demo",
            FechaAlta: new DateOnly(2026, 2, 18),
            Resultado: "Pendiente de SDD",
            Datos: "Datos personales no incluidos",
            Relacionadas: "Polizas y recibos no operativos"),
        new(
            Id: "CLI-MVP-1003",
            Referencia: "CLI-2026-0003",
            Alias: "Alias anonimo C",
            Estado: "Bloqueado PII",
            Segmento: "Colectivo demo",
            FechaAlta: new DateOnly(2026, 3, 5),
            Resultado: "Solo trazabilidad demo",
            Datos: "Contacto y bancarios bloqueados",
            Relacionadas: "Riesgos, siniestros y suplementos pendientes"),
        new(
            Id: "CLI-MVP-1004",
            Referencia: "CLI-2026-0004",
            Alias: "Alias anonimo D",
            Estado: "Activo demo",
            Segmento: "Particular demo",
            FechaAlta: new DateOnly(2026, 4, 21),
            Resultado: "Fixture local",
            Datos: "Identidad legal bloqueada",
            Relacionadas: "Ficha no operativa")
    ];

    private static readonly ClientesCatalogs Catalogs = new(
        Estados: ["Activo demo", "En revision", "Bloqueado PII"],
        Segmentos: ["Particular demo", "Empresa demo", "Colectivo demo"]);

    public Task<PagedResult<ClienteListItem>> SearchAsync(
        ClientesSearchRequest request,
        ClientesSort sort,
        CancellationToken cancellationToken)
    {
        var query = Items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            query = query.Where(item =>
                item.Referencia.Contains(request.Texto, StringComparison.OrdinalIgnoreCase) ||
                item.Alias.Contains(request.Texto, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            query = query.Where(item => item.Estado.Equals(request.Estado, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Segmento))
        {
            query = query.Where(item => item.Segmento.Equals(request.Segmento, StringComparison.OrdinalIgnoreCase));
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

        return Task.FromResult(new PagedResult<ClienteListItem>(items, request.Page, request.PageSize, total));
    }

    public Task<ClientesCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Catalogs);

    private static IEnumerable<ClienteListItem> ApplySort(IEnumerable<ClienteListItem> query, ClientesSort sort)
    {
        Func<ClienteListItem, object> keySelector = sort.Field.ToLowerInvariant() switch
        {
            "referencia" => item => item.Referencia,
            "alias" => item => item.Alias,
            "estado" => item => item.Estado,
            "segmento" => item => item.Segmento,
            "fechaalta" => item => item.FechaAlta,
            _ => item => item.FechaAlta
        };

        return sort.Descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
