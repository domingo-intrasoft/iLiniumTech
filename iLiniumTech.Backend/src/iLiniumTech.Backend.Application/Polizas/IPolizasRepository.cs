using iLiniumTech.Backend.Domain.Polizas;

namespace iLiniumTech.Backend.Application.Polizas;

public interface IPolizasRepository
{
    Task<PagedResult<PolizaListItem>> SearchAsync(PolizasSearchRequest request, PolizasSort sort, CancellationToken cancellationToken);

    Task<PolizaDetail?> GetByIdAsync(string id, CancellationToken cancellationToken);

    Task<PolizasCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
