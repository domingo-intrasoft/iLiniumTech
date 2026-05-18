namespace iLiniumTech.Backend.Domain.Recibos;

public sealed record RecibosCatalogs(
    IReadOnlyList<string> Situaciones,
    IReadOnlyList<string> Tipos,
    IReadOnlyList<string> Canales);
