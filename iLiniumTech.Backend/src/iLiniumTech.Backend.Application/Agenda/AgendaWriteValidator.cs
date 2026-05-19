using iLiniumTech.Backend.Domain.Agenda;

namespace iLiniumTech.Backend.Application.Agenda;

public static class AgendaWriteValidator
{
    private const int ReferenciaMaxLength = 50;
    private const int TituloMaxLength = 100;
    private const int PrioridadMaxLength = 50;
    private const int ObjetoRelacionadoTipoMaxLength = 50;

    public static int ValidateAndParseId(string? id)
    {
        if (!int.TryParse(id, out var parsed) || parsed <= 0)
        {
            throw new AgendaValidationException("Agenda id must be a positive integer.");
        }

        return parsed;
    }

    public static void ValidateCreate(AgendaCreateRequest? request)
    {
        if (request is null)
        {
            throw new AgendaValidationException("Agenda create payload is required.");
        }

        ValidateRequiredText(request.Referencia, "Referencia", ReferenciaMaxLength);
        ValidateMvpReferencia(request.Referencia);
        ValidateRequiredText(request.Titulo, "Titulo", TituloMaxLength);
        ValidateRequiredDateTime(request.Inicio, "Inicio");
        ValidateOptionalDateRange(request.Inicio, request.Fin);
        ValidateOptionalText(request.Prioridad, "Prioridad", PrioridadMaxLength);
        ValidateOptionalText(request.ObjetoRelacionadoTipo, "ObjetoRelacionadoTipo", ObjetoRelacionadoTipoMaxLength);
    }

    public static void ValidateUpdate(AgendaUpdateRequest? request)
    {
        if (request is null)
        {
            throw new AgendaValidationException("Agenda update payload is required.");
        }

        ValidateOptionalText(request.Titulo, "Titulo", TituloMaxLength);
        ValidateOptionalText(request.Prioridad, "Prioridad", PrioridadMaxLength);
        ValidateOptionalText(request.ObjetoRelacionadoTipo, "ObjetoRelacionadoTipo", ObjetoRelacionadoTipoMaxLength);
        ValidateOptionalDateRange(request.Inicio, request.Fin);

        if (request.Titulo is null &&
            request.Inicio is null &&
            request.Fin is null &&
            request.Prioridad is null &&
            request.ObjetoRelacionadoTipo is null)
        {
            throw new AgendaValidationException("At least one editable field is required.");
        }
    }

    private static void ValidateRequiredText(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new AgendaValidationException($"{fieldName} is required.");
        }

        ValidateTextLength(value, fieldName, maxLength);
    }

    private static void ValidateOptionalText(string? value, string fieldName, int maxLength)
    {
        if (value is null)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(value))
        {
            throw new AgendaValidationException($"{fieldName} cannot be empty.");
        }

        ValidateTextLength(value, fieldName, maxLength);
    }

    private static void ValidateTextLength(string value, string fieldName, int maxLength)
    {
        if (value.Trim().Length > maxLength)
        {
            throw new AgendaValidationException($"{fieldName} length must be lower than or equal to {maxLength}.");
        }
    }

    private static void ValidateMvpReferencia(string? value)
    {
        if (value is null ||
            !value.Trim().StartsWith(AgendaMvpWriteDefaults.ReferenciaPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new AgendaValidationException(
                $"Referencia must start with {AgendaMvpWriteDefaults.ReferenciaPrefix} for MVP writes.");
        }

        if (value.Trim().StartsWith(AgendaMvpWriteDefaults.DeletedReferenciaPrefix, StringComparison.OrdinalIgnoreCase))
        {
            throw new AgendaValidationException("Referencia cannot use the reserved MVP deleted prefix.");
        }
    }

    private static void ValidateRequiredDateTime(DateTime? value, string fieldName)
    {
        if (value is null)
        {
            throw new AgendaValidationException($"{fieldName} is required.");
        }
    }

    private static void ValidateOptionalDateRange(DateTime? inicio, DateTime? fin)
    {
        if (inicio is not null &&
            fin is not null &&
            fin.Value < inicio.Value)
        {
            throw new AgendaValidationException("Fin must be greater than or equal to Inicio.");
        }
    }
}
