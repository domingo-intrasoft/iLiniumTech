using iLiniumTech.Backend.Domain.Siniestros;

namespace iLiniumTech.Backend.Application.Siniestros;

public static class SiniestrosSearchValidator
{
    public const int MaxPageSize = 100;

    public static readonly ISet<string> AllowedSortFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "referencia",
        "poliza",
        "estado",
        "prioridad",
        "compania",
        "fechaSiniestro",
        "fechaParte"
    };

    public static SiniestrosSort ValidateAndParseSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return new SiniestrosSort("fechaSiniestro", Descending: true);
        }

        var parts = sort.Split(':', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var field = parts[0];
        if (!AllowedSortFields.Contains(field))
        {
            throw new SiniestrosValidationException("Sort field is not allowed.");
        }

        var descending = parts.Length == 1 || parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
        if (parts.Length == 2 &&
            !parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new SiniestrosValidationException("Sort direction must be 'asc' or 'desc'.");
        }

        return new SiniestrosSort(field, descending);
    }

    public static void Validate(SiniestrosSearchRequest request)
    {
        if (request.Page < 1)
        {
            throw new SiniestrosValidationException("Page must be greater than zero.");
        }

        if (request.PageSize is < 1 or > MaxPageSize)
        {
            throw new SiniestrosValidationException($"PageSize must be between 1 and {MaxPageSize}.");
        }

        if (request.FechaSiniestroDesde.HasValue &&
            request.FechaSiniestroHasta.HasValue &&
            request.FechaSiniestroDesde.Value > request.FechaSiniestroHasta.Value)
        {
            throw new SiniestrosValidationException("FechaSiniestroDesde must be lower than FechaSiniestroHasta.");
        }
    }
}

public sealed record SiniestrosSort(string Field, bool Descending);

public sealed class SiniestrosValidationException(string message) : InvalidOperationException(message);
