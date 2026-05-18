namespace iLiniumTech.Backend.Domain.Propuestas;

public sealed record PropuestaListItem(
    string Id,
    string Referencia,
    string Estado,
    string Ramo,
    DateOnly FechaAlta,
    string Canal);
