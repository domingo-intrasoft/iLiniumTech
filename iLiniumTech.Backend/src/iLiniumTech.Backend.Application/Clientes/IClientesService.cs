using iLiniumTech.Backend.Domain.Clientes;
using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Clientes;

public interface IClientesService
{
    Task<PagedResult<ClienteListItem>> SearchAsync(
        ClientesSearchRequest request,
        CancellationToken cancellationToken);

    Task<ClientesCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
