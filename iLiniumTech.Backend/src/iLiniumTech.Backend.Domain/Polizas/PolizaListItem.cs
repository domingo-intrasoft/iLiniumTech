namespace iLiniumTech.Backend.Domain.Polizas;

public sealed record PolizaListItem(
    string Id,
    string Numero,
    string Aplicacion,
    string Estado,
    string Ramo,
    string ClienteId,
    string ClienteNombre,
    string Documento,
    string Compania,
    string Riesgo,
    DateOnly FechaEfecto,
    DateOnly FechaVencimiento,
    decimal PrimaAnual,
    string Moneda);
