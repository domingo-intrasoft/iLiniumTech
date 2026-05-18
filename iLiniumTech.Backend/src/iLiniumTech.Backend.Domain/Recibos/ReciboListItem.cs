namespace iLiniumTech.Backend.Domain.Recibos;

public sealed record ReciboListItem(
    string Id,
    string Recibo,
    string Poliza,
    string Cliente,
    string Compania,
    string Tipo,
    string Situacion,
    DateOnly FechaEfecto,
    DateOnly FechaVencimiento,
    string EstadoCobro,
    string Canal);
