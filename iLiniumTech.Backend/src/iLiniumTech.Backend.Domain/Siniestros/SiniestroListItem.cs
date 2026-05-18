namespace iLiniumTech.Backend.Domain.Siniestros;

public sealed record SiniestroListItem(
    string Id,
    string Referencia,
    string Poliza,
    string Cliente,
    string Compania,
    string Situacion,
    string Estado,
    string Prioridad,
    DateOnly FechaSiniestro,
    DateOnly FechaParte,
    string Tramitador);
