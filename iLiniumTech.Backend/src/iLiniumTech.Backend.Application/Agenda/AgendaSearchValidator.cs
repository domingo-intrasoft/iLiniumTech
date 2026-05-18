using iLiniumTech.Backend.Domain.Agenda;

namespace iLiniumTech.Backend.Application.Agenda;

public static class AgendaSearchValidator
{
    public const int MaxPageSize = 100;
    public const int MaxRangeDays = 93;

    public static readonly ISet<string> AllowedSortFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "referencia",
        "titulo",
        "estado",
        "prioridad",
        "inicio",
        "fin",
        "origen"
    };

    public static AgendaSort ValidateAndParseSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return new AgendaSort("inicio", Descending: false);
        }

        var parts = sort.Split(':', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var field = parts[0];
        if (!AllowedSortFields.Contains(field))
        {
            throw new AgendaValidationException("Sort field is not allowed.");
        }

        var descending = parts.Length == 1 || parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
        if (parts.Length == 2 &&
            !parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new AgendaValidationException("Sort direction must be 'asc' or 'desc'.");
        }

        return new AgendaSort(field, descending);
    }

    public static void Validate(AgendaSearchRequest request)
    {
        if (request.Page < 1)
        {
            throw new AgendaValidationException("Page must be greater than zero.");
        }

        if (request.PageSize is < 1 or > MaxPageSize)
        {
            throw new AgendaValidationException($"PageSize must be between 1 and {MaxPageSize}.");
        }

        if (request.FechaDesde.HasValue &&
            request.FechaHasta.HasValue &&
            request.FechaDesde.Value > request.FechaHasta.Value)
        {
            throw new AgendaValidationException("FechaDesde must be lower than FechaHasta.");
        }

        if (request.FechaDesde.HasValue &&
            request.FechaHasta.HasValue &&
            request.FechaHasta.Value.DayNumber - request.FechaDesde.Value.DayNumber > MaxRangeDays)
        {
            throw new AgendaValidationException($"Date range must not exceed {MaxRangeDays} days.");
        }
    }
}

public sealed record AgendaSort(string Field, bool Descending);

public sealed class AgendaValidationException(string message) : InvalidOperationException(message);
