namespace iLiniumTech.Backend.Domain.Suplementos;

public sealed record SuplementosCatalogs(
    IReadOnlyList<string> Tipos,
    IReadOnlyList<string> Situaciones);
