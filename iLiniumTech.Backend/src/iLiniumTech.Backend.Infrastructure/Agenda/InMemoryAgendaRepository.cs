using System.Globalization;
using iLiniumTech.Backend.Application.Agenda;
using iLiniumTech.Backend.Domain.Agenda;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Infrastructure.Agenda;

public sealed class InMemoryAgendaRepository : IAgendaRepository, IAgendaWriteRepository
{
    private readonly object _lock = new();
    private readonly List<AgendaEventListItem> _items =
    [
        new(
            Id: "1001",
            Referencia: "AGE-2026-0001",
            Titulo: "Revision demo de documentacion",
            Inicio: new DateTime(2026, 5, 18, 9, 30, 0),
            Fin: new DateTime(2026, 5, 18, 10, 0, 0),
            Estado: "Pendiente",
            Prioridad: "Alta",
            Origen: "Fixture local",
            ObjetoRelacionadoTipo: "Poliza demo"),
        new(
            Id: "1002",
            Referencia: "AGE-2026-0002",
            Titulo: "Seguimiento demo de tramite",
            Inicio: new DateTime(2026, 5, 21, 12, 0, 0),
            Fin: new DateTime(2026, 5, 21, 12, 30, 0),
            Estado: "Programado",
            Prioridad: "Media",
            Origen: "Fixture local",
            ObjetoRelacionadoTipo: "Siniestro demo"),
        new(
            Id: "1003",
            Referencia: "AGE-2026-0003",
            Titulo: "Cierre demo de tarea interna",
            Inicio: new DateTime(2026, 5, 24, 16, 0, 0),
            Fin: new DateTime(2026, 5, 24, 16, 20, 0),
            Estado: "Cerrado",
            Prioridad: "Baja",
            Origen: "Fixture local",
            ObjetoRelacionadoTipo: "Recibo demo"),
        new(
            Id: "1004",
            Referencia: "AGE-2026-0004",
            Titulo: "Control demo de agenda semanal",
            Inicio: new DateTime(2026, 6, 2, 11, 15, 0),
            Fin: new DateTime(2026, 6, 2, 11, 45, 0),
            Estado: "Programado",
            Prioridad: "Alta",
            Origen: "Fixture local",
            ObjetoRelacionadoTipo: "Generico demo")
    ];

    private static readonly AgendaCatalogs Catalogs = new(
        Estados: ["Pendiente", "Programado", "Cerrado"],
        Prioridades: ["Alta", "Media", "Baja"],
        Origenes: ["Fixture local"]);

    public Task<PagedResult<AgendaEventListItem>> SearchAsync(
        AgendaSearchRequest request,
        AgendaSort sort,
        CancellationToken cancellationToken)
    {
        AgendaEventListItem[] snapshot;
        lock (_lock)
        {
            snapshot = [.. _items];
        }

        var query = snapshot
            .Where(item => !item.Referencia.StartsWith(
                AgendaMvpWriteDefaults.DeletedReferenciaPrefix,
                StringComparison.OrdinalIgnoreCase))
            .AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Texto))
        {
            query = query.Where(item =>
                item.Referencia.Contains(request.Texto, StringComparison.OrdinalIgnoreCase) ||
                item.Titulo.Contains(request.Texto, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Estado))
        {
            query = query.Where(item => item.Estado.Equals(request.Estado, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Prioridad))
        {
            query = query.Where(item => item.Prioridad.Equals(request.Prioridad, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.Origen))
        {
            query = query.Where(item => item.Origen.Equals(request.Origen, StringComparison.OrdinalIgnoreCase));
        }

        if (request.FechaDesde.HasValue)
        {
            query = query.Where(item => DateOnly.FromDateTime(item.Inicio) >= request.FechaDesde.Value);
        }

        if (request.FechaHasta.HasValue)
        {
            query = query.Where(item => DateOnly.FromDateTime(item.Inicio) <= request.FechaHasta.Value);
        }

        query = ApplySort(query, sort);

        var total = query.Count();
        var items = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToArray();

        return Task.FromResult(new PagedResult<AgendaEventListItem>(items, request.Page, request.PageSize, total));
    }

    public Task<AgendaCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Catalogs);

    public Task<AgendaCreateResult> CreateAsync(AgendaCreateRequest request, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            var nextId = _items.Count == 0
                ? 1001
                : _items
                    .Select(item => int.TryParse(item.Id, out var id) ? id : 0)
                    .DefaultIfEmpty(1000)
                    .Max() + 1;
            var id = nextId.ToString(CultureInfo.InvariantCulture);
            _items.Add(new AgendaEventListItem(
                Id: id,
                Referencia: request.Referencia!.Trim(),
                Titulo: request.Titulo!.Trim(),
                Inicio: request.Inicio!.Value,
                Fin: request.Fin,
                Estado: "Programado",
                Prioridad: request.Prioridad?.Trim() ?? "Media",
                Origen: "Fixture local",
                ObjetoRelacionadoTipo: request.ObjetoRelacionadoTipo?.Trim() ?? "Generico demo"));

            return Task.FromResult(new AgendaCreateResult(id));
        }
    }

    public Task<bool> UpdateAsync(string id, AgendaUpdateRequest request, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            var index = _items.FindIndex(item => string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                return Task.FromResult(false);
            }

            var current = _items[index];
            _items[index] = current with
            {
                Titulo = request.Titulo?.Trim() ?? current.Titulo,
                Inicio = request.Inicio ?? current.Inicio,
                Fin = request.Fin ?? current.Fin,
                Prioridad = request.Prioridad?.Trim() ?? current.Prioridad,
                ObjetoRelacionadoTipo = request.ObjetoRelacionadoTipo?.Trim() ?? current.ObjetoRelacionadoTipo
            };

            return Task.FromResult(true);
        }
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        lock (_lock)
        {
            var index = _items.FindIndex(item => string.Equals(item.Id, id, StringComparison.OrdinalIgnoreCase));
            if (index < 0)
            {
                return Task.FromResult(false);
            }

            var current = _items[index];
            _items[index] = current with
            {
                Referencia = $"{AgendaMvpWriteDefaults.DeletedReferenciaPrefix}{current.Id}",
                Estado = "Cerrado"
            };

            return Task.FromResult(true);
        }
    }

    private static IEnumerable<AgendaEventListItem> ApplySort(IEnumerable<AgendaEventListItem> query, AgendaSort sort)
    {
        Func<AgendaEventListItem, object> keySelector = sort.Field.ToLowerInvariant() switch
        {
            "referencia" => item => item.Referencia,
            "titulo" => item => item.Titulo,
            "estado" => item => item.Estado,
            "prioridad" => item => item.Prioridad,
            "inicio" => item => item.Inicio,
            "fin" => item => item.Fin ?? DateTime.MaxValue,
            "origen" => item => item.Origen,
            _ => item => item.Inicio
        };

        return sort.Descending
            ? query.OrderByDescending(keySelector)
            : query.OrderBy(keySelector);
    }
}
