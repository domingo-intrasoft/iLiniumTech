namespace iLiniumTech.Backend.Domain.Polizas;

public sealed record PolizaListItem(
    string Id,
    string Numero,
    string Aplicacion,
    string Estado,
    string Ramo,
    string ClienteId,
    string ClienteNombre,
    string Compania,
    DateOnly FechaEfecto,
    DateOnly FechaVencimiento,
    decimal PrimaAnual,
    string Moneda);
