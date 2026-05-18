namespace iLiniumTech.Backend.Domain.Siniestros;

public sealed record SiniestrosCatalogs(
    IReadOnlyList<string> Estados,
    IReadOnlyList<string> Prioridades);
