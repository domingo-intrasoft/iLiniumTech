namespace iLiniumTech.Backend.Domain.Agenda;

public sealed record AgendaEventListItem(
    string Id,
    string Referencia,
    string Titulo,
    DateTime Inicio,
    DateTime? Fin,
    string Estado,
    string Prioridad,
    string Origen,
    string ObjetoRelacionadoTipo);
