using iLiniumTech.Backend.Domain.Clientes;

namespace iLiniumTech.Backend.Application.Clientes;

public static class ClientesWriteValidator
{
    private const int NombreMostrableMaxLength = 120;

    public static int ValidateAndParseId(string? id)
    {
        if (!int.TryParse(id, out var parsed) || parsed <= 0)
        {
            throw new ClientesValidationException("Cliente id must be a positive integer.");
        }

        return parsed;
    }

    public static void ValidateCreate(ClienteCreateRequest? request)
    {
        if (request is null)
        {
            throw new ClientesValidationException("Cliente create payload is required.");
        }

        ValidateRequiredText(request.NombreMostrable, "NombreMostrable", NombreMostrableMaxLength);
        ValidateTipoCliente(request.TipoCliente);
    }

    public static void ValidateUpdate(ClienteUpdateRequest? request)
    {
        if (request is null)
        {
            throw new ClientesValidationException("Cliente update payload is required.");
        }

        ValidateOptionalText(request.NombreMostrable, "NombreMostrable", NombreMostrableMaxLength);
        ValidateTipoCliente(request.TipoCliente);

        if (request.NombreMostrable is null && request.TipoCliente is null)
        {
            throw new ClientesValidationException("At least one editable field is required.");
        }
    }

    public static string NormalizeTipoCliente(string? tipoCliente)
    {
        if (string.IsNullOrWhiteSpace(tipoCliente))
        {
            return "particular";
        }

        return tipoCliente.Trim().Equals("empresa", StringComparison.OrdinalIgnoreCase)
            ? "empresa"
            : "particular";
    }

    private static void ValidateRequiredText(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ClientesValidationException($"{fieldName} is required.");
        }

        ValidateTextLength(value, fieldName, maxLength);
        ValidateNoLineBreaks(value, fieldName);
    }

    private static void ValidateOptionalText(string? value, string fieldName, int maxLength)
    {
        if (value is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ClientesValidationException($"{fieldName} cannot be empty.");
        }

        ValidateTextLength(value, fieldName, maxLength);
        ValidateNoLineBreaks(value, fieldName);
    }

    private static void ValidateTextLength(string value, string fieldName, int maxLength)
    {
        if (value.Trim().Length > maxLength)
        {
            throw new ClientesValidationException($"{fieldName} length must be lower than or equal to {maxLength}.");
        }
    }

    private static void ValidateNoLineBreaks(string value, string fieldName)
    {
        if (value.Any(character => character is '\r' or '\n'))
        {
            throw new ClientesValidationException($"{fieldName} cannot contain line breaks.");
        }
    }

    private static void ValidateTipoCliente(string? value)
    {
        if (value is null)
        {
            return;
        }

        var normalized = value.Trim();
        if (normalized.Equals("particular", StringComparison.OrdinalIgnoreCase) ||
            normalized.Equals("empresa", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        throw new ClientesValidationException("TipoCliente must be 'particular' or 'empresa'.");
    }
}
