using iLiniumTech.Backend.Domain.Clientes;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Clientes;

public sealed class ClientesService(IClientesRepository repository, IClientesWriteRepository writeRepository) : IClientesService
{
    public Task<PagedResult<ClienteListItem>> SearchAsync(
        ClientesSearchRequest request,
        CancellationToken cancellationToken)
    {
        ClientesSearchValidator.Validate(request);
        var sort = ClientesSearchValidator.ValidateAndParseSort(request.Sort);
        return repository.SearchAsync(request, sort, cancellationToken);
    }

    public Task<ClientesCatalogs> GetCatalogsAsync(CancellationToken cancellationToken) =>
        repository.GetCatalogsAsync(cancellationToken);

    public Task<ClienteCreateResult> CreateAsync(ClienteCreateRequest request, CancellationToken cancellationToken)
    {
        ClientesWriteValidator.ValidateCreate(request);
        return writeRepository.CreateAsync(request, cancellationToken);
    }

    public Task<bool> UpdateAsync(string id, ClienteUpdateRequest request, CancellationToken cancellationToken)
    {
        _ = ClientesWriteValidator.ValidateAndParseId(id);
        ClientesWriteValidator.ValidateUpdate(request);
        return writeRepository.UpdateAsync(id, request, cancellationToken);
    }

    public Task<bool> DeleteAsync(string id, CancellationToken cancellationToken)
    {
        _ = ClientesWriteValidator.ValidateAndParseId(id);
        return writeRepository.DeleteAsync(id, cancellationToken);
    }
}
