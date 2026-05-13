using iLiniumTech.Backend.Application.Polizas;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Infrastructure.Polizas;

public sealed class InMemoryPolizasRepository : IPolizasRepository
{
    private static readonly IReadOnlyList<PolizaListItem> Items =
    [
        new(
            Id: "POL-1001",
            Numero: "POL-2026-0001",
            Aplicacion: "Auto",
            Estado: "Vigor",
            Ramo: "Autos",
            ClienteId: "CLI-001",
            ClienteNombre: "Cliente anonimo 1",
            Compania: "Compania demo",
            FechaEfecto: new DateOnly(2026, 1, 1),
            FechaVencimiento: new DateOnly(2026, 12, 31),
            PrimaAnual: 420.50m,
            Moneda: "EUR"),
        new(
            Id: "POL-1002",
            Numero: "POL-2026-0002",
            Aplicacion: "Hogar",
            Estado: "Pendiente",
            Ramo: "Hogar",
            ClienteId: "CLI-002",
            ClienteNombre: "Cliente anonimo 2",
            Compania: "Compania demo",
            FechaEfecto: new DateOnly(2026, 2, 15),
            FechaVencimiento: new DateOnly(2027, 2, 14),
            PrimaAnual: 315.75m,
            Moneda: "EUR")
    ];

    public Task<PagedResult<PolizaListItem>> SearchAsync(PolizasSearchRequest request, PolizasSort sort, CancellationToken cancellationToken)
    {
        var query = Items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Numero))
        {
            query = query.Where(item => item.Numero.Contains(request.Numero, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Cliente))
        {
            query = query.Where(item => item.ClienteNombre.Contains(request.Cliente, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            query = query.Where(item => item.Estado.Equals(request.Estado, StringComparison.OrdinalIgnoreCase));
        }

        if (request.FechaEfectoDesde.HasValue)
        {
            query = query.Where(item => item.FechaEfecto >= request.FechaEfectoDesde.Value);
        }

        if (request.FechaEfectoHasta.HasValue)
        {
            query = query.Where(item => item.FechaEfecto <= request.FechaEfectoHasta.Value);
        }

        query = ApplySort(query, sort);

        var total = query.Count();
        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();

        return Task.FromResult(new PagedResult<PolizaListItem>(items, request.Page, request.PageSize, total));
    }

    public Task<PolizaDetail?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        var item = Items.FirstOrDefault(poliza => poliza.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
        if (item is null)
        {
            return Task.FromResult<PolizaDetail?>(null);
        }

        var detail = new PolizaDetail(
            Id: item.Id,
            Numero: item.Numero,
            Aplicacion: item.Aplicacion,
            Estado: item.Estado,
            Ramo: item.Ramo,
            Compania: item.Compania,
            Cliente: new PolizaCliente(item.ClienteId, item.ClienteNombre, Documento: null),
            Producto: new PolizaProducto(item.Aplicacion, "Modalidad demo"),
            Vigencia: new PolizaVigencia(item.FechaEfecto, item.FechaVencimiento, "Anual"),
            Financiero: new PolizaFinanciero(item.PrimaAnual, item.Moneda),
            Riesgos: [new PolizaRiesgo("R-001", "Riesgo anonimizado")]);

        return Task.FromResult<PolizaDetail?>(detail);
    }

    private static IEnumerable<PolizaListItem> ApplySort(IEnumerable<PolizaListItem> query, PolizasSort sort)
    {
        Func<PolizaListItem, object> keySelector = sort.Field.ToLowerInvariant() switch
        {
            "numero" => item => item.Numero,
            "aplicacion" => item => item.Aplicacion,
            "estado" => item => item.Estado,
            "ramo" => item => item.Ramo,
            "compania" => item => item.Compania,
            "cliente" => item => item.ClienteNombre,
            "fechaefecto" => item => item.FechaEfecto,
            "fechavencimiento" => item => item.FechaVencimiento,
            "primaanual" => item => item.PrimaAnual,
            _ => item => item.FechaEfecto
        };

        return sort.Descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
