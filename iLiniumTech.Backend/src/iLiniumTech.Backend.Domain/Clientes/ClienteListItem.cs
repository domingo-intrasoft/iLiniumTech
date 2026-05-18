namespace iLiniumTech.Backend.Domain.Clientes;

public sealed record ClienteListItem(
    string Id,
    string Referencia,
    string Alias,
    string Estado,
    string Segmento,
    DateOnly FechaAlta,
    string Resultado,
    string Datos,
    string Relacionadas);
