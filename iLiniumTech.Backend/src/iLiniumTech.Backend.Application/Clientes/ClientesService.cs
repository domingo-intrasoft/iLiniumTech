using iLiniumTech.Backend.Domain.Clientes;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Clientes;

public sealed class ClientesService(IClientesRepository repository) : IClientesService
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
}
