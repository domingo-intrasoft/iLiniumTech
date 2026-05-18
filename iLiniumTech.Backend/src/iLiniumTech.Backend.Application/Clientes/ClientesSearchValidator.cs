using iLiniumTech.Backend.Domain.Clientes;

namespace iLiniumTech.Backend.Application.Clientes;

public static class ClientesSearchValidator
{
    public const int MaxPageSize = 100;

    public static readonly ISet<string> AllowedSortFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "referencia",
        "alias",
        "estado",
        "segmento",
        "fechaAlta"
    };

    public static ClientesSort ValidateAndParseSort(string? sort)
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return new ClientesSort("fechaAlta", Descending: true);
        }

        var parts = sort.Split(':', 2, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        var field = parts[0];
        if (!AllowedSortFields.Contains(field))
        {
            throw new ClientesValidationException("Sort field is not allowed.");
        }

        var descending = parts.Length == 1 || parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase);
        if (parts.Length == 2 &&
            !parts[1].Equals("asc", StringComparison.OrdinalIgnoreCase) &&
            !parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase))
        {
            throw new ClientesValidationException("Sort direction must be 'asc' or 'desc'.");
        }

        return new ClientesSort(field, descending);
    }

    public static void Validate(ClientesSearchRequest request)
    {
        if (request.Page < 1)
        {
            throw new ClientesValidationException("Page must be greater than zero.");
        }

        if (request.PageSize is < 1 or > MaxPageSize)
        {
            throw new ClientesValidationException($"PageSize must be between 1 and {MaxPageSize}.");
        }

        if (request.FechaAltaDesde.HasValue &&
            request.FechaAltaHasta.HasValue &&
            request.FechaAltaDesde.Value > request.FechaAltaHasta.Value)
        {
            throw new ClientesValidationException("FechaAltaDesde must be lower than FechaAltaHasta.");
        }
    }
}

public sealed record ClientesSort(string Field, bool Descending);

public sealed class ClientesValidationException(string message) : InvalidOperationException(message);
