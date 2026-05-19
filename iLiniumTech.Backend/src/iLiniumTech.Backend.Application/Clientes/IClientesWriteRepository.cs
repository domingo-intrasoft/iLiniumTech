using iLiniumTech.Backend.Domain.Clientes;

namespace iLiniumTech.Backend.Application.Clientes;

public interface IClientesWriteRepository
{
    Task<ClienteCreateResult> CreateAsync(ClienteCreateRequest request, CancellationToken cancellationToken);

    Task<bool> UpdateAsync(string id, ClienteUpdateRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteAsync(string id, CancellationToken cancellationToken);
}
