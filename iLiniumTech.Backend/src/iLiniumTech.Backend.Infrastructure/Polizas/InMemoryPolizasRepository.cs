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

    private static readonly PolizasCatalogs Catalogs = PolizasCatalogProvider.CreateDefault();

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

        if (!string.IsNullOrWhiteSpace(request.Compania))
        {
            query = query.Where(item => item.Compania.Equals(request.Compania, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Ramo))
        {
            query = query.Where(item => item.Ramo.Equals(request.Ramo, StringComparison.OrdinalIgnoreCase));
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
            Certificado: item.Id.Replace("POL", "CERT", StringComparison.OrdinalIgnoreCase),
            TipoPoliza: item.Estado,
            Aplicacion: item.Aplicacion,
            Estado: item.Estado,
            Ramo: item.Ramo,
            ClienteId: item.ClienteId,
            ClienteNombre: item.ClienteNombre,
            Compania: item.Compania,
            Riesgo: item.Ramo == "Autos" ? "1234 ABC" : "Vivienda demo",
            FechaEfecto: item.FechaEfecto,
            FechaVencimiento: item.FechaVencimiento,
            PrimaAnual: item.PrimaAnual,
            Moneda: item.Moneda,
            Oficina: item.Ramo == "Autos" ? "Las Palmas" : "Tenerife",
            Division: "Particulares",
            Colaborador1: item.Ramo == "Autos" ? "COL-01" : "COL-02",
            Administrativo: item.Ramo == "Autos" ? "ADM-01" : "ADM-02",
            Comercial: item.Ramo == "Autos" ? "COM-01" : "COM-02",
            Siniestros: item.Ramo == "Autos" ? "SIN-01" : "SIN-02",
            Gestor: item.Ramo == "Autos" ? "GES-01" : "GES-02",
            CanalCobro: item.Ramo == "Autos" ? "Banco" : "Tarjeta",
            FraccionPago: item.Ramo == "Autos" ? "Anual" : "Semestral",
            Ccaa: "Canarias",
            Documento: item.Ramo == "Autos" ? "00000001A" : "00000002B",
            Apellido1: "Anonimo",
            Apellido2: item.Ramo == "Autos" ? "Uno" : "Dos",
            Nombre: "Cliente",
            Sexo: item.Ramo == "Autos" ? "M" : "H",
            FechaNacimiento: item.Ramo == "Autos" ? new DateOnly(1988, 5, 10) : new DateOnly(1991, 9, 22),
            Edad: item.Ramo == "Autos" ? 37 : 34,
            EstadoCivil: item.Ramo == "Autos" ? "Casado/a" : "Soltero/a",
            Hijos: item.Ramo == "Autos" ? 1 : 0,
            RegimenLaboral: item.Ramo == "Autos" ? "Cuenta ajena" : "Autonomo",
            Profesion: item.Ramo == "Autos" ? "Administracion" : "Ingenieria",
            Email: item.Ramo == "Autos" ? "cliente1@example.test" : "cliente2@example.test",
            Telefono: item.Ramo == "Autos" ? "+34 600 000 001" : "+34 600 000 002");

        return Task.FromResult<PolizaDetail?>(detail);
    }

    public Task<PolizasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Catalogs);

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
