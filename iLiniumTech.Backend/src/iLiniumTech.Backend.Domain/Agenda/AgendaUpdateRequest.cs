namespace iLiniumTech.Backend.Domain.Agenda;

public sealed record AgendaUpdateRequest(
    string? Titulo,
    DateTime? Inicio,
    DateTime? Fin,
    string? Prioridad,
    string? ObjetoRelacionadoTipo);
