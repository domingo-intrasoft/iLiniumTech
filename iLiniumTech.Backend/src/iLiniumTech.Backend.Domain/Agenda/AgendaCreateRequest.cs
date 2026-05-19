namespace iLiniumTech.Backend.Domain.Agenda;

public sealed record AgendaCreateRequest(
    string? Referencia,
    string? Titulo,
    DateTime? Inicio,
    DateTime? Fin,
    string? Prioridad,
    string? ObjetoRelacionadoTipo);
