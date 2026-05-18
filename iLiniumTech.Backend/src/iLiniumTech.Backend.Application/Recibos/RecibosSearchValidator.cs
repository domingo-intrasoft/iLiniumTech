using iLiniumTech.Backend.Domain.Recibos;

namespace iLiniumTech.Backend.Application.Recibos;

public static class RecibosSearchValidator
{
    public const int MaxPageSize = 100;

    public static readonly ISet<string> AllowedSortFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "recibo",
        "poliza",
        "situacion",
        "tipo",
        "compania",
        "fechaEfecto",
        "fechaVencimiento"
    };

    public static RecibosSort ValidateAndParseSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return new RecibosSort("fechaVencimiento", Descending: true);
        }

        var parts = sort.Split(':', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var field = parts[0];
        if (!AllowedSortFields.Contains(field))
        {
            throw new RecibosValidationException("Sort field is not allowed.");
        }

        var descending = parts.Length == 1 || parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
        if (parts.Length == 2 &&
            !parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new RecibosValidationException("Sort direction must be 'asc' or 'desc'.");
        }

        return new RecibosSort(field, descending);
    }

    public static void Validate(RecibosSearchRequest request)
    {
        if (request.Page < 1)
        {
            throw new RecibosValidationException("Page must be greater than zero.");
        }

        if (request.PageSize is < 1 or > MaxPageSize)
        {
            throw new RecibosValidationException($"PageSize must be between 1 and {MaxPageSize}.");
        }

        if (request.FechaVencimientoDesde.HasValue &&
            request.FechaVencimientoHasta.HasValue &&
            request.FechaVencimientoDesde.Value > request.FechaVencimientoHasta.Value)
        {
            throw new RecibosValidationException("FechaVencimientoDesde must be lower than FechaVencimientoHasta.");
        }
    }
}

public sealed record RecibosSort(string Field, bool Descending);

public sealed class RecibosValidationException(string message) : InvalidOperationException(message);
