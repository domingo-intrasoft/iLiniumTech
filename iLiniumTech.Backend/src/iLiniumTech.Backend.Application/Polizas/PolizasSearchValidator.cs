using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Polizas;

public static class PolizasSearchValidator
{
    public const int MaxPageSize = 100;

    public static readonly ISet<string> AllowedSortFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "numero",
        "aplicacion",
        "estado",
        "ramo",
        "compania",
        "cliente",
        "fechaEfecto",
        "fechaVencimiento",
        "primaAnual"
    };

    public static PolizasSort ValidateAndParseSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return new PolizasSort("fechaEfecto", Descending: true);
        }

        var parts = sort.Split(':', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var field = parts[0];
        if (!AllowedSortFields.Contains(field))
        {
            throw new PolizasValidationException($"Sort field '{field}' is not allowed.");
        }

        var descending = parts.Length == 1 || parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
        if (parts.Length == 2 &&
            !parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new PolizasValidationException("Sort direction must be 'asc' or 'desc'.");
        }

        return new PolizasSort(field, descending);
    }

    public static void Validate(PolizasSearchRequest request)
    {
        if (request.Page < 1)
        {
            throw new PolizasValidationException("Page must be greater than zero.");
        }

        if (request.PageSize is < 1 or > MaxPageSize)
        {
            throw new PolizasValidationException($"PageSize must be between 1 and {MaxPageSize}.");
        }

        if (request.FechaEfectoDesde.HasValue &&
            request.FechaEfectoHasta.HasValue &&
            request.FechaEfectoDesde.Value > request.FechaEfectoHasta.Value)
        {
            throw new PolizasValidationException("FechaEfectoDesde must be lower than FechaEfectoHasta.");
        }
    }
}

public sealed class PolizasValidationException(string message) : InvalidOperationException(message);
