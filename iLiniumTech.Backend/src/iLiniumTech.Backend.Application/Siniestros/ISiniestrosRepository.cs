using iLiniumTech.Backend.Domain.Polizas;
using iLiniumTech.Backend.Domain.Siniestros;

namespace iLiniumTech.Backend.Application.Siniestros;

public interface ISiniestrosRepository
{
    Task<PagedResult<SiniestroListItem>> SearchAsync(
        SiniestrosSearchRequest request,
        SiniestrosSort sort,
        CancellationToken cancellationToken);

    Task<SiniestrosCatalogs> GetCatalogsAsync(CancellationToken cancellationToken);
}
