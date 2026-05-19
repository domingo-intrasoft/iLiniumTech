namespace iLiniumTech.Backend.Domain.Clientes;

public sealed record ClienteUpdateRequest(
    string? NombreMostrable,
    string? TipoCliente);
