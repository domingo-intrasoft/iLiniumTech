using iLiniumTech.Backend.Domain.Clientes;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Clientes;

public interface IClientesRepository
{
    Task<PagedResult<ClienteListItem>> SearchAsync(
        ClientesSearchRequest request,
        ClientesSort sort,
        CancellationToken cancellationToken);

    Task<ClientesCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
