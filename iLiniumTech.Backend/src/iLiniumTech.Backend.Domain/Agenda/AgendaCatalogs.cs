namespace iLiniumTech.Backend.Domain.Agenda;

public sealed record AgendaCatalogs(
    IReadOnlyList<string> Estados,
    IReadOnlyList<string> Prioridades,
    IReadOnlyList<string> Origenes);
