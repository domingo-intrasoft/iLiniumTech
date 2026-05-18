using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Polizas;

public static class PolizasWriteValidator
{
    private const int NumeroMaxLength = 50;
    private const int AplicacionMaxLength = 30;
    private const int CatalogValueMaxLength = 100;
    private const decimal PrimaAnualMaxValue = 999_999_999.99m;

    public static void ValidateId(string? id)
    {
        if (!int.TryParse(id, out var parsed) || parsed <= 0)
        {
            throw new PolizasValidationException("Poliza id must be a positive integer.");
        }
    }

    public static void ValidateCreate(PolizaCreateRequest? request)
    {
        if (request is null)
        {
            throw new PolizasValidationException("Poliza create payload is required.");
        }

        ValidateRequiredText(request.Numero, "Numero", NumeroMaxLength);
        ValidateRequiredText(request.Aplicacion, "Aplicacion", AplicacionMaxLength);
        ValidatePositive(request.CiaId, "CiaId");
        ValidatePositive(request.ClienteId, "ClienteId");
        ValidateRequiredText(request.Estado, "Estado", CatalogValueMaxLength);
        ValidateRequiredText(request.Ramo, "Ramo", CatalogValueMaxLength);
        ValidateRequiredText(request.TipoPoliza, "TipoPoliza", CatalogValueMaxLength);
        ValidateRequiredDate(request.FechaEfecto, "FechaEfecto");
        ValidateOptionalDateRange(request.FechaEfecto, request.FechaVencimiento);
        ValidateOptionalMoney(request.PrimaAnual, "PrimaAnual");
    }

    public static void ValidateUpdate(PolizaUpdateRequest? request)
    {
        if (request is null)
        {
            throw new PolizasValidationException("Poliza update payload is required.");
        }

        if (HasNoChanges(request))
        {
            throw new PolizasValidationException("At least one editable field is required.");
        }

        ValidateOptionalText(request.Numero, "Numero", NumeroMaxLength);
        ValidateOptionalText(request.Aplicacion, "Aplicacion", AplicacionMaxLength);
        ValidateOptionalText(request.Estado, "Estado", CatalogValueMaxLength);
        ValidateOptionalText(request.Ramo, "Ramo", CatalogValueMaxLength);
        ValidateOptionalText(request.TipoPoliza, "TipoPoliza", CatalogValueMaxLength);
        ValidateOptionalDateRange(request.FechaEfecto, request.FechaVencimiento);
        ValidateOptionalMoney(request.PrimaAnual, "PrimaAnual");
    }

    private static bool HasNoChanges(PolizaUpdateRequest request) =>
        request.Numero is null &&
        request.Aplicacion is null &&
        request.Estado is null &&
        request.Ramo is null &&
        request.TipoPoliza is null &&
        request.FechaEfecto is null &&
        request.FechaVencimiento is null &&
        request.PrimaAnual is null;

    private static void ValidateRequiredText(string? value, string fieldName, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new PolizasValidationException($"{fieldName} is required.");
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
            throw new PolizasValidationException($"{fieldName} cannot be empty.");
        }

        ValidateTextLength(value, fieldName, maxLength);
    }

    private static void ValidateTextLength(string value, string fieldName, int maxLength)
    {
        if (value.Trim().Length > maxLength)
        {
            throw new PolizasValidationException($"{fieldName} length must be lower than or equal to {maxLength}.");
        }
    }

    private static void ValidatePositive(int? value, string fieldName)
    {
        if (value is null or <= 0)
        {
            throw new PolizasValidationException($"{fieldName} must be a positive integer.");
        }
    }

    private static void ValidateRequiredDate(DateOnly? value, string fieldName)
    {
        if (value is null)
        {
            throw new PolizasValidationException($"{fieldName} is required.");
        }
    }

    private static void ValidateOptionalDateRange(DateOnly? fechaEfecto, DateOnly? fechaVencimiento)
    {
        if (fechaEfecto is not null &&
            fechaVencimiento is not null &&
            fechaVencimiento.Value < fechaEfecto.Value)
        {
            throw new PolizasValidationException("FechaVencimiento must be greater than or equal to FechaEfecto.");
        }
    }

    private static void ValidateOptionalMoney(decimal? value, string fieldName)
    {
        if (value is null)
        {
            return;
        }

        if (value < 0 || value > PrimaAnualMaxValue)
        {
            throw new PolizasValidationException($"{fieldName} must be between 0 and {PrimaAnualMaxValue}.");
        }
    }
}
