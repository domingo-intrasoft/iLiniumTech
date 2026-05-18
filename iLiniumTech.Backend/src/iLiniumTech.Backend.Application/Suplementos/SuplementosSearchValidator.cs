using iLiniumTech.Backend.Domain.Suplementos;

namespace iLiniumTech.Backend.Application.Suplementos;

public static class SuplementosSearchValidator
{
    public const int MaxPageSize = 100;

    public static readonly ISet<string> AllowedSortFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "referencia",
        "poliza",
        "tipo",
        "situacion",
        "fechaEfecto"
    };

    public static SuplementosSort ValidateAndParseSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return new SuplementosSort("fechaEfecto", Descending: true);
        }

        var parts = sort.Split(':', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var field = parts[0];
        if (!AllowedSortFields.Contains(field))
        {
            throw new SuplementosValidationException("Sort field is not allowed.");
        }

        var descending = parts.Length == 1 || parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
        if (parts.Length == 2 &&
            !parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new SuplementosValidationException("Sort direction must be 'asc' or 'desc'.");
        }

        return new SuplementosSort(field, descending);
    }

    public static void Validate(SuplementosSearchRequest request)
    {
        if (request.Page < 1)
        {
            throw new SuplementosValidationException("Page must be greater than zero.");
        }

        if (request.PageSize is < 1 or > MaxPageSize)
        {
            throw new SuplementosValidationException($"PageSize must be between 1 and {MaxPageSize}.");
        }

        if (request.FechaEfectoDesde.HasValue &&
            request.FechaEfectoHasta.HasValue &&
            request.FechaEfectoDesde.Value > request.FechaEfectoHasta.Value)
        {
            throw new SuplementosValidationException("FechaEfectoDesde must be lower than FechaEfectoHasta.");
        }
    }
}

public sealed record SuplementosSort(string Field, bool Descending);

public sealed class SuplementosValidationException(string message) : InvalidOperationException(message);
