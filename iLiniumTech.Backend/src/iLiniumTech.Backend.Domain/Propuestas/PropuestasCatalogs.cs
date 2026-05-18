namespace iLiniumTech.Backend.Domain.Propuestas;

public sealed record PropuestasCatalogs(
    IReadOnlyList<string> Estados,
    IReadOnlyList<string> Ramos,
    IReadOnlyList<string> Canales);
