namespace iLiniumTech.Backend.Domain.Clientes;

public sealed record ClienteCreateRequest(
    string? NombreMostrable,
    string? TipoCliente);
