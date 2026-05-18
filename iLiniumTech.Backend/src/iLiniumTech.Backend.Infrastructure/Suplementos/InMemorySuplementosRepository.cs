using iLiniumTech.Backend.Application.Suplementos;
using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Suplementos;

namespace iLiniumTech.Backend.Infrastructure.Suplementos;

public sealed class InMemorySuplementosRepository : ISuplementosRepository
{
    private static readonly IReadOnlyList<SuplementoListItem> Items =
    [
        new(
            Id: "SUP-MVP-1001",
            Referencia: "SUP-2026-0001",
            Poliza: "POL-2026-0001",
            Tipo: "Alta de riesgo",
            Situacion: "Pendiente",
            FechaEfecto: new DateOnly(2026, 2, 1),
            Concepto: "Incorporacion de cobertura",
            Resumen: "Cambio operativo pendiente de contrato API"),
        new(
            Id: "SUP-MVP-1002",
            Referencia: "SUP-2026-0002",
            Poliza: "POL-2026-0002",
            Tipo: "Regularizacion",
            Situacion: "En revision",
            FechaEfecto: new DateOnly(2026, 3, 15),
            Concepto: "Revision de condiciones",
            Resumen: "Movimiento read-only sin datos restringidos"),
        new(
            Id: "SUP-MVP-1003",
            Referencia: "SUP-2026-0003",
            Poliza: "POL-2026-0003",
            Tipo: "Domiciliacion",
            Situacion: "Bloqueado",
            FechaEfecto: new DateOnly(2026, 1, 20),
            Concepto: "Cambio administrativo",
            Resumen: "Datos restringidos ocultos hasta SDD"),
        new(
            Id: "SUP-MVP-1004",
            Referencia: "SUP-2026-0004",
            Poliza: "POL-2026-0004",
            Tipo: "Renovacion",
            Situacion: "Validado",
            FechaEfecto: new DateOnly(2026, 4, 5),
            Concepto: "Actualizacion de vigencia",
            Resumen: "Lectura de muestra sin workflow")
    ];

    private static readonly SuplementosCatalogs Catalogs = new(
        Tipos: ["Alta de riesgo", "Regularizacion", "Domiciliacion", "Renovacion"],
        Situaciones: ["Pendiente", "En revision", "Validado", "Bloqueado"]);

    public Task<PagedResult<SuplementoListItem>> SearchAsync(
        SuplementosSearchRequest request,
        SuplementosSort sort,
        CancellationToken cancellationToken)
    {
        var query = Items.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Referencia))
        {
            query = query.Where(item => item.Referencia.Contains(request.Referencia, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Poliza))
        {
            query = query.Where(item => item.Poliza.Contains(request.Poliza, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Tipo))
        {
            query = query.Where(item => item.Tipo.Equals(request.Tipo, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Situacion))
        {
            query = query.Where(item => item.Situacion.Equals(request.Situacion, StringComparison.OrdinalIgnoreCase));
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

        return Task.FromResult(new PagedResult<SuplementoListItem>(items, request.Page, request.PageSize, total));
    }

    public Task<SuplementosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Catalogs);

    private static IEnumerable<SuplementoListItem> ApplySort(IEnumerable<SuplementoListItem> query, SuplementosSort sort)
    {
        Func<SuplementoListItem, object> keySelector = sort.Field.ToLowerInvariant() switch
        {
            "referencia" => item => item.Referencia,
            "poliza" => item => item.Poliza,
            "tipo" => item => item.Tipo,
            "situacion" => item => item.Situacion,
            "fechaefecto" => item => item.FechaEfecto,
            _ => item => item.FechaEfecto
        };

        return sort.Descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
