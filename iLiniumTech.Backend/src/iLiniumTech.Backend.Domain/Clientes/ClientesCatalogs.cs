namespace iLiniumTech.Backend.Domain.Clientes;

public sealed record ClientesCatalogs(
    IReadOnlyList<string> Estados,
    IReadOnlyList<string> Segmentos);
