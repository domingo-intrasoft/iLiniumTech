namespace iLiniumTech.Backend.Domain.Suplementos;

public sealed record SuplementoListItem(
    string Id,
    string Referencia,
    string Poliza,
    string Tipo,
    string Situacion,
    DateOnly FechaEfecto,
    string Concepto,
    string Resumen);
