using iLiniumTech.Backend.Domain.Propuestas;

namespace iLiniumTech.Backend.Application.Propuestas;

public static class PropuestasSearchValidator
{
    public const int MaxPageSize = 100;

    public static readonly ISet<string> AllowedSortFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "referencia",
        "estado",
        "ramo",
        "canal",
        "fechaAlta"
    };

    public static PropuestasSort ValidateAndParseSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return new PropuestasSort("fechaAlta", Descending: true);
        }

        var parts = sort.Split(':', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var field = parts[0];
        if (!AllowedSortFields.Contains(field))
        {
            throw new PropuestasValidationException("Sort field is not allowed.");
        }

        var descending = parts.Length == 1 || parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
        if (parts.Length == 2 &&
            !parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new PropuestasValidationException("Sort direction must be 'asc' or 'desc'.");
        }

        return new PropuestasSort(field, descending);
    }

    public static void Validate(PropuestasSearchRequest request)
    {
        if (request.Page < 1)
        {
            throw new PropuestasValidationException("Page must be greater than zero.");
        }

        if (request.PageSize is < 1 or > MaxPageSize)
        {
            throw new PropuestasValidationException($"PageSize must be between 1 and {MaxPageSize}.");
        }

        if (request.FechaAltaDesde.HasValue &&
            request.FechaAltaHasta.HasValue &&
            request.FechaAltaDesde.Value > request.FechaAltaHasta.Value)
        {
            throw new PropuestasValidationException("FechaAltaDesde must be lower than FechaAltaHasta.");
        }
    }
}

public sealed record PropuestasSort(string Field, bool Descending);

public sealed class PropuestasValidationException(string message) : InvalidOperationException(message);
