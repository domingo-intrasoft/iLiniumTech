using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Polizas;

public interface IPolizasService
{
    Task<PagedResult<PolizaListItem>> SearchAsync(PolizasSearchRequest request, CancellationToken cancellationToken);

    Task<PolizaDetail?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<PolizasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
